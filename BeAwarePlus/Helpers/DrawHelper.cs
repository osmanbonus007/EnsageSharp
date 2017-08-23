using Color = System.Drawing.Color;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Helpers;

using SharpDX;

using BeAwarePlus.GlobalList;
using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class DrawHelper
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private GlobalWorld GlobalWorld { get; }

        public DrawHelper(
            MenuManager menumanager,
            Unit myhero,
            GlobalMiniMap globalminiMap,
            GlobalWorld globalworld)
        {
            MenuManager = menumanager;
            MyHero = myhero;
            GlobalMiniMap = globalminiMap;
            GlobalWorld = globalworld;
        }

        public void Draw(
            Hero Hero, 
            string ParticleName, 
            string AbilityTexturName,
            string HeroTextureName, 
            string HeroName, 
            Color HeroColor, 
            Vector3 Position)
        {
            var GameTime = Game.GameTime;

            GlobalMiniMap.MiniMapList.RemoveAll(
                x =>
                x.GetParticleName.Contains(ParticleName));

            GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                End(ParticleName),
                false,
                ParticleName,
                Position.WorldToMinimap(),
                HeroName,
                HeroColor));

            UpdateManager.BeginInvoke(
                () =>
                {
                    GlobalMiniMap.MiniMapList.RemoveAll(x =>
                    x.GetRemoveTime == GameTime);
                },
                MenuManager.TimerItem.Value * 1000);

            GlobalWorld.WorldList.RemoveAll(x =>
            x.GetParticleName.Contains(ParticleName));

            GlobalWorld.WorldList.Add(new GlobalWorld.World(
                ParticleName,
                Position,
                HeroTextureName,
                AbilityTexturName));

            UpdateManager.BeginInvoke(
                () =>
                {
                    GlobalWorld.WorldList.RemoveAll(x =>
                    x.GetRemoveTime == GameTime);
                },
                MenuManager.TimerItem.Value * 1000);
        }

        private bool End(string ParticleName)
        {
            return !ParticleName.Contains("furion_teleport.vpcf")
                && !ParticleName.Contains("wisp_relocate_channel.vpcf")
                && !ParticleName.Contains("abbysal_underlord_darkrift_ambient.vpcf");
        }
    }
}
