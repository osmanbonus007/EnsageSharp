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
    internal class ParticleSpells
    {
        private MenuManager MenuManager { get; }

        private Unit MyHero { get; }

        private Dangerous Dangerous { get; }

        private MessageCreator MessageCreator { get; }

        private SoundPlayer SoundPlayer { get; }

        private Colors Colors { get; }

        private DrawHelper DrawHelper { get; }

        public ParticleSpells(
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

        public void Spells(
            Hero Hero, 
            string ParticleName, 
            string AbilityTexturName, 
            Vector3 Position)
        {
            if (Hero == null) 
            {
                var HeroTexturName = ParticleName.Substring("particles/units/heroes/hero_".Length).
                    Split(char.Parse("/"))[0];

                var HeroName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase
                    (ParticleName.Substring("particles/units/heroes/hero_".Length).
                    Split(char.Parse("/"))[0]);

                DrawHelper.Draw(
                    Hero,
                    ParticleName,
                    AbilityTexturName,
                    "default",
                    HeroName,
                    Color.Red,
                    Position);

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
                    var HeroName = Hero.Name.Substring("npc_dota_hero_".Length);
                    var Vector3 = Colors.Vector3ToID[Hero.Player.Id] * 255;
                    var HeroColor = Color.FromArgb((int)Vector3.X, (int)Vector3.Y, (int)Vector3.Z);

                    if (MenuManager.DangerousSpellsMSG.Value
                        && DangerousSpell
                        && End(ParticleName))
                    {
                        MessageCreator.MessageEnemyCreator(
                            HeroName, 
                            AbilityTexturName, 
                            Game.GameTime);
                    }

                    if (MenuManager.DangerousSpellsSound.Value
                        && DangerousSpell
                        && End(ParticleName))
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
                        HeroName,
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

        private bool End(string ParticleName)
        {
            return !ParticleName.Contains("furion_teleport.vpcf")
                && !ParticleName.Contains("wisp_relocate_channel.vpcf")
                && !ParticleName.Contains("abbysal_underlord_darkrift_ambient.vpcf");
        }
    }
}
