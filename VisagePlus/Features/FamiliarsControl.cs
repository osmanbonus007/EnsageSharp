using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
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

        private IServiceContext Context { get; }

        private TaskHandler Handler { get; }

        public FamiliarsControl(Config config)
        {
            Config = config;
            Context = config.VisagePlus.Context;

            config.FollowKeyItem.PropertyChanged += FollowKeyChanged;

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
                        x.NetworkName == "CDOTA_Unit_VisageFamiliar").ToArray();

                var Others =
                    EntityManager<Unit>.Entities.Where(
                        x => !x.IsIllusion &&
                        x.IsValid &&
                        x.IsVisible &&
                        x.IsAlive &&
                        Context.Owner.IsEnemy(x));

                foreach (var Familiar in Familiars)
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

                    var Courier = Others.OrderBy(x => x.Distance2D(Familiar)).FirstOrDefault(x => x.NetworkName == "CDOTA_Unit_Courier");

                    if (Courier != null && Familiar.Distance2D(Courier) <= 600 && !Config.FollowKeyItem)
                    {
                        Familiar.Attack(Courier);
                        await Await.Delay(100, token);
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