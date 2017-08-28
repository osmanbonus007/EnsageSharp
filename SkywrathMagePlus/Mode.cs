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
                && (!Config.BladeMailItem.Value || !Target.HasModifier("modifier_item_blade_mail_reflect")) 
                && !CancelCombo())
            {
                var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var IsDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");
                if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !AntimageShield(Target))
                {
                    // Hex
                    if (Main.Hex != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Hex.Item.Name)
                        && Main.Hex.CanBeCasted
                        && (IsStun == null || IsStun.RemainingTime <= 0.3))
                    {
                        Main.Hex.UseAbility(Target);
                        await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                    }

                    // Orchid
                    if (Main.Orchid != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                        && Main.Orchid.CanBeCasted)
                    {
                        Main.Orchid.UseAbility(Target);
                        await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                    }

                    // Bloodthorn
                    if (Main.Bloodthorn != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                        && Main.Bloodthorn.CanBeCasted)
                    {
                        Main.Bloodthorn.UseAbility(Target);
                        await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                    }

                    // MysticFlare
                    if (Main.MysticFlare != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.MysticFlare.Ability.Name)
                        && Main.MysticFlare.CanBeCasted 
                        && ActiveMysticFlare(IsStun))
                    {
                        var CheckHero = EntityManager<Hero>.Entities.Where(
                            x =>
                            !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Owner.Team &&
                            x.Distance2D(Owner) <= Main.MysticFlare.CastRange);

                        var UltimateScepter = Owner.GetItemById(AbilityId.item_ultimate_scepter) != null;
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
                        && (IsStun == null || IsStun.RemainingTime <= 0.5)
                        && (IsDebuff == null || IsDebuff.RemainingTime <= 0.5))
                    {
                        Main.RodofAtos.UseAbility(Target);
                        await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                    }

                    // AncientSeal
                    if (Main.AncientSeal != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name)
                        && Main.AncientSeal.CanBeCasted)
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
                        && Owner.Distance2D(Target.Position) <= Config.WRadiusItem.Value + 25)
                    {
                        Main.ConcussiveShot.UseAbility();
                        await Await.Delay(Main.ConcussiveShot.GetCastDelay(), token);
                    }

                    // ArcaneBolt
                    if (Main.ArcaneBolt != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.ArcaneBolt.Ability.Name)
                        && Main.ArcaneBolt.CanBeCasted)
                    {
                        Main.ArcaneBolt.UseAbility(Target);
                        await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
                    }

                    // Veil
                    if (Main.Veil != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Veil.Item.Name)
                        && Main.Veil.CanBeCasted)
                    {
                        Main.Veil.UseAbility(Target.Position);
                        await Await.Delay(Main.Veil.GetCastDelay(Target), token);
                    }

                    // Ethereal
                    if (Main.Ethereal != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)
                        && Main.Ethereal.CanBeCasted)
                    {
                        Main.Ethereal.UseAbility(Target);
                        await Await.Delay(Main.Ethereal.GetCastDelay(Target), token);
                    }

                    // Dagon
                    if (Main.Dagon != null
                        && Config.ItemsToggler.Value.IsEnabled("item_dagon_5")
                        && Main.Dagon.CanBeCasted
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
                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    Orbwalker.OrbwalkTo(Target);
                }
            }
            else
            {
                Orbwalker.Move(Game.MousePosition);
            }
        }

        public bool AntimageShield(Hero Target)
        {
            var Shield = Target.GetAbilityById(AbilityId.antimage_spell_shield);

            return Shield != null 
                && Shield.Cooldown == 0
                && Shield.Level > 0
                && Target.GetItemById(AbilityId.item_ultimate_scepter) != null;
        }

        private bool ActiveMysticFlare(Modifier IsStun)
        {
            return Target.MovementSpeed < 240
                || (IsStun != null && IsStun.Duration >= 1)
                || Target.HasModifier("modifier_rod_of_atos_debuff");
        }

        private bool CancelCombo()
        {
            return Target.HasModifier("modifier_eul_cyclone");
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
