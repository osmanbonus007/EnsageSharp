using System;
using System.ComponentModel;

using Ensage;

using SharpDX;

namespace MagnusPlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private Vector2 Screen {get;}

        public Renderer(Config config)
        {
            Config = config;

            Screen = new Vector2(Drawing.Width - 200, Drawing.Height);

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

            if (Config.AutoComboItem)
            {
                Text($"Auto Combo {(!Config.ComboKeyItem ? "ON" : "OFF")}",
                    0.75f,
                    (!Config.ComboKeyItem ? Color.Aqua : Color.Yellow));

                Text($"Auto Combo Amount {Config.AutoComboAmountItem.Value.Value}",
                    0.78f,
                    Color.Aqua);
            }
        }
    }
}
