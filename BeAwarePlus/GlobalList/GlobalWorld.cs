using System.Collections.Generic;

using Ensage;

using SharpDX;

namespace BeAwarePlus.GlobalList
{
    internal class GlobalWorld
    {
        public List<World> WorldList { get; } = new List<World>();

        public class World
        {
            public string GetParticleName { get; }

            public Vector3 GetWorldPos { get; }

            public string GetHeroTexturName { get; }

            public string GetAbilityTexturName { get; }

            public float GetRemoveTime { get; }

            public World(
                string particlename,
                Vector3 worldpos,
                string herotexturname, 
                string abilitytexturname)
            {
                GetParticleName = particlename;
                GetWorldPos = worldpos;
                GetHeroTexturName = herotexturname;
                GetAbilityTexturName = abilitytexturname;
                GetRemoveTime = Game.GameTime;
            }
        }
    }
}
