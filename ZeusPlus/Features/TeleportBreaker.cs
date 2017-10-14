using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

using SharpDX;

namespace ZeusPlus.Features
{
    internal class TeleportBreaker
    {
        private ZeusPlus Main { get; }

        private Unit Owner { get; }

        private MenuManager Menu { get; }

        private Vector3 Position { get; set; }

        private IUpdateHandler Update { get; set; }

        public TeleportBreaker(Config config)
        {
            Menu = config.Menu;
            Main = config.Main;
            Owner = config.Main.Context.Owner;

            if (config.Menu.TeleportBreakerItem)
            {
                Entity.OnParticleEffectAdded += OnParticle;
                Update = UpdateManager.Subscribe(Execute, 50, false);
            }
            
            config.Menu.TeleportBreakerItem.PropertyChanged += TeleportBreakerChanged;
        }

        public void Dispose()
        {
            if (Menu.TeleportBreakerItem)
            {
                UpdateManager.Unsubscribe(Execute);
                Entity.OnParticleEffectAdded -= OnParticle;
            }
        }

        private void TeleportBreakerChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Menu.TeleportBreakerItem)
            {
                Entity.OnParticleEffectAdded += OnParticle;
                Update = UpdateManager.Subscribe(Execute, 50, false);
            }
            else
            {
                UpdateManager.Unsubscribe(Execute);
                Entity.OnParticleEffectAdded -= OnParticle;
            }
        }

        private void OnParticle(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (!args.Name.Contains("/teleport_start"))
            {
                return;
            }

            if (Update.IsEnabled)
            {
                return;
            }

            UpdateManager.BeginInvoke(
                () =>
                {
                    Position = args.ParticleEffect.GetControlPoint(0);

                    var ignore = EntityManager<Hero>.Entities.Any(x =>
                                                                  x.IsValid &&
                                                                  (x.IsAlly(Owner) && 
                                                                  x.Distance2D(Position) < 50) || 
                                                                  (x.IsEnemy(Owner) &&
                                                                  x.IsVisible &&
                                                                  x.IsMagicImmune()));

                    if (!ignore)
                    {
                        Update.IsEnabled = true;

                        UpdateManager.BeginInvoke(
                            () =>
                            {
                                Update.IsEnabled = false;
                            },
                            3000);
                    }
                },
                20);
        }

        private void Execute()
        {
            if (Game.IsPaused || !Owner.IsValid || !Owner.IsAlive || Owner.IsStunned())
            {
                return;
            }

            // LightningBolt
            var LightningBolt = Main.LightningBolt;
            if (Menu.TeleportBreakerToggler.Value.IsEnabled(LightningBolt.ToString())
                && Owner.Distance2D(Position) < LightningBolt.CastRange + 100
                && LightningBolt.CanBeCasted)
            {
                LightningBolt.UseAbility(Position);

                Update.IsEnabled = false;
                return;
            }

            // Nimbus
            var Nimbus = Main.Nimbus;
            if (Menu.TeleportBreakerToggler.Value.IsEnabled(Nimbus.ToString())
                && Owner.Distance2D(Position) > LightningBolt.CastRange + 250 && Owner.Distance2D(Position) < Menu.NimbusRangeItem
                && Nimbus.CanBeCasted)
            {
                Nimbus.UseAbility(Position);

                Update.IsEnabled = false;
                return;
            }
        }
    }
}
