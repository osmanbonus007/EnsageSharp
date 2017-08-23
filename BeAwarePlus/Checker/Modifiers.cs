using System.Drawing;

using Ensage;

using Ensage.Common;

using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;

using BeAwarePlus.Data;
using BeAwarePlus.GlobalList;
using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class Modifiers
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private Dangerous Dangerous { get; }

        private MessageCreator MessageCreator { get; }

        private SoundPlayer SoundPlayer { get; }

        private Colors Colors { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private GlobalWorld GlobalWorld { get; }
        public Modifiers(
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

        public void ModifierAlly(Unit sender, ModifierChangedEventArgs args)
        {
            if (MenuManager.SpellsItem.Value)
            {
                var HeroTexturName = sender.Name.Substring("npc_dota_hero_".Length);
                var HeroName = sender.GetDisplayName();
                var Hero = sender as Hero;
                var TextureName = args.Modifier.TextureName;
                var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);
                var DangerousSpell = Dangerous.DangerousSpellList.Contains(TextureName);
                var GameTime = Game.GameTime;

                if (Utils.SleepCheck(HeroTexturName))
                {
                    if (MenuManager.DangerousSpellsMSG.Value
                        && DangerousSpell)
                    {
                        MessageCreator.MessageAllyCreator(
                            HeroTexturName,
                            TextureName,
                            GameTime);
                    }

                    if (MenuManager.DangerousSpellsSound.Value
                        && DangerousSpell)
                    {
                        try
                        {
                            SoundPlayer.Play(TextureName);
                        }
                        catch
                        {
                            SoundPlayer.Play("default");
                        }
                    }

                    Utils.Sleep(5000, HeroTexturName);
                }

                GlobalMiniMap.MiniMapList.RemoveAll(
                    x => 
                    x.GetHeroName.Contains(HeroName));

                GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                    true,
                    false,
                    "",
                    sender.Position.WorldToMinimap(),
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
                    sender.Position,
                    HeroTexturName,
                    TextureName));

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

        public void ModifierEnemy(Unit sender, ModifierChangedEventArgs args)
        {
            if (MenuManager.SpellsItem.Value)
            {
                var HeroTexturName = sender.Name.Substring("npc_dota_hero_".Length);
                var HeroName = sender.GetDisplayName();
                var Hero = sender as Hero;
                var TextureName = args.Modifier.TextureName;
                var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);
                var DangerousSpell = Dangerous.DangerousSpellList.Contains(TextureName);
                var GameTime = Game.GameTime;

                if (Utils.SleepCheck(HeroTexturName))
                {
                    if (MenuManager.DangerousSpellsMSG.Value
                        && DangerousSpell)
                    {
                        MessageCreator.MessageAllyCreator(
                            HeroTexturName,
                            TextureName,
                            GameTime);
                    }

                    if (MenuManager.DangerousSpellsSound.Value
                        && DangerousSpell)
                    {
                        try
                        {
                            SoundPlayer.Play(TextureName);
                        }
                        catch
                        {
                            SoundPlayer.Play("default");
                        }
                    }

                    Utils.Sleep(5000, HeroTexturName);
                }

                GlobalMiniMap.MiniMapList.RemoveAll(
                    x => 
                    x.GetHeroName.Contains(HeroName));

                GlobalMiniMap.MiniMapList.Add(new GlobalMiniMap.MiniMap(
                    true,
                    false,
                    "",
                    sender.Position.WorldToMinimap(),
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
                    sender.Position,
                    HeroTexturName,
                    TextureName));

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

        public void ModifierOthers(Unit sender, ModifierChangedEventArgs args)
        {
            if (MenuManager.ItemsItem.Value)
            {
                var HeroTexturName = sender.Name.Substring("npc_dota_hero_".Length);
                var HeroName = sender.GetDisplayName();
                var TextureName = args.Modifier.TextureName;
                var Hero = sender as Hero;
                var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);
                var DangerousItem = Dangerous.DangerousItemList.Contains(TextureName);
                var GameTime = Game.GameTime;

                if (MenuManager.DangerousItemsMSG.Value
                        && DangerousItem)
                {
                    if (TextureName.Contains("item_"))
                    {
                        MessageCreator.MessageItemCreator(
                        HeroTexturName,
                        TextureName.Substring("item_".Length),
                        GameTime);
                    }
                    else
                    {
                        MessageCreator.MessageRuneCreator(
                        HeroTexturName,
                        TextureName,
                        GameTime);
                    }
                }

                if (MenuManager.DangerousItemsSound.Value
                        && DangerousItem)
                {
                    try
                    {
                        SoundPlayer.Play(TextureName);
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
                    sender.Position.WorldToMinimap(),
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
                    sender.Position,
                    HeroTexturName,
                    TextureName));

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
}
