
using Ensage.Common.Menu;

namespace LoneDruidSharpRewrite.Utilities
{
    public class MenuManager
    {
        public readonly MenuItem OnlyBearLastHitMenu;

        public readonly MenuItem CombinedLastHitMenu;

        public readonly MenuItem AutoTalonMenu;

        public readonly MenuItem AutoMidasMenu;

        public readonly MenuItem BearChaseMenu;

        public Menu Menu { get; private set; }

        public MenuManager(string heroName)
        {
            this.Menu = new Menu("LoneDruidSharp", "LoneDruidSharp", true, "npc_dota_hero_lone_druid", true);
            this.OnlyBearLastHitMenu = new MenuItem("OnlyBearLastHitMenu", "OnlyBearLastHitMenu").SetValue(new KeyBind('W', KeyBindType.Press)).SetTooltip("only bear will go last hit");
            this.CombinedLastHitMenu = new MenuItem("CombinedLastHitMenu", "CombinedLastHitMenu").SetValue(new KeyBind('X', KeyBindType.Press)).SetTooltip("both hero and bear last hit");
            this.AutoTalonMenu = new MenuItem("Auto Iron Talon", "Auto Iron Talon").SetValue(new KeyBind('Z', KeyBindType.Toggle));
            this.AutoMidasMenu = new MenuItem("Auto Midas", "Auto Midas").SetValue(new KeyBind('Z', KeyBindType.Toggle));
            this.BearChaseMenu = new MenuItem("BearChaseMenu", "BearChaseMenu").SetValue(new KeyBind('D', KeyBindType.Press)).SetTooltip("press it and rightclick enemy, bear will keep chasing until you control bear again");
            this.Menu.AddItem(this.AutoMidasMenu);
            this.Menu.AddItem(this.OnlyBearLastHitMenu);
            this.Menu.AddItem(this.CombinedLastHitMenu);
            this.Menu.AddItem(this.AutoTalonMenu);
            this.Menu.AddItem(this.BearChaseMenu);
        }

        public bool AutoTalonActive
        {
            get
            {
                return this.AutoTalonMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool OnlyBearLastHitModeOn
        {
            get
            {
                return this.OnlyBearLastHitMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool CombineLastHitModeOn
        {
            get
            {
                return this.CombinedLastHitMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool AutoMidasModeOn
        {
            get
            {
                return this.AutoMidasMenu.GetValue<KeyBind>().Active; ;
            }
        }

        public bool BearChaseModeOn
        {
            get
            {
                return this.BearChaseMenu.GetValue<KeyBind>().Active;
            }
        }




    }
}
