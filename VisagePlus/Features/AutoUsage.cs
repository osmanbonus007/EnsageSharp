using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace VisagePlus.Features
{
    internal class AutoUsage
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private IServiceContext Context { get; }

        private TaskHandler Handler { get; }

        public AutoUsage(Config config)
        {
            Config = config;
            Main = config.VisagePlus;
            Context = config.VisagePlus.Context;

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
                if (Config.Mode.CanExecute)
                {
                    return;
                }

                if (Config.AutoSoulAssumptionItem.Value)
                {
                    var EnemyHero =
                        EntityManager<Hero>.Entities.OrderBy(x => x.Health).FirstOrDefault(
                            x => !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Context.Owner.Team &&
                            x.Distance2D(Context.Owner) <= Main.SoulAssumption.CastRange);

                    // SoulAssumption
                    if (Main.SoulAssumption != null
                        && Config.AbilityToggler.Value.IsEnabled(Main.SoulAssumption.Ability.Name)
                        && Main.SoulAssumption.CanBeCasted
                        && Main.SoulAssumption.CanHit(EnemyHero)
                        && Main.SoulAssumption.MaxStack)
                    {
                        Main.SoulAssumption.UseAbility(EnemyHero);
                        await Await.Delay(Main.SoulAssumption.GetCastDelay(EnemyHero), token);
                    }

                }

                if (Config.KillStealItem.Value)
                {
                    var EnemyHero =
                        EntityManager<Hero>.Entities.OrderBy(x => x.Health).FirstOrDefault(
                            x => !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Context.Owner.Team);

                    if (EnemyHero == null)
                    {
                        return;
                    }

                    if (EnemyHero.Health <= Damage(EnemyHero))
                    {
                        // SoulAssumption
                        if (Main.SoulAssumption != null
                            && Config.KillStealToggler.Value.IsEnabled(Main.SoulAssumption.Ability.Name)
                            && Main.SoulAssumption.CanBeCasted
                            && Main.SoulAssumption.CanHit(EnemyHero)
                            && (Main.Dagon == null || Main.Dagon.CanHit(EnemyHero)))
                        {
                            Main.SoulAssumption.UseAbility(EnemyHero);
                            await Await.Delay(Main.SoulAssumption.GetCastDelay(EnemyHero), token);
                        }

                        // Dagon
                        if (Main.Dagon != null
                             && Config.KillStealToggler.Value.IsEnabled("item_dagon_5")
                             && Main.Dagon.CanBeCasted
                             && Main.Dagon.CanHit(EnemyHero)
                             && (Main.Ethereal == null || (EnemyHero.IsEthereal() && !Main.Ethereal.CanBeCasted)
                             || !Config.ItemsToggler.Value.IsEnabled(Main.Ethereal.Item.Name)))
                        {
                            Main.Dagon.UseAbility(EnemyHero);
                            await Await.Delay(Main.Dagon.GetCastDelay(EnemyHero), token);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public float Damage(Hero EnemyHero)
        {
            var SoulAssumption = Main.SoulAssumption != null 
                && Main.SoulAssumption.CanBeCasted 
                && Config.KillStealToggler.Value.IsEnabled(Main.SoulAssumption.Ability.Name)
                ? Main.SoulAssumption.GetDamage(EnemyHero) 
                : 0;

            var Dagon = Main.Dagon != null 
                && Main.Dagon.CanBeCasted
                && Config.KillStealToggler.Value.IsEnabled("item_dagon_5")
                ? Main.Dagon.GetDamage(EnemyHero) 
                : 0;

            return SoulAssumption += Dagon;
        }
    }
}