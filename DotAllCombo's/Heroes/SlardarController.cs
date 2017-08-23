using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
    using Service.Debug;

	class SlardarController : Variables, IHeroController
	{
		public void Combo()
		{

		}	// Combo

		public void OnLoadEvent()
		{
		AssemblyExtensions.InitAssembly("TODO", "0");
		Print.LogMessage.Error("This hero not Supported!");
		}

		public void OnCloseEvent()
		{	
		}
	}
}
