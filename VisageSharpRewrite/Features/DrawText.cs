using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;
using SharpDX;
using System;

namespace VisageSharpRewrite.Features
{
    public class DrawText
    {
        private string text;

        private Vector2 textSize;

        public Color Color { get; set; }

        private readonly Sleeper sleeper;

        public FontFlags FontFlags { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; private set; }

        public DrawText()
        {
            this.sleeper = new Sleeper();
        }

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

        public void DrawAutoLastHit(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.54));

            this.text = "Last Hit" + "[" + Utils.KeyToText(Variables.MenuManager.AutoFamiliarLastHitMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawAutoNuke(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.58));

            this.text = "AutoNuke" + "[" + Utils.KeyToText(Variables.MenuManager.AutoSoulAssumpMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawFollow(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.62));

            this.text = "Follow" + "[" + Utils.KeyToText(Variables.MenuManager.FamiliarFollowMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextCombo(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.66));
            this.text = "Combo" + "[" + Utils.KeyToText(Variables.MenuManager.ComboMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        
    }
}
