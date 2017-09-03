using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace VisagePlus.Features
{
    internal class FamiliarsLastHit
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        public FamiliarsLastHit(Config config)
        {
            Config = config;
            Context = config.VisagePlus.Context;

            config.FamiliarsLastHitItem.PropertyChanged += FamiliarsLastHitChanged;

            if (Config.FamiliarsLastHitItem.Value)
            {
                UpdateManager.Subscribe(LastHitFollow, 50);

                config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        public void Dispose()
        {
            Config.FamiliarsLastHitItem.PropertyChanged -= FamiliarsLastHitChanged;

            UpdateManager.Unsubscribe(LastHitFollow);
        }

        private void FamiliarsLastHitChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FamiliarsLastHitItem.Value)
            {
                UpdateManager.Subscribe(LastHitFollow, 50);

                Config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
            else
            {
                UpdateManager.Unsubscribe(LastHitFollow);
            }
        }

        private void LastHitFollow()
        {
            var Familiars =
                EntityManager<Unit>.Entities.Where(
                    x =>
                    x.IsValid &&
                    x.IsAlive &&
                    x.IsControllable &&
                    x.Team == Context.Owner.Team &&
                    x.Name.Contains("npc_dota_visage_familiar")).ToArray();

            foreach (var Familiar in Familiars)
            {
                var EnemyHero =
                    EntityManager<Hero>.Entities.FirstOrDefault(
                        x =>
                        x.IsAlive &&
                        x.IsVisible &&
                        x.Team != Context.Owner.Team &&
                        x.Distance2D(Familiar) <= x.AttackRange + 500);

                var ClosestAllyTower =
                    EntityManager<Unit>.Entities.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(
                        x =>
                        x.ClassId == ClassId.CDOTA_BaseNPC_Tower &&
                        x.IsAlive &&
                        x.Team == Context.Owner.Team &&
                        x.Distance2D(Familiar) >= 100);

                if (EnemyHero != null)
                {
                    if (ClosestAllyTower == null)
                    {
                        if (Utils.SleepCheck($"Fountain{Familiar.Handle}"))
                        {
                            var ClosestAllyFountain =
                                EntityManager<Unit>.Entities.FirstOrDefault(
                                    x =>
                                    x.ClassId == ClassId.CDOTA_Unit_Fountain &&
                                    x.IsAlive &&
                                    x.Team == Context.Owner.Team);

                            Familiar.Follow(ClosestAllyFountain);
                            Utils.Sleep(200, $"Fountain{Familiar.Handle}");
                        }
                    }
                    else
                    {
                        if (Utils.SleepCheck($"Tower{Familiar.Handle}"))
                        {
                            Familiar.Follow(ClosestAllyTower);
                            Utils.Sleep(200, $"Tower{Familiar.Handle}");
                        }
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
                            if (Utils.SleepCheck($"Fountain{Familiar.Handle}"))
                            {
                                var ClosestAllyFountain =
                                    EntityManager<Unit>.Entities.FirstOrDefault(
                                        x =>
                                        x.ClassId == ClassId.CDOTA_Unit_Fountain &&
                                        x.IsAlive &&
                                        x.Team == Context.Owner.Team);

                                Familiar.Follow(ClosestAllyFountain);
                                Utils.Sleep(200, $"Fountain{Familiar.Handle}");
                            }
                        }
                        else
                        {
                            if (Utils.SleepCheck($"Tower{Familiar.Handle}"))
                            {
                                Familiar.Follow(ClosestAllyTower);
                                Utils.Sleep(200, $"Tower{Familiar.Handle}");
                            }
                        }
                    }
                    else if (ClosestAllyCreep != null && ClosestEnemyUnit == null)
                    {
                        Familiar.Follow(ClosestAllyCreep);
                    }
                    else if (ClosestAllyCreep != null && ClosestEnemyUnit != null)
                    {
                        if (ClosestEnemyUnit.Health >= Familiar.GetAttackDamage(ClosestEnemyUnit))
                        {
                            Familiar.Follow(ClosestEnemyUnit);
                        }

                        LastHit(Familiar, ClosestEnemyUnit);
                    }
                }
            }
        }

        private void LastHit(Unit Familiar, Unit ClosestEnemyCreep)
        {
            if (ClosestEnemyCreep.Health <= Familiar.GetAttackDamage(ClosestEnemyCreep))
            {
                Familiar.Attack(ClosestEnemyCreep);
            }
        }
    }
}