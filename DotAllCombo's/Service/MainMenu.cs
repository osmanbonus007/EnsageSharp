using System.Security.Permissions;
using DotaAllCombo.Extensions;

namespace DotaAllCombo.Service
{
	using SharpDX;
	using Ensage.Common.Menu;

	using Debug;

	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	internal class MainMenu
	{
	    // private static readonly Menu dodgeMenu = new Menu("AutoDodgeSpells", "Auto Dodge All Spell's");
        //private static readonly Menu stackMenu = new Menu("Stack Camp's", "Stack Camp's");
	    public static Menu Menu { get; private set; }

	    public static Menu AddonsMenu { get; private set; }

	    // public static Menu LastHitMenu { get { return _lastHitMenu; } }
        public static Menu GlobalSetting { get; private set; }

	    public static Menu KeySetting { get; private set; }

	    //public static Menu DodgeMenu { get { return dodgeMenu; } }
        //public static Menu StackMenu { get { return stackMenu; } }
        public static Menu OthersMenu { get; } = new Menu("Others Addon's", "Others Addon's");

	    public static Menu CcMenu { get; } = new Menu("Auto Controll All Unit's", "Auto Controll All Unit's");

	    public static void Load()
		{
			// Инициализируем главное меню
			Menu = new Menu("DotaAllCombo's", "menuName", true);
			AddonsMenu = new Menu("Addons", "_addonsMenu");

          //  _lastHitMenu = new Menu("LastHit", "_lastHitMenu");

            KeySetting = new Menu("Keys Setting", "Keys Setting");
            GlobalSetting = new Menu("Global Setting", "Global Setting");
            //Menu ul = new Menu("Escape", "Auto Escape Target Attack");
            // Инициализация меню для аддонов
            //dodgeMenu.AddItem(new MenuItem("dodge", "Auto Dodge Spell's").SetValue(true));
            //_addonsMenu.AddSubMenu(dodgeMenu);

            OthersMenu.AddItem(new MenuItem("others", "Others Addon's").SetValue(true));
			OthersMenu.AddItem(new MenuItem("ShowTargetMarker", "Show Target Marker").SetValue(true));
			OthersMenu.AddItem(new MenuItem("ShowAttakRange", "Show AttackRange").SetValue(true));

			//ul.AddItem(new MenuItem("minLVL", "Min Hero Level to Escape").SetValue(new Slider(7, 5, 25)));
			//ul.AddItem(new MenuItem("EscapeAttack", "Auto Escape Target Attack").SetValue(true));
			OthersMenu.AddItem(new MenuItem("Auto Un Aggro", "Auto Un Aggro Towers|Fountain").SetValue(true));
			//othersMenu.AddSubMenu(ul);
			AddonsMenu.AddSubMenu(OthersMenu);

			//stackMenu.AddItem(new MenuItem("Stack", "Stack Camp's").SetValue(new KeyBind('T', KeyBindType.Toggle)));
			//stackMenu.AddItem(new MenuItem("mepos", "Stack Meepo's Camp's").SetValue(false));
			//stackMenu.AddItem(new MenuItem("mestack", "Stack Me Camp's").SetValue(false));
			//_addonsMenu.AddSubMenu(stackMenu);

			CcMenu.AddItem(new MenuItem("controll", "Auto Controll Unit's").SetValue(true));
            KeySetting.AddItem(new MenuItem("Toogle Key", "Toogle Key").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            KeySetting.AddItem(new MenuItem("Press Key", "Press Key").SetValue(new KeyBind('F', KeyBindType.Press)));
            KeySetting.AddItem(new MenuItem("Lock target Key", "Lock target Key").SetValue(new KeyBind('G', KeyBindType.Press)).SetTooltip("Lock a target closest mouse."));
            CcMenu.AddSubMenu(KeySetting);
            GlobalSetting.AddItem(new MenuItem("Target find range", "Target find range").SetValue(new Slider(1550, 0, 2000)).SetTooltip("Range from mouse to find TargetNow Hero."));
            GlobalSetting.AddItem(new MenuItem("Target mode", "Target mode").SetValue(new StringList(new[] { "ClosesFindSource", "LowestHealth" })));
            GlobalSetting.AddItem(new MenuItem("Target find source", "Target find source").SetValue(new StringList(new[] { "Me", "Mouse" })));
            GlobalSetting.AddItem(new MenuItem("Delete lock target when Off", "Delete lock target when Off").SetValue(false));
            CcMenu.AddSubMenu(GlobalSetting);
            AddonsMenu.AddSubMenu(CcMenu);
			
			// Добавление меню с аддонами в главное
			Menu.AddSubMenu(AddonsMenu);

           // _lastHitMenu.AddItem(new MenuItem("LastOn", "Auto LastHit creeps").SetValue(true));
           // _lastHitMenu.AddItem(new MenuItem("LastHitKey", "LastHit Key").SetValue(new KeyBind('C', KeyBindType.Press)));
           // _mainMenu.AddSubMenu(_lastHitMenu);


            if (!HeroSelector.IsSelected)
			{
				Print.ConsoleMessage.Success("[DotaAllCombo's] Initialization complete!"); return;
			}
			
			// Подключаем меню настроек героя
			Menu.AddSubMenu((Menu)HeroSelector.HeroClass.GetField("Menu").GetValue(HeroSelector.HeroInst));

            // Выводим автора текущего скрипта
            // _mainMenu.AddItem(new MenuItem("scriptAutor", HeroSelector.HeroName + " by " + AssemblyExtensions.GetAuthor(), true).SetFontStyle(
			//	System.Drawing.FontStyle.Bold, Color.Coral));
            // Выводим версию текущего скрипта
            //_mainMenu.AddItem(new MenuItem("scriptVersion", "Version " + AssemblyExtensions.GetVersion()).SetFontStyle(
			//	System.Drawing.FontStyle.Bold, Color.Coral));
            Menu.AddItem(new MenuItem("scriptAutor", HeroSelector.HeroName + " by " + AssemblyExtensions.GetAuthor(), true).SetFontColor(Color.Coral));
            // Выводим версию текущего скрипта
            Menu.AddItem(new MenuItem("scriptVersion", "Version " + AssemblyExtensions.GetVersion()).SetFontColor(Color.Coral));
            Menu.AddToMainMenu();

			Print.ConsoleMessage.Success("[DotaAllCombo's] Initialization complete!");
		}

		public static void Unload()
		{
			if (Menu == null) return;
			Menu.RemoveFromMainMenu();
			Menu = null;
		}
	}
}
