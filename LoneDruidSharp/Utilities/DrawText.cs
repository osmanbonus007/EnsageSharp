using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using System;

namespace LoneDruidSharpRewrite.Utilities
{
    public class DrawText
    {
        private string text;

        private Vector2 textSize;

        private readonly Sleeper sleeper;

        public DrawText()
        {
            this.sleeper = new Sleeper();
        }

        public Color Color { get; set; }

        public FontFlags FontFlags { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; private set; }

        

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                if (this.sleeper.Sleeping)
                {
                    return;
                }

                this.Size = Drawing.MeasureText(this.text, "Arial", this.textSize, this.FontFlags);
                this.sleeper.Sleep(2000);
            }
        }

        public Vector2 TextSize
        {
            get
            {
                return this.textSize;
            }

            set
            {
                this.textSize = value;
                this.Size = Drawing.MeasureText(this.text, "Arial", this.textSize, this.FontFlags);
                this.sleeper.Sleep(2000);
            }
        }

        public void Draw()
        {
            Drawing.DrawText(this.text, this.Position, this.textSize, this.Color, this.FontFlags);
        }

        public void DrawTextAutoIronTalonText(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 200, Convert.ToSingle(Drawing.Height * 0.50));
            this.text = "IronTalon" + " [" + Utils.KeyToText(Variable.MenuManager.AutoTalonMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = on ? Color.Yellow : Color.Red;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextAutoMidasText(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 200, Convert.ToSingle(Drawing.Height * 0.54));
            this.text = "Midas" + " [" + Utils.KeyToText(Variable.MenuManager.AutoMidasMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = on ? Color.Yellow : Color.Red;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextOnlyBearLastHitText(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 200, Convert.ToSingle(Drawing.Height * 0.58));
            this.text = "Bear Last Hit" + " [" + Utils.KeyToText(Variable.MenuManager.OnlyBearLastHitMenu.GetValue<KeyBind>().Key) + "] " + (on? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = on? Color.Yellow : Color.Red;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextCombinedLastHitText(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 200, Convert.ToSingle(Drawing.Height * 0.66));
            this.text = "Combined Last Hit" + " [" + Utils.KeyToText(Variable.MenuManager.CombinedLastHitMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = on ? Color.Yellow : Color.Red;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextBearChaseText(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 200, Convert.ToSingle(Drawing.Height * 0.62));
            this.text = "Bear Chasing!" + " [" + Utils.KeyToText(Variable.MenuManager.BearChaseMenu.GetValue<KeyBind>().Key) + "] ";
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = on? Color.HotPink : Color.Red;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }


    }
}
