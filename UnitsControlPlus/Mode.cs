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

namespace UnitsControlPlus
{
    internal class Mode : Extensions
    {
        private Config Config { get; }

        private Unit Owner { get; }

        private TaskHandler Handler { get; }

        public Mode(Config config)
        {
            Config = config;
            Owner = config.Main.Context.Owner;

            config.PressKeyItem.PropertyChanged += PressKeyChanged;
            config.ToggleKeyItem.PropertyChanged += ToggleKeyChanged;

            if (config.ToggleKeyItem)
            {
                config.ToggleKeyItem.Item.SetValue(new KeyBind(
                    config.ToggleKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }

            Handler = UpdateManager.Run(ExecuteAsync, true, false);
        }

        public void Dispose()
        {
            if (Config.PressKeyItem)
            {
                Handler?.Cancel();
                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler?.Cancel();
                }
            }

            if (Config.PressKeyItem)
            {
                Handler?.Cancel();

                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler?.Cancel();
                }
            }

            Config.ToggleKeyItem.PropertyChanged -= ToggleKeyChanged;
            Config.PressKeyItem.PropertyChanged -= PressKeyChanged;
        }

        private void ToggleKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Handler.IsRunning)
            {
                Handler?.Cancel();

                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler?.Cancel();
                }
            }

            if (Config.ToggleKeyItem)
            {
                if (Config.FollowKeyItem)
                {
                    Config.FollowKeyItem.Item.SetValue(new KeyBind(
                        Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }

                Handler.RunAsync();

                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler.RunAsync();
                }
            }
        }

        private void PressKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.ToggleKeyItem)
            {
                return;
            }

            if (Config.PressKeyItem)
            {
                if (Config.FollowKeyItem)
                {
                    Config.FollowKeyItem.Item.SetValue(new KeyBind(
                        Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }

                Handler.RunAsync();

                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler.RunAsync();
                }
            }
            else
            {
                Handler?.Cancel();

                if (Config.ControlHeroesItem)
                {
                    Config.HeroControl.Handler?.Cancel();
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

                var Units =
                    EntityManager<Unit>.Entities.Where(
                                                       x =>
                                                       x.IsValid &&
                                                       x.IsAlive &&
                                                       x.IsControllable &&
                                                       Owner.IsAlly(x) &&
                                                       (x.IsIllusion ||
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
                                                       x.NetworkName == "CDOTA_NPC_WitchDoctor_Ward" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_Venomancer_PlagueWard" ||
                                                       x.NetworkName == "CDOTA_BaseNPC_ShadowShaman_SerpentWard" ||
                                                       x.NetworkName == "CDOTA_Unit_Elder_Titan_AncestralSpirit" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalEarth" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalStorm" ||
                                                       x.NetworkName == "CDOTA_Unit_Brewmaster_PrimalFire"));

                var Target = Config.UpdateMode.Target;

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
                                                                                                x => x.IsValid &&
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
                            var AllyUnit = FrostArmorCast.FirstOrDefault(
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

                    if (Target != null)
                    {
                        if (Unit.Distance2D(Target) > Config.RadiusTargetUnitsItem)
                        {
                            continue;
                        }

                        if (Unit.NetworkName == "CDOTA_BaseNPC_Venomancer_PlagueWard")
                        {
                            var attackRange = Unit.AttackRange - 20;
                            if (Unit.Distance2D(Target) < attackRange)
                            {
                                Attack(Unit, Target);
                            }

                            continue;
                        }

                        if (Unit.NetworkName == "CDOTA_BaseNPC_ShadowShaman_SerpentWard")
                        {
                            float attackRange = Unit.AttackRange - 50;
                            if (Owner.HasAghanimsScepter())
                            {
                                var SerpentWard = Owner.GetAbilityById(AbilityId.shadow_shaman_mass_serpent_ward);
                                attackRange += SerpentWard.GetAbilitySpecialData("scepter_range");
                            }

                            if (Unit.Distance2D(Target) < attackRange)
                            {
                                Attack(Unit, Target);
                            }

                            continue;
                        }

                        if (Unit.NetworkName == "CDOTA_NPC_WitchDoctor_Ward")
                        {
                            var attackRange = Unit.AttackRange - 50;
                            if (Unit.Distance2D(Target) < attackRange && !Unit.IsStunned())
                            {
                                Attack(Unit, Target);
                            }

                            continue;
                        }

                        if (!Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse") && !Unit.IsChanneling())
                        {
                            if (!Target.IsMagicImmune())
                            {
                                // Brewmaster Storm Drunken Haze
                                var DrunkenHaze = Unit.GetAbilityById(AbilityId.brewmaster_drunken_haze);
                                if (CanBeCasted(DrunkenHaze, Unit)
                                    && CanHit(DrunkenHaze, Unit, Target)
                                    && !DrunkenHaze.IsInAbilityPhase
                                    && !Unit.IsInvisible())
                                {
                                    UseAbility(DrunkenHaze, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Brewmaster Earth Hurl Boulde
                                var HurlBoulde = Unit.GetAbilityById(AbilityId.brewmaster_earth_hurl_boulder);
                                if (CanBeCasted(HurlBoulde, Unit)
                                    && CanHit(HurlBoulde, Unit, Target)
                                    && !HurlBoulde.IsInAbilityPhase
                                    && !Target.IsRooted()
                                    && !Target.IsStunned()
                                    && !Target.IsMuted())
                                {
                                    UseAbility(HurlBoulde, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Brewmaster Earth Thunder Clap
                                var EarthThunderClap = Unit.GetAbilityById(AbilityId.brewmaster_thunder_clap);
                                if (CanBeCasted(HurlBoulde, Unit)
                                    && Unit.Distance2D(Target) < EarthThunderClap.GetAbilitySpecialData("radius") - 100
                                    && !EarthThunderClap.IsInAbilityPhase)
                                {
                                    UseAbility(EarthThunderClap, Unit);
                                    await Await.Delay(GetDelay, token);
                                }

                                //Item Medallion Of Courage
                                var MedallionOfCourage = Unit.GetItemById(AbilityId.item_medallion_of_courage);
                                if (CanBeCasted(MedallionOfCourage, Unit)
                                    && CanHit(MedallionOfCourage, Unit, Target)
                                    && !MedallionOfCourage.IsInAbilityPhase)
                                {
                                    UseAbility(MedallionOfCourage, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                //Item Solar Crest
                                var SolarCrest = Unit.GetItemById(AbilityId.item_solar_crest);
                                if (CanBeCasted(SolarCrest, Unit)
                                    && CanHit(SolarCrest, Unit, Target)
                                    && !SolarCrest.IsInAbilityPhase)
                                {
                                    UseAbility(SolarCrest, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Necronomicon Mana Burn
                                var ManaBurn = Unit.GetAbilityById(AbilityId.necronomicon_archer_mana_burn);
                                if (CanBeCasted(ManaBurn, Unit)
                                    && CanHit(ManaBurn, Unit, Target)
                                    && !ManaBurn.IsInAbilityPhase)
                                {
                                    UseAbility(ManaBurn, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Centaur Conqueror War Stomp
                                var WarStomp = Unit.GetAbilityById(AbilityId.centaur_khan_war_stomp);
                                if (CanBeCasted(WarStomp, Unit)
                                    && Unit.Distance2D(Target) < 150
                                    && !WarStomp.IsInAbilityPhase
                                    && !Target.IsRooted()
                                    && !Target.IsStunned()
                                    && !Target.IsMuted())
                                {
                                    UseAbility(WarStomp, Unit);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Harpy Stormcrafter Chain Lightning
                                var ChainLightning = Unit.GetAbilityById(AbilityId.harpy_storm_chain_lightning);
                                if (CanBeCasted(ChainLightning, Unit)
                                    && CanHit(ChainLightning, Unit, Target)
                                    && !ChainLightning.IsInAbilityPhase)
                                {
                                    UseAbility(ChainLightning, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Mud Golem Hurl Boulder
                                var HurlBoulder = Unit.GetAbilityById(AbilityId.mud_golem_hurl_boulder);
                                if (CanBeCasted(HurlBoulder, Unit)
                                    && CanHit(HurlBoulder, Unit, Target)
                                    && !HurlBoulder.IsInAbilityPhase
                                    && !Target.IsRooted()
                                    && !Target.IsStunned()
                                    && !Target.IsMuted())
                                {
                                    UseAbility(HurlBoulder, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Hellbear Smasher Thunder Clap
                                var ThunderClap = Unit.GetAbilityById(AbilityId.polar_furbolg_ursa_warrior_thunder_clap);
                                if (CanBeCasted(ThunderClap, Unit)
                                    && Unit.Distance2D(Target) < ThunderClap.GetAbilitySpecialData("radius") - 100
                                    && !ThunderClap.IsInAbilityPhase)
                                {
                                    UseAbility(ThunderClap, Unit);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Satyr Banisher Purge
                                var Purge = Unit.GetAbilityById(AbilityId.satyr_trickster_purge);
                                if (CanBeCasted(Purge, Unit)
                                    && CanHit(Purge, Unit, Target)
                                    && !Purge.IsInAbilityPhase)
                                {
                                    UseAbility(Purge, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Satyr Mindstealer Mana Burn
                                var SatyrManaBurn = Unit.GetAbilityById(AbilityId.satyr_soulstealer_mana_burn);
                                if (CanBeCasted(SatyrManaBurn, Unit)
                                    && CanHit(SatyrManaBurn, Unit, Target)
                                    && !SatyrManaBurn.IsInAbilityPhase)
                                {
                                    UseAbility(SatyrManaBurn, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Satyr Tormenter Shockwave
                                var Shockwave = Unit.GetAbilityById(AbilityId.satyr_hellcaller_shockwave);
                                if (CanBeCasted(Shockwave, Unit)
                                    && CanHit(Shockwave, Unit, Target)
                                    && !Shockwave.IsInAbilityPhase)
                                {
                                    UseAbility(Shockwave, Unit, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Wildwing Ripper Tornado
                                var Tornado = Unit.GetAbilityById(AbilityId.enraged_wildkin_tornado);
                                if (CanBeCasted(Tornado, Unit)
                                    && CanHit(Tornado, Unit, Target)
                                    && !Tornado.IsInAbilityPhase)
                                {
                                    UseAbility(Tornado, Unit, Target.Position);
                                    await Await.Delay(GetDelay + 200, token);
                                }

                                // Ancient Black Dragon Fireball
                                var Fireball = Unit.GetAbilityById(AbilityId.black_dragon_fireball);
                                if (CanBeCasted(Fireball, Unit)
                                    && CanHit(Fireball, Unit, Target)
                                    && !Fireball.IsInAbilityPhase)
                                {
                                    UseAbility(Fireball, Unit, Target.Position);
                                    await Await.Delay(GetDelay, token);
                                }

                                // Ancient Prowler Shaman Stomp
                                var Stomp = Unit.GetAbilityById(AbilityId.spawnlord_master_stomp);
                                if (CanBeCasted(Stomp, Unit)
                                    && Unit.Distance2D(Target) < 150
                                    && !Stomp.IsInAbilityPhase)
                                {
                                    UseAbility(Stomp, Unit);
                                    await Await.Delay(GetDelay, token);
                                }
                            }

                            // Brewmaster Storm Dispel Magic
                            var DispelMagic = Unit.GetAbilityById(AbilityId.brewmaster_storm_dispel_magic);
                            if (CanBeCasted(DispelMagic, Unit)
                                && CanHit(DispelMagic, Unit, Target)
                                && !DispelMagic.IsInAbilityPhase
                                && !Unit.IsInvisible())
                            {
                                UseAbility(DispelMagic, Unit, Target.Position);
                                await Await.Delay(GetDelay, token);
                            }

                            //Item Abyssal Blade
                            var AbyssalBlade = Unit.GetItemById(AbilityId.item_abyssal_blade);
                            if (CanBeCasted(AbyssalBlade, Unit)
                                && CanHit(AbyssalBlade, Unit, Target)
                                && !AbyssalBlade.IsInAbilityPhase)
                            {
                                UseAbility(AbyssalBlade, Unit, Target);
                                await Await.Delay(GetDelay, token);
                            }

                            //Item Mjollnir
                            var Mjollnir = Unit.GetItemById(AbilityId.item_mjollnir);
                            if (CanBeCasted(Mjollnir, Unit)
                                && Unit.Distance2D(Target) < 500
                                && !Mjollnir.IsInAbilityPhase)
                            {
                                UseAbility(Mjollnir, Unit, Unit);
                                await Await.Delay(GetDelay, token);
                            }

                            //Item Mask Of Madness
                            var MaskOfMadness = Unit.GetItemById(AbilityId.item_mask_of_madness);
                            if (CanBeCasted(MaskOfMadness, Unit)
                                && Unit.Distance2D(Target) < 1000
                                && !MaskOfMadness.IsInAbilityPhase)
                            {
                                UseAbility(MaskOfMadness, Unit);
                                await Await.Delay(GetDelay, token);
                            }

                            // Dark Troll Summoner Ensnare
                            var Ensnare = Unit.GetAbilityById(AbilityId.dark_troll_warlord_ensnare);
                            if (CanBeCasted(Ensnare, Unit)
                                && CanHit(Ensnare, Unit, Target)
                                && !Ensnare.IsInAbilityPhase
                                && !Target.IsRooted()
                                && !Target.IsStunned()
                                && !Target.IsMuted())
                            {
                                UseAbility(Ensnare, Unit, Target);
                                await Await.Delay(GetDelay, token);
                            }
                            
                            // Dark Troll Summoner Raise Dead
                            var RaiseDead = Unit.GetAbilityById(AbilityId.dark_troll_warlord_raise_dead);
                            if (CanBeCasted(RaiseDead, Unit)
                                && !RaiseDead.IsInAbilityPhase)
                            {
                                var DeadUnit =
                                    EntityManager<Unit>.Entities.Any(x => x.IsValid && !x.IsAlive && x.Distance2D(Unit) < 460);

                                if (DeadUnit)
                                {
                                    UseAbility(RaiseDead, Unit);
                                    await Await.Delay(GetDelay, token);
                                }
                            }
                        }

                        if (Target.IsInvulnerable() || Target.IsAttackImmune() || !Unit.CanAttack())
                        {
                            Move(Unit, Target);
                        }
                        else
                        {
                            Attack(Unit, Target);
                        }
                    }
                    else
                    {
                        if (Unit.NetworkName == "CDOTA_BaseNPC_Venomancer_PlagueWard"
                            || Unit.NetworkName == "CDOTA_BaseNPC_ShadowShaman_SerpentWard"
                            || Unit.NetworkName == "CDOTA_NPC_WitchDoctor_Ward")
                        {
                            continue;
                        }

                        if (Unit.Distance2D(Owner) > Config.ControlWithoutTargetItem)
                        {
                            continue;
                        }

                        switch (Config.WithoutTargetItem.Value.SelectedIndex)
                        {
                            case 0:
                                Move(Unit, Game.MousePosition);
                                break;

                            case 1:
                                Follow(Unit, Owner);
                                break;
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
