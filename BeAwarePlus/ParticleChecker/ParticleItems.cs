using System;
using System.Globalization;
using Color = System.Drawing.Color;

using Ensage;
using Ensage.SDK.Extensions;

using SharpDX;

using BeAwarePlus.Data;
using BeAwarePlus.Checker;
using BeAwarePlus.Menus;

namespace BeAwarePlus.ParticleChecker
{
    internal class ParticleItems
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private Dangerous Dangerous { get; }

        private MessageCreator MessageCreator { get; }

        private SoundPlayer SoundPlayer { get; }

        private Colors Colors { get; }

        private DrawHelper DrawHelper { get; }

        public ParticleItems(
            MenuManager menumanager,
            Unit myhero,
            Dangerous dangerous,
            MessageCreator messagecreator,
            SoundPlayer soundplayer,
            Colors colors,
            DrawHelper drawhelper)
        {
            MenuManager = menumanager;
            MyHero = myhero;
            Dangerous = dangerous;
            MessageCreator = messagecreator;
            SoundPlayer = soundplayer;
            Colors = colors;
            DrawHelper = drawhelper;
        }

        public void Items(
            Hero Hero,
            string ParticleName,
            string AbilityTexturName,
            Vector3 Position)
        {
            try
            {
                if (Hero == null)
                {
                    DrawHelper.Draw(
                        Hero,
                        ParticleName,
                        AbilityTexturName,
                        "default",
                        "Unknown",
                        Color.Red,
                        Position);

                    return;
                }
                var DangerousItems = Dangerous.DangerousItemList.Contains(AbilityTexturName);

                if (MenuManager.ItemsItem.Value
                    && Hero.Team != MyHero.Team
                    && (!Hero.IsVisible
                    || MenuManager.DangerousItemsItem.Value
                    && DangerousItems))
                {
                    var HeroName = Hero.Name.Substring("npc_dota_hero_".Length);

                    var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                    var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);

                    if (MenuManager.DangerousSpellsMSG.Value
                        && DangerousItems)
                    {
                        MessageCreator.MessageEnemyCreator(
                            HeroName,
                            AbilityTexturName,
                            Game.GameTime);
                    }

                    if (MenuManager.DangerousSpellsSound.Value
                        && DangerousItems)
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

                    DrawHelper.Draw(
                        Hero,
                        ParticleName,
                        AbilityTexturName,
                        Hero.Name.Substring("npc_dota_hero_".Length),
                        Hero.GetDisplayName(),
                        HeroColor,
                        Position);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void ItemsNull(
            Hero Hero,
            string ParticleName,
            string AbilityTexturName,
            Vector3 Position)
        {
            try
            {
                var HeroNull = Hero == null;

                var DangerousItems = Dangerous.DangerousItemList.Contains(AbilityTexturName);

                if (MenuManager.ItemsItem.Value
                    && (HeroNull
                    || Hero.Team != MyHero.Team
                    && (!Hero.IsVisible
                    || MenuManager.DangerousItemsItem.Value
                    && DangerousItems)))
                {
                    var HeroTexturName = HeroNull
                        ? "default"
                        : Hero.Name.Substring("npc_dota_hero_".Length);

                    var HeroName = HeroNull
                        ? CultureInfo.CurrentCulture.TextInfo.
                        ToTitleCase(AbilityTexturName.Substring("item_".Length).Replace("_", " "))
                        : Hero.GetDisplayName();

                    var Vector3 = HeroNull ? new Vector3(255, 0, 0) : Colors.Vector3ToID[Hero.Player.Id] * 255;

                    var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);

                    if (MenuManager.DangerousItemsMSG.Value
                        && DangerousItems)
                    {
                        MessageCreator.MessageItemCreator(
                            HeroTexturName,
                            AbilityTexturName.Substring("item_".Length),
                            Game.GameTime);
                    }

                    if (MenuManager.DangerousItemsSound.Value
                        && DangerousItems)
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

                    DrawHelper.Draw(
                        Hero,
                        ParticleName,
                        AbilityTexturName,
                        HeroTexturName,
                        HeroName,
                        HeroColor,
                        Position);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
