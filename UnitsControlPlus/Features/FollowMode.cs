using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace UnitsControlPlus.Features
{
    internal class FollowMode : Extensions
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        private Unit Owner { get; }

        private TaskHandler Handler { get; }

        public FollowMode(Config config)
        {
            Config = config;
            Context = config.Main.Context;
            Owner = config.Main.Context.Owner;

            config.FollowKeyItem.PropertyChanged += FollowKeyChanged;

            if (config.FollowKeyItem)
            {
                config.FollowKeyItem.Item.SetValue(new KeyBind(
                    config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }

            Handler = UpdateManager.Run(ExecuteAsync, true, false);
        }

        public void Dispose()
        {
            if (Config.FollowKeyItem)
            {
                Handler?.Cancel();
            }

            Config.FollowKeyItem.PropertyChanged -= FollowKeyChanged;
        }

        private void FollowKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FollowKeyItem)
            {
                if (Config.ToggleKeyItem)
                {
                    Config.ToggleKeyItem.Item.SetValue(new KeyBind(
                        Config.ToggleKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }

                if (Config.PressKeyItem)
                {
                    Config.PressKeyItem.Item.SetValue(new KeyBind(
                        Config.PressKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Press, false));
                }

                Handler.RunAsync();
            }
            else
            {
                Handler?.Cancel();
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

                var Units =
                    EntityManager<Unit>.Entities.Where(
                                                       x =>
                                                       x.IsValid &&
                                                       x.IsAlive &&
                                                       x.IsControllable &&
                                                       Owner.IsAlly(x) &&
                                                       x != Owner &&
                                                       ((x is Hero) ||
                                                       x.IsIllusion ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Additive" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Creep" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Lane" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Siege" ||
                                                       x.NetworkName == "CDOTA_Unit_Hero_Beastmaster_Boar" ||
                                                       x.NetworkName == "CDOTA_Unit_SpiritBear" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Neutral" ||
                                                       x.NetworkName == "CDOTA_Unit_Broodmother_Spiderling" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Invoker_Forged_Spirit" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Warlock_Golem" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Tusk_Sigil" ||
                                                       x.NetworkName == "CDOTA_Unit_Elder_Titan_AncestralSpirit" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalEarth" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalStorm" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalFire"));

                foreach (var Unit in Units.ToArray())
                {
                    //Brewmaster Storm Wind Walk
                    var WindWalk = Unit.GetAbilityById(AbilityId.brewmaster_storm_wind_walk);
                    if (CanBeCasted(WindWalk, Unit)
                        && !WindWalk.IsInAbilityPhase
                        && !Unit.IsInvisible())
                    {
                        UseAbility(WindWalk, Unit);
                        await Await.Delay(GetDelay, token);
                    }

                    //Item Phase Boots
                    var PhaseBoots = Unit.GetItemById(AbilityId.item_phase_boots);
                    if (CanBeCasted(PhaseBoots, Unit)
                        && !PhaseBoots.IsInAbilityPhase)
                    {
                        UseAbility(PhaseBoots, Unit);
                        await Await.Delay(GetDelay, token);
                    }

                    // Hill Troll Priest Heal
                    var Heal = Unit.GetAbilityById(AbilityId.forest_troll_high_priest_heal);
                    if (Heal != null
                        && !Heal.IsAutoCastEnabled)
                    {
                        Heal.ToggleAutocastAbility();
                    }

                    // Ogre Frostmage Frost Armor
                    var FrostArmor = Unit.GetAbilityById(AbilityId.ogre_magi_frost_armor);
                    if (CanBeCasted(FrostArmor, Unit)
                        && !FrostArmor.IsInAbilityPhase)
                    {
                        var FrostArmorCast =
                            EntityManager<Unit>.Entities.OrderBy(x => Unit.Distance2D(x)).Where(
                                                                                                x => 
                                                                                                x.IsValid &&
                                                                                                x.IsAlive &&
                                                                                                Owner.IsAlly(x) &&
                                                                                                !x.HasModifier("modifier_ogre_magi_frost_armor"));

                        var AllyHero = FrostArmorCast.FirstOrDefault(x => Unit.Distance2D(x) < FrostArmor.CastRange && (x == Owner || (x is Hero)));
                        if (AllyHero != null
                            && CanHit(FrostArmor, Unit, AllyHero))
                        {
                            UseAbility(FrostArmor, Unit, AllyHero);
                            await Await.Delay(GetDelay, token);
                        }
                        else
                        {
                            var AllyUnit = 
                                FrostArmorCast.FirstOrDefault(
                                                              x => 
                                                              x.IsControllable &&
                                                              !x.IsMagicImmune() &&
                                                              !x.IsInvulnerable() &&
                                                              !x.HasModifier("modifier_ogre_magi_frost_armor"));

                            if (AllyUnit != null
                               && CanHit(FrostArmor, Unit, AllyUnit))
                            {
                                UseAbility(FrostArmor, Unit, AllyUnit);
                                await Await.Delay(GetDelay, token);
                            }
                        }
                    }

                    //Ancient Thunderhide Frenzy
                    var Frenzy = Unit.GetAbilityById(AbilityId.big_thunder_lizard_frenzy);
                    if (CanBeCasted(Frenzy, Unit)
                        && CanHit(Frenzy, Unit, Owner)
                        && !Frenzy.IsInAbilityPhase)
                    {
                        UseAbility(Frenzy, Unit, Owner);
                        await Await.Delay(GetDelay, token);
                    }

                    Follow(Unit, Owner);
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
