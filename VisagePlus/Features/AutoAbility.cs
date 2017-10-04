using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace VisagePlus.Features
{
    internal class AutoAbility
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private Unit Owner { get; }

        private TaskHandler Handler { get; }

        public AutoAbility(Config config)
        {
            Config = config;
            Main = config.Main;
            Owner = config.Main.Context.Owner;

            Handler = UpdateManager.Run(ExecuteAsync, true, true);
        }

        public void Dispose()
        {
            Handler?.Cancel();
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Game.IsPaused || !Owner.IsValid || !Owner.IsAlive || Owner.IsStunned())
                {
                    return;
                }

                if (Config.AutoSoulAssumptionItem && !Config.ComboKeyItem)
                {
                    // SoulAssumption
                    var SoulAssumption = Main.SoulAssumption;
                    if (SoulAssumption.CanBeCasted && SoulAssumption.MaxCharges)
                    {
                        var Target =
                            EntityManager<Hero>.Entities.OrderBy(x => x.Health).FirstOrDefault(x =>
                                                                                               !x.IsIllusion &&
                                                                                               x.IsAlive &&
                                                                                               x.IsVisible &&
                                                                                               x.IsValid &&
                                                                                               x.IsEnemy(Owner) &&
                                                                                               SoulAssumption.CanHit(x));

                        if (Target != null && !Target.IsMagicImmune() && !Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse"))
                        {
                            Main.SoulAssumption.UseAbility(Target);
                            await Await.Delay(Main.SoulAssumption.GetCastDelay(Target), token);
                        }
                    }
                }

                if (Config.KillStealItem)
                {
                    var Target =
                        EntityManager<Hero>.Entities.OrderBy(x => x.Health).FirstOrDefault(x => 
                                                                                           !x.IsIllusion &&
                                                                                           x.IsAlive &&
                                                                                           x.IsVisible &&
                                                                                           x.IsValid &&
                                                                                           x.IsEnemy(Owner));

                    if (Target != null && !Target.IsMagicImmune() && !Target.IsInvulnerable() && !Target.HasModifier("modifier_winter_wyvern_winters_curse"))
                    {
                        if (Target.Health <= Damage(Target))
                        {
                            // SoulAssumption
                            var Dagon = Main.Dagon;
                            var SoulAssumption = Main.SoulAssumption;
                            if (Config.KillStealToggler.Value.IsEnabled(SoulAssumption.ToString())
                                && SoulAssumption.CanBeCasted
                                && SoulAssumption.CanHit(Target)
                                && SoulAssumption.MaxCharges
                                && (Dagon == null || Dagon.CanHit(Target)))
                            {
                                SoulAssumption.UseAbility(Target);
                                await Await.Delay(SoulAssumption.GetCastDelay(Target), token);
                            }

                            // Dagon
                            var Ethereal = Main.Ethereal;
                            if (Dagon != null
                                 && Config.KillStealToggler.Value.IsEnabled("item_dagon_5")
                                 && Dagon.CanBeCasted
                                 && Dagon.CanHit(Target)
                                 && (Ethereal == null || (Target.IsEthereal() && !Ethereal.CanBeCasted)
                                 || !Config.ItemsToggler.Value.IsEnabled(Ethereal.ToString())))
                            {
                                Dagon.UseAbility(Target);
                                await Await.Delay(Dagon.GetCastDelay(Target), token);
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
                Main.Log.Error(e);
            }
        }

        private float Damage(Hero EnemyHero)
        {
            var damage = 0.0f;

            var SoulAssumption = Main.SoulAssumption;
            if (Config.KillStealToggler.Value.IsEnabled(SoulAssumption.ToString())
                && SoulAssumption.IsReady
                && SoulAssumption.MaxCharges)
            {
                damage += SoulAssumption.GetDamage(EnemyHero);
            }

            var Dagon = Main.Dagon;
            if (Dagon != null
                && Config.KillStealToggler.Value.IsEnabled("item_dagon_5")
                && Dagon.IsReady)
            {
                damage += Dagon.GetDamage(EnemyHero);
            }

            return damage;
        }
    }
}