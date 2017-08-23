using System;
using System.Collections.Generic;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using SkywrathMage.Features;

namespace SkywrathMagePlus
{
    internal class SkywrathMagePlusConfig : IDisposable
    {
        private SkywrathMagePlus SkywrathMagePlus { get; }

        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<bool> WDrawItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<bool> WTargetItem { get; }

        public MenuItem<Slider> WRadiusItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public ComboItems ComboItems { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private bool Disposed { get; set; }

        public SkywrathMagePlusConfig(SkywrathMagePlus skywrathMagePlus)
        {
            SkywrathMagePlus = skywrathMagePlus;

            Factory = MenuFactory.CreateWithTexture("SkywrathMagePlus", "npc_dota_hero_skywrath_mage");
            Factory.Target.SetFontColor(Color.Aqua);
            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_mystic_flare", true },
                { "skywrath_mage_ancient_seal", true },
                { "skywrath_mage_concussive_shot", true },
                { "skywrath_mage_arcane_bolt", true }
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_ghost", true },
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var LinkenBreakerMenu = Factory.Menu("Linken Breaker");
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_ancient_seal", true },
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_cyclone", true },
                { "item_force_staff", true },
                { "item_medallion_of_courage", true },
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "skywrath_mage_ancient_seal" },
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_cyclone" },
                { "item_force_staff" },
                { "item_medallion_of_courage" },
            }));

            var DrawingMenu = Factory.Menu("Drawing");
            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");
            WDrawItem = DrawingMenu.Item("W Show Target", true);

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            WTargetItem = Factory.Item("Use W Only Target", true);
            WRadiusItem = Factory.Item("Use W in Radius", new Slider(1100, 0, 1600));
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboItems = new ComboItems(this);

            LinkenBreaker = new LinkenBreaker(this);
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
