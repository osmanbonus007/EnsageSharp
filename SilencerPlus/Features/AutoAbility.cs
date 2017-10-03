using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace SilencerPlus.Features
{
    internal class AutoAbility
    {
        private Config Config { get; }

        private SilencerPlus Main { get; }

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

                var Target =
                    EntityManager<Hero>.Entities.FirstOrDefault(x =>
                                                                !x.IsIllusion &&
                                                                x.IsValid &&
                                                                x.IsVisible &&
                                                                x.IsAlive &&
                                                                x.IsEnemy(Owner) &&
                                                                Disable(x));

                if (Target == null || !Config.GlobalSilenceItem)
                {
                    return;
                }

                // GlobalSilence
                var GlobalSilence = Main.GlobalSilence;
                if (GlobalSilence.CanBeCasted)
                {
                    GlobalSilence.UseAbility();
                    await Await.Delay(GlobalSilence.GetCastDelay(), token);
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

        private bool Disable(Hero target)
        {
            var BlackHole = target.GetAbilityById(AbilityId.enigma_black_hole);
            if (BlackHole != null && Config.GlobalSilenceToggler.Value.IsEnabled(BlackHole.Name))
            {
                return BlackHole.IsChanneling;
            }

            var DeathWard = target.GetAbilityById(AbilityId.witch_doctor_death_ward);
            if (DeathWard != null && Config.GlobalSilenceToggler.Value.IsEnabled(DeathWard.Name))
            {
                return DeathWard.IsChanneling;
            }

            var FreezingField = target.GetAbilityById(AbilityId.crystal_maiden_freezing_field);
            if (FreezingField != null && Config.GlobalSilenceToggler.Value.IsEnabled(FreezingField.Name))
            {
                return FreezingField.IsChanneling;
            }

            var Dismember = target.GetAbilityById(AbilityId.pudge_dismember);
            if (Dismember != null && Config.GlobalSilenceToggler.Value.IsEnabled(Dismember.Name))
            {
                return Dismember.IsChanneling;
            }

            var Epicenter = target.GetAbilityById(AbilityId.sandking_epicenter);
            if (Epicenter != null && Config.GlobalSilenceToggler.Value.IsEnabled(Epicenter.Name))
            {
                return Epicenter.IsChanneling;
            }

            var FiendsGrip = target.GetAbilityById(AbilityId.bane_fiends_grip);
            if (FiendsGrip != null && Config.GlobalSilenceToggler.Value.IsEnabled(FiendsGrip.Name))
            {
                return FiendsGrip.IsChanneling;
            }

            return false;
        }
    }
}