using System;

using Ensage.SDK.Menu;
using Ensage.SDK.Input;

using SharpDX;

namespace DotaMapPlus
{
    public class DotaMapPlusConfig : IDisposable
    {
        private MenuFactory MenuFactory { get; }

        private ZoomHack ZoomHack { get; }

        private ConsoleCommands ConsoleCommands { get; }

        private WeatherHack WeatherHack { get; }

        private bool Disposed { get; set; }

        public DotaMapPlusConfig(Lazy<IInputManager> InputManager)
        {
            MenuFactory = MenuFactory.CreateWithTexture("DotaMapPlus", "dotamapplus");
            MenuFactory.Target.SetFontColor(Color.Aqua);

            ZoomHack = new ZoomHack(MenuFactory, InputManager);

            ConsoleCommands = new ConsoleCommands(MenuFactory);

            WeatherHack = new WeatherHack(MenuFactory);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                ZoomHack.Dispose();
                ConsoleCommands.Dispose();
                WeatherHack.Dispose();
                MenuFactory.Dispose();
            }

            Disposed = true;
        }
    }
}
