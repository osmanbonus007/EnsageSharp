using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace VisagePlus.Features
{
    internal class FamiliarsControl
    {
        private Config Config { get; }

        private Data Data { get; }

        private IServiceContext Context { get; }

        public Sleeper FamiliarsSleeper { get; }

        private TaskHandler Handler { get; }

        public FamiliarsControl(Config config)
        {
            Config = config;
            Data = config.Data;
            Context = config.VisagePlus.Context;

            FamiliarsSleeper = new Sleeper();

            config.FollowKeyItem.PropertyChanged += FollowKeyChanged;
            config.EscapeKeyItem.PropertyChanged += EscapeKeyChanged;

            if (Config.FollowKeyItem)
            {
                config.FollowKeyItem.Item.SetValue(new KeyBind(
                    config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
                
            Handler = UpdateManager.Run(ExecuteAsync, true, true);
        }

        public void Dispose()
        {
            Handler?.Cancel();

            Config.EscapeKeyItem.PropertyChanged -= EscapeKeyChanged;
            Config.FollowKeyItem.PropertyChanged -= FollowKeyChanged;
        }

        private void FollowKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FollowKeyItem)
            {
                Config.LastHitItem.Item.SetValue(new KeyBind(
                    Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        private void EscapeKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.EscapeKeyItem)
            {
                Config.LastHitItem.Item.SetValue(new KeyBind(
                    Config.LastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FollowKeyItem.Item.SetValue(new KeyBind(
                    Config.FollowKeyItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));

                Config.FamiliarsLockItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLockItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Game.IsPaused)
                {
                    return;
                }

                var Familiars =
                    EntityManager<Unit>.Entities.Where(
                        x =>
                        x.IsValid &&
                        x.IsAlive &&
                        x.IsControllable &&
                        Context.Owner.IsAlly(x) &&
                        x.NetworkName == "CDOTA_Unit_VisageFamiliar");

                var Others =
                    EntityManager<Unit>.Entities.Where(
                        x => !x.IsIllusion &&
                        x.IsValid &&
                        x.IsVisible &&
                        x.IsAlive &&
                        Context.Owner.IsEnemy(x));

                foreach (var Familiar in Familiars.ToArray())
                {
                    var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);

                    if (Familiar.Health * 100 / Familiar.MaximumHealth <= Config.FamiliarsLowHPItem
                        && AbilityExtensions.CanBeCasted(FamiliarsStoneForm))
                    {
                        FamiliarsStoneForm.UseAbility();
                        await Await.Delay(100 + (int)Game.Ping, token);
                    }

                    if (Config.FollowKeyItem)
                    {
                        Familiar.Follow(Context.Owner);
                        await Await.Delay(100, token);
                    }

                    if (Config.FamiliarsCourierItem)
                    {
                        var courier = Others.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(x => x.NetworkName == "CDOTA_Unit_Courier");

                        if (courier != null && Familiar.Distance2D(courier) <= 600 && !Config.FollowKeyItem)
                        {
                            Familiar.Attack(courier);
                            await Await.Delay(100, token);
                        }
                    }

                    if (Config.EscapeKeyItem 
                        && !Config.ComboKeyItem 
                        && !Config.FamiliarsLockItem
                        && !Config.LastHitItem
                        && !Config.FollowKeyItem)
                    {
                        var hero = Others.OrderBy(x => x.Distance2D(Context.Owner)).FirstOrDefault(x => x is Hero);

                        if (hero == null 
                            || hero.IsMagicImmune() 
                            || Data.CancelCombo(hero) 
                            || Context.Owner.Distance2D(hero) > 800)
                        {
                            Familiar.Follow(Context.Owner);
                            await Await.Delay(100, token);
                            
                            continue;
                        }

                        if (FamiliarsStoneForm != null
                                && AbilityExtensions.CanBeCasted(FamiliarsStoneForm)
                                && Familiar.Distance2D(hero) <= 100
                                && !FamiliarsSleeper.Sleeping)
                        {
                            FamiliarsStoneForm.UseAbility();
                            FamiliarsSleeper.Sleep(FamiliarsStoneForm.GetAbilitySpecialData("stun_duration") * 1000 - 200);
                            await Await.Delay(100 + (int)Game.Ping, token);
                        }
                        else if (AbilityExtensions.CanBeCasted(FamiliarsStoneForm))
                        {
                            Familiar.Move(hero.Position);
                            await Await.Delay(100, token);
                        }
                        else
                        {
                            Familiar.Follow(Context.Owner);
                            await Await.Delay(100, token);
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
                Config.VisagePlus.Log.Error(e);
            }
        }
    }
}