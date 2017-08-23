using System;
using System.Drawing;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Helpers;
using Ensage.SDK.Extensions;

using BeAwarePlus.Data;
using BeAwarePlus.GlobalList;
using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class Entities
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private Dangerous Dangerous { get; }

        private MessageCreator MessageCreator { get; }

        private SoundPlayer SoundPlayer { get; }

        private Colors Colors { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private GlobalWorld GlobalWorld { get; }

        public Entities(
            MenuManager menumanager,
            Unit myhero,
            Dangerous dangerous,
            MessageCreator messagecreator,
            SoundPlayer soundplayer,
            Colors colors,
            GlobalMiniMap globalminimap,
            GlobalWorld globalworld)
        {
            MenuManager = menumanager;
            MyHero = myhero;
            Dangerous = dangerous;
            MessageCreator = messagecreator;
            SoundPlayer = soundplayer;
            Colors = colors;
            GlobalMiniMap = globalminimap;
            GlobalWorld = globalworld;
        }

        public void Entity(
            Hero Hero, 
            EntityEventArgs Args, 
            string AbilityTexturName)
        {
            if (Hero == null)
            {
                var HeroTexturName = "default";
                var HeroName = "Unknown";
                var HeroColor = Color.Red;
                var GameTime = Game.GameTime;
                var MinimapPos = Args.Entity.Position.WorldToMinimap();

                GlobalMiniMap.MiniMapList.RemoveAll(x => 
                x.GetHeroName.Contains(HeroName));

                GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                    true,
                    false,
                    "",
                    MinimapPos, 
                    HeroName, 
                    HeroColor));

                UpdateManager.BeginInvoke(
                    () =>
                    {
                        GlobalMiniMap.MiniMapList.RemoveAll(x => 
                        x.GetRemoveTime == GameTime);
                    },
                    MenuManager.TimerItem.Value * 1000);

                var WorldPos = Args.Entity.Position;

                GlobalWorld.WorldList.RemoveAll(x => 
                x.GetHeroTexturName.Contains(HeroTexturName));

                GlobalWorld.WorldList.Add(new GlobalWorld.World(
                    "",
                    WorldPos, 
                    HeroTexturName, 
                    AbilityTexturName));

                UpdateManager.BeginInvoke(
                    () =>
                    {
                        GlobalWorld.WorldList.RemoveAll(x => 
                        x.GetRemoveTime == GameTime);
                    },
                    MenuManager.TimerItem.Value * 1000);

                return;
            }


            try
            {
                var DangerousSpell = Dangerous.DangerousSpellList.Contains(AbilityTexturName);

                if (MenuManager.SpellsItem.Value
                    && Hero.Team != MyHero.Team
                    && (!Hero.IsVisible
                    || MenuManager.DangerousSpellsItem.Value
                    && DangerousSpell))
                {
                    var HeroTexturName = Hero.Name.Substring("npc_dota_hero_".Length);
                    var HeroName = Hero.GetDisplayName();
                    var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                    var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);
                    var GameTime = Game.GameTime;

                    if (MenuManager.DangerousSpellsMSG.Value
                        && DangerousSpell)
                    {
                        MessageCreator.MessageEnemyCreator(
                            HeroTexturName,
                            AbilityTexturName,
                            GameTime);
                    }

                    if (MenuManager.DangerousSpellsSound.Value
                        && DangerousSpell)
                    {
                        try
                        {
                            SoundPlayer.Play(AbilityTexturName);
                        }
                        catch
                        {
                            SoundPlayer.Play("default");
                        }
                    }

                    GlobalMiniMap.MiniMapList.RemoveAll(
                        x =>
                        x.GetHeroName.Contains(HeroName));

                    GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                        true,
                        false,
                        "",
                        Args.Entity.Position.WorldToMinimap(),
                        HeroName,
                        HeroColor));

                    UpdateManager.BeginInvoke(
                        () =>
                        {
                            GlobalMiniMap.MiniMapList.RemoveAll(
                                x =>
                                x.GetRemoveTime == GameTime);
                        },
                        MenuManager.TimerItem.Value * 1000);

                    GlobalWorld.WorldList.RemoveAll(
                        x =>
                        x.GetHeroTexturName.Contains(HeroTexturName));

                    GlobalWorld.WorldList.Add(new GlobalWorld.World(
                        "",
                        Args.Entity.Position,
                        HeroTexturName,
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
            catch (Exception)
            {

            }
        }
    }
}
