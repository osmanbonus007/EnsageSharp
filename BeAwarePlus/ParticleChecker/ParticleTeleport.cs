using System;
using Color = System.Drawing.Color;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Helpers;
using Ensage.SDK.Extensions;

using SharpDX;

using BeAwarePlus.Data;
using BeAwarePlus.GlobalList;
using BeAwarePlus.Checker;
using BeAwarePlus.Menus;

namespace BeAwarePlus.ParticleChecker
{
    internal class ParticleTeleport
    {
        private SoundPlayer SoundPlayer { get; }

        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private MessageCreator MessageCreator { get; }

        private Colors Colors { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private GlobalWorld GlobalWorld { get; }

        public ParticleTeleport(
            MenuManager menumanager,
            Unit myhero,
            MessageCreator messagecreator,
            SoundPlayer soundplayer,
            Colors colors,
            GlobalMiniMap globalminiMap,
            GlobalWorld globalworld)
        {
            MenuManager = menumanager;
            MyHero = myhero;
            MessageCreator = messagecreator;
            SoundPlayer = soundplayer;
            Colors = colors;
            GlobalMiniMap = globalminiMap;
            GlobalWorld = globalworld;
        }

        public void Teleport(
            Hero Hero,
            Vector3 Position, 
            string AbilityTexturName, 
            Vector3 ParticleColor,
            bool End)
        {
            try
            {
                var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);

                var GameTime = Game.GameTime;

                var Team = Hero.Team == MyHero.Team;

                if (!Team && MenuManager.TeleportEnemyItem
                    || Team && MenuManager.TeleportAllyItem)
                {
                    if (!Team && End)
                    {
                        MessageCreator.MessageItemCreator(
                            Hero.Name.Substring("npc_dota_hero_".Length),
                            "tpscroll",
                            Game.GameTime);

                        SoundPlayer.Play("default");
                    }

                    GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                        End,
                        true,
                        "TP",
                        Position.WorldToMinimap(),
                        Hero.GetDisplayName(),
                        HeroColor));

                    UpdateManager.BeginInvoke(
                        () =>
                        {
                            GlobalMiniMap.MiniMapList.RemoveAll(
                                x =>
                                x.GetRemoveTime == GameTime);
                        },
                        MenuManager.TimerItem.Value * 1000);

                    if (!Team)
                    {
                        GlobalWorld.WorldList.Add(new GlobalWorld.World(
                            "TP",
                            Position,
                            Hero.Name.Substring("npc_dota_hero_".Length),
                            AbilityTexturName));

                        UpdateManager.BeginInvoke(
                            () =>
                            {
                                GlobalWorld.WorldList.RemoveAll(
                                    x =>
                                    x.GetRemoveTime == GameTime);
                            },
                            MenuManager.TimerItem.Value * 1000);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //ignore
            }
        }
    }
}
