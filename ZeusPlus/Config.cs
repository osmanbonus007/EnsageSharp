using System;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;

using SharpDX;

using ZeusPlus.Features;

namespace ZeusPlus
{
    internal class Config : IDisposable
    {
        public ZeusPlus Main { get; }

        public Vector2 Screen { get; }

        public MenuManager Menu { get; }

        public UpdateMode UpdateMode { get; }

        public DamageCalculation DamageCalculation { get; }

        public AutoKillSteal AutoKillSteal { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private TeleportBreaker TeleportBreaker { get; }

        private FarmMode FarmMode { get; }

        private Mode Mode { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }
        
        public Config(ZeusPlus main)
        {
            Main = main;
            Screen = new Vector2(Drawing.Width - 160, Drawing.Height);

            Menu = new MenuManager(this);
            UpdateMode = new UpdateMode(this);
            LinkenBreaker = new LinkenBreaker(this);
            DamageCalculation = new DamageCalculation(this);
            AutoKillSteal = new AutoKillSteal(this);
            TeleportBreaker = new TeleportBreaker(this);
            FarmMode = new FarmMode(this, main.Context);
            Main.Context.Orbwalker.RegisterMode(FarmMode);

            Menu.ComboKeyItem.Item.ValueChanged += ComboKeyChanged;
            var ModeKey = KeyInterop.KeyFromVirtualKey((int)Menu.ComboKeyItem.Value.Key);
            Mode = new Mode(Main.Context, ModeKey, this);
            Main.Context.Orbwalker.RegisterMode(Mode);

            Renderer = new Renderer(this);
        }

        private void ComboKeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var keyCode = e.GetNewValue<KeyBind>().Key;
            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var key = KeyInterop.KeyFromVirtualKey((int)keyCode);
            Mode.Key = key;
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
                Renderer.Dispose();
                Main.Context.Orbwalker.UnregisterMode(Mode);
                Menu.ComboKeyItem.Item.ValueChanged -= ComboKeyChanged;
                Main.Context.Orbwalker.UnregisterMode(FarmMode);
                TeleportBreaker.Dispose();
                AutoKillSteal.Dispose();
                DamageCalculation.Dispose();
                UpdateMode.Dispose();
                Main.Context.Particle.Dispose();
                Menu.Dispose();
            }

            Disposed = true;
        }
    }
}
