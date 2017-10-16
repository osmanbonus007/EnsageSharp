using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace ZeusPlus.Features
{
    internal class AutoKillSteal
    {
        private Config Config { get; }

        private MenuManager Menu { get; }

        private ZeusPlus Main { get; }

        private DamageCalculation DamageCalculation { get; }

        private Unit Owner { get; }

        public Sleeper Sleeper { get; }

        private DamageCalculation.Damage Damage { get; set; }

        private TaskHandler Handler { get; }

        private IUpdateHandler Update { get; set; }
        
        public AutoKillSteal(Config config)
        {
            Config = config;
            Menu = config.Menu;
            Main = config.Main;
            DamageCalculation = config.DamageCalculation;
            Owner = config.Main.Context.Owner;

            Sleeper = new Sleeper();

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (Menu.AutoKillStealItem)
            {
                Handler.RunAsync();
            }

            config.Menu.AutoKillStealItem.PropertyChanged += AutoKillStealChanged;

            Update = UpdateManager.Subscribe(Stop, 0, false);
        }

        public void Dispose()
        {
            Menu.AutoKillStealItem.PropertyChanged -= AutoKillStealChanged;

            if (Menu.AutoKillStealItem)
            {
                Handler?.Cancel();
            }
        }

        private void AutoKillStealChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Menu.AutoKillStealItem)
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
                if (Game.IsPaused || !Owner.IsValid || !Owner.IsAlive || Owner.IsStunned())
                {
                    return;
                }

                var damageCalculation = DamageCalculation.DamageList.OrderByDescending(x => x.GetHealth).OrderByDescending(x => x.GetHero.Player.Kills).ToList();
                var damage = damageCalculation.FirstOrDefault(x => (x.GetHealth - x.GetDamage) / x.GetHero.MaximumHealth <= 0.0f);               

                Damage = damage;

                if (damage == null)
                {
                    return;
                }

                if (!Update.IsEnabled)
                {
                    Update.IsEnabled = true;
                }

                var target = damage.GetHero;

                if (Cancel(target))
                {
                    return;
                }

                if (!target.IsLinkensProtected() && !Config.LinkenBreaker.AntimageShield(target))
                {
                    // Veil
                    var Veil = Main.Veil;
                    if (Veil != null
                        && Menu.AutoKillStealToggler.Value.IsEnabled(Veil.ToString())
                        && Veil.CanBeCasted
                        && Veil.CanHit(target))
                    {
                        Veil.UseAbility(target.Position);
                        await Await.Delay(Veil.GetCastDelay(target.Position), token);
                    }

                    // Ethereal
                    var Ethereal = Main.Ethereal;
                    if (Ethereal != null
                        && Menu.AutoKillStealToggler.Value.IsEnabled(Ethereal.ToString())
                        && Ethereal.CanBeCasted
                        && Ethereal.CanHit(target))
                    {
                        Ethereal.UseAbility(target);
                        Sleeper.Sleep(Ethereal.GetHitTime(target));
                        await Await.Delay(Ethereal.GetCastDelay(target), token);
                    }

                    // Shivas
                    var Shivas = Main.Shivas;
                    if (Shivas != null
                        && Menu.AutoKillStealToggler.Value.IsEnabled(Shivas.ToString())
                        && Shivas.CanBeCasted
                        && Shivas.CanHit(target))
                    {
                        Shivas.UseAbility();
                        await Await.Delay(Shivas.GetCastDelay(), token);
                    }

                    if (!Sleeper.Sleeping || target.IsEthereal())
                    {
                        // Lightning Bolt
                        var LightningBolt = Main.LightningBolt;
                        if (Menu.AutoKillStealToggler.Value.IsEnabled(LightningBolt.ToString())
                            && LightningBolt.CanBeCasted
                            && LightningBolt.CanHit(target))
                        {
                            LightningBolt.UseAbility(target);
                            await Await.Delay(LightningBolt.GetCastDelay(target), token);
                        }

                        // Arc Lightning
                        var ArcLightning = Main.ArcLightning;
                        if (Menu.AutoKillStealToggler.Value.IsEnabled(ArcLightning.ToString())
                            && ArcLightning.CanBeCasted
                            && ArcLightning.CanHit(target))
                        {
                            ArcLightning.UseAbility(target);
                            await Await.Delay(ArcLightning.GetCastDelay(target), token);
                            return;
                        }

                        // Dagon
                        var Dagon = Main.Dagon;
                        if (Dagon != null
                            && Menu.AutoKillStealToggler.Value.IsEnabled("item_dagon_5")
                            && Dagon.CanBeCasted
                            && Dagon.CanHit(target))
                        {
                            Dagon.UseAbility(target);
                            await Await.Delay(Dagon.GetCastDelay(target), token);
                            return;
                        }

                        // Nimbus
                        var Nimbus = Main.Nimbus;
                        if (Menu.AutoKillStealToggler.Value.IsEnabled(Nimbus.ToString()) && Nimbus.CanBeCasted)
                        {
                            Nimbus.UseAbility(target.Position);
                            await Await.Delay(Nimbus.GetCastDelay(target.Position), token);
                            return;
                        }

                        // Thundergods Wrath
                        var ThundergodsWrath = Main.ThundergodsWrath;
                        if (Menu.AutoKillStealToggler.Value.IsEnabled(ThundergodsWrath.ToString()) && ThundergodsWrath.CanBeCasted)
                        {
                            ThundergodsWrath.UseAbility();
                            await Await.Delay(ThundergodsWrath.GetCastDelay(), token);
                        }
                    }
                }
                else
                {
                    Config.LinkenBreaker.Handler.RunAsync();
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

        private bool Cancel(Hero target)
        {
            var reincarnation = target.GetAbilityById(AbilityId.skeleton_king_reincarnation);

            return target.IsMagicImmune()
                || target.IsInvulnerable()
                || target.HasAnyModifiers("modifier_dazzle_shallow_grave")
                || (reincarnation != null && reincarnation.Cooldown == 0 && reincarnation.Level > 0);
        }

        private void Stop()
        {
            if (Damage == null)
            {
                Update.IsEnabled = false;
                return;
            }

            var stop = EntityManager<Hero>.Entities.Any(x => !x.IsAlive && x == Damage.GetHero);
            if (stop && Owner.Animation.Name.Contains("cast"))
            {
                Owner.Stop();
                Update.IsEnabled = false;
            }
        }
    }
}