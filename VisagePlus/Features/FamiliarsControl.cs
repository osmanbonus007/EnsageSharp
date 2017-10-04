using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace VisagePlus.Features
{
    internal class FamiliarsControl : Extensions
    {
        private Config Config { get; }

        private Unit Owner { get; }

        private Sleeper FamiliarsSleeper { get; }

        private TaskHandler Handler { get; }

        public FamiliarsControl(Config config)
        {
            Config = config;
            Owner = config.Main.Context.Owner;

            FamiliarsSleeper = new Sleeper();

            config.FollowKeyItem.PropertyChanged += FollowKeyChanged;
            config.EscapeKeyItem.PropertyChanged += EscapeKeyChanged;

            if (config.FollowKeyItem)
            {
                config.FollowKeyItem.Item.SetValue(new KeyBind(
                    config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
                
            Handler = UpdateManager.Run(ExecuteAsync, true, true);
        }

        public void Dispose()
        {
            Handler?.Cancel();

            Config.EscapeKeyItem.PropertyChanged -= EscapeKeyChanged;
            Config.FollowKeyItem.PropertyChanged -= FollowKeyChanged;
        }

        private void FollowKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FollowKeyItem)
            {
                Config.LastHitItem.Item.SetValue(new KeyBind(
                    Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        private void EscapeKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.EscapeKeyItem)
            {
                Config.LastHitItem.Item.SetValue(new KeyBind(
                    Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
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
                                                       x.IsAlly(Owner) &&
                                                       x.NetworkName == "CDOTA_Unit_VisageFamiliar").ToList();

                var Others =
                    EntityManager<Unit>.Entities.Where(x => 
                                                       !x.IsIllusion &&
                                                       x.IsValid &&
                                                       x.IsVisible &&
                                                       x.IsAlive &&
                                                       x.IsEnemy(Owner)).ToList();

                foreach (var Familiar in Familiars)
                {
                    var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);

                    // Auto Stone Form
                    if (Familiar.Health * 100 / Familiar.MaximumHealth <= Config.FamiliarsLowHPItem && CanBeCasted(FamiliarsStoneForm, Familiar))
                    {
                        FamiliarsStoneForm.UseAbility();
                        await Await.Delay(GetDelay, token);
                    }

                    // Follow
                    if (Config.FollowKeyItem)
                    {
                        Follow(Familiar, Owner);
                    }

                    // Courier
                    if (Config.FamiliarsCourierItem)
                    {
                        var courier = Others.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(x => x.NetworkName == "CDOTA_Unit_Courier");

                        if (courier != null && Familiar.Distance2D(courier) <= 600 && !Config.FollowKeyItem)
                        {
                            Attack(Familiar, courier);
                        }
                    }

                    // Escape
                    if (Config.EscapeKeyItem 
                        && !Config.ComboKeyItem 
                        && !Config.FamiliarsLockItem
                        && !Config.LastHitItem
                        && !Config.FollowKeyItem)
                    {
                        var hero = Others.OrderBy(x => x.Distance2D(Owner)).FirstOrDefault(x => x is Hero);

                        if (hero == null 
                            || hero.IsMagicImmune() || hero.IsInvulnerable() || hero.HasModifier("modifier_winter_wyvern_winters_curse")
                            || Owner.Distance2D(hero) > 800)
                        {
                            Follow(Familiar, Owner);

                            continue;
                        }

                        if (CanBeCasted(FamiliarsStoneForm, Familiar)
                            && Familiar.Distance2D(hero) <= 100
                            && !FamiliarsSleeper.Sleeping)
                        {
                            UseAbility(FamiliarsStoneForm, Familiar);
                            FamiliarsSleeper.Sleep(FamiliarsStoneForm.GetAbilitySpecialData("stun_duration") * 1000 - 200);
                            await Await.Delay(GetDelay, token);
                        }
                        else if (CanBeCasted(FamiliarsStoneForm, Familiar))
                        {
                            Move(Familiar, hero.Position);
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