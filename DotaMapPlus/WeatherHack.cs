using System.ComponentModel;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Menu;

namespace DotaMapPlus
{
    internal class WeatherHack
    {
        private MenuItem<StringList> WeatherItem { get; }

        private ConVar Weather { get; }

        public WeatherHack(MenuFactory MenuFactory)
        {
            var WeatherHackMenu = MenuFactory.Menu("Weather Hack");
            WeatherItem = WeatherHackMenu.Item("Selected", new StringList(new[] 
            {
                "Default",
                "Snow",
                "Rain",
                "Moonbeam",
                "Pestilence",
                "Harvest",
                "Sirocco",
                "Ash",
                "Aurora"
            }));

            Weather = Game.GetConsoleVar("cl_weather");
            Weather.SetValue(WeatherItem.Value.SelectedIndex);

            WeatherItem.PropertyChanged += WeatherItemChanged;
        }

        public void Dispose()
        {
            Weather.SetValue(0);

            WeatherItem.PropertyChanged -= WeatherItemChanged;
        }

        private void WeatherItemChanged(object sender, PropertyChangedEventArgs e)
        {
            Weather.SetValue(WeatherItem.Value.SelectedIndex);
        }
    }
}
