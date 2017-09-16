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
using Ensage.SDK.Input;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;

using MouseButtons = System.Windows.Forms.MouseButtons;

namespace MagnusPlus.Features
{
    internal class AutoCombo
    {
        private Config Config { get; }

        private MagnusPlus Main { get; }

        private IInputManager Input { get; }

        private Unit Owner { get; }

        private IServiceContext Context { get; }

        private IPredictionManager Prediction { get; }

        private TaskHandler Handler { get; }

        private bool Click { get; set; }

        private int Value { get; set; }

        public AutoCombo(Config config)
        {
            Config = config;
            Context = config.MagnusPlus.Context;
            Main = config.MagnusPlus;
            Owner = config.MagnusPlus.Context.Owner;
            Prediction = config.MagnusPlus.Context.Prediction;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.AutoComboItem)
            {
                Handler.RunAsync();
            }
            
            config.AutoComboItem.PropertyChanged += AutoComboChanged;

            config.MagnusPlus.Context.Input.MouseClick += MouseClick;
            config.MagnusPlus.Context.Input.MouseWheel += MouseWheel;

            Value = Config.AutoComboAmountItem;
        }

        public void Dispose()
        {
            Context.Input.MouseWheel -= MouseWheel;
            Context.Input.MouseClick -= MouseClick;

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
                    Click = false;
                    return;
                }

                var Targets =
                    EntityManager<Hero>.Entities.Where(
                        x => x.IsValid &&
                        x.Team != Owner.Team &&
                        !x.IsIllusion).ToList();

                foreach (var InputTarget in Targets)
                {
                    var Input =
                        new PredictionInput(
                            Owner,
                            InputTarget,
                            0.5f,
                            float.MaxValue,
                            float.MaxValue,
                            410,
                            PredictionSkillshotType.SkillshotCircle,
                            true,
                            Targets.Where(x => x.IsVisible).ToList())
                        {
                            CollisionTypes = CollisionTypes.None
                        };

                    var Output = Prediction.GetPrediction(Input);

                    if (Output != null && Output.AoeTargetsHit.Count >= Config.AutoComboAmountItem)
                    {
                        var IsEmpower = Owner.Modifiers.FirstOrDefault(x => x.Name == "modifier_magnataur_empower");
                        var Target = Output.Unit;

                        if (Owner.Distance2D(Output.CastPosition) <= 2000)
                        {
                            // Empower
                            if (Main.Empower != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.Empower.Ability.Name)
                                && Main.Empower.CanBeCasted
                                && (IsEmpower == null || IsEmpower.RemainingTime <= 7))
                            {
                                Main.Empower.UseAbility(Owner);
                                await Await.Delay(Main.Empower.GetCastDelay(Owner), token);
                            }
                        }

                        if (Main.ReversePolarity != null
                            && Main.ReversePolarity.CanBeCasted)
                        {
                            // Blink
                            if (Main.Blink != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Blink.Item.Name)
                                && Main.Blink.CanBeCasted
                                && Owner.Distance2D(Output.CastPosition) <= Main.Blink.CastRange)
                            {
                                Main.Blink.UseAbility(Output.CastPosition);
                                await Await.Delay(Main.Blink.GetCastDelay(Output.CastPosition), token);

                                if (Owner.Distance2D(Output.CastPosition) <= 300)
                                {
                                    await ReversePolarity(token, Output);
                                }
                            }
                            else

                            // ForceStaff
                            if (Main.ForceStaff != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.ForceStaff.Item.Name)
                                && Main.ForceStaff.CanBeCasted
                                && Owner.Distance2D(Output.CastPosition) >= 400
                                && Owner.FindRotationAngle(Output.CastPosition) < 0.2f
                                && (Owner.Distance2D(Output.CastPosition) <= Main.ForceStaff.PushLength
                                || (Main.Blink != null && Main.Blink.CanBeCasted
                                && Owner.Distance2D(Output.CastPosition) <= Main.Blink.CastRange + Main.ForceStaff.PushLength)))
                            {
                                Main.ForceStaff.UseAbility(Owner);
                                await Await.Delay(Main.ForceStaff.GetCastDelay(Owner), token);
                            }
                            else

