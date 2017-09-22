using System;
using System.Collections.Generic;
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
    internal class LinkenBreaker
    {
        private Config Config { get; }

        private VisagePlus Main { get; set; }

        public TaskHandler Handler { get; }

        private IOrderedEnumerable<KeyValuePair<string, uint>> BreakerChanger { get; set; }

        public LinkenBreaker(Config config)
        {
            Config = config;
            Main = config.VisagePlus;

            Handler = UpdateManager.Run(ExecuteAsync, false, false);
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Game.IsPaused)
                {
                    return;
                }

                var Target = Config.UpdateMode.Target;

                if (Target.IsLinkensProtected())
                {
                    BreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                        z => Config.LinkenBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
                }
                else if (AntimageShield(Target))
                {
                    BreakerChanger = Config.AntimageBreakerChanger.Value.Dictionary.Where(
                        z => Config.AntimageBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
                }

                if (BreakerChanger == null)
                {
                    return;
                }

                foreach (var Order in BreakerChanger.ToList())
                {
                    // Eul
                    if (Main.Eul != null
                        && Main.Eul.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.Eul.CanBeCasted
                        && Main.Eul.CanHit(Target))
                    {
                        Main.Eul.UseAbility(Target);
                        await Await.Delay(Main.Eul.GetCastDelay(Target), token);
                    }

                    // ForceStaff
                    if (Main.ForceStaff != null
                        && Main.ForceStaff.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.ForceStaff.CanBeCasted
                        && Main.ForceStaff.CanHit(Target))
                    {
                        Main.ForceStaff.UseAbility(Target);
                        await Await.Delay(Main.ForceStaff.GetCastDelay(Target), token);
                    }

                    // Orchid
                    if (Main.Orchid != null
                        && Main.Orchid.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.Orchid.CanBeCasted
                        && Main.Orchid.CanHit(Target))
                    {
                        Main.Orchid.UseAbility(Target);
                        await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                    }

                    // Bloodthorn
                    if (Main.Bloodthorn != null
                        && Main.Bloodthorn.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.Bloodthorn.CanBeCasted
                        && Main.Bloodthorn.CanHit(Target))
                    {
                        Main.Bloodthorn.UseAbility(Target);
                        await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                    }

                    // RodofAtos
                    if (Main.RodofAtos != null
                        && Main.RodofAtos.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.RodofAtos.CanBeCasted
                        && Main.RodofAtos.CanHit(Target))
                    {
                        Main.RodofAtos.UseAbility(Target);
                        await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                    }

                    // SoulAssumption
                    if (Main.SoulAssumption != null
                        && Main.SoulAssumption.Ability.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.SoulAssumption.CanBeCasted
                        && Main.SoulAssumption.CanHit(Target))
                    {
                        Main.SoulAssumption.UseAbility(Target);
                        await Await.Delay(Main.SoulAssumption.GetCastDelay(Target), token);
                    }

                    // Hex
                    if (Main.Hex != null
                        && Main.Hex.Item.Name == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Main.Hex.CanBeCasted
                        && Main.Hex.CanHit(Target))
                    {
                        Main.Hex.UseAbility(Target);
                        await Await.Delay(Main.Hex.GetCastDelay(Target), token);
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

        public bool AntimageShield(Hero Target)
        {
            var Shield = Target.GetAbilityById(AbilityId.antimage_spell_shield);

            return Shield != null
                && Shield.Cooldown == 0
                && Shield.Level > 0
                && Target.GetItemById(AbilityId.item_ultimate_scepter) != null;
        }
    }
}
