using System;
using System.ComponentModel;
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
    internal class AutoCombo
    {
        private Config Config { get; }

        public Mode Mode { get; }

        public SkywrathMagePlus Main { get; }

        private IServiceContext Context { get; }

        private IPredictionManager Prediction { get; }

        private TaskHandler Handler { get; }

        public AutoCombo(Config config)
        {
            Config = config;
            Context = config.SkywrathMagePlus.Context;
            Main = config.SkywrathMagePlus;
            Mode = config.Mode;
            Prediction = config.SkywrathMagePlus.Context.Prediction;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.AutoComboItem)
            {
                Handler.RunAsync();
            }
            
            config.AutoComboItem.PropertyChanged += AutoComboChanged;
        }

        public void Dispose()
        {
            Config.AutoComboItem.PropertyChanged -= AutoComboChanged;

            if (Config.AutoComboItem)
            {
                Handler?.Cancel();
            }
        }

        private void AutoComboChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.AutoComboItem)
            {
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
                if (Config.ComboKeyItem)
                {
                    return;
                }

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
                    if (Config.Data.Active(Target, IsStun) && !Config.Data.CancelCombo(Target))
                    {
                        if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !Config.Data.AntimageShield(Target))
                        {
                            var QueenofPainBlink = Target.GetAbilityById(AbilityId.queenofpain_blink);
                            var AntiMageBlink = Target.GetAbilityById(AbilityId.antimage_blink);

                            // Hex
                            if (Main.Hex != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Hex.Item.Name)
                                && Main.Hex.CanBeCasted
                                && Main.Hex.CanHit(Target)
                                && (IsStun == null || IsStun.RemainingTime <= 0.3))
                            {
                                Main.Hex.UseAbility(Target);
                                await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                            }

                            // Orchid
                            if (Main.Orchid != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                                && Main.Orchid.CanBeCasted
                                && Main.Orchid.CanHit(Target))
                            {
                                Main.Orchid.UseAbility(Target);
                                await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                            }

                            // Bloodthorn
                            if (Main.Bloodthorn != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                                && Main.Bloodthorn.CanBeCasted
                                && Main.Bloodthorn.CanHit(Target))
                            {
                                Main.Bloodthorn.UseAbility(Target);
                                await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                            }

                            // AncientSeal
                            if (Main.AncientSeal != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name)
                                && Main.AncientSeal.CanBeCasted
                                && Main.AncientSeal.CanHit(Target))
                            {
                                Main.AncientSeal.UseAbility(Target);
                                await Await.Delay(Main.AncientSeal.GetCastDelay(Target), token);
                            }

                            // MysticFlare
                            if (Main.MysticFlare != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.MysticFlare.Ability.Name)
                                && Main.MysticFlare.CanBeCasted
                                && Main.MysticFlare.CanHit(Target))
                            {
                                var CheckHero = EntityManager<Hero>.Entities.Where(
                                    x => !x.IsIllusion &&
                                    x.IsAlive &&
                                    x.IsVisible &&
                                    x.IsValid &&
                                    x.Team != Context.Owner.Team &&
                                    x.Distance2D(Context.Owner) <= Main.MysticFlare.CastRange);

                                var UltimateScepter = Context.Owner.HasAghanimsScepter();
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
                                && Main.RodofAtos.CanHit(Target)
                                && (IsStun == null || IsStun.RemainingTime <= 0.5)
                                && (IsDebuff == null || IsDebuff.RemainingTime <= 0.5))
                            {
                                Main.RodofAtos.UseAbility(Target);
                                await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                            }

                            // ConcussiveShot
                            if (Main.ConcussiveShot != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.ConcussiveShot.Ability.Name)
                                && (!Config.WTargetItem
                                || (Target == Config.UpdateMode.WShow
                                || (Config.UpdateMode.WShow != null && Target.Distance2D(Config.UpdateMode.WShow) <= 250)))
                                && Main.ConcussiveShot.CanBeCasted
                                && Context.Owner.Distance2D(Target.Position) <= Config.WRadiusItem + 25)
                            {
                                Main.ConcussiveShot.UseAbility();
                                await Await.Delay(Main.ConcussiveShot.GetCastDelay(), token);
                            }

                            // ArcaneBolt
                            if (Main.ArcaneBolt != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.ArcaneBolt.Ability.Name)
                                && Main.ArcaneBolt.CanBeCasted
                                && Main.ArcaneBolt.CanHit(Target))
                            {
                                Main.ArcaneBolt.UseAbility(Target);
                                await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
                            }

                            // Veil
                            if (Main.Veil != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Veil.Item.Name)
                                && Main.Veil.CanBeCasted
                                && Main.Veil.CanHit(Target))
                            {
                                Main.Veil.UseAbility(Target.Position);
                                await Await.Delay(Main.Veil.GetCastDelay(Target), token);
                            }

                            // Ethereal
                            if (Main.Ethereal != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)
                                && Main.Ethereal.CanBeCasted
                                && Main.Ethereal.CanHit(Target))
                            {
                                Main.Ethereal.UseAbility(Target);
                                await Await.Delay(Main.Ethereal.GetCastDelay(Target), token);
                            }

                            // Dagon
                            if (Main.Dagon != null
                                && Config.AutoItemsToggler.Value.IsEnabled("item_dagon_5")
                                && Main.Dagon.CanBeCasted
                                && Main.Dagon.CanHit(Target)
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
