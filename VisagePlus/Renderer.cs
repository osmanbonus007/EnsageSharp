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

        private void Texture(float heightpos, string texture, Vector2 setpos)
        {
            var pos = new Vector2(Config.Screen.X, Config.Screen.Y * heightpos) - setpos;

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

            if (Config.FamiliarsLockItem)
            {
                var active = UpdateMode.FamiliarTarget != null;
                Text($"Familiars: {(active ? "Lock" : "Search")}",
                    0.66f,
                    (active ? Color.Aqua : Color.Yellow),
                    setPos);

                Texture(0.55f, active
                    ? UpdateMode.FamiliarTarget.Name.Substring("npc_dota_hero_".Length)
                    : "default",
                    setPos);
            }

            var combo = Config.ComboKeyItem;
            Text($"Combo {(combo ? "ON" : "OFF")}",
                0.70f,
                (combo ? Color.Aqua : Color.Yellow),
                setPos);

            var lasthit = Config.LastHitItem;
            Text($"Last Hit {(lasthit ? "ON" : "OFF")}", 
                0.74f, 
                (lasthit ? Color.Aqua : Color.Yellow),
                setPos);

            var follow = Config.FollowKeyItem;
            Text($"Follow {(follow ? "ON" : "OFF")}",
                0.78f,
                (follow ? Color.Aqua : Color.Yellow),
                setPos);

            float pos = 0;
            if (Config.KillStealItem)
            {
                pos += 0.04f;
                var active = !Config.ComboKeyItem;
                Text($"Kill Steal {(active ? "ON" : "OFF")}",
                    0.78f + pos,
                    (active ? Color.Aqua : Color.Yellow),
                    setPos);
            }

            if (Config.EscapeKeyItem)
            {
                pos += 0.04f;
                var active = !Config.ComboKeyItem && !Config.FamiliarsLockItem && !Config.LastHitItem && !Config.FollowKeyItem;
                Text($"Escape {(active ? "ON" : "OFF")}",
                    0.78f + pos,
                    (active ? Color.Aqua : Color.Yellow),
                    setPos);
            }
        }
    }
}
