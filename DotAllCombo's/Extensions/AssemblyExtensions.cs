namespace DotaAllCombo.Extensions
{
    internal class AssemblyExtensions
	{
		private static string _version;
		private static string _author;

		public static void InitAssembly(string creator, string version)
		{
			_version = version;
			_author = creator;
		}

		public static string GetVersion()
		{
			return _version;
		}

		public static string GetAuthor()
		{
			return _author;
		}

		public static bool IsInitialized()
		{
			return _version != null && _author != null;
		}

		public static void Dispose()
		{
			_version = null;
			_author = null;
		}
	}
}
