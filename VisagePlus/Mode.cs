using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

using Ensage.Common.Menu;
using SharpDX;
using System;

namespace VisagePlus
{
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private ITargetSelectorManager TargetSelector { get; }

        public Hero Target { get; set;}

        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
            Main = config.VisagePlus;

            TargetSelector = context.TargetSelector;
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

            Config.FollowKeyItem.Item.SetValue(new KeyBind(
                Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            Config.LastHitItem.Item.SetValue(new KeyBind(
                Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

            Config.FamiliarsCombo.Familiars(Target);

            if (Target != null
                && (!Config.BladeMailItem.Value || !Target.HasModifier("modifier_item_blade_mail_reflect")))
            {
                var StunDebuff = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var HexDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_sheepstick_debuff");
                var AtosDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");

                if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !AntimageShield(Target))
                {
                    // Hex
                    if (Main.Hex != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Hex.Item.Name)
                        && Main.Hex.CanBeCasted
                        && Main.Hex.CanHit(Target)
                        && (StunDebuff == null || StunDebuff.RemainingTime <= 0.3)
                        && (HexDebuff == null || HexDebuff.RemainingTime <= 0.3))
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

                    // RodofAtos
                    if (Main.RodofAtos != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.RodofAtos.Item.Name)
                        && Main.RodofAtos.CanBeCasted
                        && Main.RodofAtos.CanHit(Target)
                        && (StunDebuff == null || StunDebuff.RemainingTime <= 0.5)
                        && (AtosDebuff == null || AtosDebuff.RemainingTime <= 0.5))
                    {
                        Main.RodofAtos.UseAbility(Target);
                        await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                    }

                    // SoulAssumption
                    if (Main.SoulAssumption != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.SoulAssumption.Ability.Name)
                        && Main.SoulAssumption.CanBeCasted
                        && Main.SoulAssumption.CanHit(Target)
                        && Main.SoulAssumption.MaxCharges)
                    {
                        Main.SoulAssumption.UseAbility(Target);
                        await Await.Delay(Main.SoulAssumption.GetCastDelay(Target), token);
                    }

                    // GraveChill
                    if (Main.GraveChill != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.GraveChill.Ability.Name)
                        && Main.GraveChill.CanBeCasted
                        && Main.GraveChill.CanHit(Target))
                    {
                        Main.GraveChill.UseAbility(Target);
                        await Await.Delay(Main.GraveChill.GetCastDelay(Target), token);
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

                    // Medallion
                    if (Main.Medallion != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Medallion.Item.Name)
                        && Main.Medallion.CanBeCasted
                        && Main.Medallion.CanHit(Target))
                    {
                        Main.Medallion.UseAbility(Target);
                        await Await.Delay(Main.Medallion.GetCastDelay(Target), token);
                    }

                    // SolarCrest
                    if (Main.SolarCrest != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.SolarCrest.Item.Name)
                        && Main.SolarCrest.CanBeCasted
                        && Main.SolarCrest.CanHit(Target))
                    {
                        Main.SolarCrest.UseAbility(Target);
                        await Await.Delay(Main.SolarCrest.GetCastDelay(Target), token);
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
                        && (Main.Ethereal == null || (Target.IsEthereal() && !Main.Ethereal.CanBeCasted)
                        || !Config.ItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)))
                    {
                        Main.Dagon.UseAbility(Target);
                        await Await.Delay(Main.Dagon.GetCastDelay(Target), token);
                    }

                    // Necronomicon
                    if (Main.Necronomicon != null
                        && Config.ItemsToggler.Value.IsEnabled("item_necronomicon_3")
                        && Main.Necronomicon.CanBeCasted
                        && Owner.Distance2D(Target) <= Owner.AttackRange)
                    {
                        Main.Necronomicon.UseAbility();
                        await Await.Delay(Main.Necronomicon.GetCastDelay(), token);
                    }

                    // Armlet
                    if (Main.Armlet != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Armlet.Item.Name)
                        && !Main.Armlet.Enabled
                        && Owner.Distance2D(Target) <= Owner.AttackRange)
                    {
                        Main.Armlet.UseAbility();
                        await Await.Delay(Main.Armlet.GetCastDelay(), token);
                    }
                }
                else
                {
                    await Config.LinkenBreaker.Breaker(token, Target);
                }
                
                if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    Orbwalker.Settings.Move.Item.SetValue(true);
                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    if (Owner.Distance2D(Target) <= Config.MinDisInOrbwalk.Value
                        && Target.Distance2D(Game.MousePosition) <= Config.MinDisInOrbwalk.Value)
                    {
                        Orbwalker.Settings.Move.Item.SetValue(false);
                        Orbwalker.OrbwalkTo(Target);
                    }
                    else
                    {
                        Orbwalker.Settings.Move.Item.SetValue(true);
                        Orbwalker.OrbwalkTo(Target);
                    }
                }
            }
            else
            {
                Orbwalker.Settings.Move.Item.SetValue(true);
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
