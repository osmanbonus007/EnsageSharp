using Ensage;
using global::LoneDruidSharpRewrite.Abilities;
using global::LoneDruidSharpRewrite.Utilities;
using System;

namespace LoneDruidSharpRewrite
{
    public static class Variable
    {
        

        public static bool AutoTalonActive
        {
            get
            {
                return MenuManager.AutoTalonActive;
            }
        }

        public static bool OnlyBearLastHitActive
        {
            get
            {
                return MenuManager.OnlyBearLastHitModeOn;
            }
        }

        public static bool CombinedLastHitActive
        {
            get
            {
                return MenuManager.CombineLastHitModeOn;
            }
        }

        public static bool AutoMidasModeOn
        {
            get
            {
                return MenuManager.AutoMidasModeOn;
            }
        }

        public static bool BearChaseModeOn
        {
            get
            {
                return MenuManager.BearChaseModeOn;
            }
        }

        public static Hero Hero { get; set; }

        public static Unit Bear { get; set; }

        public static SummonSpiritBear SummonSpiritBear { get; set; }

        public static Rabid Rabid { get; set; }

        public static MenuManager MenuManager { get; set; }

        public static Team EnemyTeam{ get; set; }

        public static float TickCount
        {
            get
            {
                return Environment.TickCount & int.MaxValue;
            }
        }
      
    }
}
