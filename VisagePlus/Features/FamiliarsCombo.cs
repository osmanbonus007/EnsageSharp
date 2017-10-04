using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace VisagePlus.Features
{
    internal class FamiliarsCombo : Extensions
    {
        private Config Config { get; }

        private Unit Owner { get; }

        private Sleeper FamiliarsSleeper { get; }

        private TaskHandler Handler { get; }

        private Hero Target { get; set; }

        public FamiliarsCombo(Config config)
        {
            Config = config;
            Owner = config.Main.Context.Owner;

            FamiliarsSleeper = new Sleeper();

            config.ComboKeyItem.PropertyChanged += ComboChanged;
            config.FamiliarsLockItem.PropertyChanged += FamiliarsLockChanged;

            if (config.FamiliarsLockItem)
            {
                config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }

            Handler = UpdateManager.Run(ExecuteAsync, true, false);
        }

        public void Dispose()
        {
            if (Config.ComboKeyItem)
            {
                Handler?.Cancel();
            }

            Config.FamiliarsLockItem.PropertyChanged -= FamiliarsLockChanged;
            Config.ComboKeyItem.PropertyChanged -= ComboChanged;
        }

        private void FamiliarsLockChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FamiliarsLockItem)
            {
                Config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.LastHitItem.Item.SetValue(new KeyBind(
                    Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }

            if (Handler.IsRunning)
            {
                Handler?.Cancel();
            }

            if (Config.FamiliarsLockItem)
            {
                Handler.RunAsync();
            }
            else
            {
                Handler?.Cancel();
            }
        }

        private void ComboChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!Config.FamiliarsLockItem)
            {
                if (Config.ComboKeyItem)
                {
                    Handler.RunAsync();
                }
                else
                {
                    Handler?.Cancel();
                }
            }
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Game.IsPaused)
                {
                    return;
                }

                var Familiars =
                    EntityManager<Unit>.Entities.Where(x =>
                                                       x.IsValid &&
                                                       x.IsAlive &&
                                                       x.IsControllable &&
                                                       !x.IsStunned() &&
                                                       x.IsAlly(Owner) &&
                                                       x.NetworkName == "CDOTA_Unit_VisageFamiliar").ToList();

                if (Config.FamiliarsLockItem)
                {
                    Target = Config.UpdateMode.FamiliarTarget;
                }
                else
                {
                    Target = Config.UpdateMode.Target;
                }

                foreach (var Familiar in Familiars)
                {
                    if (Target != null)
                    {
                        var FamiliarDamage = Familiar.Modifiers.Any(x => 
                                                                    x.IsValid && 
                                                                    x.StackCount > Config.FamiliarsChargeItem && 
                                                                    x.Name == "modifier_visage_summon_familiars_damage_charge");

                        var StunDebuff = Target.Modifiers.Any(x => x.IsValid && x.IsStunDebuff && x.RemainingTime > 0.5f);
                        var HexDebuff = Target.Modifiers.Any(x => x.IsValid && x.RemainingTime > 0.5f && x.Name == "modifier_sheepstick_debuff");
                        var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);

                        if (!Target.IsMagicImmune() && !Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse"))
                        {
                            // FamiliarsStoneForm
                            if (Config.AbilityToggler.Value.IsEnabled(FamiliarsStoneForm.Name)
                                && CanBeCasted(FamiliarsStoneForm, Familiar)
                                && Familiar.Distance2D(Target) <= 100
                                && !StunDebuff && !HexDebuff
                                && (!Config.FamiliarsStoneControlItem || SmartStone(Target) || !FamiliarDamage)
                                && !FamiliarsSleeper.Sleeping)
                            {
                                UseAbility(FamiliarsStoneForm, Familiar);
                                FamiliarsSleeper.Sleep(FamiliarsStoneForm.GetAbilitySpecialData("stun_duration") * 1000 - 200);
                                await Await.Delay(GetDelay);
                            }
                        }

                        if (Target.IsInvulnerable() || Target.IsAttackImmune())
                        {
                            Move(Familiar, Target.Position);
                        }
                        else if (Config.FamiliarsStoneControlItem  && FamiliarDamage && !SmartStone(Target))
                        {
                            Attack(Familiar, Target);
                        }
                        else if ((Target.IsMagicImmune() || !CanBeCasted(FamiliarsStoneForm, Familiar)) || (StunDebuff || HexDebuff))
                        {
                            Attack(Familiar, Target);
                        }
                        else
                        {
                            Move(Familiar, Target.Position);
                        }
                    }
                    else
                    {
                        if (Config.FamiliarsFollowItem)
                        {
                            Move(Familiar, Game.MousePosition);
                        }
                        else
                        {
                            Follow(Familiar, Owner);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Config.Main.Log.Error(e);
            }
        }
    }
}
