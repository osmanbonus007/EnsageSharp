using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

namespace SkywrathMagePlus
{
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private SkywrathMagePlus Main { get; }

        private ITargetSelectorManager TargetSelector { get; }

        private IPredictionManager Prediction { get; }

        public Hero Target { get; set;}

        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
            Main = config.SkywrathMagePlus;
            TargetSelector = context.TargetSelector;
            Prediction = context.Prediction;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (Config.TargetItem.Value.SelectedValue.Contains("Lock") 
                && (Target == null || !Target.IsValid || !Target.IsAlive))
            {
                if (!TargetSelector.IsActive)
                {
                    TargetSelector.Activate();
                }

                if (TargetSelector.IsActive)
                {
                    Target = TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }

                if (Target != null)
                {
                    if (TargetSelector.IsActive)
                    {
                        TargetSelector.Deactivate();
                    }
                }
            }
            else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && TargetSelector.IsActive)
            {
                Target = TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (Target != null
                && (!Config.BladeMailItem || !Target.HasModifier("modifier_item_blade_mail_reflect")) 
                && !Config.Data.CancelCombo(Target))
            {
                var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var IsDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");

                if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !Config.Data.AntimageShield(Target))
                {
                    // Hex
                    if (Main.Hex != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Hex.Item.Name)
                        && Main.Hex.CanBeCasted
                        && Main.Hex.CanHit(Target)
                        && (IsStun == null || IsStun.RemainingTime <= 0.3))
                    {
                        Main.Hex.UseAbility(Target);
                        await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                    }

                    // Orchid
                    if (Main.Orchid != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                        && Main.Orchid.CanBeCasted
                        && Main.Orchid.CanHit(Target))
                    {
                        Main.Orchid.UseAbility(Target);
                        await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                    }

                    // Bloodthorn
                    if (Main.Bloodthorn != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                        && Main.Bloodthorn.CanBeCasted
                        && Main.Bloodthorn.CanHit(Target))
                    {
                        Main.Bloodthorn.UseAbility(Target);
                        await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                    }
                    
                    // MysticFlare
                    if (Main.MysticFlare != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.MysticFlare.Ability.Name)
                        && Main.MysticFlare.CanBeCasted
                        && Main.MysticFlare.CanHit(Target)
                        && Config.Data.Active(Target, IsStun))
                    {
                        var CheckHero = EntityManager<Hero>.Entities.Where(
                            x =>
                            !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Owner.Team &&
                            x.Distance2D(Owner) <= Main.MysticFlare.CastRange);

                        var UltimateScepter = Owner.HasAghanimsScepter();
                        var DubleMysticFlare = UltimateScepter && CheckHero.Count() == 1;

                        var Input =
                            new PredictionInput(
                                Owner,
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
                        && Config.ItemsToggler.Value.IsEnabled(Main.RodofAtos.Item.Name)
                        && Main.RodofAtos.CanBeCasted
                        && Main.RodofAtos.CanHit(Target)
                        && (IsStun == null || IsStun.RemainingTime <= 0.5)
                        && (IsDebuff == null || IsDebuff.RemainingTime <= 0.5))
                    {
                        Main.RodofAtos.UseAbility(Target);
                        await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                    }

                    // AncientSeal
                    if (Main.AncientSeal != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name)
                        && Main.AncientSeal.CanBeCasted
                        && Main.AncientSeal.CanHit(Target))
                    {
                        Main.AncientSeal.UseAbility(Target);
                        await Await.Delay(Main.AncientSeal.GetCastDelay(Target), token);
                    }

                    // ConcussiveShot
                    if (Main.ConcussiveShot != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.ConcussiveShot.Ability.Name)
                        && (!Config.WTargetItem
                        || (Target == Config.UpdateMode.WShow
                        || (Config.UpdateMode.WShow != null && Target.Distance2D(Config.UpdateMode.WShow) <= 250)))
                        && Main.ConcussiveShot.CanBeCasted
                        && Owner.Distance2D(Target.Position) <= Config.WRadiusItem + 25)
                    {
                        Main.ConcussiveShot.UseAbility();
                        await Await.Delay(Main.ConcussiveShot.GetCastDelay(), token);
                    }

                    // ArcaneBolt
                    if (Main.ArcaneBolt != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.ArcaneBolt.Ability.Name)
                        && Main.ArcaneBolt.CanBeCasted
                        && Main.ArcaneBolt.CanHit(Target))
                    {
                        Main.ArcaneBolt.UseAbility(Target);
                        await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
                    }

                    // Veil
                    if (Main.Veil != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Veil.Item.Name)
                        && Main.Veil.CanBeCasted
                        && Main.Veil.CanHit(Target))
                    {
                        Main.Veil.UseAbility(Target.Position);
                        await Await.Delay(Main.Veil.GetCastDelay(Target), token);
                    }

                    // Ethereal
                    if (Main.Ethereal != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)
                        && Main.Ethereal.CanBeCasted
                        && Main.Ethereal.CanHit(Target))
                    {
                        Main.Ethereal.UseAbility(Target);
                        await Await.Delay(Main.Ethereal.GetCastDelay(Target), token);
                    }

                    // Dagon
                    if (Main.Dagon != null
                        && Config.ItemsToggler.Value.IsEnabled("item_dagon_5")
                        && Main.Dagon.CanBeCasted
                        && Main.Dagon.CanHit(Target)
                        && (Main.AncientSeal == null || (Target.HasModifier("modifier_skywrath_mage_ancient_seal") && !Main.AncientSeal.CanBeCasted)
                        || !Config.AbilityToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name))
                        && (Main.Ethereal == null || (Target.IsEthereal() && !Main.Ethereal.CanBeCasted)
                        || !Config.ItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)))
                    {
                        Main.Dagon.UseAbility(Target);
                        await Await.Delay(Main.Dagon.GetCastDelay(Target), token);
                    }
                }
                else
                {
                    await Config.LinkenBreaker.Breaker(token, Target);
                }
                
                if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    if (!Orbwalker.Settings.Move)
                    {
                        Orbwalker.Settings.Move.Item.SetValue(true);
                    }

                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    if (Owner.Distance2D(Target) <= Config.MinDisInOrbwalk
                        && Target.Distance2D(Game.MousePosition) <= Config.MinDisInOrbwalk)
                    {
                        if (Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(false);
                        }

                        Orbwalker.OrbwalkTo(Target);
                    }
                    else
                    {
                        if (!Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(true);
                        }

                        Orbwalker.OrbwalkTo(Target);
                    }
                }
            }
            else
            {
                if (!Orbwalker.Settings.Move)
                {
                    Orbwalker.Settings.Move.Item.SetValue(true);
                }
                
                Orbwalker.Move(Game.MousePosition);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }
    }
}
