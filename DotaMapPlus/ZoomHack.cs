using System;
using System.ComponentModel;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Input;
using Ensage.SDK.Menu;

namespace DotaMapPlus
{
    internal class ZoomHack
    {
        private MenuItem<KeyBind> ZoomKeyItem { get; }

        private MenuItem<Slider> ZoomSliderItem { get; }

        private ConVar ZoomVar { get; }

        private ConVar RVar { get; }

        private int DefaultZoomValue { get; } = 1134;

        private int MaxZoomValue { get; } = 9000;

        private int MinZoomValue { get; } = 1134;

        private Lazy<IInputManager> InputManage { get; }

        public ZoomHack(MenuFactory MenuFactory, Lazy<IInputManager> inputmanager)
        {
            InputManage = inputmanager;

            var ZoomHackMenu = MenuFactory.Menu("Zoom Hack");
            ZoomKeyItem = ZoomHackMenu.Item("Key", new KeyBind(0x11, KeyBindType.Press));
            ZoomSliderItem = ZoomHackMenu.Item("Camera Distance", new Slider(DefaultZoomValue, MinZoomValue, MaxZoomValue));

            ZoomVar = Game.GetConsoleVar("dota_camera_distance");
            ZoomVar.SetValue(ZoomSliderItem.Value);

            RVar = Game.GetConsoleVar("r_farz");
            RVar.SetValue(2 * ZoomSliderItem.Value);

            ZoomSliderItem.PropertyChanged += ZoomSliderItemChanged;
            InputManage.Value.MouseWheel += InputManagerMouseWheel;

            Game.ExecuteCommand("dota_camera_disable_zoom true");
        }

        public void Dispose()
        {
            Game.ExecuteCommand("dota_camera_disable_zoom false");

            ZoomVar.SetValue(DefaultZoomValue);

            ZoomSliderItem.PropertyChanged -= ZoomSliderItemChanged;
            InputManage.Value.MouseWheel -= InputManagerMouseWheel;
        }

        private void InputManagerMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ZoomKeyItem.Value)
            {
                var ZoomValue = ZoomVar.GetInt();

                if (e.Delta < 0)
                {
                    ZoomValue += 50;
                    ZoomValue = Math.Min(ZoomValue, MaxZoomValue);
                }
                else
                {
                    ZoomValue -= 50;
                    ZoomValue = Math.Max(ZoomValue, MinZoomValue);
                }

                ZoomVar.SetValue(ZoomValue);

                ZoomSliderItem.Item.SetValue(new Slider(ZoomValue, MinZoomValue, MaxZoomValue));
            }
        }

        private void ZoomSliderItemChanged(object sender, PropertyChangedEventArgs e)
        {
            ZoomVar.SetValue(ZoomSliderItem.Value);
            RVar.SetValue(2 * ZoomSliderItem.Value);
        }
    }
}
