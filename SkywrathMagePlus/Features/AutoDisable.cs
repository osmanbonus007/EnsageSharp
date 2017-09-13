using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace SkywrathMagePlus.Features
{
    internal class AutoDisable
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        private SkywrathMagePlus Main { get; }

        private TaskHandler Handler { get; }

        public AutoDisable(Config config)
        {
            Config = config;
            Context = config.SkywrathMagePlus.Context;
            Main = config.SkywrathMagePlus;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.AutoDisableItem)
            {
                Handler.RunAsync();
            }

            config.AutoDisableItem.PropertyChanged += AutoDisableChanged;
        }

        public void Dispose()
        {
            Config.AutoDisableItem.PropertyChanged -= AutoDisableChanged;

            if (Config.AutoDisableItem)
            {
                Handler?.Cancel();
            }
        }

        private void AutoDisableChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.AutoDisableItem)
            {
                Handler.RunAsync();
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
                var Hero = 
                    EntityManager<Hero>.Entities.Where(
                        x => !x.IsIllusion &&
                        x.IsAlive &&
                        x.IsVisible &&
                        x.IsValid &&
                        x.Team != Context.Owner.Team).ToList();

                foreach (var Target in Hero)
                {
                    if (Config.Data.Disable(Target))
                    {
                        // Hex
                        if (Main.Hex != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.Hex.Item.Name)
                            && Main.Hex.CanBeCasted
                            && Main.Hex.CanHit(Target))
                        {
                            Main.Hex.UseAbility(Target);
                            await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                        }

                        // Orchid
                        if (Main.Orchid != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                            && Main.Orchid.CanBeCasted
                            && Main.Orchid.CanHit(Target))
                        {
                            Main.Orchid.UseAbility(Target);
                            await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                        }

                        // Bloodthorn
                        if (Main.Bloodthorn != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                            && Main.Bloodthorn.CanBeCasted
                            && Main.Bloodthorn.CanHit(Target))
                        {
                            Main.Bloodthorn.UseAbility(Target);
                            await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                        }

                        // AncientSeal
                        if (Main.AncientSeal != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.AncientSeal.Ability.Name)
                            && Main.AncientSeal.CanBeCasted
                            && Main.AncientSeal.CanHit(Target))
                        {
                            Main.AncientSeal.UseAbility(Target);
                            await Await.Delay(Main.AncientSeal.GetCastDelay(Target), token);
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
    }
}
