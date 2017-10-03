using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using SilencerPlus.Features;

namespace SilencerPlus
{
    internal class Config : IDisposable
    {
        public SilencerPlus Main { get; }

        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

        public MenuItem<AbilityToggler> AntiMageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntiMageBreakerChanger { get; }

        public MenuItem<bool> UseOnlyFromRangeItem { get; }

        public MenuItem<bool> GlobalSilenceItem { get; }

        public MenuItem<AbilityToggler> GlobalSilenceToggler { get; }

        public MenuItem<bool> DrawTargetItem { get; }

        public MenuItem<Slider> TargetRedItem { get; }

        public MenuItem<Slider> TargetGreenItem { get; }

        public MenuItem<Slider> TargetBlueItem { get; }

        public MenuItem<bool> DrawOffTargetItem { get; }

        public MenuItem<Slider> OffTargetRedItem { get; }

        public MenuItem<Slider> OffTargetGreenItem { get; }

        public MenuItem<Slider> OffTargetBlueItem { get; }

        public MenuItem<bool> ComboRadiusItem { get; }

        public MenuItem<KeyBind> ComboKeyItem { get; }

        public MenuItem<Slider> MinDisInOrbwalkItem { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<bool> BladeMailItem { get; }

        public UpdateMode UpdateMode { get; }

        public Mode Mode { get; }

        public AutoAbility AutoAbility { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private bool Disposed { get; set; }

        public Config(SilencerPlus main)
        {
            Main = main;

            Factory = MenuFactory.CreateWithTexture("SilencerPlus", "npc_dota_hero_silencer");
            Factory.Target.SetFontColor(Color.Aqua);

            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "silencer_last_word", true },
                { "silencer_glaives_of_wisdom", true },
                { "silencer_curse_of_the_silent", true }
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_shivas_guard", true },
                { "item_dagon_5", true },
                { "item_veil_of_discord", true },
                { "item_ethereal_blade", true },
                { "item_heavens_halberd", true },
                { "item_hurricane_pike", true },
                { "item_rod_of_atos", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerMenu.Target.AddItem(new MenuItem("linkensphere", "Linkens Sphere:"));
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", "linkentoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "silencer_last_word", true },
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
                { "silencer_last_word" },
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
                { "silencer_last_word", true },
                { "item_rod_of_atos", true},
                { "item_heavens_halberd", true },
                { "item_cyclone", true },
                { "item_force_staff", true }
            }));

            AntiMageBreakerChanger = LinkenBreakerMenu.Item("Priority: ", "antimagechanger", new PriorityChanger(new List<string>
            {
                { "silencer_last_word" },
                { "item_rod_of_atos" },
                { "item_heavens_halberd" },
                { "item_cyclone" },
                { "item_force_staff" }
            }));

            UseOnlyFromRangeItem = LinkenBreakerMenu.Item("Use Only From Range", false);
            UseOnlyFromRangeItem.Item.SetTooltip("Use only from the Range and do not use another Ability");

            var GlobalSilenceMenu = Factory.MenuWithTexture("Global Silence", "silencer_global_silence");
            GlobalSilenceItem = GlobalSilenceMenu.Item("Enable", true);
            GlobalSilenceToggler = GlobalSilenceMenu.Item("Use: ", "globalsilencetoggler", new AbilityToggler(new Dictionary<string, bool>
            {
                { "enigma_black_hole", true },
                { "witch_doctor_death_ward", true},
                { "crystal_maiden_freezing_field", true },
                { "pudge_dismember", true },
                { "sandking_epicenter", true},
                { "bane_fiends_grip", true }
            }));

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

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
            MinDisInOrbwalkItem = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));

            BladeMailItem = Factory.Item("Blade Mail Cancel", false);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            UpdateMode = new UpdateMode(this);

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);
            Mode = new Mode(Main.Context, Key, this);
            Main.Context.Orbwalker.RegisterMode(Mode);

            AutoAbility = new AutoAbility(this);
            LinkenBreaker = new LinkenBreaker(this);
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
                UpdateMode.Dispose();
                Main.Context.Orbwalker.UnregisterMode(Mode);
                AutoAbility.Dispose();
                Main.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
