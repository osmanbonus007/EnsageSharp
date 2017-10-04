using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using VisagePlus.Features;

namespace VisagePlus
{
    internal class Config : IDisposable
    {
        public VisagePlus Main { get; }

        public Vector2 Screen { get; }

        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuItem<AbilityToggler> AntiMageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntiMageBreakerChanger { get; }

        public MenuItem<bool> UseOnlyFromRangeItem { get; }

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

        public MenuItem<Slider> TextXItem { get; }

        public MenuItem<Slider> TextYItem { get; }

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

        public MenuItem<bool> BladeMailItem { get; }

        public UpdateMode UpdateMode { get; }

        private Mode Mode { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private FamiliarsControl FamiliarsControl { get; }

        private FamiliarsCombo FamiliarsCombo { get; }

        private FamiliarsLastHit FamiliarsLastHit { get; }

        private AutoAbility AutoAbility { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }

        public Config(VisagePlus main)
        {
            Main = main;
            Screen = new Vector2(Drawing.Width - 160, Drawing.Height);

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
                { "item_heavens_halberd", true },
                { "item_hurricane_pike", true },
                { "item_solar_crest", true },
                { "item_medallion_of_courage", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerMenu.Target.AddItem(new MenuItem("linkensphere", "Linkens Sphere:"));
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", "linkentoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "visage_soul_assumption", true },
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_heavens_halberd", true },
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", "linkenchanger", new PriorityChanger(new List<string>
            {
                { "visage_soul_assumption" },
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_heavens_halberd" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            LinkenBreakerMenu.Target.AddItem(new MenuItem("empty", ""));

            LinkenBreakerMenu.Target.AddItem(new MenuItem("antiMagespellshield", "AntiMage Spell Shield:"));
            AntiMageBreakerToggler = LinkenBreakerMenu.Item("Use: ", "antimagetoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "visage_soul_assumption", true },
                { "item_rod_of_atos", true},
                { "item_heavens_halberd", true },
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            AntiMageBreakerChanger = LinkenBreakerMenu.Item("Priority: ", "antimagechanger", new PriorityChanger(new List<string>
            {
                { "visage_soul_assumption" },
                { "item_rod_of_atos" },
                { "item_heavens_halberd" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            UseOnlyFromRangeItem = LinkenBreakerMenu.Item("Use Only From Range", false);
            UseOnlyFromRangeItem.Item.SetTooltip("Use only from the Range and do not use another Ability");

            var AutoAbilityMenu = Factory.Menu("Auto Ability");
            KillStealItem = AutoAbilityMenu.Item("Auto Kill Steal", true);
            KillStealToggler = AutoAbilityMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_dagon_5", true },
                { "visage_soul_assumption", true}
            }));

            AutoSoulAssumptionItem = AutoAbilityMenu.Item("Auto Soul Assumption", true);

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

            var DrawingMenu = Factory.Menu("Drawing");
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
            TextXItem = DrawingMenu.Item("X", new Slider(0, 0, (int)Screen.X - 60));
            TextYItem = DrawingMenu.Item("Y", new Slider(0, 0, (int)Screen.Y - 200));

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            MinDisInOrbwalkItem = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));
            EscapeKeyItem = Factory.Item("Escape Key", new KeyBind('0'));

            BladeMailItem = Factory.Item("Blade Mail Cancel", false);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            UpdateMode = new UpdateMode(this);

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);
            Mode = new Mode(Main.Context, Key, this);
            Main.Context.Orbwalker.RegisterMode(Mode);

            LinkenBreaker = new LinkenBreaker(this);
            FamiliarsControl = new FamiliarsControl(this);
            FamiliarsCombo = new FamiliarsCombo(this);
            FamiliarsLastHit = new FamiliarsLastHit(this);
            AutoAbility = new AutoAbility(this);
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
                Renderer.Dispose();
                AutoAbility.Dispose();
                FamiliarsLastHit.Dispose();
                FamiliarsCombo.Dispose();
                FamiliarsControl.Dispose();
                UpdateMode.Dispose();
                Main.Context.Orbwalker.UnregisterMode(Mode);
                Main.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
