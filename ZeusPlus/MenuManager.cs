using System.Collections.Generic;
using System.ComponentModel;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

namespace ZeusPlus
{
    internal class MenuManager
    {
        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilitiesToggler { get; }

        public MenuItem<Slider> MinHealhItem { get; }

        public MenuItem<Slider> MinRangeItem { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<Slider> BlinkActivationItem { get; }

        public MenuItem<Slider> BlinkDistanceEnemyItem { get; }

        public MenuItem<bool> AutoKillStealItem { get; }

        public MenuItem<AbilityToggler> AutoKillStealToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuItem<AbilityToggler> AntiMageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntiMageBreakerChanger { get; }

        public MenuItem<bool> UseOnlyFromRangeItem { get; }

        public MenuItem<bool> TeleportBreakerItem { get; }

        public MenuItem<AbilityToggler> TeleportBreakerToggler { get; }

        public MenuItem<Slider> NimbusRangeItem { get; }

        public MenuItem<StringList> TargetEffectTypeItem { get; }

        public MenuItem<bool> DrawTargetItem { get; }

        public MenuItem<Slider> TargetRedItem { get; }

        public MenuItem<Slider> TargetGreenItem { get; }

        public MenuItem<Slider> TargetBlueItem { get; }

        public MenuItem<bool> DrawOffTargetItem { get; }

        public MenuItem<Slider> OffTargetRedItem { get; }

        public MenuItem<Slider> OffTargetGreenItem { get; }

        public MenuItem<Slider> OffTargetBlueItem { get; }

        public MenuItem<bool> ArcLightningRadiusItem { get; }

        public MenuItem<bool> LightningBoltRadiusItem { get; }

        public MenuItem<bool> BlinkRadiusItem { get; }

        public MenuItem<bool> OnDrawItem { get; }

        public MenuItem<Slider> OnDrawXItem { get; }

        public MenuItem<Slider> OnDrawYItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<StringList> OrbwalkerItem { get; }

        public MenuItem<Slider> MinDisInOrbwalkItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<KeyBind> FarmKeyItem { get; }

        public MenuItem<bool> BladeMailItem { get; }

        public MenuManager(Config config)
        {
            Factory = MenuFactory.CreateWithTexture("ZeusPlus", "npc_dota_hero_zuus");
            Factory.Target.SetFontColor(Color.Aqua);

            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilitiesToggler = AbilitiesMenu.Item("Use: ", "AbilitiesToggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_thundergods_wrath", false },
                { "zuus_cloud", true },
                { "zuus_lightning_bolt", true },
                { "zuus_arc_lightning", true }
            }));

            MinHealhItem = AbilitiesMenu.Item("Min Healh % To Ult", new Slider(20, 10, 100));
            MinRangeItem = AbilitiesMenu.Item("Min Range To Ult", new Slider(1200, 300, 2000));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", "ItemsToggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_blink", true },
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            BlinkActivationItem = ItemsMenu.Item("Blink Activation Distance Mouse", new Slider(1000, 0, 1200));
            BlinkDistanceEnemyItem = ItemsMenu.Item("Blink Distance From Enemy", new Slider(300, 0, 500));

