using System.Security.Permissions;
using DotaAllCombo.Extensions;

namespace DotaAllCombo.Service
{
	using System;
	using Ensage;
	using Ensage.Common.Menu;
	using Debug;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal class HeroSelector
    {
        public static bool IsSelected { get; set; }
		public static string HeroName { get; set; }	// Имя текущего героя
		public static object HeroInst { get; set; } // Экземпляр класса соответствующего этому герою
		public static Type HeroClass  { get; set; } // Класс соответствующий этому герою

		public static void Load()
		{
			var me = ObjectManager.LocalHero;
			Toolset.InitToolset(me);

			HeroName = Utilites.FirstUpper(Utilites.GetHeroName(me.Name)).Replace("_", "");
			
			HeroClass = Type.GetType("DotaAllCombo.Heroes." + HeroName + "Controller");
			
			if (HeroClass == null)
			{
				Print.LogMessage.Error("[" + HeroName + "Controller] not founded!");
				Print.ConsoleMessage.Error("[DotaAllCombo's] Your hero is not supported!");
				IsSelected = false;
				return;
			}
			else {
				Print.LogMessage.Success("[" + HeroName + "Controller] Initialized!");
			}
			
			// Создаем экземпляр класса контрола героя
			HeroInst = Activator.CreateInstance(HeroClass);
			
			// Инициализация героя в его классе
			HeroClass.GetField("Me").SetValue(HeroInst, me);
			
			// Создаем меню опций персонажа
			HeroClass.GetField("Menu").SetValue(HeroInst, new Menu(HeroName, "options", false, me.Name, HeroName != null));
			IsSelected = true;
		}

		public static void Combo()
		{
			if (!IsSelected) return;
			// Запуск метода Combo() из контрола героя
			HeroClass.GetMethod("Combo").Invoke(HeroInst, null);
		}

		public static void ControllerLoadEvent()
		{
			if (!IsSelected) return;
			// Запуск метода OnLoadEvent() из контрола героя
			HeroClass.GetMethod("OnLoadEvent").Invoke(HeroInst, null);
		}

		public static void ControllerCloseEvent()
		{
			if (!IsSelected) return;
			// Запуск метода OnCloseEvent() из контрола героя
			HeroClass.GetMethod("OnCloseEvent").Invoke(HeroInst, null);
		}

		public static void Unload()
		{
			IsSelected = false;
			HeroName   = null;
			HeroInst   = null;
			HeroClass  = null;
		}
	}
}
