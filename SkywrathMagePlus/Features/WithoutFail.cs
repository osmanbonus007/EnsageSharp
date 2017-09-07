using System.ComponentModel;

using Ensage;

namespace SkywrathMagePlus.Features
{
    internal class WithoutFail
    {
        private Config Config { get; }

        private UpdateMode UpdateMode { get; }

        public WithoutFail(Config config)
        {
            Config = config;
            UpdateMode = config.UpdateMode;

            if (config.WWithoutFailItem)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }

            config.WWithoutFailItem.PropertyChanged += WWithoutFailChanged;
        }

        public void Dispose()
        {
            Config.WWithoutFailItem.PropertyChanged -= WWithoutFailChanged;

            if (Config.WWithoutFailItem)
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
            }
        }

        private void WWithoutFailChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.WWithoutFailItem)
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
                && args.Ability.Name == "skywrath_mage_concussive_shot" 
                && UpdateMode.WShow == null)
            {
                args.Process = false;
                Game.PrintMessage($"<font color='#FF6666'>There is no one in the radius.</font>");
            }
        }
    }
}
