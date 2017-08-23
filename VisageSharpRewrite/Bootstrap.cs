using Ensage;
using Ensage.Common;
using System;

namespace VisageSharpRewrite
{
    public class Bootstrap
    {
        private readonly VisageSharp visageSharp;

        public Bootstrap()
        {
            this.visageSharp = new VisageSharp();
        }

        public void SubscribeEvents()
        {
            Events.OnLoad += this.Events_Onload;
            Events.OnClose += this.Events_OnClose;
            Game.OnUpdate += this.Game_OnUpdate;
            //Game.OnWndProc += this.Game_OnWndProc;
            Drawing.OnDraw += this.Drawing_OnDraw;          
            Player.OnExecuteOrder += this.Player_OnExecuteOrder;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            this.visageSharp.OnDraw();
        }

        private void Events_Onload(object sender, EventArgs e)
        {
            this.visageSharp.OnLoad();
        }

        private void Events_OnClose(object sender, EventArgs e)
        {
            this.visageSharp.OnClose();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            this.visageSharp.OnUpdate_AutoLastHit();
            this.visageSharp.OnUpdate_AutoNuke();
            this.visageSharp.OnUpdate_Follow();
            this.visageSharp.OnUpdate_MenuControl();
            this.visageSharp.OnUpdate_Combo();
            this.visageSharp.OnUpdate_LowHP();
            //this.visageSharp.OnUpdate_TalentAbuse();
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            //this.visageSharp.OnWndProc(args);
        }

        private void Player_OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (sender.Equals(ObjectManager.LocalPlayer))
            {
                this.visageSharp.Player_OnExecuteOrder(sender, args);
            }
        }
    }
}
