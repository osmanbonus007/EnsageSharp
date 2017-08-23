using System.Security.Permissions;
namespace DotaAllCombo.Service
{
	using Addons;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal class AddonsManager
    {
        public static bool IsLoaded { get; private set; }
		//private static AutoDodge autoDodge;
		//private static AutoStack autoStack;
		private static CreepControl _creepControl;
        //private static LastHit lastHit;
        private static OthersAddons _othersAddons;
		public static void RunAddons()
		{
			if (!IsLoaded) return;

			_creepControl.RunScript();
            //autoStack.RunScript();
            //autoDodge.RunScript();
            //lastHit.RunScript();
            _othersAddons.RunScript();
		}

		public static void Load()
		{

            //autoDodge = new AutoDodge();
            //lastHit = new LastHit();
			_creepControl = new CreepControl();
			_othersAddons = new OthersAddons();

			_creepControl.Load();
			//autoStack.Load();
			//autoDodge.Load();
			_othersAddons.Load();
			IsLoaded = true;
		}

		public static void Unload()
        {
           // lastHit.Unload();
            _othersAddons.Unload();
			//autoDodge.Unload();
			//autoStack.Unload();
			_creepControl.Unload();
			IsLoaded = false;
		}
	}
}
