using System;

using SharpDX;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

namespace JungleScanPlus
{
    internal class JungleScanPlusConfig : IDisposable
    {
        private bool Disposed { get; set; }

        public MenuFactory Factory { get; }

        public MenuItem<bool> DrawWorldItem { get; }

        public MenuItem<Slider> RedItem { get; set; }

        public MenuItem<Slider> GreenItem { get; }

        public MenuItem<Slider> BlueItem { get; }

        public MenuItem<Slider> AlphaItem { get; }

        public MenuItem<Slider> TimerItem { get; }

        public JungleScanPlusConfig()
        {
            Factory = MenuFactory.CreateWithTexture("JungleScanPlus", "junglescanplus");
            Factory.Target.SetFontColor(Color.Aqua);

            DrawWorldItem = Factory.Item("Draw On World", true);

            RedItem = Factory.Item("Red", new Slider(0, 0, 255));
            RedItem.Item.SetFontColor(Color.Red);

            GreenItem = Factory.Item("Green", new Slider(255, 0, 255));
            GreenItem.Item.SetFontColor(Color.Green);

            BlueItem = Factory.Item("Blue", new Slider(255, 0, 255));
            BlueItem.Item.SetFontColor(Color.Blue);

            AlphaItem = Factory.Item("Alpha", new Slider(255, 0, 255));
            TimerItem = Factory.Item("Timer", new Slider(6, 1, 9));
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
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
