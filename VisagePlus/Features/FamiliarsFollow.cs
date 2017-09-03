using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace VisagePlus.Features
{
    internal class FamiliarsFollow
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        public FamiliarsFollow(Config config)
        {
            Config = config;
            Context = config.VisagePlus.Context;

            config.FollowKeyItem.PropertyChanged += FollowKeyChanged;

            if (Config.FollowKeyItem.Value)
            {
                UpdateManager.Subscribe(Follow, 200);

                config.FamiliarsLastHitItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
        }

        public void Dispose()
        {
            Config.FollowKeyItem.PropertyChanged -= FollowKeyChanged;

            UpdateManager.Unsubscribe(Follow);
        }

        private void FollowKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.FollowKeyItem.Value)
            {
                UpdateManager.Subscribe(Follow, 200);

                Config.FamiliarsLastHitItem.Item.SetValue(new KeyBind(
                    Config.FamiliarsLastHitItem.Item.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
            }
            else
            {
                UpdateManager.Unsubscribe(Follow);
            }
        }

        private void Follow()
        {
            var Familiars =
                EntityManager<Unit>.Entities.Where(
                    x =>
                    x.IsValid &&
                    x.IsAlive &&
                    x.IsControllable &&
                    x.Team == Context.Owner.Team &&
                    x.Name.Contains("npc_dota_visage_familiar")).ToArray();

            foreach (var Familiar in Familiars)
            {
                Familiar.Follow(Context.Owner);
            }
        }
    }
}