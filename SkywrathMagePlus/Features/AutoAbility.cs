using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;

namespace SkywrathMagePlus.Features
{
    internal class AutoAbility
    {
        private Config Config { get; }

        public SkywrathMagePlus Main { get; }

        private IServiceContext Context { get; }

        private IPredictionManager Prediction { get; }

        private TaskHandler Handler { get; }

        public AutoAbility(Config config, Mode mode, IServiceContext context)
        {
            Config = config;
            Main = config.SkywrathMagePlus;
            Context = context;

            Prediction = context.Prediction;
            Handler = UpdateManager.Run(ExecuteAsync, true, true);
        }

        public void Dispose()
        {
            Handler?.Cancel();
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                var Hero = EntityManager<Hero>.Entities.Where(
                    x => !x.IsIllusion &&
                    x.IsAlive &&
                    x.IsVisible &&
                    x.IsValid &&
                    x.Team != Context.Owner.Team).ToList();

                foreach (var Target in Hero)
                {
                    var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                    var IsDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");
                    if (ActiveAutoAbility(Target, IsStun) && !CancelAutoAbility(Target))
                    {
                        if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !AntimageShield(Target))
                        {
                            // Hex
                            if (Main.Hex != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Hex.Item.Name)
                                && Main.Hex.CanBeCasted
                                && (IsStun == null || IsStun.RemainingTime <= 0.3)
                                && Context.Owner.Distance2D(Target) <= Main.Hex.CastRange)
                            {
                                Main.Hex.UseAbility(Target);
                                await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                            }

                            // Orchid
                            if (Main.Orchid != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                                && Main.Orchid.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.Orchid.CastRange)
                            {
                                Main.Orchid.UseAbility(Target);
                                await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                            }

                            // Bloodthorn
                            if (Main.Bloodthorn != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                                && Main.Bloodthorn.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.Bloodthorn.CastRange)
                            {
                                Main.Bloodthorn.UseAbility(Target);
                                await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                            }

                            // MysticFlare
                            if (Main.MysticFlare != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.MysticFlare.Ability.Name)
                                && Main.MysticFlare.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.MysticFlare.CastRange + 100)
                            {
                                var CheckHero = EntityManager<Hero>.Entities.Where(
                                    x => !x.IsIllusion &&
                                    x.IsAlive &&
                                    x.IsVisible &&
                                    x.IsValid &&
                                    x.Team != Context.Owner.Team &&
                                    x.Distance2D(Context.Owner) <= Main.MysticFlare.CastRange);

                                var UltimateScepter = Context.Owner.GetItemById(AbilityId.item_ultimate_scepter) != null;
                                var DubleMysticFlare = UltimateScepter && CheckHero.Count() == 1;

                                var Input =
                                    new PredictionInput(
                                        Context.Owner,
                                        Target,
                                        0,
                                        float.MaxValue,
                                        Main.MysticFlare.CastRange,
                                        DubleMysticFlare ? -250 : -100,
                                        PredictionSkillshotType.SkillshotCircle,
                                        true)
                                    {
                                        CollisionTypes = CollisionTypes.None
                                    };

                                var Output = Prediction.GetPrediction(Input);

                                Main.MysticFlare.UseAbility(Output.CastPosition);
                                await Await.Delay(Main.MysticFlare.GetCastDelay(Target), token);
                            }

                            // RodofAtos
                            if (Main.RodofAtos != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.RodofAtos.Item.Name)
                                && Main.RodofAtos.CanBeCasted
                                && (IsStun == null || IsStun.RemainingTime <= 0.5)
                                && (IsDebuff == null || IsDebuff.RemainingTime <= 0.5)
                                && Context.Owner.Distance2D(Target) <= Main.RodofAtos.CastRange)
                            {
                                Main.RodofAtos.UseAbility(Target);
                                await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                            }

                            // AncientSeal
                            if (Main.AncientSeal != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name)
                                && Main.AncientSeal.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.AncientSeal.CastRange)
                            {
                                Main.AncientSeal.UseAbility(Target);
                                await Await.Delay(Main.AncientSeal.GetCastDelay(Target), token);
                            }

                            // ConcussiveShot
                            if (Main.ConcussiveShot != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.ConcussiveShot.Ability.Name)
                                && (!Config.WTargetItem
                                || (Target == Config.UpdateMode.WShow
                                || (Config.UpdateMode.WShow != null && Target.Distance2D(Config.UpdateMode.WShow) <= 250)))
                                && Main.ConcussiveShot.CanBeCasted
                                && Context.Owner.Distance2D(Target.Position) <= Config.WRadiusItem.Value + 25)
                            {
                                Main.ConcussiveShot.UseAbility();
                                await Await.Delay(Main.ConcussiveShot.GetCastDelay(), token);
                            }

                            // ArcaneBolt
                            if (Main.ArcaneBolt != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.ArcaneBolt.Ability.Name)
                                && Main.ArcaneBolt.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.ArcaneBolt.CastRange)
                            {
                                Main.ArcaneBolt.UseAbility(Target);
                                await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
                            }

                            // Veil
                            if (Main.Veil != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Veil.Item.Name)
                                && Main.Veil.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.Veil.CastRange)
                            {
                                Main.Veil.UseAbility(Target.Position);
                                await Await.Delay(Main.Veil.GetCastDelay(Target), token);
                            }

                            // Ethereal
                            if (Main.Ethereal != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)
                                && Main.Ethereal.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.Ethereal.CastRange)
                            {
                                Main.Ethereal.UseAbility(Target);
                                await Await.Delay(Main.Ethereal.GetCastDelay(Target), token);
                            }

                            // Dagon
                            if (Main.Dagon != null
                                && Config.AutoItemsToggler.Value.IsEnabled("item_dagon_5")
                                && Main.Dagon.CanBeCasted
                                && Context.Owner.Distance2D(Target) <= Main.Dagon.CastRange
                                && (Main.AncientSeal == null || (Target.HasModifier("modifier_skywrath_mage_ancient_seal") && !Main.AncientSeal.CanBeCasted)
                                || !Config.AutoAbilitiesToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name))
                                && (Main.Ethereal == null || (Target.IsEthereal() && !Main.Ethereal.CanBeCasted)
                                || !Config.AutoItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)))
                            {
                                Main.Dagon.UseAbility(Target);
                                await Await.Delay(Main.Dagon.GetCastDelay(Target), token);
                            }
                        }
                        else
                        {
                            await Config.LinkenBreaker.Breaker(token, Target);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private bool AntimageShield(Hero Target)
        {
            var Shield = Target.GetAbilityById(AbilityId.antimage_spell_shield);

            return Shield != null
                && Shield.Cooldown == 0
                && Shield.Level > 0
                && Target.GetItemById(AbilityId.item_ultimate_scepter) != null;
        }

        private bool ActiveAutoAbility(Hero Target, Modifier IsStun)
        {
            return Target.MovementSpeed < 240
                || (IsStun != null && IsStun.Duration >= 1)
                || Target.HasModifier("modifier_rod_of_atos_debuff");
        }

        private bool CancelAutoAbility(Hero Target)
        {
            return Target.HasModifier("modifier_eul_cyclone");
        }
    }
}
