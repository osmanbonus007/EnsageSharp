using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;

using Ensage.SDK.Abilities;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

namespace EnigmaPlus
{
    [ExportPlugin(
        name: "EnigmaPlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.0.0.0",
        units: HeroId.npc_dota_hero_enigma)]
    internal class EnigmaPlus : Plugin
    {
        private Config Config { get; set; }

        private EnigmaPlusCombo Combo { get; set; }

        private IServiceContext Context { get; }

        private Lazy<AbilityFactory> AbilityFactory { get; }

        [ImportingConstructor]
        public EnigmaPlus([Import] IServiceContext context)
        {
            Context = context;
        }

        protected override void OnActivate()
        {
            Config = new Config(this);
            Config.ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)Config.ComboKeyItem.Value.Key);

            Combo = new EnigmaPlusCombo(Context, Key, Config);
            Context.Orbwalker.RegisterMode(Combo);
        }

        protected override void OnDeactivate()
        {
            Context.Orbwalker.UnregisterMode(Combo);

            Config.ComboKeyItem.Item.ValueChanged -= HotkeyChanged;

            Combo.Deactivate();
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
            Combo.Key = Key;
        }
    }
}
