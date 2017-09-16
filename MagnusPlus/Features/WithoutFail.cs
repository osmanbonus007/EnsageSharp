using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;

namespace MagnusPlus.Features
{
    internal class WithoutFail
    {
        private Config Config { get; }

        public WithoutFail(Config config)
        {
            Config = config;

            if (config.RPWithoutFailItem)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }

            config.RPWithoutFailItem.PropertyChanged += WWithoutFailChanged;
        }

        public void Dispose()
        {
            Config.RPWithoutFailItem.PropertyChanged -= WWithoutFailChanged;

            if (Config.RPWithoutFailItem)
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
            }
        }

        private void WWithoutFailChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.RPWithoutFailItem)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            else
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (args.OrderId == OrderId.Ability 
                && args.Ability.Name == "magnataur_reverse_polarity")
            {
                var Targets =
                    EntityManager<Hero>.Entities.FirstOrDefault(
                        x => x.IsValid &&
                        x.IsVisible &&
                        x.Team != Config.MagnusPlus.Context.Owner.Team &&
                        !x.IsIllusion &&
                        Config.MagnusPlus.Context.Owner.Distance2D(x) 
                        <= Config.MagnusPlus.ReversePolarity.Radius);

                if (Targets == null)
                {
                    Config.MagnusPlus.Context.Owner.Stop();
                    Game.PrintMessage($"<font color='#FF6666'>There is no one in the radius.</font>");
                }
            }
        }
    }
}
