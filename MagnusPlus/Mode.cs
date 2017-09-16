using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Prediction;
using Ensage.SDK.Service;

namespace MagnusPlus
{
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

        private MagnusPlus Main { get; }

        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
            Main = config.MagnusPlus;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var IsEmpower = Owner.Modifiers.FirstOrDefault(x => x.Name == "modifier_magnataur_empower");

            // Empower
            if (Main.Empower != null
                && Config.AbilityToggler.Value.IsEnabled(Main.Empower.Ability.Name)
                && Main.Empower.CanBeCasted
                && (IsEmpower == null || IsEmpower.RemainingTime <= 7))
            {
                Main.Empower.UseAbility(Owner);
                await Await.Delay(Main.Empower.GetCastDelay(Owner), token);
            }

            var OffOutput = Config.UpdateMode.OffOutput;

            if (OffOutput != null)
            {
                var Target = Config.UpdateMode.OffOutput.Unit;

                if (Main.ReversePolarity != null
                    && Main.ReversePolarity.CanBeCasted)
                {
                    // Blink
                    if (Main.Blink != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Blink.Item.Name)
                        && Main.Blink.CanBeCasted
                        && Owner.Distance2D(OffOutput.CastPosition) <= Main.Blink.CastRange)
                    {
                        Main.Blink.UseAbility(OffOutput.CastPosition);
                        await Await.Delay(Main.Blink.GetCastDelay(OffOutput.CastPosition), token);

                        if (Owner.Distance2D(OffOutput.CastPosition) <= 300)
                        {
                            await ReversePolarity(token, OffOutput);
                        }
                    }
                    else

                    // ForceStaff
                    if (Main.ForceStaff != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.ForceStaff.Item.Name)
                        && Main.ForceStaff.CanBeCasted
                        && Owner.Distance2D(OffOutput.CastPosition) >= 400
                        && Context.Owner.FindRotationAngle(OffOutput.CastPosition) < 0.2f
                        && (Owner.Distance2D(OffOutput.CastPosition) <= Main.ForceStaff.PushLength
                        || (Main.Blink != null && Main.Blink.CanBeCasted
                        && Owner.Distance2D(OffOutput.CastPosition) <= Main.Blink.CastRange + Main.ForceStaff.PushLength)))
                    {
                        Main.ForceStaff.UseAbility(Owner);
                        await Await.Delay(Main.ForceStaff.GetCastDelay(Owner), token);
                    }
                    else

                    // Skewer
                    if (Main.Skewer != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.Skewer.Ability.Name)
                        && Main.Skewer.CanBeCasted
                        && Owner.Distance2D(OffOutput.CastPosition) <= Main.Skewer.CastRange
                        && Main.Blink == null)
                    {
                        Main.Skewer.UseAbility(OffOutput.CastPosition);
                        await Await.Delay(Main.Skewer.GetCastDelay(OffOutput.CastPosition), token);
                    }

                    if (Owner.Distance2D(OffOutput.CastPosition) <= 200)
                    {
                        await ReversePolarity(token, OffOutput);
                    }
                }

                if (Owner.Distance2D(OffOutput.CastPosition) <= 500)
                {
                    // BKB
                    if (Main.BKB != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.BKB.Item.Name)
                        && Main.BKB.CanBeCasted)
                    {
                        Main.BKB.UseAbility();
                        await Await.Delay(Main.BKB.GetCastDelay(), token);
                    }

                    // Shivas
                    if (Main.Shivas != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Shivas.Item.Name)
                        && Main.Shivas.CanBeCasted)
                    {
                        Main.Shivas.UseAbility();
                        await Await.Delay(Main.Shivas.GetCastDelay(), token);
                    }
                }
                
                var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                if ((IsStun == null || IsStun.RemainingTime <= 0.8f)
                    && Main.ReversePolarity != null
                    && !Main.ReversePolarity.CanBeCasted)
                {
                    // Refresher
                    if (Main.Refresher != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Refresher.Item.Name)
                        && Main.Refresher.CanBeCasted)
                    {
                        Main.Refresher.UseAbility();
                        await Await.Delay(Main.Refresher.GetCastDelay(), token);
                    }
                }

                if (Main.ReversePolarity == null
                    || !Main.ReversePolarity.CanBeCasted
                    || Main.Blink == null
                    || !Main.Blink.CanBeCasted)
                {
                    // Shockwave
                    if (Main.Shockwave != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.Shockwave.Ability.Name)
                        && Main.Shockwave.CanBeCasted
                        && Main.Shockwave.CanHit(OffOutput.Unit))
                    {
                        Main.Shockwave.UseAbility(OffOutput.Unit);
                        await Await.Delay(Main.Shockwave.GetCastDelay(OffOutput.Unit), token);
                    }
                }

                // ArcaneBoots
                if (Main.ArcaneBoots != null
                    && Config.ItemsToggler.Value.IsEnabled(Main.ArcaneBoots.Item.Name)
                    && Main.ArcaneBoots.CanBeCasted
                    && Owner.Mana * 100 / Owner.MaximumMana <= 92)
                {
                    Main.ArcaneBoots.UseAbility();
                    await Await.Delay(Main.ArcaneBoots.GetCastDelay(), token);
                }

                // Guardian
                if (Main.Guardian != null
                    && Config.ItemsToggler.Value.IsEnabled(Main.Guardian.Item.Name)
                    && Main.Guardian.CanBeCasted
                    && Owner.Mana * 100 / Owner.MaximumMana <= 92)
                {
                    Main.Guardian.UseAbility();
                    await Await.Delay(Main.Guardian.GetCastDelay(), token);
                }

                if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    if (Owner.Distance2D(OffOutput.CastPosition) >= 600)
                    {
                        Orbwalker.Move(OffOutput.CastPosition);
                    }
                    else
                    {
                        Orbwalker.OrbwalkTo(Target);
                    }
                }
            }
            else
            {
                Orbwalker.Move(Game.MousePosition);
            }
        }

        private async Task ReversePolarity(CancellationToken token, PredictionOutput OffOutput)
        {
            if (Config.AbilityToggler.Value.IsEnabled(Main.ReversePolarity.Ability.Name))
            {
                // ReversePolarity
                Main.ReversePolarity.UseAbility();
                await Await.Delay(Main.ReversePolarity.GetCastDelay(), token);
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
