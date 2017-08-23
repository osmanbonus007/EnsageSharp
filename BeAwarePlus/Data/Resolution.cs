using System;

using Ensage;

using SharpDX;

using BeAwarePlus.Checker;

namespace BeAwarePlus.Data
{
    internal class Resolution
    {
        public Resolution(MessageCreator MessageCreator)
        {
            var ScreenSize = new Vector2(Drawing.Width, Drawing.Height);
            
            //1920x1080 (16:9)
            if (ScreenSize == new Vector2(1920, 1080))
            {
                MessageCreator.MsgX = 256;
                MessageCreator.MsgY = 128;
                MessageCreator.SizeheroX = 97;
                MessageCreator.SizeheroYspell = 55;
                MessageCreator.SizeitemX = 113;
                MessageCreator.HerospellY = 62;
                MessageCreator.HeroallyX = 152;
                MessageCreator.HeroX = 9;
                MessageCreator.SpellX = 193;
                MessageCreator.ItemX = 170;
            }

            //1768x992 (16:9)
            else if (ScreenSize == new Vector2(1768, 992))
            {
                MessageCreator.MsgX = 248;
                MessageCreator.MsgY = 120;
                MessageCreator.SizeheroX = 97;
                MessageCreator.SizeheroYspell = 55;
                MessageCreator.SizeitemX = 113;
                MessageCreator.HerospellY = 56;
                MessageCreator.HeroallyX = 145;
                MessageCreator.HeroX = 9;
                MessageCreator.SpellX = 186;
                MessageCreator.ItemX = 165;
            }

            //1600x900 (16:9)
            else if (ScreenSize == new Vector2(1600, 900))
            {
                MessageCreator.MsgX = 235;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 88;
                MessageCreator.SizeheroYspell = 50;
                MessageCreator.SizeitemX = 100;
                MessageCreator.HerospellY = 52;
                MessageCreator.HeroallyX = 140;
                MessageCreator.HeroX = 8;
                MessageCreator.SpellX = 177;
                MessageCreator.ItemX = 157;
            }

            //1360x768 (16:9)
            else if (ScreenSize == new Vector2(1360, 768)) 
            {
                MessageCreator.MsgX = 195;
                MessageCreator.MsgY = 95;
                MessageCreator.SizeheroX = 68;
                MessageCreator.SizeheroYspell = 42;
                MessageCreator.SizeitemX = 85;
                MessageCreator.HerospellY = 46;
                MessageCreator.HeroallyX = 121;
                MessageCreator.HeroX = 7;
                MessageCreator.SpellX = 146;
                MessageCreator.ItemX = 130;
            }
            //1366x768 (16:9)
            else if (ScreenSize == new Vector2(1366, 768))
            {
                MessageCreator.MsgX = 195;
                MessageCreator.MsgY = 95;
                MessageCreator.SizeheroX = 68;
                MessageCreator.SizeheroYspell = 42;
                MessageCreator.SizeitemX = 85;
                MessageCreator.HerospellY = 46;
                MessageCreator.HeroallyX = 121;
                MessageCreator.HeroX = 7;
                MessageCreator.SpellX = 146;
                MessageCreator.ItemX = 130;
            }

            //1280x720 (16:9)
            else if (ScreenSize == new Vector2(1280, 720))
            {
                MessageCreator.MsgX = 190;
                MessageCreator.MsgY = 90;
                MessageCreator.SizeheroX = 64;
                MessageCreator.SizeheroYspell = 39;
                MessageCreator.SizeitemX = 80;
                MessageCreator.HerospellY = 44;
                MessageCreator.HeroallyX = 121;
                MessageCreator.HeroX = 6;
                MessageCreator.SpellX = 146;
                MessageCreator.ItemX = 130;
            }

            //1680x1050 (16:10)
            else if (ScreenSize == new Vector2(1680, 1050))
            {
                MessageCreator.MsgX = 256;
                MessageCreator.MsgY = 128;
                MessageCreator.SizeheroX = 97;
                MessageCreator.SizeheroYspell = 55;
                MessageCreator.SizeitemX = 113;
                MessageCreator.HerospellY = 62;
                MessageCreator.HeroallyX = 152;
                MessageCreator.HeroX = 9;
                MessageCreator.SpellX = 193;
                MessageCreator.ItemX = 170;
            }

            //1600x1024 (16:10)
            else if (ScreenSize == new Vector2(1600, 1024))
            {
                MessageCreator.MsgX = 256;
                MessageCreator.MsgY = 128;
                MessageCreator.SizeheroX = 92;
                MessageCreator.SizeheroYspell = 55;
                MessageCreator.SizeitemX = 110;
                MessageCreator.HerospellY = 62;
                MessageCreator.HeroallyX = 152;
                MessageCreator.HeroX = 9;
                MessageCreator.SpellX = 190;
                MessageCreator.ItemX = 170;
            }

            //1440x960 (16:10) //1440x900 (16:10)
            else if (ScreenSize == new Vector2(1440, 960)
                || ScreenSize == new Vector2(1440, 900))
            {
                MessageCreator.MsgX = 220;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 73;
                MessageCreator.SizeheroYspell = 48;
                MessageCreator.SizeitemX = 92;
                MessageCreator.HerospellY = 53;
                MessageCreator.HeroallyX = 141;
                MessageCreator.HeroX = 8;
                MessageCreator.SpellX = 166;
                MessageCreator.ItemX = 150;
            }

            //1280x800 (16:10)
            else if (ScreenSize == new Vector2(1280, 800))
            {
                MessageCreator.MsgX = 195;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 70;
                MessageCreator.SizeheroYspell = 48;
                MessageCreator.SizeitemX = 88;
                MessageCreator.HerospellY = 53;
                MessageCreator.HeroallyX = 120;
                MessageCreator.HeroX = 7;
                MessageCreator.SpellX = 142;
                MessageCreator.ItemX = 130;
            }

            //1280x768 (16:10)
            else if (ScreenSize == new Vector2(1280, 768))
            {
                MessageCreator.MsgX = 195;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 70;
                MessageCreator.SizeheroYspell = 48;
                MessageCreator.SizeitemX = 88;
                MessageCreator.HerospellY = 53;
                MessageCreator.HeroallyX = 120;
                MessageCreator.HeroX = 7;
                MessageCreator.SpellX = 142;
                MessageCreator.ItemX = 130;
            }

            //1280x1024 (4:3)
            else if (ScreenSize == new Vector2(1280, 1024))
            {
                MessageCreator.MsgX = 228;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 75;
                MessageCreator.SizeheroYspell = 48;
                MessageCreator.SizeitemX = 92;
                MessageCreator.HerospellY = 53;
                MessageCreator.HeroallyX = 146;
                MessageCreator.HeroX = 8;
                MessageCreator.SpellX = 174;
                MessageCreator.ItemX = 159;
            }

            //1280x960 (4:3)
            else if (ScreenSize == new Vector2(1280, 960))
            {
                MessageCreator.MsgX = 228;
                MessageCreator.MsgY = 110;
                MessageCreator.SizeheroX = 75;
                MessageCreator.SizeheroYspell = 48;
                MessageCreator.SizeitemX = 92;
                MessageCreator.HerospellY = 53;
                MessageCreator.HeroallyX = 146;
                MessageCreator.HeroX = 8;
                MessageCreator.SpellX = 174;
                MessageCreator.ItemX = 159;
            }
            else
            {
                Console.WriteLine(@"Your screen resolution is not supported and drawings might have wrong size (" + ScreenSize + ")");
                MessageCreator.MsgX = 256;
                MessageCreator.MsgY = 128;
                MessageCreator.SizeheroX = 97;
                MessageCreator.SizeheroYspell = 55;
                MessageCreator.SizeitemX = 113;
                MessageCreator.HerospellY = 62;
                MessageCreator.HeroallyX = 152;
                MessageCreator.HeroX = 9;
                MessageCreator.SpellX = 193;
                MessageCreator.ItemX = 170;
            }
        }
    }
}
