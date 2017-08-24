using System;
using System.Collections.Generic;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

namespace EnigmaPlus
{
    internal class Config : IDisposable
    {
        private EnigmaPlus EnigmaPlus { get; }

        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<Slider> AmountItem { get; }

        private bool Disposed { get; set; }

        public Config(EnigmaPlus enigmaplus)
        {
            EnigmaPlus = enigmaplus;

            Factory = MenuFactory.CreateWithTexture("EnigmaPlus", "npc_dota_hero_enigma");
            Factory.Target.SetFontColor(Color.Aqua);
            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "enigma_black_hole", true },
                { "enigma_midnight_pulse", true },
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_refresher",true},
                {"item_veil_of_discord",true},
                {"item_shivas_guard",true},
                {"item_black_king_bar",true},
                {"item_glimmer_cape",true},
                {"item_guardian_greaves",true},
                {"item_arcane_boots",true},
                {"item_soul_ring",true},
                {"item_blink",true}
            }));

            var DrawingMenu = Factory.Menu("Drawing");
            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");

            AmountItem = Factory.Item("Amount", new Slider(2, 1, 5));

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
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
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
