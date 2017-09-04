using System;

using Ensage;

using SharpDX;
using Ensage.SDK.Helpers;
using System.Linq;
using Ensage.Common;

namespace VisagePlus
{
    internal class Renderer
    {
        private Config Config { get; }

        private Vector2 Screen {get;}

        public Renderer(Config config)
        {
            Config = config;

            Screen = new Vector2(Drawing.Width - 130, Drawing.Height);
            Drawing.OnDraw += OnDraw;
        }

        public void Dispose()
        {
            Drawing.OnDraw -= OnDraw;
        }

        private void Text(string text, float heightpos, Color color)
        {
            var pos = new Vector2(Screen.X, Screen.Y * heightpos);

            Drawing.DrawText(text, "Arial", pos, new Vector2(22), color, FontFlags.None);
        }

        private void OnDraw(EventArgs args)
        {
            Text($"Combo {(Config.ComboKeyItem.Value ? "ON" : "OFF")}",
                0.60f,
                (Config.ComboKeyItem.Value ? Color.Aqua : Color.Yellow));

            Text($"Last Hit {(Config.LastHitItem.Value ? "ON" : "OFF")}", 
                0.65f, 
                (Config.LastHitItem.Value ? Color.Aqua : Color.Yellow));

            Text($"Follow {(Config.FollowKeyItem.Value ? "ON" : "OFF")}",
                0.70f,
                (Config.FollowKeyItem.Value ? Color.Aqua : Color.Yellow));

            if (Config.KillStealItem.Value)
            {
                Text($"Kill Steal {(!Config.Mode.CanExecute ? "ON" : "OFF")}",
                0.75f,
                (!Config.Mode.CanExecute ? Color.Aqua : Color.Yellow));
            }
            
            var EnemyHeroes =
                EntityManager<Hero>.Entities.Where(
                    x =>
                    x.IsAlive &&
                    x.IsVisible &&
                    x.Team != Config.VisagePlus.Context.Owner.Team);

            foreach (var EnemyHero in EnemyHeroes.ToList())
            {
                if (EnemyHero.Health <= Config.AutoUsage.Damage(EnemyHero))
                {
                    Drawing.DrawText(
                        "Kill", 
                        "Arial Black",
                        HUDInfo.GetHPbarPosition(EnemyHero) - new Vector2(25, 5), 
                        new Vector2(15), 
                        Color.Red, 
                        FontFlags.None);
                }

                /*if (EnemyHero.Health <= Damage(EnemyHero))
                {
                    var ssss = Damage(EnemyHero) / EnemyHero.Health * 1000;
                    Console.WriteLine(ssss);
                    Context.Particle.AddOrUpdate(
                        EnemyHero,
                        $"Draw{EnemyHero.Handle}",
                        "materials/ensage_ui/particles/smart_radius.vpcf",
                        ParticleAttachment.AbsOrigin,
                        true,
                        0,
                        EnemyHero.Position + new Vector3(0, 0, 100),
                        1,
                        new Vector3(200, 0, 10),
                        2,
                        new Vector3(200, 500, 0),
                        3,
                        new Vector3(255, 0, 0));
                }
                /*else
                {
                    Context.Particle.Remove($"Enemy{EnemyHero.Handle}");
                }*/
            }
        }
    }
}
