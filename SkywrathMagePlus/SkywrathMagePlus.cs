using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

using SkywrathMage;

namespace SkywrathMagePlus
{
    [ExportPlugin(
        name: "SkywrathMagePlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.0.1.0",
        units: HeroId.npc_dota_hero_skywrath_mage)]
    internal class SkywrathMagePlus : Plugin
    {
        private Config Config { get; set; }

        public Mode Mode { get; set; }

        public SpamMode SpamMode { get; set; }

        public IServiceContext Context { get; }

        private Lazy<AbilityFactory> AbilityFactory { get; }

        [ImportingConstructor]
        public SkywrathMagePlus([Import] IServiceContext context)
        {
            Context = context;
        }

        protected override void OnActivate()
        {
            Config = new Config(this);
            Config.ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)Config.ComboKeyItem.Value.Key);

            Mode = new Mode(Context, Key, Config);
            Context.Orbwalker.RegisterMode(Mode);

            SpamMode = new SpamMode(Config, Mode);
        }

        protected override void OnDeactivate()
        {
            Context.Orbwalker.UnregisterMode(Mode);

            Config.ComboKeyItem.Item.ValueChanged -= HotkeyChanged;

            Mode.Deactivate();
            Config?.Dispose();
        }

        private void HotkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var KeyCode = e.GetNewValue<KeyBind>().Key;

            if (KeyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var Key = KeyInterop.KeyFromVirtualKey((int)KeyCode);
            Mode.Key = Key;
        }
    }
}
