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

using PlaySharp.Toolkit.Helper.Annotations;

namespace VisagePlus
{
    [PublicAPI]
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private ITargetSelectorManager TargetSelector { get; }

        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
            Main = config.Main;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var Target = Config.UpdateMode.Target;

            if (Target != null && (!Config.BladeMailItem || !Target.HasModifier("modifier_item_blade_mail_reflect")))
            {
                var StunDebuff = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var HexDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name =="modifier_sheepstick_debuff");
                var AtosDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name == "modifier_rod_of_atos_debuff");

                if (!Target.IsMagicImmune() && !Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse"))
                {
                    if (!Target.IsLinkensProtected() && !Config.LinkenBreaker.AntimageShield(Target))
                    {
                        // Hex
                        var Hex = Main.Hex;
                        if (Hex != null
                            && Config.ItemsToggler.Value.IsEnabled(Hex.ToString())
                            && Hex.CanBeCasted
                            && Hex.CanHit(Target)
                            && (StunDebuff == null || StunDebuff.RemainingTime <= 0.3)
                            && (HexDebuff == null || HexDebuff.RemainingTime <= 0.3))
                        {
                            Hex.UseAbility(Target);
                            await Await.Delay(Hex.GetCastDelay(Target), token);
                        }

                        // Orchid
                        var Orchid = Main.Orchid;
                        if (Orchid != null
                            && Config.ItemsToggler.Value.IsEnabled(Orchid.ToString())
                            && Orchid.CanBeCasted
                            && Orchid.CanHit(Target))
                        {
                            Main.Orchid.UseAbility(Target);
                            await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                        }

                        // Bloodthorn
                        var Bloodthorn = Main.Bloodthorn;
                        if (Bloodthorn != null
                            && Config.ItemsToggler.Value.IsEnabled(Bloodthorn.ToString())
                            && Bloodthorn.CanBeCasted
                            && Bloodthorn.CanHit(Target))
                        {
                            Bloodthorn.UseAbility(Target);
                            await Await.Delay(Bloodthorn.GetCastDelay(Target), token);
                        }

                        // RodofAtos
                        var RodofAtos = Main.RodofAtos;
                        if (RodofAtos != null
                            && Config.ItemsToggler.Value.IsEnabled(RodofAtos.ToString())
                            && RodofAtos.CanBeCasted
                            && RodofAtos.CanHit(Target)
                            && (StunDebuff == null || StunDebuff.RemainingTime <= 0.5)
                            && (AtosDebuff == null || AtosDebuff.RemainingTime <= 0.5))
                        {
                            RodofAtos.UseAbility(Target);
                            await Await.Delay(RodofAtos.GetCastDelay(Target), token);
                        }


                        // SoulAssumption
                        var SoulAssumption = Main.SoulAssumption;
                        if (Config.AbilityToggler.Value.IsEnabled(SoulAssumption.ToString())
                            && SoulAssumption.CanBeCasted
                            && SoulAssumption.CanHit(Target)
                            && SoulAssumption.MaxCharges)
                        {
                            SoulAssumption.UseAbility(Target);
                            await Await.Delay(SoulAssumption.GetCastDelay(Target), token);
                        }

                        // GraveChill
                        var GraveChill = Main.GraveChill;
                        if (Config.AbilityToggler.Value.IsEnabled(GraveChill.ToString())
                            && GraveChill.CanBeCasted
                            && GraveChill.CanHit(Target))
                        {
                            GraveChill.UseAbility(Target);
                            await Await.Delay(GraveChill.GetCastDelay(Target), token);
                        }

                        // HurricanePike
                        var HurricanePike = Main.HurricanePike;
                        if (HurricanePike != null
                            && Config.ItemsToggler.Value.IsEnabled(HurricanePike.ToString())
                            && HurricanePike.CanBeCasted
                            && Owner.Distance2D(Target) < 400)
                        {
                            HurricanePike.UseAbility(Target);
                            await Await.Delay(HurricanePike.GetCastDelay(Target), token);
                        }

                        // HeavensHalberd
                        var HeavensHalberd = Main.HeavensHalberd;
                        if (HeavensHalberd != null
                            && Config.ItemsToggler.Value.IsEnabled(HeavensHalberd.ToString())
                            && HeavensHalberd.CanBeCasted
                            && HeavensHalberd.CanHit(Target))
                        {
                            HeavensHalberd.UseAbility(Target);
                            await Await.Delay(HeavensHalberd.GetCastDelay(Target), token);
                        }

                        // Veil
                        var Veil = Main.Veil;
                        if (Veil != null
                            && Config.ItemsToggler.Value.IsEnabled(Veil.ToString())
                            && Veil.CanBeCasted
                            && Veil.CanHit(Target))
                        {
                            Veil.UseAbility(Target.Position);
                            await Await.Delay(Veil.GetCastDelay(Target.Position), token);
                        }

                        // Medallion
                        var Medallion = Main.Medallion;
                        if (Medallion != null
                            && Config.ItemsToggler.Value.IsEnabled(Medallion.ToString())
                            && Medallion.CanBeCasted
                            && Medallion.CanHit(Target))
                        {
                            Medallion.UseAbility(Target);
                            await Await.Delay(Medallion.GetCastDelay(Target), token);
                        }

                        // SolarCrest
                        var SolarCrest = Main.SolarCrest;
                        if (SolarCrest != null
                            && Config.ItemsToggler.Value.IsEnabled(SolarCrest.ToString())
                            && SolarCrest.CanBeCasted
                            && SolarCrest.CanHit(Target))
                        {
                            SolarCrest.UseAbility(Target);
                            await Await.Delay(SolarCrest.GetCastDelay(Target), token);
                        }

                        // Shivas
                        var Shivas = Main.Shivas;
                        if (Shivas != null
                            && Config.ItemsToggler.Value.IsEnabled(Shivas.ToString())
                            && Shivas.CanBeCasted
                            && Owner.Distance2D(Target) <= Shivas.Radius)
                        {
                            Shivas.UseAbility();
                            await Await.Delay(Shivas.GetCastDelay(), token);
                        }

                        // Ethereal
                        var Ethereal = Main.Ethereal;
                        if (Ethereal != null
                            && Config.ItemsToggler.Value.IsEnabled(Ethereal.ToString())
                            && Ethereal.CanBeCasted
                            && Ethereal.CanHit(Target))
                        {
                            Ethereal.UseAbility(Target);
                            await Await.Delay(Ethereal.GetCastDelay(Target), token);
                        }

                        // Dagon
                        var Dagon = Main.Dagon;
                        if (Dagon != null
                            && Config.ItemsToggler.Value.IsEnabled("item_dagon_5")
                            && Dagon.CanBeCasted
                            && Dagon.CanHit(Target)
                            && (Ethereal == null || (Target.IsEthereal() && !Ethereal.CanBeCasted)
                            || !Config.ItemsToggler.Value.IsEnabled(Ethereal.ToString())))
                        {
                            Dagon.UseAbility(Target);
                            await Await.Delay(Dagon.GetCastDelay(Target), token);
                        }
                    }
                    else
                    {
                        Config.LinkenBreaker.Handler.RunAsync();
                    }
                }

                // Necronomicon
                var Necronomicon = Main.Necronomicon;
                if (Necronomicon != null
                    && Config.ItemsToggler.Value.IsEnabled("item_necronomicon_3")
                    && Necronomicon.CanBeCasted
                    && Owner.Distance2D(Target) <= Owner.AttackRange)
                {
                    Necronomicon.UseAbility();
                    await Await.Delay(Necronomicon.GetCastDelay(), token);
                }

                // Armlet
                var Armlet = Main.Armlet;
                if (Armlet != null
                    && Config.ItemsToggler.Value.IsEnabled(Armlet.ToString())
                    && !Armlet.Enabled
                    && Owner.Distance2D(Target) <= Owner.AttackRange)
                {
                    Armlet.UseAbility();
                    await Await.Delay(Armlet.GetCastDelay(), token);
                }

                if (Target.IsInvulnerable() || Target.IsAttackImmune())
                {
                    if (!Orbwalker.Settings.Move)
                    {
                        Orbwalker.Settings.Move.Item.SetValue(true);
                    }

                    Orbwalker.Move(Game.MousePosition);
                }
                else
                {
                    if (Owner.Distance2D(Target) <= Config.MinDisInOrbwalkItem && Target.Distance2D(Game.MousePosition) <= Config.MinDisInOrbwalkItem)
                    {
                        if (Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(false);
                        }
                    }
                    else
                    {
                        if (!Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(true);
                        }
                    }

                    Orbwalker.OrbwalkTo(Target);
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
    }
}
