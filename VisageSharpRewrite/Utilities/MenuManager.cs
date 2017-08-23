using Ensage.Common.Menu;

namespace VisageSharpRewrite.Utilities
{
    public class MenuManager
    {
        public Menu Menu { get; private set; }

        public readonly MenuItem AutoFamiliarLastHitMenu;

        public readonly MenuItem AutoSoulAssumpMenu;

        public readonly MenuItem ComboMenu;

        public readonly MenuItem FamiliarFollowMenu;

        public readonly MenuItem FamiliarsLowHP;

        public MenuManager(string heroName)
        {
            this.Menu = new Menu("VisageSharp", "VisageSharp", true, heroName, true);
            this.AutoFamiliarLastHitMenu = new MenuItem("Auto Familar Lasthit", "Auto Familar Lasthit").SetValue(new KeyBind('W', KeyBindType.Toggle));
            this.AutoSoulAssumpMenu = new MenuItem("AutoSoulAssump", "AutoSoulAssump").SetValue(new KeyBind('X', KeyBindType.Toggle, true)).SetTooltip("Max Damage Mode/KillSteal");
            this.ComboMenu = new MenuItem("Combo", "ComboMode").SetValue(new KeyBind('D', KeyBindType.Press)).SetTooltip("Combo Mode");
            this.FamiliarFollowMenu = new MenuItem("FamiliarFollow", "FamiliarFollow").SetValue(new KeyBind('E', KeyBindType.Toggle, true)).SetTooltip("let familiars follow you in position, but never auto-attack");
            this.FamiliarsLowHP = new MenuItem("FamiliarsLowHP", "Familiars Low HP").SetValue(new Slider(300, 0, 600));
            this.Menu.AddItem(AutoFamiliarLastHitMenu);
            this.Menu.AddItem(AutoSoulAssumpMenu);
            this.Menu.AddItem(ComboMenu);
            this.Menu.AddItem(FamiliarFollowMenu);
            this.Menu.AddItem(FamiliarsLowHP);
        }

        public bool AutoFamiliarLastHitOn
        {
            get
            {
                return this.AutoFamiliarLastHitMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool AutoSoulAssumpOn
        {
            get
            {
                return this.AutoSoulAssumpMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool ComboOn
        {
            get
            {
                return this.ComboMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool FamiliarFollowOn
        {
            get
            {
                return this.FamiliarFollowMenu.GetValue<KeyBind>().Active;
            }
        }


    }
}
