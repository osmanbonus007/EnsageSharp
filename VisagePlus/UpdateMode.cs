using System.Linq;

using Ensage;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

using SharpDX;

namespace VisagePlus
{
    internal class UpdateMode
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private IServiceContext Context { get; }

        private Unit OffTarget { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
            Main = config.VisagePlus;
            Context = config.VisagePlus.Context;

            UpdateManager.Subscribe(OnUpdate, 25);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void OnUpdate()
        {
            if (Config.ComboRadiusItem.Value)
            {
                Context.Particle.DrawRange(
                    Context.Owner,
                    "ComboRadius",
                    Main.GraveChill.CastRange,
                    Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("ComboRadius");
            }

            if (Context.TargetSelector.IsActive
                || OffTarget == null || !OffTarget.IsValid || !OffTarget.IsAlive)
            {
                OffTarget = Context.TargetSelector.Active.GetTargets().FirstOrDefault();
            }

            if (OffTarget != null)
            {
                Context.Particle.DrawTargetLine(
                    Context.Owner,
                    "Target",
                    OffTarget.Position,
                    Config.Mode.CanExecute ? Color.Red : Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (!Config.Mode.CanExecute && Config.Mode.Target != null)
            {
                if (!Context.TargetSelector.IsActive)
                {
                    Context.TargetSelector.Activate();
                }

                Config.Mode.Target = null;
            }
        }
    }
}
