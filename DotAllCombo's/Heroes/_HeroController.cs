using System.Security.Permissions;

namespace DotaAllCombo.Heroes
{
    using Ensage;
	using Ensage.Common.Menu;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal class Variables
	{
		protected Hero E;
		public Hero Me;
		public Menu Menu;
	    public bool Active, CastW, CastE, Push, CastQ;
	}

    internal interface IHeroController
	{
		void Combo();
		void OnLoadEvent();
		void OnCloseEvent();
	}
}
