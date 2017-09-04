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

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<Slider> MinDisInOrbwalk { get; }

        public MenuItem<KeyBind> FollowKeyItem { get; }

        public MenuItem<KeyBind> LastHitItem { get; }

        public MenuItem<bool> CommonAttackItem { get; }

        public MenuItem<bool> KillStealItem { get; }

        public MenuItem<AbilityToggler> KillStealToggler { get; private set; }

        public MenuItem<bool> AutoSoulAssumptionItem { get; }

        public MenuItem<Slider> FamiliarsLowHPItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<AbilityToggler> AntimageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntimageBreakerChanger { get; }

        public MenuItem<bool> BladeMailItem { get; }

        public LinkenBreaker LinkenBreaker { get; }

        public FamiliarsCombo FamiliarsCombo { get; }

        private FamiliarsFollow FamiliarsFollow { get; }

        private FamiliarsLastHit FamiliarsLastHit { get; }

        private FamiliarsLowHP FamiliarsLowHP { get; }

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

            var AntimageBreakerMenu = Factory.MenuWithTexture("Antimage Breaker", "antimage_spell_shield");
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

            var FamiliarsLastHitMenu = Factory.Menu("Familiars Last Hit");
            LastHitItem = FamiliarsLastHitMenu.Item("LastHit Key", new KeyBind('W', KeyBindType.Toggle, false));
            CommonAttackItem = FamiliarsLastHitMenu.Item("Common Attack", true);

            var DrawingMenu = Factory.Menu("Drawing");
            ComboRadiusItem = DrawingMenu.Item("Combo Stable Radius", true);
            ComboRadiusItem.Item.SetTooltip("I suggest making a combo in this radius");

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            MinDisInOrbwalk = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));
            FollowKeyItem = Factory.Item("Follow Key", new KeyBind('E', KeyBindType.Toggle, false));

            FamiliarsLowHPItem = Factory.Item("Familiars Low HP %", new Slider(30, 0, 80));
            BladeMailItem = Factory.Item("Blade Mail Cancel", false);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);

            Mode = new Mode(VisagePlus.Context, Key, this);
            VisagePlus.Context.Orbwalker.RegisterMode(Mode);

            LinkenBreaker = new LinkenBreaker(this);
            FamiliarsCombo = new FamiliarsCombo(this);
            FamiliarsFollow = new FamiliarsFollow(this);
            FamiliarsLastHit = new FamiliarsLastHit(this);
            FamiliarsLowHP = new FamiliarsLowHP(this);
            AutoUsage = new AutoUsage(this);
            UpdateMode = new UpdateMode(this);
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
                Factory.Dispose();
                Renderer.Dispose();
                UpdateMode.Dispose();
                FamiliarsFollow.Dispose();
                FamiliarsLastHit.Dispose();
                AutoUsage.Dispose();
                FamiliarsLowHP.Dispose();
                VisagePlus.Context.Orbwalker.UnregisterMode(Mode);
                Mode.Deactivate();
                VisagePlus.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
            }

            Disposed = true;
        }
    }
}
