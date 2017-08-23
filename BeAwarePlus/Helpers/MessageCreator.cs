using Ensage;
using Ensage.Common;

using SharpDX;

using BeAwarePlus.Menus;

namespace BeAwarePlus.Checker
{
    internal class MessageCreator
    {
        private MenuManager MenuManager { get; }

        public SideMessage SideMessage { get; set; }

        public float MsgX { get; set; }

        public float MsgY { get; set; }

        public float SizeheroX { get; set; }

        public float SizeheroYspell { get; set; }

        public float SizeitemX { get; set; }

        public float HerospellY { get; set; }

        public float HeroallyX { get; set; }

        public float HeroX { get; set; }

        public float SpellX { get; set; }

        public float ItemX { get; set; }

        private int Lang => MenuManager.LanguageItem.Value.SelectedIndex;

        public MessageCreator(MenuManager menumanager)
        {
            MenuManager = menumanager;
        }

        public void MessageAllyCreator(string hero, string spell, float GameTime)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    hero + spell + GameTime, 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg0_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(HeroallyX, HerospellY), 
                    new Vector2(SizeheroX, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/heroes_horizontal/" + hero + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(HeroX, HerospellY), 
                    new Vector2(SizeheroYspell, SizeheroYspell),
                    Drawing.GetTexture("ensage_ui/spellicons/" + spell + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageEnemyCreator(string hero, string spell, float GameTime)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    hero + spell + GameTime,
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY),
                    Drawing.GetTexture("ensage_ui/other/msg1_" + MenuManager.LangList[Lang]));

                SideMessage.AddElement(
                    new Vector2(HeroX, HerospellY), 
                    new Vector2(SizeheroX, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/heroes_horizontal/" + hero + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(SpellX, HerospellY), 
                    new Vector2(SizeheroYspell, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/spellicons/" + spell + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageRuneCreator(string hero, string rune, float GameTime)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    hero + rune + GameTime,
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg2_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(HeroX, HerospellY), 
                    new Vector2(SizeheroX, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/heroes_horizontal/" + hero + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(SpellX, HerospellY), 
                    new Vector2(SizeheroYspell, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/modifier_textures/" + rune + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageItemCreator(string hero, string item, float GameTime)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    hero + item + GameTime, 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg3_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(HeroX, HerospellY), 
                    new Vector2(SizeheroX, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/heroes_horizontal/" + hero + ".vmat"));

                SideMessage.AddElement(
                    new Vector2(ItemX, HerospellY),
                    new Vector2(SizeitemX, SizeheroYspell), 
                    Drawing.GetTexture("ensage_ui/items/" + item + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageCheckRuneCreator(string check_rune)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    "check_rune", 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg4_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageUseMidasCreator(string use_midas)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    "use_midas", 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg5_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageRoshanAliveCreator(string roshan_alive)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    "roshan_alive", 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0), 
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg6_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
        public void MessageRoshanMBAliveCreator(string roshan_mb_alive)
        {
            if (MenuManager.SideMessageItem.Value)
            {
                SideMessage = new SideMessage(
                    "roshan_mb_alive", 
                    new Vector2(MsgX, MsgY), 
                    stayTime: 6000);

                SideMessage.AddElement(
                    new Vector2(0, 0),
                    new Vector2(MsgX, MsgY), 
                    Drawing.GetTexture("ensage_ui/other/msg7_" + MenuManager.LangList[Lang] + ".vmat"));

                SideMessage.CreateMessage();
            }
        }
    }
}
