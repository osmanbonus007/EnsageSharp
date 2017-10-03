using System;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace EnchantressPlus.Features
{
    internal class AutoAbility
    {
        private Config Config { get; }

        private EnchantressPlus Main { get; }

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

                // NaturesAttendants
                var NaturesAttendants = Main.NaturesAttendants;
                if (Config.NaturesAttendantsItem
                    && (float)Owner.Health / Owner.MaximumHealth * 100 < Config.MinHPItem
                    && NaturesAttendants.CanBeCasted)
                {
                    NaturesAttendants.UseAbility();
                    await Await.Delay(NaturesAttendants.GetCastDelay() + 100, token);
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