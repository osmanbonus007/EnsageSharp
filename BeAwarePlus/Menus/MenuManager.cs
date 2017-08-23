using System.Collections.Generic;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

namespace BeAwarePlus.Menus
{
    internal class MenuManager
    {
        public MenuFactory Factory { get; }

        public MenuItem<bool> SideMessageItem { get; }

        public MenuItem<bool> SoundItem { get; }

        public MenuItem<bool> DefaultSoundItem { get; }

        public MenuItem<StringList> LanguageItem { get; }

        public MenuItem<bool> SpellsItem { get; }

        public MenuItem<bool> DangerousSpellsItem { get; }

        public MenuItem<bool> DangerousSpellsMSG { get; }

        public MenuItem<bool> DangerousSpellsSound { get; }

        public MenuItem<bool> ItemsItem { get; }

        public MenuItem<bool> DangerousItemsItem { get; }

        public MenuItem<bool> DangerousItemsMSG { get; }

        public MenuItem<bool> DangerousItemsSound { get; }

        public MenuItem<bool> TeleportEnemyItem { get; }

        public MenuItem<bool> TeleportAllyItem { get; }

        public MenuItem<AbilityToggler> OtherItem { get; }

        public MenuItem<bool> OnMinimapItem { get; }

        public MenuItem<bool> OnWorldItem { get; }

        public MenuItem<Slider> TimerItem { get; }

        public MenuManager()
        {
            Factory = MenuFactory.CreateWithTexture("BeAwarePlus", "beawareplus");
            Factory.Target.SetFontColor(Color.Aqua);

            var SettingsMenu = Factory.Menu("Settings");

            var SpellsMenu = SettingsMenu.Menu("Spells");
            SpellsItem = SpellsMenu.Item("Enable Spells", true);
            DangerousSpellsItem = SpellsMenu.Item("Dangerous Spells", true);
            DangerousSpellsItem.Item.SetTooltip("Enable When Enemy Is Visible");
            DangerousSpellsMSG = SpellsMenu.Item("Dangerous Side Message", true);
            DangerousSpellsSound = SpellsMenu.Item("Dangerous Sound", true);

            var ItemsMenu = SettingsMenu.Menu("Items");
            ItemsItem = ItemsMenu.Item("Enable Items", true);
            DangerousItemsItem = ItemsMenu.Item("Dangerous Spells", true);
            DangerousItemsItem.Item.SetTooltip("Enable When Enemy Is Visible");
            DangerousItemsMSG = ItemsMenu.Item("Dangerous Side Message", true);
            DangerousItemsSound = ItemsMenu.Item("Dangerous Sound", true);

            var TeleportMenu = ItemsMenu.MenuWithTexture("Teleport", "item_tpscroll");
            TeleportEnemyItem = TeleportMenu.Item("Teleport Enemy", true);
            TeleportAllyItem = TeleportMenu.Item("Teleport Ally", true);

            OtherItem = SettingsMenu.Item("Other: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "radar_scan", true },
                { "rune_bounty", true },
                { "item_hand_of_midas", true },
                { "roshan_halloween_levels", true }
            }));

            //Menu
            SideMessageItem = Factory.Item("Enable Side Message", true);
            SoundItem = Factory.Item("Enable Sound", true);
            DefaultSoundItem = Factory.Item("Enable Default Sound", false);
            DefaultSoundItem.Item.SetTooltip("All Sounds Becomes Default");
            OnMinimapItem = Factory.Item("Enable Draw On Minimap", true);
            OnWorldItem = Factory.Item("Enable Draw On World", true);
            TimerItem = Factory.Item("Timer", new Slider(6, 1, 9));
            LanguageItem = Factory.Item("Language", new StringList(new[] { "EN", "RU" }));
        }

        public List<string> LangList { get; } = new List<string>()
        {
            "en",
            "ru"
        };
    }
}