                            // Skewer
                            if (Main.Skewer != null
                                && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.Skewer.Ability.Name)
                                && Main.Skewer.CanBeCasted
                                && Owner.Distance2D(Output.CastPosition) <= Main.Skewer.CastRange
                                && Main.Blink == null)
                            {
                                Main.Skewer.UseAbility(Output.CastPosition);
                                await Await.Delay(Main.Skewer.GetCastDelay(Output.CastPosition), token);
                            }

                            if (Owner.Distance2D(Output.CastPosition) <= 200)
                            {
                                await ReversePolarity(token, Output);
                            }

                            if (Owner.Distance2D(Output.CastPosition) <= 500)
                            {
                                // BKB
                                if (Main.BKB != null
                                    && Config.AutoItemsToggler.Value.IsEnabled(Main.BKB.Item.Name)
                                    && Main.BKB.CanBeCasted)
                                {
                                    Main.BKB.UseAbility();
                                    await Await.Delay(Main.BKB.GetCastDelay(), token);
                                }

                                // Shivas
                                if (Main.Shivas != null
                                    && Config.AutoItemsToggler.Value.IsEnabled(Main.Shivas.Item.Name)
                                    && Main.Shivas.CanBeCasted)
                                {
                                    Main.Shivas.UseAbility();
                                    await Await.Delay(Main.Shivas.GetCastDelay(), token);
                                }
                            }

                            if (Main.ReversePolarity == null
                                || !Main.ReversePolarity.CanBeCasted
                                || Main.Blink == null
                                || !Main.Blink.CanBeCasted)
                            {
                                // Shockwave
                                if (Main.Shockwave != null
                                    && Config.AutoAbilitiesToggler.Value.IsEnabled(Main.Shockwave.Ability.Name)
                                    && Main.Shockwave.CanBeCasted
                                    && Main.Shockwave.CanHit(Output.Unit))
                                {
                                    Main.Shockwave.UseAbility(Output.Unit);
                                    await Await.Delay(Main.Shockwave.GetCastDelay(Output.Unit), token);
                                }
                            }

                            // ArcaneBoots
                            if (Main.ArcaneBoots != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.ArcaneBoots.Item.Name)
                                && Main.ArcaneBoots.CanBeCasted
                                && Owner.Mana * 100 / Owner.MaximumMana <= 92)
                            {
                                Main.ArcaneBoots.UseAbility();
                                await Await.Delay(Main.ArcaneBoots.GetCastDelay(), token);
                            }

                            // Guardian
                            if (Main.Guardian != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Guardian.Item.Name)
                                && Main.Guardian.CanBeCasted
                                && Owner.Mana * 100 / Owner.MaximumMana <= 92)
                            {
                                Main.Guardian.UseAbility();
                                await Await.Delay(Main.Guardian.GetCastDelay(), token);
                            }
                        }

                        var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff && x.TextureName == "magnataur_reverse_polarity");
                        if ((IsStun == null || IsStun.RemainingTime <= 0.4f)
                            && Owner.Distance2D(Target) <= 400
                            && Main.ReversePolarity != null
                            && !Main.ReversePolarity.CanBeCasted)
                        {
                            // Refresher
                            if (Main.Refresher != null
                                && Config.AutoItemsToggler.Value.IsEnabled(Main.Refresher.Item.Name)
                                && Main.Refresher.CanBeCasted)
                            {
                                Main.Refresher.UseAbility();
                                await Await.Delay(Main.Refresher.GetCastDelay(), token);
                            }
                        }

                        if (Config.AutoAtackItem)
                        {
                            if (Target != null
                                && !Target.IsAttackImmune()
                                && !Target.IsInvulnerable()
                                && IsStun != null
                                && Click == true)
                            {
                                Context.Orbwalker.Attack(Target);
                            }
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

        private async Task ReversePolarity(CancellationToken token, PredictionOutput OffOutput)
        {
            if (Config.AutoAbilitiesToggler.Value.IsEnabled(Main.ReversePolarity.Ability.Name))
            {
                // ReversePolarity
                Main.ReversePolarity.UseAbility();
                Click = true;
                await Await.Delay(Main.ReversePolarity.GetCastDelay(), token);
            }
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Right && Click == true)
            {
                Click = false;
            }
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Value += 1;
                Value = Math.Min(Value, 5);
            }
            else
            {
                Value -= 1;
                Value = Math.Max(Value, 1);
            }

            Config.AutoComboAmountItem.Item.SetValue(new Slider(Value, 1, 5));
        }
    }
}
