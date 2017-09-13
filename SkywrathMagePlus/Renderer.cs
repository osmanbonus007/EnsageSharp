using System;
using System.ComponentModel;
using Ensage;

using SharpDX;

namespace SkywrathMagePlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private Vector2 Screen {get;}

        public Renderer(Config config)
        {
            Config = config;

            Screen = new Vector2(Drawing.Width - 160, Drawing.Height);

            config.TextItem.PropertyChanged += TextChanged;

            if (config.TextItem)
            {
                Drawing.OnDraw += OnDraw;
            }
        }

        public void Dispose()
        {
            if (Config.TextItem)
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void TextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.TextItem)
            {
                Drawing.OnDraw += OnDraw;
            }
            else
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void Text(string text, float heightpos, Color color)
        {
            var pos = new Vector2(Screen.X, Screen.Y * heightpos);

            Drawing.DrawText(text, "Arial", pos, new Vector2(21), color, FontFlags.None);
        }

        private void OnDraw(EventArgs args)
        {
            Text($"Combo {(Config.ComboKeyItem ? "ON" : "OFF")}",
                0.72f,
                (Config.ComboKeyItem ? Color.Aqua : Color.Yellow));

            Text($"Spam Q {(Config.SpamKeyItem ? "ON" : "OFF")}",
                0.75f,
                (Config.SpamKeyItem ? Color.Aqua : Color.Yellow));

            Text($"Auto Q {(!Config.ComboKeyItem && !Config.SpamKeyItem && Config.AutoQKeyItem ? "ON" : "OFF")}",
                0.78f,
                (!Config.ComboKeyItem && !Config.SpamKeyItem && Config.AutoQKeyItem ? Color.Aqua : Color.Yellow));

            float pos = 0;
            if (Config.AutoComboItem)
            {
                pos += 0.03f;
                Text($"Auto Combo {(!Config.ComboKeyItem ? "ON" : "OFF")}",
                0.78f + pos,
                (!Config.ComboKeyItem ? Color.Aqua : Color.Yellow));
            }

            if (Config.AutoDisableItem)
            {
                pos += 0.03f;
                Text("Auto Disable ON",
                0.78f + pos,
                Color.Aqua);
            }
        }
    }
}
