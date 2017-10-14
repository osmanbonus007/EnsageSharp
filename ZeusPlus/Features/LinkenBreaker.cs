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

namespace ZeusPlus.Features
{
    internal class LinkenBreaker
    {
        private Config Config { get; }

        private MenuManager Menu { get; }

        private ZeusPlus Main { get; set; }

        private Unit Owner { get; }

        public TaskHandler Handler { get; }

        public LinkenBreaker(Config config)
        {
            Config = config;
            Menu = config.Menu;
            Main = config.Main;
            Owner = config.Main.Context.Owner;

            Handler = UpdateManager.Run(ExecuteAsync, false, false);
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                var target = Config.UpdateMode.Target;

                if (target == null)
                {
                    return;
                }

                List<KeyValuePair<string, uint>> BreakerChanger = new List<KeyValuePair<string, uint>>();

                if (target.IsLinkensProtected())
                {
                    BreakerChanger = Menu.LinkenBreakerChanger.Value.Dictionary.Where(
                        x => Menu.LinkenBreakerToggler.Value.IsEnabled(x.Key)).OrderByDescending(x => x.Value).ToList();
                }
                else if (AntimageShield(target))
                {
                    BreakerChanger = Menu.AntiMageBreakerChanger.Value.Dictionary.Where(
                        x => Menu.AntiMageBreakerToggler.Value.IsEnabled(x.Key)).OrderByDescending(x => x.Value).ToList();
                }

                foreach (var Order in BreakerChanger)
                {
                    // Eul
                    var Eul = Main.Eul;
                    if (Eul != null
                        && Eul.ToString() == Order.Key
                        && Eul.CanBeCasted)
                    {
                        if (Eul.CanHit(target))
                        {
                            Eul.UseAbility(target);
                            await Await.Delay(Eul.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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
                        if (ForceStaff.CanHit(target))
                        {
                            ForceStaff.UseAbility(target);
                            await Await.Delay(ForceStaff.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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
                        if (Orchid.CanHit(target))
                        {
                            Orchid.UseAbility(target);
                            await Await.Delay(Orchid.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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
                        if (Bloodthorn.CanHit(target))
                        {
                            Bloodthorn.UseAbility(target);
                            await Await.Delay(Bloodthorn.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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
                        if (RodofAtos.CanHit(target))
                        {
                            RodofAtos.UseAbility(target);
                            await Await.Delay(RodofAtos.GetCastDelay(target) + RodofAtos.GetHitTime(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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
                        if (Hex.CanHit(target))
                        {
                            Hex.UseAbility(target);
                            await Await.Delay(Hex.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // ArcLightning
                    var ArcLightning = Main.ArcLightning;
                    if (ArcLightning.ToString() == Order.Key
                        && ArcLightning.CanBeCasted)
                    {
                        if (ArcLightning.CanHit(target))
                        {
                            ArcLightning.UseAbility(target);
                            await Await.Delay(ArcLightning.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
                        {
                            return;
                        }
                    }

                    // LightningBolt
                    var LightningBolt = Main.LightningBolt;
                    if (LightningBolt.ToString() == Order.Key
                        && LightningBolt.CanBeCasted)
                    {
                        if (LightningBolt.CanHit(target))
                        {
                            LightningBolt.UseAbility(target);
                            await Await.Delay(LightningBolt.GetCastDelay(target), token);
                            return;
                        }
                        else if (Menu.UseOnlyFromRangeItem)
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

            return Shield != null && Shield.Cooldown == 0 && Shield.Level > 0 && Target.HasAghanimsScepter();
        }
    }
}
