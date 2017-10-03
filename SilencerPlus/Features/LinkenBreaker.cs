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

namespace SilencerPlus.Features
{
    internal class LinkenBreaker
    {
        private Config Config { get; }

        private SilencerPlus Main { get; set; }

        private Unit Owner { get; }

        public TaskHandler Handler { get; }

        public LinkenBreaker(Config config)
        {
            Config = config;
            Main = config.Main;
            Owner = config.Main.Context.Owner;

            Handler = UpdateManager.Run(ExecuteAsync, false, false);
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                var Target = Config.UpdateMode.Target;

                if (Target == null)
                {
                    return;
                }

                List<KeyValuePair<string, uint>> BreakerChanger = new List<KeyValuePair<string, uint>>();

                if (Target.IsLinkensProtected())
                {
                    BreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                        x => Config.LinkenBreakerToggler.Value.IsEnabled(x.Key)).OrderByDescending(x => x.Value).ToList();
                }
                else if (AntimageShield(Target))
                {
                    BreakerChanger = Config.AntiMageBreakerChanger.Value.Dictionary.Where(
                        x => Config.AntiMageBreakerToggler.Value.IsEnabled(x.Key)).OrderByDescending(x => x.Value).ToList();
                }

                foreach (var Order in BreakerChanger)
                {
                    // Eul
                    var Eul = Main.Eul;
                    if (Eul != null
                        && Eul.ToString() == Order.Key
                        && Eul.CanBeCasted)
                    {
                        if (Eul.CanHit(Target))
                        {
                            Eul.UseAbility(Target);
                            await Await.Delay(Eul.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // ForceStaff
                    var ForceStaff = Main.ForceStaff;
                    if (ForceStaff != null
                        && ForceStaff.ToString() == Order.Key
                        && ForceStaff.CanBeCasted)
                    {
                        if (ForceStaff.CanHit(Target))
                        {
                            ForceStaff.UseAbility(Target);
                            await Await.Delay(ForceStaff.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // Orchid
                    var Orchid = Main.Orchid;
                    if (Orchid != null
                        && Orchid.ToString() == Order.Key
                        && Orchid.CanBeCasted)
                    {
                        if (Orchid.CanHit(Target))
                        {
                            Orchid.UseAbility(Target);
                            await Await.Delay(Orchid.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // Bloodthorn
                    var Bloodthorn = Main.Bloodthorn;
                    if (Bloodthorn != null
                        && Bloodthorn.ToString() == Order.Key
                        && Bloodthorn.CanBeCasted)
                    {
                        if (Bloodthorn.CanHit(Target))
                        {
                            Bloodthorn.UseAbility(Target);
                            await Await.Delay(Bloodthorn.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // RodofAtos
                    var RodofAtos = Main.RodofAtos;
                    if (RodofAtos != null
                        && RodofAtos.ToString() == Order.Key
                        && RodofAtos.CanBeCasted)
                    {
                        if (RodofAtos.CanHit(Target))
                        {
                            RodofAtos.UseAbility(Target);
                            await Await.Delay(RodofAtos.GetCastDelay(Target) + (int)(Owner.Distance2D(Target) / RodofAtos.Speed * 1000f), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // HeavensHalberd
                    var HeavensHalberd = Main.HeavensHalberd;
                    if (HeavensHalberd != null
                        && HeavensHalberd.ToString() == Order.Key
                        && HeavensHalberd.CanBeCasted)
                    {
                        if (HeavensHalberd.CanHit(Target))
                        {
                            HeavensHalberd.UseAbility(Target);
                            await Await.Delay(HeavensHalberd.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // Enchant
                    var LastWord = Main.LastWord;
                    if (LastWord.ToString() == Order.Key
                        && LastWord.CanBeCasted)
                    {
                        if (LastWord.CanHit(Target))
                        {
                            LastWord.UseAbility(Target);
                            await Await.Delay(LastWord.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // Hex
                    var Hex = Main.Hex;
                    if (Hex != null
                        && Hex.ToString() == Order.Key
                        && Hex.CanBeCasted)
                    {
                        if (Hex.CanHit(Target))
                        {
                            Hex.UseAbility(Target);
                            await Await.Delay(Hex.GetCastDelay(Target), token);
                            return;
                        }
                        else if (Config.UseOnlyFromRangeItem)
                        {
                            return;
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
