using System;
using System.ComponentModel;

using Ensage;

using SharpDX;
using Ensage.SDK.Extensions;

namespace ZeusPlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private MenuManager Menu { get; }

        private Unit Owner { get; }

        private UpdateMode UpdateMode { get; }

        private int AlarmNumber { get; set; }

        public Renderer(Config config)
        {
            Config = config;
            Menu = config.Menu;
            Owner = config.Main.Context.Owner;
            UpdateMode = config.UpdateMode;

            config.Menu.OnDrawItem.PropertyChanged += TextChanged;

            if (config.Menu.OnDrawItem)
            {
                Drawing.OnDraw += OnDraw;
            }
        }

        public void Dispose()
        {
            if (Menu.OnDrawItem)
            {
                Drawing.OnDraw -= OnDraw;
            }

            Menu.OnDrawItem.PropertyChanged -= TextChanged;
        }

        private void TextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Menu.OnDrawItem)
            {
                Drawing.OnDraw += OnDraw;
            }
            else
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void Texture(Vector2 pos, Vector2 size, string texture)
        {
            Drawing.DrawRect(
                pos,
                size,
                Drawing.GetTexture($"materials/ensage_ui/{texture}.vmat"));
        }

        private void OnDraw(EventArgs args)
        {
            var i = 0;
            foreach (var Data in Config.DamageCalculation.DamageList)
            {
                var setPos = new Vector2(
                    Math.Min((Config.Screen.X - 20) - Menu.OnDrawXItem, Config.Screen.X - 20),
                    Math.Min(Menu.OnDrawYItem - 100, Config.Screen.Y - 90));

                var pos = new Vector2(Config.Screen.X, Config.Screen.Y * 0.65f + i) - setPos;

                var hero = Data.GetHero;
                var health = Data.GetHealth;

                var ph = Math.Ceiling((float)health / hero.MaximumHealth * 100);
                var doNotKill = DoNotKill(hero);

                if (!hero.IsVisible)
                {
                    Texture(pos + 5, new Vector2(55), $"heroes_round/{ hero.Name.Substring("npc_dota_hero_".Length) }");
                    Texture(pos, new Vector2(65), "other/round_percentage/frame/white");
                    Texture(pos, new Vector2(65), $"other/round_percentage/hp/{ Math.Min(ph, 100) }");

                    if (doNotKill != null)
                    {
                        Texture(pos + new Vector2(42, 45), new Vector2(20), $"modifier_textures/round/{ doNotKill }");
                    }

                    i += 80;
                    continue;
                }

                var damage = Data.GetDamage;
                var readyDamage = Data.GetReadyDamage;
                var totalDamage = Data.GetTotalDamage;

                var maxHealth = hero.MaximumHealth + (health - hero.MaximumHealth);
                var damagePercent = Math.Ceiling(100 - (health - Math.Max(damage, 0)) / maxHealth * 100);
                var readyDamagePercent = Math.Ceiling(100 - (health - Math.Max(readyDamage, 0)) / maxHealth * 100);
                var totalDamagePercent = Math.Ceiling(100 - (health - Math.Max(totalDamage, 0)) / maxHealth * 100);

                if (damagePercent >= 100)
                {
                    Texture(pos - 10, new Vector2(85), $"other/round_percentage/alert/{ Alert() }");
                }

                Texture(pos + 5, new Vector2(55), $"heroes_round/{ hero.Name.Substring("npc_dota_hero_".Length) }");

                Texture(pos, new Vector2(65), "other/round_percentage/frame/white");
                Texture(pos, new Vector2(65), $"other/round_percentage/no_percent_gray/{ Math.Min(totalDamagePercent, 100) }");
                Texture(pos, new Vector2(65), $"other/round_percentage/no_percent_yellow/{ Math.Min(readyDamagePercent, 100) }");

                var color = damagePercent >= 100 ? "green" : "red";
                Texture(pos, new Vector2(65), $"other/round_percentage/{ color }/{ Math.Min(damagePercent, 100) }");

                if (damagePercent >= 100)
                {
                    Texture(pos, new Vector2(65), $"other/round_percentage/no_percent_gray/{ Math.Min(damagePercent - 100, 100) }");
                }

                if (doNotKill != null)
                {
                    Texture(pos + new Vector2(42, 45), new Vector2(20), $"modifier_textures/round/{ doNotKill }");
                }

                i += 80;
            }
        }

        private string DoNotKill(Hero hero)
        {
            var reincarnation = hero.GetAbilityById(AbilityId.skeleton_king_reincarnation);
            if (reincarnation != null && reincarnation.Cooldown == 0 && reincarnation.Level > 0)
            {
                return reincarnation.TextureName;
            }


            return null;
        }

        private string Alert()
        {
            AlarmNumber += 1;
            if (AlarmNumber < 10)
            {
                return 0.ToString();
            }
            else if (AlarmNumber < 20)
            {
                return 1.ToString();
            }
            else if (AlarmNumber < 30)
            {
                return 2.ToString();
            }
            else if (AlarmNumber < 40)
            {
                return 1.ToString();
            }
            else
            {
                AlarmNumber = 0;
            }

            return 0.ToString();
        }
    }
}
