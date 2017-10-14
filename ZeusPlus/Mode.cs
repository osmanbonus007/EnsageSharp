using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;

using SharpDX;

using PlaySharp.Toolkit.Helper.Annotations;

namespace ZeusPlus
{
    [PublicAPI]
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private MenuManager Menu { get; }

        private ZeusPlus Main { get; }

        public Mode(IServiceContext context, Key key, Config config) : base(context, key)
        {
            Config = config;
            Menu = config.Menu;
            Main = config.Main;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = Config.UpdateMode.Target;

            if (target != null && (!Menu.BladeMailItem || !target.HasModifier("modifier_item_blade_mail_reflect")))
            {
                var StunDebuff = target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var HexDebuff = target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name =="modifier_sheepstick_debuff");
                var AtosDebuff = target.Modifiers.FirstOrDefault(x => x.IsValid && x.Name == "modifier_rod_of_atos_debuff");
                var Sleeper = Config.AutoKillSteal.Sleeper;

                // Blink
                var Blink = Main.Blink;
                if (Blink != null
                    && Menu.ItemsToggler.Value.IsEnabled(Blink.ToString())
                    && Owner.Distance2D(Game.MousePosition) > Menu.BlinkActivationItem
                    && Owner.Distance2D(target) > 600
                    && Blink.CanBeCasted)
                {
                    var blinkPos = target.Position.Extend(Game.MousePosition, Menu.BlinkDistanceEnemyItem);
                    if (Owner.Distance2D(blinkPos) < Blink.CastRange)
                    {
                        Blink.UseAbility(blinkPos);
                        await Await.Delay(Blink.GetCastDelay(blinkPos), token);
                    }
                }

                if (!target.IsMagicImmune() && !target.IsInvulnerable()
                    && !target.HasModifier("modifier_abaddon_borrowed_time")
                    && !target.HasAnyModifiers("modifier_winter_wyvern_winters_curse_aura", "modifier_winter_wyvern_winters_curse"))
                {
                    if (!target.IsLinkensProtected() && !Config.LinkenBreaker.AntimageShield(target))
                    {
                        // Hex
                        var Hex = Main.Hex;
                        if (Hex != null
                            && Menu.ItemsToggler.Value.IsEnabled(Hex.ToString())
                            && Hex.CanBeCasted
                            && Hex.CanHit(target)
                            && (StunDebuff == null || StunDebuff.RemainingTime <= 0.3f)
                            && (HexDebuff == null || HexDebuff.RemainingTime <= 0.3f))
                        {
                            Hex.UseAbility(target);
                            await Await.Delay(Hex.GetCastDelay(target), token);
                        }

                        // Orchid
                        var Orchid = Main.Orchid;
                        if (Orchid != null
                            && Menu.ItemsToggler.Value.IsEnabled(Orchid.ToString())
                            && Orchid.CanBeCasted
                            && Orchid.CanHit(target))
                        {
                            Main.Orchid.UseAbility(target);
                            await Await.Delay(Main.Orchid.GetCastDelay(target), token);
                        }

                        // Bloodthorn
                        var Bloodthorn = Main.Bloodthorn;
                        if (Bloodthorn != null
                            && Menu.ItemsToggler.Value.IsEnabled(Bloodthorn.ToString())
                            && Bloodthorn.CanBeCasted
                            && Bloodthorn.CanHit(target))
                        {
                            Bloodthorn.UseAbility(target);
                            await Await.Delay(Bloodthorn.GetCastDelay(target), token);
                        }

                        // RodofAtos
                        var RodofAtos = Main.RodofAtos;
                        if (RodofAtos != null
                            && Menu.ItemsToggler.Value.IsEnabled(RodofAtos.ToString())
                            && RodofAtos.CanBeCasted
                            && RodofAtos.CanHit(target)
                            && (StunDebuff == null || StunDebuff.RemainingTime <= 0.5f)
                            && (AtosDebuff == null || AtosDebuff.RemainingTime <= 0.5f))
                        {
                            RodofAtos.UseAbility(target);
                            await Await.Delay(RodofAtos.GetCastDelay(target), token);
                        }

                        // Veil
                        var Veil = Main.Veil;
                        if (Veil != null
                            && Menu.ItemsToggler.Value.IsEnabled(Veil.ToString())
                            && Veil.CanBeCasted
                            && Veil.CanHit(target))
                        {
                            Veil.UseAbility(target.Position);
                            await Await.Delay(Veil.GetCastDelay(target.Position), token);
                        }

                        // Ethereal
                        var Ethereal = Main.Ethereal;
                        if (Ethereal != null
                            && Menu.ItemsToggler.Value.IsEnabled(Ethereal.ToString())
                            && Ethereal.CanBeCasted
                            && Ethereal.CanHit(target))
                        {
                            Ethereal.UseAbility(target);
                            Sleeper.Sleep(Ethereal.GetHitTime(target));
                            await Await.Delay(Ethereal.GetCastDelay(target), token);
                        }

                        // Shivas
                        var Shivas = Main.Shivas;
                        if (Shivas != null
                            && Menu.ItemsToggler.Value.IsEnabled(Shivas.ToString())
                            && Shivas.CanBeCasted
                            && Shivas.CanHit(target))
                        {
                            Shivas.UseAbility();
                            await Await.Delay(Shivas.GetCastDelay(), token);
                        }

                        if (!Sleeper.Sleeping || target.IsEthereal())
                        {
                            // LightningBolt
                            var LightningBolt = Main.LightningBolt;
                            if (Menu.AbilitiesToggler.Value.IsEnabled(LightningBolt.ToString())
                                && LightningBolt.CanBeCasted
                                && LightningBolt.CanHit(target))
                            {
                                LightningBolt.UseAbility(target);
                                await Await.Delay(LightningBolt.GetCastDelay(target), token);
                            }

                            // ArcLightning
                            var ArcLightning = Main.ArcLightning;
                            if (Menu.AbilitiesToggler.Value.IsEnabled(ArcLightning.ToString())
                                && ArcLightning.CanBeCasted
                                && ArcLightning.CanHit(target))
                            {
                                ArcLightning.UseAbility(target);
                                await Await.Delay(ArcLightning.GetCastDelay(target), token);
                                return;
                            }

                            // Dagon
                            var Dagon = Main.Dagon;
                            if (Dagon != null
                                && Menu.ItemsToggler.Value.IsEnabled("item_dagon_5")
                                && Dagon.CanBeCasted
                                && Dagon.CanHit(target))
                            {
                                Dagon.UseAbility(target);
                                await Await.Delay(Dagon.GetCastDelay(target), token);
                                return;
                            }

                            // Nimbus
                            var Nimbus = Main.Nimbus;
                            if (Menu.AbilitiesToggler.Value.IsEnabled(Nimbus.ToString()) && Nimbus.CanBeCasted)
                            {
                                Nimbus.UseAbility(target.Position);
                                await Await.Delay(Nimbus.GetCastDelay(target.Position), token);
                                return;
                            }

                            // Thundergods Wrath
                            var ThundergodsWrath = Main.ThundergodsWrath;
                            if (Menu.AbilitiesToggler.Value.IsEnabled(ThundergodsWrath.ToString())
                                && (float)target.Health / target.MaximumHealth * 100 < Menu.MinHealhItem
                                && Owner.Distance2D(target) < Menu.MinRangeItem
                                && ThundergodsWrath.CanBeCasted)
                            {
                                ThundergodsWrath.UseAbility();
                                await Await.Delay(ThundergodsWrath.GetCastDelay(), token);
                            }
                        }
                    }
                    else
                    {
                        Config.LinkenBreaker.Handler.RunAsync();
                    }
                }

                if (target.IsInvulnerable() || target.IsAttackImmune())
                {
                    Orbwalker.Move(Game.MousePosition);
                }
                else
                {
                    if (Menu.OrbwalkerItem.Value.SelectedValue.Contains("Default"))
                    {
                        Orbwalker.OrbwalkingPoint = Vector3.Zero;
                        Orbwalker.OrbwalkTo(target);
                    }
                    else if (Menu.OrbwalkerItem.Value.SelectedValue.Contains("Distance"))
                    {
                        var ownerDis = Math.Min(Owner.Distance2D(Game.MousePosition), 230);
                        var ownerPos = Owner.Position.Extend(Game.MousePosition, ownerDis);
                        var pos = target.Position.Extend(ownerPos, Menu.MinDisInOrbwalkItem);

                        Orbwalker.OrbwalkTo(target);
                        Orbwalker.OrbwalkingPoint = pos;
                    }
                    else if (Menu.OrbwalkerItem.Value.SelectedValue.Contains("Free"))
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target) && target.Distance2D(Game.MousePosition) < Owner.AttackRange(target))
                        {
                            Orbwalker.OrbwalkingPoint = Vector3.Zero;
                            Orbwalker.OrbwalkTo(target);
                        }
                        else
                        {
                            Orbwalker.Move(Game.MousePosition);
                        }
                    }
                }
            }
            else
            {
                Orbwalker.Move(Game.MousePosition);
            }
        }
    }
}
