using System;
using System.ComponentModel;

using Ensage;

using SharpDX;


namespace VisagePlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private UpdateMode UpdateMode { get; }

        private Vector2 Screen {get;}

        public Renderer(Config config)
        {
            Config = config;
            UpdateMode = config.UpdateMode;
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

        private void Text(string text, float heightpos, Color color)
        {
            var pos = new Vector2(Screen.X, Screen.Y * heightpos);

            Drawing.DrawText(text, "Arial", pos, new Vector2(22), color, FontFlags.None);
        }

        private void Texture(float heightpos, string texture)
        {
            var pos = new Vector2(Screen.X, Screen.Y * heightpos);

            Drawing.DrawRect(
                pos, 
                new Vector2(110, 70), 
                Drawing.GetTexture("materials/ensage_ui/heroes_horizontal/" + texture + ".vmat"));
        }

        private void OnDraw(EventArgs args)
        {
            if (Config.FamiliarsLockItem)
            {
                var Lock = UpdateMode.FamiliarTarget != null;
                Text($"Familiars: {(Lock ? "Lock" : "Search")}",
                    0.66f,
                    (Lock ? Color.Aqua : Color.Yellow));

                Texture(0.55f, Lock
                    ? UpdateMode.FamiliarTarget.Name.Substring("npc_dota_hero_".Length)
                    : "default");
            }

            Text($"Combo {(Config.ComboKeyItem ? "ON" : "OFF")}",
                0.70f,
                (Config.ComboKeyItem ? Color.Aqua : Color.Yellow));

            Text($"Last Hit {(Config.LastHitItem ? "ON" : "OFF")}", 
                0.74f, 
                (Config.LastHitItem ? Color.Aqua : Color.Yellow));

            Text($"Follow {(Config.FollowKeyItem ? "ON" : "OFF")}",
                0.78f,
                (Config.FollowKeyItem ? Color.Aqua : Color.Yellow));

            if (Config.KillStealItem)
            {
                Text($"Kill Steal {(!Config.ComboKeyItem ? "ON" : "OFF")}",
                    0.82f,
                    (!Config.ComboKeyItem ? Color.Aqua : Color.Yellow));
            }
        }
    }
}
