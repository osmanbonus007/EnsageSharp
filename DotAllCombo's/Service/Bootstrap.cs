namespace DotaAllCombo.Service
{
	using System;
	using Ensage;
	using Ensage.Common;
	using Debug;

    internal class Bootstrap
	{
		//private const uint LEN_THREADS = 2;

		//private static Thread[] test = new Thread[LEN_THREADS];

		public static void Initialize()
		{
			Events.OnLoad += OnLoadEvent;
			Events.OnClose += OnCloseEvent;
		}

		private static void OnUpdateEvent(EventArgs args)
		{
			try
			{
				AddonsManager.RunAddons();

				HeroSelector.Combo();
			}
			catch (Exception)
			{
				// e.GetBaseException();
			}
		}

		private static void OnLoadEvent(object sender, EventArgs e)
		{
			try
			{
				AddonsManager.Load();
				HeroSelector.Load();
				HeroSelector.ControllerLoadEvent();
				MainMenu.Load();
				Game.OnUpdate += OnUpdateEvent;

			}
			catch (Exception)
			{
				// e.GetBaseException();
			}
		} // OnLoad

		private static void OnCloseEvent(object sender, EventArgs e)
		{
			try
			{
				Game.OnUpdate -= OnUpdateEvent;
				HeroSelector.ControllerCloseEvent();
				MainMenu.Unload();
				HeroSelector.Unload();

				// Выгрузка аддонов
				AddonsManager.Unload();

				Print.ConsoleMessage.Info("> DotAllCombo's is waiting for the next game to start.");
			}
			catch (Exception)
			{
				// e.GetBaseException();
			}
		} // OnClose
	}
}
