using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using MagnusPlus.Features;

namespace MagnusPlus
{
    internal class Config : IDisposable
    {
        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<bool> AutoComboItem { get; }

        public MenuItem<Slider> AutoComboAmountItem { get; }

        public MenuItem<bool> AutoAtackItem { get; }

        public MenuItem<AbilityToggler> AutoAbilitiesToggler { get; }

        public MenuItem<AbilityToggler> AutoItemsToggler { get; }

        public MenuItem<bool> TextItem { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<Slider> AmountItem { get; }

        public MenuItem<bool> RPWithoutFailItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MagnusPlus MagnusPlus { get; }

        public AutoCombo AutoCombo { get; }

        public UpdateMode UpdateMode { get; }

        private WithoutFail WithoutFail { get; }

        private Renderer Renderer { get; }

        public Mode Mode { get; }

        private bool Disposed { get; set; }

        public Config(MagnusPlus magnusplus)
        {
            MagnusPlus = magnusplus;

            Factory = MenuFactory.CreateWithTexture("MagnusPlus", "npc_dota_hero_magnataur");
            Factory.Target.SetFontColor(Color.Aqua);

            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "magnataur_reverse_polarity", true },
                { "magnataur_skewer", true },
                { "magnataur_empower", true },
                { "magnataur_shockwave", true },
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_refresher", true },
                { "item_shivas_guard", true },
                { "item_black_king_bar", true },
                { "item_guardian_greaves", true },
                { "item_arcane_boots", true },
                { "item_force_staff", true },
                { "item_blink", true }
            }));

            var AutoComboMenu = Factory.Menu("Auto Combo");
            AutoComboItem = AutoComboMenu.Item("Use Auto Combo", true);
            AutoComboAmountItem = AutoComboMenu.Item("Amount", new Slider(4, 1, 5));
            AutoComboAmountItem.Item.SetTooltip("Can be Changed Using the Mouse Wheel");
            AutoAtackItem = AutoComboMenu.Item("Auto Atack", true);
            AutoAtackItem.Item.SetTooltip("1 Times Right Mouse Click Disable Auto Attack");
            AutoAbilitiesToggler = AutoComboMenu.Item("Abilities: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "magnataur_reverse_polarity", true },
                { "magnataur_skewer", false },
                { "magnataur_empower", true },
                { "magnataur_shockwave", true }
            }));

            AutoItemsToggler = AutoComboMenu.Item("Items: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_refresher", true },
                { "item_shivas_guard", true },
                { "item_black_king_bar", true },
                { "item_guardian_greaves", true },
                { "item_arcane_boots", true },
                { "item_force_staff", true },
                { "item_blink", true }
            }));

            var DrawingMenu = Factory.Menu("Drawing");
            TextItem = DrawingMenu.Item("Text", true);
            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            AmountItem = Factory.Item("Amount", new Slider(1, 1, 5));

            RPWithoutFailItem = Factory.Item("RP Without Fail", true);
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            AutoCombo = new AutoCombo(this);
            UpdateMode = new UpdateMode(this);
            WithoutFail = new WithoutFail(this);
            Renderer = new Renderer(this);

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);
            Mode = new Mode(MagnusPlus.Context, Key, this);
            MagnusPlus.Context.Orbwalker.RegisterMode(Mode);

            Game.ExecuteCommand("dota_camera_disable_zoom true");
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                MagnusPlus.Context.Orbwalker.UnregisterMode(Mode);
                Mode.Deactivate();

                Renderer.Dispose();
                WithoutFail.Dispose();
                UpdateMode.Dispose();
                AutoCombo.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;

                MagnusPlus.Context.Particle.Dispose();
                Factory.Dispose();

                Game.ExecuteCommand("dota_camera_disable_zoom false");
            }

            Disposed = true;
        }
    }
}
