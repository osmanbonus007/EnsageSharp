using System;
using System.ComponentModel;

using Ensage;

using SharpDX;

namespace UnitsControlPlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private UpdateMode UpdateMode { get; }

        public Renderer(Config config)
        {
            Config = config;
            UpdateMode = config.UpdateMode;

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

            Config.TextItem.PropertyChanged -= TextChanged;
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

        private void Text(string text, float heightpos, Color color, Vector2 setpos)
        {
            var pos = new Vector2(Config.Screen.X, Config.Screen.Y * heightpos) - setpos;

            Drawing.DrawText(text, "Arial", pos, new Vector2(22), color, FontFlags.None);
        }

        private void Texture(float heightpos, string texture, Vector2 setpos, bool active)
        {
            var pos = new Vector2(Config.Screen.X, Config.Screen.Y * heightpos) - setpos;

            if (active)
            {
                Drawing.DrawRect(pos - new Vector2(5, 5), new Vector2(120, 80), Color.Aqua);
            }

            Drawing.DrawRect(
                pos, 
                new Vector2(110, 70), 
                Drawing.GetTexture("materials/ensage_ui/heroes_horizontal/" + texture + ".vmat"));
        }

        private void OnDraw(EventArgs args)
        {
            var setPos = new Vector2(
                Math.Min(Config.TextXItem, Config.Screen.X - 60),
                Math.Min(Config.TextYItem, Config.Screen.Y - 230));

            var target = UpdateMode.Target;
            var active = target != null;
            var combolock = Config.ToggleKeyItem;
            var combopress = Config.PressKeyItem;

            Texture(0.65f, active
                    ? target.Name.Substring("npc_dota_hero_".Length)
                    : "default",
                    setPos,
                    active && (combolock || combopress));

            Text($"Target: {(active ? TargetSelector() : "Search")}",
                    0.76f,
                    (active && (combolock || combopress) ? Color.Aqua : Color.Yellow),
                    setPos);

            Text($"Control: {(combolock || combopress ? "ON" : "OFF")}",
                    0.80f,
                    (combolock || combopress ? Color.Aqua : Color.Yellow),
                    setPos);

            var follow = Config.FollowKeyItem;
            Text($"Follow: {(follow ? "ON" : "OFF")}",
                    0.84f,
                    (follow ? Color.Aqua : Color.Yellow),
                    setPos);
        }

        private string TargetSelector()
        {
            if (Config.ToggleKeyItem)
            {
                return "Lock";
            }

            if (Config.PressKeyItem)
            {
                return "Press";
            }

            return "Found";
        }
    }
}
