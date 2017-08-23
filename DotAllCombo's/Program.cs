namespace DotaAllCombo
{
	using System.Security.Permissions;
	internal class Program
    {
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void Main()
		{

            /*new Thread(delegate () {
				
			}).Start();*/
			Service.Bootstrap.Initialize();
			Service.Debug.Print.ConsoleMessage.Success("[DotaAllCombo's] Pre-initialization complete!");


		}
	}
}

