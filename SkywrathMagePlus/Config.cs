using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using SkywrathMagePlus.Features;

namespace SkywrathMagePlus
{
    internal class Config : IDisposable
    {
        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<bool> AutoComboItem { get; }

        public MenuItem<AbilityToggler> AutoAbilitiesToggler { get; }

        public MenuItem<AbilityToggler> AutoItemsToggler { get; }

        public MenuItem<bool> AutoDisableItem { get; }

        public MenuItem<AbilityToggler> AutoDisableToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuItem<bool> TextItem { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<bool> WDrawItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<KeyBind> AutoQKeyItem { get; }

        public MenuItem<KeyBind> SpamKeyItem { get; }

        public MenuItem<bool> SpamUnitItem { get; }

        public MenuItem<bool> WWithoutFailItem { get; }

        public MenuItem<bool> WTargetItem { get; }

        public MenuItem<Slider> WRadiusItem { get; }

        public MenuItem<Slider> MinDisInOrbwalk { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<AbilityToggler> AntimageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntimageBreakerChanger { get; }

        public MenuItem<bool> BladeMailItem { get; }

        public MenuItem<bool> EulBladeMailItem { get; }

        public SkywrathMagePlus SkywrathMagePlus { get; }

        public Mode Mode { get; }

        public Data Data { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private SpamMode SpamMode { get; }

        private AutoCombo AutoCombo { get; }

        private AutoDisable AutoDisable { get; }

        public UpdateMode UpdateMode { get; }

        private WithoutFail WithoutFail { get; }

        private AutoUsage AutoUsage { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }

        public Config(SkywrathMagePlus skywrathmageplus)
        {
            SkywrathMagePlus = skywrathmageplus;

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
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var AutoComboMenu = Factory.Menu("Auto Combo");
            AutoComboItem = AutoComboMenu.Item("Use Auto Combo", true);
            AutoAbilitiesToggler = AutoComboMenu.Item("Abilities: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_mystic_flare", true },
                { "skywrath_mage_ancient_seal", true },
                { "skywrath_mage_concussive_shot", true },
                { "skywrath_mage_arcane_bolt", true }
            }));

            AutoItemsToggler = AutoComboMenu.Item("Items: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var AutoDisableMenu = Factory.MenuWithTexture("Auto Disable", "item_sheepstick");
            AutoDisableItem = AutoDisableMenu.Item("Use Auto Disable", true);
            AutoDisableToggler = AutoDisableMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_ancient_seal", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_ancient_seal", false },
                { "skywrath_mage_arcane_bolt", true },
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "skywrath_mage_ancient_seal" },
                { "skywrath_mage_arcane_bolt" },
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            var AntimageBreakerMenu = Factory.MenuWithTexture("Antimage Breaker", "antimage_spell_shield");
            AntimageBreakerToggler = AntimageBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "skywrath_mage_ancient_seal", false },
                { "skywrath_mage_arcane_bolt", true },
                { "item_rod_of_atos", true},
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            AntimageBreakerChanger = AntimageBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "skywrath_mage_ancient_seal" },
                { "skywrath_mage_arcane_bolt" },
                { "item_rod_of_atos" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            var BladeMailMenu = Factory.MenuWithTexture("Blade Mail", "item_blade_mail");
            BladeMailItem = BladeMailMenu.Item("Cancel Combo", true);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
            EulBladeMailItem = BladeMailMenu.Item("Use Eul", true);
            EulBladeMailItem.Item.SetTooltip("Use Eul if there is BladeMail with ULT");

            var ConcussiveShotMenu = Factory.MenuWithTexture("Smart Concussive Shot", "skywrath_mage_concussive_shot");
            WWithoutFailItem = ConcussiveShotMenu.Item("Without Fail", true);
            WTargetItem = ConcussiveShotMenu.Item("Use Only Target", true);
            WTargetItem.Item.SetTooltip("This only works with Combo");
            WRadiusItem = ConcussiveShotMenu.Item("Use in Radius", new Slider(1400, 800, 1600));
            WRadiusItem.Item.SetTooltip("This only works with Combo");

            var DrawingMenu = Factory.Menu("Drawing");
            TextItem = DrawingMenu.Item("Text", true);
            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");
            WDrawItem = DrawingMenu.Item("Show W Target", true);

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            AutoQKeyItem = Factory.Item("Auto Q Key", new KeyBind('F', KeyBindType.Toggle, false));
            AutoQKeyItem.Item.SetValue(new KeyBind(AutoQKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            SpamKeyItem = Factory.Item("Spam Q Key", new KeyBind('Q'));
            SpamUnitItem = Factory.Item("Spam Q Units", true);
            SpamUnitItem.Item.SetTooltip("Creeps, Neutrals and Roshan");
            MinDisInOrbwalk = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);

            Mode = new Mode(SkywrathMagePlus.Context, Key, this);
            SkywrathMagePlus.Context.Orbwalker.RegisterMode(Mode);

            Data = new Data();
            LinkenBreaker = new LinkenBreaker(this);
            SpamMode = new SpamMode(this);
            AutoCombo = new AutoCombo(this);
            AutoDisable = new AutoDisable(this);
            UpdateMode = new UpdateMode(this);
            WithoutFail = new WithoutFail(this);
            AutoUsage = new AutoUsage(this);
            Renderer = new Renderer(this);
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
                Renderer.Dispose();
                AutoUsage.Dispose();
                WithoutFail.Dispose();
                UpdateMode.Dispose();
                AutoDisable.Dispose();
                AutoCombo.Dispose();
                SkywrathMagePlus.Context.Orbwalker.UnregisterMode(Mode);
                Mode.Deactivate();
                SpamMode.Dispose();
                SkywrathMagePlus.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