            var AutoKillStealMenu = Factory.Menu("Auto Kill Steal");
            AutoKillStealItem = AutoKillStealMenu.Item("Enable", true);
            AutoKillStealToggler = AutoKillStealMenu.Item("Use: ", "AutoKillStealToggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_thundergods_wrath", true },
                { "zuus_cloud", true },
                { "zuus_static_field", true },
                { "zuus_lightning_bolt", true },
                { "zuus_arc_lightning", true },
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_ethereal_blade", true },
                { "item_veil_of_discord", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerMenu.Target.AddItem(new MenuItem("linkensphere", "Linkens Sphere:"));
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", "linkentoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_lightning_bolt", true },
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_cyclone", true },
                { "item_force_staff", true },
                { "zuus_arc_lightning", true }
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", "linkenchanger", new PriorityChanger(new List<string>
            {
                { "zuus_lightning_bolt" },
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_cyclone" },
                { "item_force_staff" },
                { "zuus_arc_lightning" },
            }));

            LinkenBreakerMenu.Target.AddItem(new MenuItem("empty", ""));

            LinkenBreakerMenu.Target.AddItem(new MenuItem("antiMagespellshield", "AntiMage Spell Shield:"));
            AntiMageBreakerToggler = LinkenBreakerMenu.Item("Use: ", "antimagetoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_lightning_bolt", true },
                { "item_rod_of_atos", true},
                { "item_cyclone", true },
                { "item_force_staff", true },
                { "zuus_arc_lightning", true }
            }));

            AntiMageBreakerChanger = LinkenBreakerMenu.Item("Priority: ", "antimagechanger", new PriorityChanger(new List<string>
            {
                { "zuus_lightning_bolt" },
                { "item_rod_of_atos" },
                { "item_cyclone" },
                { "item_force_staff" },
                { "zuus_arc_lightning" }
            }));

            UseOnlyFromRangeItem = LinkenBreakerMenu.Item("Use Only From Range", false);
            UseOnlyFromRangeItem.Item.SetTooltip("Use only from the Range and do not use another Ability");

            var TeleportBreakerMenu = Factory.MenuWithTexture("Teleport Breaker", "item_tpscroll");
            TeleportBreakerItem = TeleportBreakerMenu.Item("Enable", true);
            TeleportBreakerToggler = TeleportBreakerMenu.Item("Use: ", "linkentoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_cloud", false },
                { "zuus_lightning_bolt", true }
            }));

            NimbusRangeItem = TeleportBreakerMenu.Item("Nimbus Range", new Slider(2500, 1000, 5000));
            
            var DrawingMenu = Factory.Menu("Drawing");
            var TargetMenu = DrawingMenu.Menu("Target");
            TargetEffectTypeItem = TargetMenu.Item("Target Effect Type", new StringList(EffectsName));
            DrawTargetItem = TargetMenu.Item("Target Enable", true);
            TargetRedItem = TargetMenu.Item("Red", "red", new Slider(255, 0, 255));
            TargetRedItem.Item.SetFontColor(Color.Red);
            TargetGreenItem = TargetMenu.Item("Green", "green", new Slider(0, 0, 255));
            TargetGreenItem.Item.SetFontColor(Color.Green);
            TargetBlueItem = TargetMenu.Item("Blue", "blue", new Slider(0, 0, 255));
            TargetBlueItem.Item.SetFontColor(Color.Blue);

            DrawOffTargetItem = TargetMenu.Item("Off Target Enable", true);
            OffTargetRedItem = TargetMenu.Item("Red", "offred", new Slider(0, 0, 255));
            OffTargetRedItem.Item.SetFontColor(Color.Red);
            OffTargetGreenItem = TargetMenu.Item("Green", "offgreen", new Slider(255, 0, 255));
            OffTargetGreenItem.Item.SetFontColor(Color.Green);
            OffTargetBlueItem = TargetMenu.Item("Blue", "offblue", new Slider(255, 0, 255));
            OffTargetBlueItem.Item.SetFontColor(Color.Blue);

            ArcLightningRadiusItem = DrawingMenu.Item("Arc Lightning Radius", true);
            LightningBoltRadiusItem = DrawingMenu.Item("Lightning Bolt Radius", true);
            BlinkRadiusItem = DrawingMenu.Item("Blink Radius", true);

            OnDrawItem = DrawingMenu.Item("On Draw", true);
            OnDrawXItem = DrawingMenu.Item("X", new Slider(0, 0, (int)config.Screen.X + 65));
            OnDrawYItem = DrawingMenu.Item("Y", new Slider(500, 0, (int)config.Screen.Y - 90));

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            OrbwalkerItem = Factory.Item("Orbwalker", new StringList("Free", "Distance", "Default"));
            MinDisInOrbwalkItem = Factory.Item("Min Distance In Orbwalker", new Slider(350, 200, 350));
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            Factory.Target.AddItem(new MenuItem("FarmMode", "Ensage.SDK > Orbwalker > Zeus Farm Mode"));
            FarmKeyItem = Factory.Item("Farm Key", new KeyBind(32));

            BladeMailItem = Factory.Item("Blade Mail Cancel", false);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");

            AbilitiesToggler.PropertyChanged += Changed;
            ItemsToggler.PropertyChanged += Changed;
            TeleportBreakerToggler.PropertyChanged += Changed;
            DrawTargetItem.PropertyChanged += Changed;
            DrawOffTargetItem.PropertyChanged += Changed;
            OrbwalkerItem.PropertyChanged += Changed;

            Changed(null, null);
        }

        private void Changed(object sender, PropertyChangedEventArgs e)
        {
            // Thundergods Wrath
            if (AbilitiesToggler.Value.IsEnabled("zuus_thundergods_wrath"))
            {
                MinHealhItem.Item.ShowItem = true;
                MinRangeItem.Item.ShowItem = true;
            }
            else
            {
                MinHealhItem.Item.ShowItem = false;
                MinRangeItem.Item.ShowItem = false;
            }

            // Blink
            if (ItemsToggler.Value.IsEnabled("item_blink"))
            {
                BlinkActivationItem.Item.ShowItem = true;
                BlinkDistanceEnemyItem.Item.ShowItem = true;
            }
            else
            {
                BlinkActivationItem.Item.ShowItem = false;
                BlinkDistanceEnemyItem.Item.ShowItem = false;
            }

            // Teleport Breaker
            if (TeleportBreakerToggler.Value.IsEnabled("zuus_cloud"))
            {
                NimbusRangeItem.Item.ShowItem = true;
            }
            else
            {
                NimbusRangeItem.Item.ShowItem = false;
            }

            // Draw Target
            if (DrawTargetItem)
            {
                TargetRedItem.Item.ShowItem = true;
                TargetGreenItem.Item.ShowItem = true;
                TargetBlueItem.Item.ShowItem = true;
            }
            else
            {
                TargetRedItem.Item.ShowItem = false;
                TargetGreenItem.Item.ShowItem = false;
                TargetBlueItem.Item.ShowItem = false;
            }

            // Draw Off Target
            if (DrawOffTargetItem)
            {
                OffTargetRedItem.Item.ShowItem = true;
                OffTargetGreenItem.Item.ShowItem = true;
                OffTargetBlueItem.Item.ShowItem = true;
            }
            else
            {
                OffTargetRedItem.Item.ShowItem = false;
                OffTargetGreenItem.Item.ShowItem = false;
                OffTargetBlueItem.Item.ShowItem = false;
            }

            // Orbwalker Distance
            if (OrbwalkerItem.Value.SelectedValue.Contains("Distance"))
            {
                MinDisInOrbwalkItem.Item.ShowItem = true;
            }
            else
            {
                MinDisInOrbwalkItem.Item.ShowItem = false;
            }
        }

        private string[] EffectsName { get; } =
        {
            "Default",
            "Without Circle",
            "VBE",
            "Omniknight",
            "Assault",
            "Arrow",
            "Glyph",
            "Energy Orb",
            "Pentagon",
            "Beam Jagged",
            "Beam Rainbow",
            "Walnut Statue",
            "Thin Thick",
            "Ring Wave"
        };

        public string[] Effects { get; } =
        {
            "",
            "",
            "materials/ensage_ui/particles/vbe.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_omniknight.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_assault.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_arrow.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_glyph.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_energy_orb.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_pentagon.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_beam_jagged.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_beam_rainbow.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_walnut_statue.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_thin_thick.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_ring_wave.vpcf"
        };

        public void Dispose()
        {
            OrbwalkerItem.PropertyChanged -= Changed;
            DrawOffTargetItem.PropertyChanged -= Changed;
            DrawTargetItem.PropertyChanged -= Changed;
            TeleportBreakerToggler.PropertyChanged -= Changed;
            ItemsToggler.PropertyChanged -= Changed;
            AbilitiesToggler.PropertyChanged -= Changed;
            Factory.Dispose();
        }
    }
}
