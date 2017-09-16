using System;
using System.ComponentModel.Composition;

using Ensage.SDK.Input;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

namespace DotaMapPlus
{
    [ExportPlugin("DotaMapPlus", StartupMode.Auto, "YEEEEEEE", "2.0.0.2")]
    internal class DotaMapPlus : Plugin
    {
        public Lazy<IInputManager> InputManager { get; }

        private Config Config { get; set; }

        [ImportingConstructor]
        public DotaMapPlus([Import] Lazy<IInputManager> inputmanager)
        {
            InputManager = inputmanager;
        }

        protected override void OnActivate()
        {
            Config = new Config(InputManager);
        }

        protected override void OnDeactivate()
        {
            Config?.Dispose();
        }
    }
}
