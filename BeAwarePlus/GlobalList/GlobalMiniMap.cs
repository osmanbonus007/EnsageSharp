using System.Collections.Generic;

using Ensage;

using SharpDX;

namespace BeAwarePlus.GlobalList
{
    internal class GlobalMiniMap
    {
        public List<MiniMap> MiniMapList { get; } = new List<MiniMap>();

        public class MiniMap
        {
            public bool GetEnd { get; }

            public bool GetTeleport { get; }

            public string GetParticleName { get; }

            public Vector2 GetMinimapPos { get; }

            public string GetHeroName { get; }

            public System.Drawing.Color GetHeroColor { get; }

            public float GetRemoveTime { get; }

            public MiniMap(
                bool end, 
                bool teleport,
                string particlename,
                Vector2 minimappos, 
                string heroname, 
                System.Drawing.Color herocolor)
            {
                GetEnd = end;
                GetTeleport = teleport;
                GetParticleName = particlename;
                GetMinimapPos = minimappos;
                GetHeroName = heroname;
                GetHeroColor = herocolor;
                GetRemoveTime = Game.GameTime;
            }
        }
    }
}
