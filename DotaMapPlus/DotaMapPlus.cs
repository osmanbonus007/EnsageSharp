using System;
using System.ComponentModel.Composition;

using Ensage.SDK.Input;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

namespace DotaMapPlus
{
    [ExportPlugin("DotaMapPlus", StartupMode.Auto, "YEEEEEEE", "2.0.0.1")]
    public class DotaMapPlus : Plugin
    {
        public Lazy<IInputManager> InputManager { get; }

        private DotaMapPlusConfig DotaMapPlusConfig { get; set; }

        [ImportingConstructor]
        public DotaMapPlus([Import] Lazy<IInputManager> inputmanager)
        {
            InputManager = inputmanager;
        }

        protected override void OnActivate()
        {
            DotaMapPlusConfig = new DotaMapPlusConfig(InputManager);
        }

        protected override void OnDeactivate()
        {
            DotaMapPlusConfig?.Dispose();
        }
    }
}
