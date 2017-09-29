using System.Linq;

using Ensage;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace UnitsControlPlus
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
            if (Context.TargetSelector.IsActive
                && !Config.PressKeyItem
                && (!Config.ToggleKeyItem || Config.ChangeTargetItem || Target == null || !Target.IsValid || !Target.IsAlive))
            {
                Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault(x => x.IsValid) as Hero;
            }

            if (!Config.ToggleKeyItem)
            {
                if (Config.PressTargetItem.Value.SelectedValue.Contains("Lock")
                && Context.TargetSelector.IsActive
                && (Target == null || !Target.IsValid || !Target.IsAlive))
                {
                    Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault(x => x.IsValid) as Hero;
                }
                else if (Config.PressTargetItem.Value.SelectedValue.Contains("Default") && Context.TargetSelector.IsActive)
                {
                    Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault(x => x.IsValid) as Hero;
                }
            }
        }
    }
}
