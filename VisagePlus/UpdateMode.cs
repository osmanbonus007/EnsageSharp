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

        public Hero Target { get; set; }

        public Hero FamiliarTarget { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
            Main = config.Main;
            Context = config.Main.Context;

            UpdateManager.Subscribe(OnUpdate, 25);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void OnUpdate()
        {
            if (Config.EscapeKeyItem && !Config.ComboKeyItem)
            {
                Context.Orbwalker.Move(Game.MousePosition);
            }

            if (Config.ComboRadiusItem)
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

            if (Config.TargetItem.Value.SelectedValue.Contains("Lock") && Context.TargetSelector.IsActive
                && (!Config.ComboKeyItem || Target == null || !Target.IsValid || !Target.IsAlive))
            {
                Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }
            else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && Context.TargetSelector.IsActive)
            {
                Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (Context.TargetSelector.IsActive
                && (!Config.FamiliarsLockItem || FamiliarTarget == null || !FamiliarTarget.IsValid || !FamiliarTarget.IsAlive))
            {
                FamiliarTarget = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (Target != null && ((Config.DrawOffTargetItem && !Config.ComboKeyItem) || (Config.DrawTargetItem && Config.ComboKeyItem)))
            {
                Context.Particle.DrawTargetLine(
                    Context.Owner,
                    "PlusTarget",
                    Target.Position,
                    Config.ComboKeyItem 
                    ? new Color(Config.TargetRedItem, Config.TargetGreenItem, Config.TargetBlueItem)
                    : new Color(Config.OffTargetRedItem, Config.OffTargetGreenItem, Config.OffTargetBlueItem));
            }
            else
            {
                Context.Particle.Remove("PlusTarget");
            }
        }
    }
}
