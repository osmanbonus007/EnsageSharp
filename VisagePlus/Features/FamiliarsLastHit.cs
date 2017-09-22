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
using Ensage.SDK.Service;

namespace VisagePlus.Features
{
    internal class FamiliarsLastHit
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        private TaskHandler Handler { get; }

        public FamiliarsLastHit(Config config)
        {
            Config = config;
            Context = config.VisagePlus.Context;

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
                    EntityManager<Unit>.Entities.Where(
                        x =>
                        x.IsValid &&
                        x.IsAlive &&
                        x.IsControllable &&
                        x.Team == Context.Owner.Team &&
                        x.NetworkName == "CDOTA_Unit_VisageFamiliar").ToArray();

                var AttackingMe =
                    ObjectManager.TrackingProjectiles.FirstOrDefault(
                        x =>
                        x.Target.NetworkName == "CDOTA_Unit_VisageFamiliar");

                foreach (var Familiar in Familiars)
                {
                    var EnemyHero =
                        EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsAlive &&
                            x.IsVisible &&
                            x.Team != Context.Owner.Team &&
                            x.Distance2D(Familiar) <= x.AttackRange + 400);

                    var ClosestAllyTower =
                        EntityManager<Unit>.Entities.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(
                            x =>
                            x.ClassId == ClassId.CDOTA_BaseNPC_Tower &&
                            x.IsAlive &&
                            x.Team == Context.Owner.Team &&
                            x.Distance2D(Familiar) >= 100);

                    if (EnemyHero != null || (AttackingMe != null && AttackingMe.Target.Handle == Familiar.Handle))
                    {
                        if (ClosestAllyTower == null)
                        {
                            var ClosestAllyFountain =
                                EntityManager<Unit>.Entities.FirstOrDefault(
                                    x =>
                                    x.ClassId == ClassId.CDOTA_BaseNPC_Fort &&
                                    x.IsAlive &&
                                    x.Team == Context.Owner.Team);

                            Familiar.Follow(ClosestAllyFountain);
                            await Await.Delay(100, token);
                        }
                        else
                        {
                            Familiar.Follow(ClosestAllyTower);
                            await Await.Delay(100, token);
                        }
                    }
                    else
                    {
                        var ClosestAllyCreep =
                            EntityManager<Unit>.Entities.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(
                                x =>
                                (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege) &&
                                x.IsAlive &&
                                x.Team == Context.Owner.Team &&
                                Familiar.Distance2D(x) <= 3000);

                        var ClosestEnemyUnit =
                            EntityManager<Unit>.Entities.OrderBy(x => (float)x.Health / x.MaximumHealth).FirstOrDefault(
                                x =>
                                ((x.ClassId == ClassId.CDOTA_BaseNPC_Tower && x.Health <= 200) ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creep ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Additive ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Barracks ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Building ||
                                x.ClassId == ClassId.CDOTA_BaseNPC_Creature) &&
                                x.IsAlive &&
                                x.IsVisible &&
                                x.Team != Context.Owner.Team &&
                                Familiar.Distance2D(x) <= 1000);

                        if (ClosestAllyCreep == null || ClosestAllyCreep.Distance2D(Familiar) >= 1000)
                        {
                            if (ClosestAllyTower == null)
                            {
                                var ClosestAllyFort =
                                    EntityManager<Unit>.Entities.FirstOrDefault(
                                        x =>
                                        x.ClassId == ClassId.CDOTA_BaseNPC_Fort &&
                                        x.IsAlive &&
                                        x.Team == Context.Owner.Team);

                                Familiar.Follow(ClosestAllyFort);
                                await Await.Delay(200, token);
                            }
                            else
                            {
                                Familiar.Follow(ClosestAllyTower);
                                await Await.Delay(200, token);
                            }
                        }
                        else if (ClosestAllyCreep != null && ClosestEnemyUnit == null)
                        {
                            Familiar.Follow(ClosestAllyCreep);
                            await Await.Delay(200, token);
                        }
                        else if (ClosestAllyCreep != null && ClosestEnemyUnit != null)
                        {
                            var CommonAttack = Config.CommonAttackItem ? Familiars.Count() : 1;
                            if (ClosestEnemyUnit.Health <= CommonAttack * Familiar.GetAttackDamage(ClosestEnemyUnit))
                            {
                                Familiar.Attack(ClosestEnemyUnit);
                                await Await.Delay(100, token);
                            }
                            else
                            {
                                Familiar.Follow(ClosestEnemyUnit);
                                await Await.Delay(200, token);
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Config.VisagePlus.Log.Error(e);
            }
        }
    }
}