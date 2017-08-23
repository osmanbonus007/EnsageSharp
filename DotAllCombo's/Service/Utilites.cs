using System.Security.Permissions;

namespace DotaAllCombo.Service
{
	using System;

    internal class Utilites
    {
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static string GetHeroName(string name)
		{
			return name.Split(new [] { "npc_dota_hero_" }, StringSplitOptions.None)[1];
		}

		public static string FirstUpper(string str)
		{
			string[] s = str.Split(' ');

			for (int i = 0; i < s.Length; ++i)
			{
				if (s[i].Length > 1)
					s[i] = s[i].Substring(0, 1).ToUpper() + s[i].Substring(1, s[i].Length - 1).ToLower();
				else s[i] = s[i].ToUpper();
			}
			str = string.Join(" ", s);

			string[] s1 = str.Split('_');

			for (int i = 0; i < s1.Length; ++i)
			{
				if (s1[i].Length > 1)
					s1[i] = s1[i].Substring(0, 1).ToUpper() + s1[i].Substring(1, s1[i].Length - 1).ToLower();
				else s1[i] = s1[i].ToUpper();
			}
			return string.Join("_", s1);
		}
	}
}
