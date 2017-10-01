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

namespace EnchantressPlus
{
    [PublicAPI]
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private EnchantressPlus Main { get; }

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

            if (Target != null
                && (!Config.BladeMailItem || !Target.HasModifier("modifier_item_blade_mail_reflect")))
            {
                var StunDebuff = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var HexDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name =="modifier_sheepstick_debuff");
                var AtosDebuff = Target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name == "modifier_rod_of_atos_debuff");

                if (!Target.IsMagicImmune() && !Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse"))
                {
                    if (Target.IsLinkensProtected() || Config.LinkenBreaker.AntimageShield(Target))
                    {
                        Config.LinkenBreaker.Handler.RunAsync();

                        return;
                    }
                    else if (Config.LinkenBreaker.Handler.IsRunning)
                    {
                        Config.LinkenBreaker.Handler?.Cancel();
                    }

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

                    // Enchant
                    var Enchant = Main.Enchant;
                    if (Enchant != null
                        && Config.AbilityToggler.Value.IsEnabled(Enchant.ToString())
                        && Enchant.CanBeCasted
                        && Enchant.CanHit(Target))
                    {
                        Enchant.UseAbility(Target);
                        await Await.Delay(Enchant.GetCastDelay(Target), token);
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
                        await Await.Delay(Veil.GetCastDelay(Target), token);
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

                // Necronomicon
                var Necronomicon = Main.Necronomicon;
                if (Necronomicon != null
                    && Config.ItemsToggler.Value.IsEnabled("item_necronomicon_3")
                    && Necronomicon.CanBeCasted
                    && Owner.Distance2D(Target) <= Owner.AttackRange(Context.Owner))
                {
                    Necronomicon.UseAbility();
                    await Await.Delay(Necronomicon.GetCastDelay(), token);
                }

                if (Target != null && (Target.IsInvulnerable() || Target.IsAttackImmune()))
                {
                    if (!Orbwalker.Settings.Move)
                    {
                        Orbwalker.Settings.Move.Item.SetValue(true);
                    }

                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    if ((Owner.Distance2D(Target) <= Config.MinDisInOrbwalkItem
                        && Target.Distance2D(Game.MousePosition) <= Config.MinDisInOrbwalkItem)
                        || Owner.HasModifier("modifier_item_hurricane_pike_range"))
                    {
                        if (Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(false);
                        }

                        await ImpetusCast(Target, token);
                    }
                    else
                    {
                        if (!Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(true);
                        }

                        await ImpetusCast(Target, token);
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

        private async Task<bool> ImpetusCast(Hero target, CancellationToken token)
        {
            var Impetus = Main.Impetus;
            var ModifierHurricanePike = Owner.HasModifier("modifier_item_hurricane_pike_range");

            if (Impetus == null || !Config.AbilityToggler.Value.IsEnabled(Impetus.ToString()))
            {
                if (ModifierHurricanePike)
                {
                    return Orbwalker.Attack(target);
                }

                return Orbwalker.OrbwalkTo(target);
            }

            // Impetus Autocast
            if (ModifierHurricanePike)
            {
                if (!Impetus.Ability.IsAutoCastEnabled)
                {
                    Impetus.Ability.ToggleAutocastAbility();
                }

                return Orbwalker.Attack(target);
            }
            else if (Impetus.Ability.IsAutoCastEnabled)
            {
                Impetus.Ability.ToggleAutocastAbility();
            }

            // Impetus
            if (Impetus.CanBeCasted
                && Orbwalker.CanAttack(target))
            {
                Impetus.UseAbility(target);
                await Await.Delay(Impetus.GetCastDelay(target), token);

                return true;
            }

            return Orbwalker.OrbwalkTo(target);
        }
    }
}
