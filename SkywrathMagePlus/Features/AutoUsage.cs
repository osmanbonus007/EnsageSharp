using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace SkywrathMagePlus.Features
{
    internal class AutoUsage
    {
        private Config Config { get; }

        private SkywrathMagePlus Main { get; }

        private TaskHandler Handler { get; }

        public AutoUsage(Config config)
        {
            Config = config;
            Main = config.SkywrathMagePlus;

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
                // Eul
                if (Config.EulBladeMailItem)
                {
                    var Heros =
                        EntityManager<Hero>.Entities.FirstOrDefault(
                            x => !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Main.Context.Owner.Team &&
                            x.HasModifier("modifier_item_blade_mail_reflect") &&
                            x.HasModifier("modifier_skywrath_mystic_flare_aura_effect"));

                    if (Heros != null 
                        && Main.Eul != null 
                        && Main.Eul.CanBeCasted)
                    {
                        Main.Eul.UseAbility(Main.Context.Owner);
                        await Await.Delay(Main.Eul.GetCastDelay(Main.Context.Owner), token);
                    }
                }

                // ArcaneBolt
                if (!Config.ComboKeyItem 
                    && !Config.SpamKeyItem 
                    && Config.AutoQKeyItem)
                {
                    var Target =
                        EntityManager<Hero>.Entities.OrderBy(
                            order => order.Health).FirstOrDefault(
                            x => !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Main.Context.Owner.Team &&
                            Main.ArcaneBolt.CanHit(x));


                    if (Target != null
                        && Main.ArcaneBolt != null
                        && Main.ArcaneBolt.CanBeCasted)
                    {
                        Main.ArcaneBolt.UseAbility(Target);
                        await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
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
    }
}
