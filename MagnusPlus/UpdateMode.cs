using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Renderer.Particle;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

using SharpDX;

namespace MagnusPlus
{
    internal class UpdateMode
    {
        private Config Config { get; }

        private MagnusPlus Main { get; }

        private IServiceContext Context { get; }

        private ITargetSelectorManager TargetSelector { get; }

        private IPredictionManager Prediction { get; }

        public PredictionOutput OffOutput { get; set; }

        private Hero OffTarget { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
            Context = config.MagnusPlus.Context;
            Main = config.MagnusPlus;
            TargetSelector = config.MagnusPlus.Context.TargetSelector;
            Prediction = config.MagnusPlus.Context.Prediction;

            UpdateManager.Subscribe(OnUpdate, 25);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void OnUpdate()
        {
            var Targets =
                EntityManager<Hero>.Entities.Where(
                    x => x.IsValid &&
                    x.Team != Context.Owner.Team &&
                    !x.IsIllusion).ToList();

            foreach (var Target in Targets)
            {
                var Input =
                    new PredictionInput(
                        Context.Owner,
                        Target,
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

                if (Output.Unit.IsVisible
                    && Output.AoeTargetsHit.Count >= (Config.AmountItem.Value == 1 ? 2 : Config.AmountItem.Value))
                {
                    Context.Particle.AddOrUpdate(
                        Context.Owner,
                        $"Radius{Output.Unit.Handle}",
                        "particles/ui_mouseactions/drag_selected_ring.vpcf",
                        ParticleAttachment.AbsOrigin,
                        RestartType.None,
                        0,
                        Output.CastPosition,
                        1,
                        Color.Aqua,
                        2,
                        420 * -1.1f);

                    Context.Particle.AddOrUpdate(
                        Context.Owner,
                        $"Text{Output.Unit.Handle}",
                        "materials/ensage_ui/particle_textures/combo.vpcf",
                        ParticleAttachment.AbsOrigin,
                        RestartType.FullRestart,
                        0,
                        Output.CastPosition,
                        1,
                        new Vector3(Output.AoeTargetsHit.Count, 0, 0),
                        2,
                        new Vector3(200, 255, 0),
                        3,
                        new Vector3(255, 255, 255));
                }
                else
                {
                    Context.Particle.Remove($"Radius{Output.Unit.Handle}");
                    Context.Particle.Remove($"Text{Output.Unit.Handle}");
                }
            }

            OffOutput = null;

            if (Config.TargetItem.Value.SelectedValue.Contains("Lock")
                && TargetSelector.IsActive
                && (Main.ReversePolarity != null && Main.ReversePolarity.Ability.IsInAbilityPhase))
            {
                OffTarget =
                    TargetSelector.Active.GetTargets().OrderBy(x => x.Distance2D(Context.Owner)).FirstOrDefault() as Hero;
            }
            else

            if (Config.TargetItem.Value.SelectedValue.Contains("Lock")
                && TargetSelector.IsActive
                && (!Config.ComboKeyItem || OffTarget == null || !OffTarget.IsValid || !OffTarget.IsAlive))
            {
                OffTarget = 
                    TargetSelector.Active.GetTargets().FirstOrDefault(
                        x => x.Distance2D(Game.MousePosition) <= 500) as Hero;
            }
            else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && TargetSelector.IsActive)
            {
                OffTarget =
                    TargetSelector.Active.GetTargets().FirstOrDefault(
                        x => x.Distance2D(Game.MousePosition) <= 500) as Hero;
            }

            if (OffTarget != null)
            {
                var Input =
                    new PredictionInput(
                        Context.Owner,
                        OffTarget,
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

                OffOutput = Prediction.GetPrediction(Input);

                if (OffOutput != null)
                {
                    Context.Particle.DrawTargetLine(
                        Context.Owner,
                        "Target",
                        RP() ? (OffOutput.AoeTargetsHit.Count < 2 ? OffOutput.Unit.Position : OffOutput.CastPosition) : OffOutput.Unit.Position,
                        Config.ComboKeyItem ? Color.Red : Color.Aqua);
                }
                else
                {
                    Context.Particle.Remove("Target");
                }
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (Config.ComboRadiusItem && Main.Blink != null)
            {
                Context.Particle.DrawRange(
                    Context.Owner,
                    "ComboRadius",
                    Main.Blink.CastRange,
                    Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("ComboRadius");
            }
        }

        private bool RP()
        {
            return Main.ReversePolarity != null && Main.ReversePolarity.CanBeCasted;
        }
    }
}
