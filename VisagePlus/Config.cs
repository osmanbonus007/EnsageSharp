using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using VisagePlus.Features;

namespace VisagePlus
{
    internal class Config : IDisposable
    {
        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuFactory DrawingMenu { get; }

        public MenuItem<bool> DrawTargetItem { get; }

        public MenuItem<Slider> TargetRedItem { get; }

        public MenuItem<Slider> TargetGreenItem { get; }

        public MenuItem<Slider> TargetBlueItem { get; }

        public MenuItem<bool> DrawOffTargetItem { get; }

        public MenuItem<Slider> OffTargetRedItem { get; }

        public MenuItem<Slider> OffTargetGreenItem { get; }

        public MenuItem<Slider> OffTargetBlueItem { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<bool> TextItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<Slider> MinDisInOrbwalkItem { get; }

        public MenuItem<KeyBind> EscapeKeyItem { get; }

        public MenuItem<KeyBind> FollowKeyItem { get; }

        public MenuItem<bool> FamiliarsFollowItem { get; }

        public MenuItem<bool> FamiliarsCourierItem { get; }

        public MenuItem<KeyBind> LastHitItem { get; }

        public MenuItem<bool> DenyItem { get; }

        public MenuItem<bool> CommonAttackItem { get; }

        public MenuItem<bool> KillStealItem { get; }

        public MenuItem<AbilityToggler> KillStealToggler { get; private set; }

        public MenuItem<bool> AutoSoulAssumptionItem { get; }

        public MenuItem<KeyBind> FamiliarsLockItem { get; }

        public MenuItem<Slider> FamiliarsLowHPItem { get; }

        public MenuItem<bool> FamiliarsStoneControlItem { get; }

        public MenuItem<Slider> FamiliarsChargeItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<AbilityToggler> AntimageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntimageBreakerChanger { get; }

        public MenuItem<bool> BladeMailItem { get; }

        public Data Data { get; }

        public LinkenBreaker LinkenBreaker { get; }

        public FamiliarsControl FamiliarsControl { get; }

        public FamiliarsCombo FamiliarsCombo { get; }

        private FamiliarsLastHit FamiliarsLastHit { get; }

        public AutoUsage AutoUsage { get; }

        public VisagePlus VisagePlus { get; }

        public Mode Mode { get; }

        public UpdateMode UpdateMode { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }

        public Config(VisagePlus visageplus)
        {
            VisagePlus = visageplus;

            Factory = MenuFactory.CreateWithTexture("VisagePlus", "npc_dota_hero_visage");
            Factory.Target.SetFontColor(Color.Aqua);
            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "visage_summon_familiars_stone_form", true },
                { "visage_soul_assumption", true },
                { "visage_grave_chill", true }
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_armlet", true },
                { "item_necronomicon_3", true },
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_solar_crest", true },
                { "item_medallion_of_courage", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "visage_soul_assumption", true },
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "visage_soul_assumption" },
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            var AntimageBreakerMenu = Factory.MenuWithTexture("Anti Mage Breaker", "antimage_spell_shield");
            AntimageBreakerToggler = AntimageBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "visage_soul_assumption", true },
                { "item_rod_of_atos", true},
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            AntimageBreakerChanger = AntimageBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "visage_soul_assumption" },
                { "item_rod_of_atos" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            var AutoUsageMenu = Factory.Menu("Auto Usage");
            KillStealItem = AutoUsageMenu.Item("Auto Kill Steal", true);
            KillStealToggler = AutoUsageMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_dagon_5", true },
                { "visage_soul_assumption", true}
            }));

            AutoSoulAssumptionItem = AutoUsageMenu.Item("Auto Soul Assumption", true);

            var FamiliarsMenu = Factory.Menu("Familiars");
            var FamiliarsLastHitMenu = FamiliarsMenu.Menu("Last Hit");
            LastHitItem = FamiliarsLastHitMenu.Item("LastHit Key", new KeyBind('W', KeyBindType.Toggle, false));
            DenyItem = FamiliarsLastHitMenu.Item("Deny", true);
            CommonAttackItem = FamiliarsLastHitMenu.Item("Common Attack", true);

            FamiliarsLockItem = FamiliarsMenu.Item("Familiars Target Lock Key", new KeyBind('F', KeyBindType.Toggle, false));
            FollowKeyItem = FamiliarsMenu.Item("Follow Key", new KeyBind('E', KeyBindType.Toggle, false));
            FamiliarsFollowItem = FamiliarsMenu.Item("Follow Mouse Position", true);
            FamiliarsCourierItem = FamiliarsMenu.Item("Attack Courier", true);
            FamiliarsFollowItem.Item.SetTooltip("When Combo if there is No Enemy then Follow Mouse Position, Otherwise he Returns to the Hero");
            FamiliarsLowHPItem = FamiliarsMenu.Item("Low HP %", new Slider(30, 0, 80));
            FamiliarsStoneControlItem = FamiliarsMenu.Item("Stone Form Control", false);
            FamiliarsStoneControlItem.Item.SetTooltip("Stone will work looking at the amount of charge");
            FamiliarsChargeItem = FamiliarsMenu.Item("Damage Charge", new Slider(3, 0, 5));

            DrawingMenu = Factory.Menu("Drawing");
            var TargetMenu = DrawingMenu.Menu("Target");
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

            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");
            TextItem = DrawingMenu.Item("Text", true);

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            MinDisInOrbwalkItem = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));
            EscapeKeyItem = Factory.Item("Escape Key", new KeyBind('0'));

            BladeMailItem = Factory.Item("Blade Mail Cancel", false);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);

            Mode = new Mode(VisagePlus.Context, Key, this);
            VisagePlus.Context.Orbwalker.RegisterMode(Mode);

            Data = new Data();
            LinkenBreaker = new LinkenBreaker(this);
            FamiliarsControl = new FamiliarsControl(this);
            FamiliarsCombo = new FamiliarsCombo(this);
            FamiliarsLastHit = new FamiliarsLastHit(this);
            AutoUsage = new AutoUsage(this);
            UpdateMode = new UpdateMode(this);
            Renderer = new Renderer(this);
        }

        private void HotkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            FollowKeyItem.Item.SetValue(new KeyBind(
               FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

            LastHitItem.Item.SetValue(new KeyBind(
                LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

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
                Factory.Dispose();
                Renderer.Dispose();
                UpdateMode.Dispose();
                FamiliarsLastHit.Dispose();
                AutoUsage.Dispose();
                FamiliarsControl.Dispose();
                VisagePlus.Context.Orbwalker.UnregisterMode(Mode);
                FamiliarsCombo.Dispose();
                Mode.Deactivate();
                VisagePlus.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
            }

            Disposed = true;
        }
    }
}
