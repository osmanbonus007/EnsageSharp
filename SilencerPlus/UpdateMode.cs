using System.Linq;

using Ensage;
using Ensage.SDK.Helpers;
using Ensage.SDK.Extensions;
using Ensage.SDK.Service;

using SharpDX;

namespace SilencerPlus
{
    internal class UpdateMode
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        public Hero Target { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
            Context = config.Main.Context;

            UpdateManager.Subscribe(OnUpdate, 25);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void OnUpdate()
        {
            if (Config.ComboRadiusItem)
            {
                Context.Particle.DrawRange(
                    Context.Owner,
                    "ComboRadius",
                    Context.Owner.AttackRange(Context.Owner),
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
