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

namespace EnchantressPlus.Features
{
    internal class LinkenBreaker
    {
        private Config Config { get; }

        private EnchantressPlus Main { get; set; }

        public TaskHandler Handler { get; }

        private IOrderedEnumerable<KeyValuePair<string, uint>> BreakerChanger { get; set; }

        public LinkenBreaker(Config config)
        {
            Config = config;
            Main = config.Main;

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
                    var Eul = Main.Eul;
                    if (Eul != null
                        && Eul.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Eul.CanBeCasted
                        && Eul.CanHit(Target))
                    {
                        Eul.UseAbility(Target);
                        await Await.Delay(Eul.GetCastDelay(Target), token);
                    }

                    // ForceStaff
                    var ForceStaff = Main.ForceStaff;
                    if (ForceStaff != null
                        && ForceStaff.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && ForceStaff.CanBeCasted
                        && ForceStaff.CanHit(Target))
                    {
                        ForceStaff.UseAbility(Target);
                        await Await.Delay(ForceStaff.GetCastDelay(Target), token);
                    }

                    // Orchid
                    var Orchid = Main.Orchid;
                    if (Orchid != null
                        && Orchid.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Orchid.CanBeCasted
                        && Orchid.CanHit(Target))
                    {
                        Orchid.UseAbility(Target);
                        await Await.Delay(Orchid.GetCastDelay(Target), token);
                    }

                    // Bloodthorn
                    var Bloodthorn = Main.Bloodthorn;
                    if (Bloodthorn != null
                        && Bloodthorn.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Bloodthorn.CanBeCasted
                        && Bloodthorn.CanHit(Target))
                    {
                        Bloodthorn.UseAbility(Target);
                        await Await.Delay(Bloodthorn.GetCastDelay(Target), token);
                    }

                    // RodofAtos
                    var RodofAtos = Main.RodofAtos;
                    if (RodofAtos != null
                        && RodofAtos.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && RodofAtos.CanBeCasted
                        && RodofAtos.CanHit(Target))
                    {
                        RodofAtos.UseAbility(Target);
                        await Await.Delay(RodofAtos.GetCastDelay(Target), token);
                    }

                    // HeavensHalberd
                    var HeavensHalberd = Main.HeavensHalberd;
                    if (HeavensHalberd != null
                        && HeavensHalberd.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && HeavensHalberd.CanBeCasted
                        && HeavensHalberd.CanHit(Target))
                    {
                        HeavensHalberd.UseAbility(Target);
                        await Await.Delay(HeavensHalberd.GetCastDelay(Target), token);
                    }

                    // Enchant
                    var Enchant = Main.Enchant;
                    if (Enchant != null
                        && Enchant.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Enchant.CanBeCasted
                        && Enchant.CanHit(Target))
                    {
                        Enchant.UseAbility(Target);
                        await Await.Delay(Enchant.GetCastDelay(Target), token);
                    }

                    // Hex
                    var Hex = Main.Hex;
                    if (Hex != null
                        && Hex.ToString() == Order.Key
                        && (Target.IsLinkensProtected() || AntimageShield(Target))
                        && Hex.CanBeCasted
                        && Hex.CanHit(Target))
                    {
                        Hex.UseAbility(Target);
                        await Await.Delay(Hex.GetCastDelay(Target), token);
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
                && Target.HasAghanimsScepter();
        }
    }
}
