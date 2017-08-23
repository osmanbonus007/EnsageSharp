using Ensage;
using System;
using System.Collections.Generic;
using VisageSharpRewrite.Abilities;
using VisageSharpRewrite.Utilities;

namespace VisageSharpRewrite
{
    public static class Variables
    {
        public static Team EnemyTeam { get; set; }

        public static Hero Hero { get; set; }

        public static List<Unit> Familiars { get; set; }

        public static GraveChill graveChill;

        public static SoulAssumption soulAssumption;

        public static FamiliarControl familiarControl;

        public static MenuManager MenuManager { get; set; }

        public static PowerTreadsSwitcher PowerTreadsSwitcher { get; set; }

        public static float TickCount
        {
            get
            {
                return Environment.TickCount & int.MaxValue;
            }
        }

        public static bool InAutoLasthiMode
        {
            get
            {
                return MenuManager.AutoFamiliarLastHitOn;
            }
        }

        public static bool AutoSoulAumptionOn
        {
            get
            {
                return MenuManager.AutoSoulAssumpOn;
            }
        }

        public static bool ComboOn
        {
            get
            {
                return MenuManager.ComboOn;
            }
        }

        public static bool FollowMode
        {
            get
            {
                return MenuManager.FamiliarFollowOn;
            }
        }
    }
}
