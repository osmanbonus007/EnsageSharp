using System;
using System.ComponentModel;

using Ensage;
using Ensage.SDK.Menu;

namespace DotaMapPlus
{
    internal class ConsoleCommands
    {
        private MenuItem<bool> FogItem { get; }

        private MenuItem<bool> FilteringItem { get; }

        private MenuItem<bool> ParticleHackItem { get; }

        private ConVar Fog { get; }

        private ConVar Filtering { get; }

        private ConVar ParticleHack { get; }

        public ConsoleCommands(MenuFactory MenuFactory)
        {
            var ConsoleCommandsMenu = MenuFactory.Menu("Console Commands");
            FogItem = ConsoleCommandsMenu.Item("Fog Disable", true);
            FilteringItem = ConsoleCommandsMenu.Item("Filtering Disable", true);
            ParticleHackItem = ConsoleCommandsMenu.Item("Particle Hack Enable", true);

            Fog = Game.GetConsoleVar("fog_enable");
            Fog.SetValue(Convert.ToInt32(!FogItem.Value));

            Filtering = Game.GetConsoleVar("fow_client_nofiltering");
            Filtering.SetValue(Convert.ToInt32(FilteringItem.Value));

            ParticleHack = Game.GetConsoleVar("dota_use_particle_fow");
            ParticleHack.SetValue(Convert.ToInt32(!ParticleHackItem.Value));

            FogItem.PropertyChanged += FogItemChanged;
            FilteringItem.PropertyChanged += FilteringItemChanged;
            ParticleHackItem.PropertyChanged += ParticleHackItemChanged;
        }

        public void Dispose()
        {
            Fog.SetValue(1);
            Filtering.SetValue(0);
            ParticleHack.SetValue(1);

            FogItem.PropertyChanged -= FogItemChanged;
            FilteringItem.PropertyChanged -= FilteringItemChanged;
            ParticleHackItem.PropertyChanged -= ParticleHackItemChanged;
        }

        private void FogItemChanged(object sender, PropertyChangedEventArgs e)
        {
            Fog.SetValue(Convert.ToInt32(!FogItem.Value));
        }

        private void FilteringItemChanged(object sender, PropertyChangedEventArgs e)
        {
            Filtering.SetValue(Convert.ToInt32(FilteringItem.Value));
        }

        private void ParticleHackItemChanged(object sender, PropertyChangedEventArgs e)
        {
            ParticleHack.SetValue(Convert.ToInt32(!ParticleHackItem.Value));
        }
    }
}
