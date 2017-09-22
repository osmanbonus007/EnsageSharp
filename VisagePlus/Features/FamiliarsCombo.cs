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
using Ensage.SDK.Service;

using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace VisagePlus.Features
{
    internal class FamiliarsCombo
    {
        private Config Config { get; }

        private Data Data { get; }

        private VisagePlus Main { get; }

        private IServiceContext Context { get; }

        private Sleeper FamiliarsSleeper { get; }

        private TaskHandler Handler { get; }

        private Hero Target { get; set; }

        public FamiliarsCombo(Config config)
        {
            Config = config;
            Data = config.Data;
            Main = config.VisagePlus;
            Context = config.VisagePlus.Context;

            FamiliarsSleeper = new Sleeper();

            config.ComboKeyItem.PropertyChanged += ComboChanged;
            config.FamiliarsLockItem.PropertyChanged += FamiliarsLockChanged;

            if (Config.FamiliarsLockItem)
            {
                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
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
                    EntityManager<Unit>.Entities.Where(
                        x =>
                        x.IsValid &&
                        x.IsAlive &&
                        x.IsControllable &&
                        Context.Owner.IsAlly(x) &&
                        (x.NetworkName == "CDOTA_Unit_VisageFamiliar" ||
                        x.Name.Contains("npc_dota_necronomicon_warrior") ||
                        x.Name.Contains("npc_dota_necronomicon_archer"))).ToArray();

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
                        var StunDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.IsStunDebuff);
                        var HexDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name == "modifier_sheepstick_debuff");
                        var FamiliarDamage = Familiar.Modifiers.FirstOrDefault(x => x.IsValid && x.Name == "modifier_visage_summon_familiars_damage_charge");
                        var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);
                        var ManaBurn = Familiar.GetAbilityById(AbilityId.necronomicon_archer_mana_burn);

                        if (!Target.IsMagicImmune() && !Data.CancelCombo(Target))
                        {
                            // FamiliarsStoneForm
                            if (FamiliarsStoneForm != null &&
                                Config.AbilityToggler.Value.IsEnabled(FamiliarsStoneForm.Name)
                                && AbilityExtensions.CanBeCasted(FamiliarsStoneForm)
                                && Familiar.Distance2D(Target) <= 100
                                && (StunDebuff == null || StunDebuff.RemainingTime <= 0.5)
                                && (HexDebuff == null || HexDebuff.RemainingTime <= 0.5)
                                && (!Config.FamiliarsStoneControlItem
                                || Data.SmartStone(Target) || FamiliarDamage.StackCount <= Config.FamiliarsChargeItem)
                                && !FamiliarsSleeper.Sleeping)
                            {
                                FamiliarsStoneForm.UseAbility();
                                FamiliarsSleeper.Sleep(FamiliarsStoneForm.GetAbilitySpecialData("stun_duration") * 1000 - 200);
                                await Await.Delay(100 + (int)Game.Ping, token);
                            }

                            // Necronomicon Mana Burn
                            if (ManaBurn != null
                                && AbilityExtensions.CanBeCasted(ManaBurn))
                            {
                                ManaBurn.UseAbility(Target);
                                await Await.Delay(100, token);
                            }
                        }

                        if (Target != null && (Target.IsInvulnerable() || Target.IsAttackImmune()))
                        {
                            Familiar.Move(Target.Position);
                            await Await.Delay(100, token);
                        }
                        else

                        if (Target != null 
                            && Config.FamiliarsStoneControlItem
                            && (FamiliarDamage != null && FamiliarDamage.StackCount > Config.FamiliarsChargeItem)
                            && !Data.SmartStone(Target))
                        {
                            Familiar.Attack(Target);
                            await Await.Delay(100, token);
                        }
                        else

                        if (Target != null && (Target.IsMagicImmune()
                            || !AbilityExtensions.CanBeCasted(FamiliarsStoneForm))
                            || ((StunDebuff != null && StunDebuff.RemainingTime >= 0.5)
                            || (HexDebuff != null && HexDebuff.RemainingTime >= 0.5)))
                        {
                            Familiar.Attack(Target);
                            await Await.Delay(100, token);
                        }
                        else
                        {
                            Familiar.Move(Target.Position);
                            await Await.Delay(100, token);
                        }
                    }
                    else
                    {
                        if (Config.FamiliarsFollowItem)
                        {
                            Familiar.Move(Game.MousePosition);
                            await Await.Delay(100, token);
                        }
                        else
                        {
                            Familiar.Follow(Context.Owner);
                            await Await.Delay(100, token);
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
                Main.Log.Error(e);
            }
        }
    }
}
