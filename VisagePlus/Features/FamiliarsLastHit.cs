using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace VisagePlus.Features
{
    internal class FamiliarsLastHit : Extensions
    {
        private Config Config { get; }

        private Unit Owner { get; }

        private TaskHandler Handler { get; }

        public FamiliarsLastHit(Config config)
        {
            Config = config;
            Owner = config.Main.Context.Owner;

            config.LastHitItem.PropertyChanged += FamiliarsLastHitChanged;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.LastHitItem)
            {
                config.LastHitItem.Item.SetValue(new KeyBind(
                    config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        public void Dispose()
        {
            Config.LastHitItem.PropertyChanged -= FamiliarsLastHitChanged;

            if (Config.LastHitItem)
            {
                Handler?.Cancel();
            }
        }

        private void FamiliarsLastHitChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.LastHitItem)
            {
                Handler.RunAsync();

                Config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
            else
            {
                Handler?.Cancel();
            }
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Game.IsPaused)
                {
                    return;
                }

                var Familiars =
                    EntityManager<Unit>.Entities.Where(x =>
                                                       x.IsValid &&
                                                       x.IsAlive &&
                                                       x.IsControllable &&
                                                       x.IsAlly(Owner) &&
                                                       x.NetworkName == "CDOTA_Unit_VisageFamiliar").ToList();

                var AttackingMe = ObjectManager.TrackingProjectiles.FirstOrDefault(x => x.Target.NetworkName == "CDOTA_Unit_VisageFamiliar");

                foreach (var Familiar in Familiars)
                {
                    var EnemyHero =
                        EntityManager<Hero>.Entities.FirstOrDefault(x =>
                                                                    x.IsAlive &&
                                                                    x.IsVisible &&
                                                                    x.IsEnemy(Owner) &&
                                                                    x.Distance2D(Familiar) <= x.AttackRange + 400);

                    var ClosestAllyTower =
                        EntityManager<Unit>.Entities.OrderBy(
                            x => x.Distance2D(Familiar)).FirstOrDefault(x =>
                                                                        x.NetworkName == "CDOTA_BaseNPC_Tower" &&
                                                                        x.IsAlive &&
                                                                        x.IsAlly(Owner) &&
                                                                        x.Distance2D(Familiar) >= 100);

                    if (EnemyHero != null || (AttackingMe != null && AttackingMe.Target.Handle == Familiar.Handle))
                    {
                        if (ClosestAllyTower == null)
                        {
                            var ClosestAllyFountain =
                                EntityManager<Unit>.Entities.FirstOrDefault(x =>
                                                                            x.NetworkName == "CDOTA_BaseNPC_Fort" &&
                                                                            x.IsAlive &&
                                                                            x.IsAlly(Owner));

                            if (ClosestAllyFountain != null)
                            {
                                Follow(Familiar, ClosestAllyFountain);
                            }
                        }
                        else
                        {
                            Follow(Familiar, ClosestAllyTower);
                        }
                    }
                    else
                    {
                        var ClosestAllyCreep =
                            EntityManager<Unit>.Entities.OrderBy(
                                x => x.Distance2D(Familiar)).FirstOrDefault(x =>
                                                                            (x.NetworkName == "CDOTA_BaseNPC_Creep_Lane" ||
                                                                            x.NetworkName == "CDOTA_BaseNPC_Creep_Siege") &&
                                                                            x.IsAlive &&
                                                                            x.IsAlly(Owner) &&
                                                                            Familiar.Distance2D(x) <= 3000);

                        var ClosestUnit =
                            EntityManager<Unit>.Entities.OrderBy(
                                x => (float)x.Health / x.MaximumHealth).FirstOrDefault(x =>
                                                                                       ((x.NetworkName == "CDOTA_BaseNPC_Tower" && x.Health <= 200) ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Lane" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Creep" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Neutral" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Creep_Siege" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Additive" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Barracks" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Building" ||
                                                                                       x.NetworkName == "CDOTA_BaseNPC_Creature") &&
                                                                                       x.IsAlive &&
                                                                                       x.IsVisible &&
                                                                                       (Config.DenyItem && x.IsAlly(Owner) || x.IsEnemy(Owner)) &&
                                                                                       Familiar.Distance2D(x) <= 1000);

                        if (ClosestAllyCreep == null || ClosestAllyCreep.Distance2D(Familiar) >= 1000)
                        {
                            if (ClosestAllyTower == null)
                            {
                                var ClosestAllyFort =
                                    EntityManager<Unit>.Entities.FirstOrDefault(
                                        x =>
                                        x.NetworkName == "CDOTA_BaseNPC_Fort" &&
                                        x.IsAlive &&
                                        x.IsAlly(Owner));

                                Follow(Familiar, ClosestAllyFort);
                            }
                            else
                            {
                                Follow(Familiar, ClosestAllyTower);
                            }
                        }
                        else if (ClosestAllyCreep != null && ClosestUnit == null)
                        {
                            Follow(Familiar, ClosestAllyCreep);
                        }
                        else if (ClosestAllyCreep != null && ClosestUnit != null)
                        {
                            var CommonAttack = Config.CommonAttackItem ? Familiars.Count() : 1;
                            if (ClosestUnit.Health <= CommonAttack * Familiar.GetAttackDamage(ClosestUnit))
                            {
                                Attack(Familiar, ClosestUnit);
                            }
                            else
                            {
                                Follow(Familiar, ClosestUnit);
                            }
                        }
                    }
                }

                await Await.Delay(25, token);
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Config.Main.Log.Error(e);
            }
        }
    }
}