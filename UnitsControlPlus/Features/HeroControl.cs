using System;
using System.Collections.Generic;
using System.Globalization;
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

using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace UnitsControlPlus.Features
{
    internal class HeroControl : Extensions
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        private Unit Owner { get; }

        public TaskHandler Handler { get; }

        private List<AbilityToggler> AbilityToggler { get; } = new List<AbilityToggler>();

        private List<MenuItem> CastRange { get; } = new List<MenuItem>();

        public HeroControl(Config config)
        {
            Config = config;
            Context = config.Main.Context;
            Owner = config.Main.Context.Owner;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            var Heroes =
                EntityManager<Hero>.Entities.Where(
                    x => !x.IsIllusion &&
                    x.IsValid &&
                    x.IsAlive &&
                    x != Owner &&
                    Owner.IsAlly(x));

            foreach (var Hero in Heroes.ToList())
            {
                var ControllableMenu = Config.ControllableMenu;
                var HeroMenu = ControllableMenu.MenuWithTexture(Hero.GetDisplayName(), Hero.NetworkName, Hero.Name);

                var Abilities = Hero.Spellbook.Spells.Where(
                                                            x =>
                                                            !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive) &&
                                                            !x.AbilityBehavior.HasFlag(AbilityBehavior.Hidden));

                Dictionary<string, bool> Toggler = new Dictionary<string, bool>();

                foreach (var Ability in Abilities.ToList())
                {
                    Toggler.Add(Ability.Name, true);
                }

                var HeroAbilityToggler = HeroMenu.Item("Use: ", new AbilityToggler(Toggler));
                AbilityToggler.Add(HeroAbilityToggler.Value);

                HeroMenu.Target.AddItem(new MenuItem("RangeText", "Cast Range:"));

                foreach (var Ability in Abilities.ToList())
                {
                    var RemoveName = Hero.Name.Substring("npc_dota_hero_".Length);
                    var AbilityName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Ability.Name.Replace(RemoveName, "").Replace("_", " "));

                    var GetCastRange = Math.Min((int)AbilityExtensions.GetCastRange(Ability), 5000);
                    if (GetCastRange == 0)
                    {
                        GetCastRange = 600;
                    }

                    if (GetCastRange == 5000)
                    {
                        GetCastRange = 600;
                    }

                    var CustomCastRange = HeroMenu.Target.AddItem(new MenuItem(Ability.Name, AbilityName).SetValue(new Slider(GetCastRange, 0, 5000)));
                    CastRange.Add(CustomCastRange);
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

                var Heroes = EntityManager<Hero>.Entities.Where(
                                                                x =>
                                                                !x.IsIllusion &&
                                                                x.IsValid &&
                                                                x.IsAlive &&
                                                                x.IsControllable &&
                                                                x != Owner &&
                                                                Owner.IsAlly(x));


                var Target = Config.UpdateMode.Target;

                foreach (var Hero in Heroes.ToList())
                {
                    //Item Phase Boots
                    var PhaseBoots = Hero.GetItemById(AbilityId.item_phase_boots);
                    if (CanBeCasted(PhaseBoots, Hero)
                        && !PhaseBoots.IsInAbilityPhase)
                    {
                        UseAbility(PhaseBoots, Hero);
                        await Await.Delay(GetDelay, token);
                    }

                    if (Target != null)
                    {
                        if (Hero.Distance2D(Target) > Config.RadiusTargetUnitsItem)
                        {
                            continue;
                        }

                        if (!Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse") && !Hero.IsChanneling())
                        {
                            if (!Target.IsMagicImmune())
                            {
                                var Abilities = Hero.Spellbook.Spells.Where(
                                                                            x =>
                                                                            !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive) &&
                                                                            !x.AbilityBehavior.HasFlag(AbilityBehavior.Hidden));

                                foreach (var Ability in Abilities.ToList())
                                {
                                    // Spell Q, W, E, R, D
                                    if (Ability != null
                                        && AbilityToggler.Any(x => x.IsEnabled(Ability.Name))
                                        && CanBeCasted(Ability, Hero)
                                        && !Ability.IsInAbilityPhase
                                        && Hero.Distance2D(Target) < CastRange.FirstOrDefault(x => x.Name == Ability.Name).GetValue<Slider>())
                                    {
                                        if (Ability.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                                        {
                                            UseAbility(Ability, Hero, Target);
                                            await Await.Delay(GetDelay, token);
                                        }
                                        else if (Ability.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                                        {
                                            UseAbility(Ability, Hero, Target.Position);
                                            await Await.Delay(GetDelay, token);
                                        }
                                        else if (Ability.AbilityBehavior.HasFlag(AbilityBehavior.Toggle))
                                        {
                                            if (Ability.IsToggled)
                                            {
                                                continue;
                                            }

                                            Ability.ToggleAbility();
                                            await Await.Delay(GetDelay, token);
                                        }
                                        else if (Ability.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                                        {
                                            UseAbility(Ability, Hero);
                                            await Await.Delay(GetDelay, token);
                                        }
                                    }
                                }

                                //Item Medallion Of Courage
                                var MedallionOfCourage = Hero.GetItemById(AbilityId.item_medallion_of_courage);
                                if (CanBeCasted(MedallionOfCourage, Hero)
                                    && CanHit(MedallionOfCourage, Hero, Target)
                                    && !MedallionOfCourage.IsInAbilityPhase)
                                {
                                    UseAbility(MedallionOfCourage, Hero, Target);
                                    await Await.Delay(GetDelay, token);
                                }

                                //Item Solar Crest
                                var SolarCrest = Hero.GetItemById(AbilityId.item_solar_crest);
                                if (CanBeCasted(SolarCrest, Hero)
                                    && CanHit(SolarCrest, Hero, Target)
                                    && !SolarCrest.IsInAbilityPhase)
                                {
                                    UseAbility(SolarCrest, Hero, Target);
                                    await Await.Delay(GetDelay, token);
                                }
                            }

                            //Item Abyssal Blade
                            var AbyssalBlade = Hero.GetItemById(AbilityId.item_abyssal_blade);
                            if (CanBeCasted(AbyssalBlade, Hero)
                                && CanHit(AbyssalBlade, Hero, Target)
                                && !AbyssalBlade.IsInAbilityPhase)
                            {
                                UseAbility(AbyssalBlade, Hero, Target);
                                await Await.Delay(GetDelay, token);
                            }

                            //Item Mjollnir
                            var Mjollnir = Hero.GetItemById(AbilityId.item_mjollnir);
                            if (CanBeCasted(Mjollnir, Hero)
                                && Hero.Distance2D(Target) < 500
                                && !Mjollnir.IsInAbilityPhase)
                            {
                                UseAbility(Mjollnir, Hero, Hero);
                                await Await.Delay(GetDelay, token);
                            }

                            //Item Mask Of Madness
                            var MaskOfMadness = Hero.GetItemById(AbilityId.item_mask_of_madness);
                            if (CanBeCasted(MaskOfMadness, Hero)
                                && Hero.Distance2D(Target) < 1000
                                && !MaskOfMadness.IsInAbilityPhase)
                            {
                                UseAbility(MaskOfMadness, Hero);
                                await Await.Delay(GetDelay, token);
                            }
                        }

                        if (Target.IsInvulnerable() || Target.IsAttackImmune() || !Hero.CanAttack())
                        {
                            Move(Hero, Target);
                        }
                        else
                        {
                            Attack(Hero, Target);
                        }
                    }
                    else
                    {
                        if (Hero.Distance2D(Owner) > 2000)
                        {
                            continue;
                        }

                        switch (Config.WithoutTargetItem.Value.SelectedIndex)
                        {
                            case 0:
                                Move(Hero, Game.MousePosition);
                                break;

                            case 1:
                                Follow(Hero, Owner);
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
