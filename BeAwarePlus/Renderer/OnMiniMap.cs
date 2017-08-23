using System;
using System.Linq;
using System.ComponentModel;

using Ensage;
using Ensage.SDK.Renderer;

using SharpDX;

using BeAwarePlus.GlobalList;
using BeAwarePlus.Menus;

namespace BeAwarePlus.Renderer
{
    internal class OnMiniMap
    {
        private MenuManager MenuManager { get; }

        private IRendererManager Render { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private int TextSize { get; }

        private int IconSize { get; }

        private Vector2 IconExtra { get; }

        private int TPTextSize { get; }

        private Vector2 TPTextExtra { get; }

        public OnMiniMap(
            MenuManager menumanager,
            IRendererManager renderer, 
            GlobalMiniMap globalminimap)
        {
            MenuManager = menumanager;
            Render = renderer;
            GlobalMiniMap = globalminimap;

            if (MenuManager.OnMinimapItem.Value)
            {
                Render.Draw += OnDraw;
            }

            MenuManager.OnMinimapItem.PropertyChanged += OnMiniMapChanged;

            if (Drawing.RenderMode == RenderMode.Dx9)
            {
                TextSize = 14;
                IconExtra = new Vector2(12, 23);
                IconSize = 35;
                TPTextSize = 12;
                TPTextExtra = new Vector2(8, 10);
            }
            else if (Drawing.RenderMode == RenderMode.Dx11)
            {
                TextSize = 15;
                IconExtra = new Vector2(12, 19);
                IconSize = 21;
                TPTextSize = 14;
                TPTextExtra = new Vector2(9, 13);
            }
        }

        private void OnMiniMapChanged(object sender, PropertyChangedEventArgs e)
        {
            if (MenuManager.OnMinimapItem.Value)
            {
                Render.Draw += OnDraw;
            }
            else
            {
                Render.Draw -= OnDraw;
            }
        }

        public void Dispose()
        {
            MenuManager.OnMinimapItem.PropertyChanged -= OnMiniMapChanged;

            if (MenuManager.OnMinimapItem.Value)
            {
                Render.Draw -= OnDraw;
            }
        }

        private void DrawShadowText(
            IRendererManager Render, 
            Vector2 pos, 
            string text, 
            System.Drawing.Color color, 
            float size,
            string font)
        {
            Render.DrawText(
                pos + 2, 
                text, 
                System.Drawing.Color.Black, 
                size,
                font);

            Render.DrawText(
                pos, 
                text, 
                color,
                size,
                font);
        }

        private void OnDraw(object sender, EventArgs e)
        {
            foreach (var MiniMap in GlobalMiniMap.MiniMapList.ToList())
            {
                var pos = MiniMap.GetMinimapPos;
                var name = MiniMap.GetHeroName;

                var ExrtaPos = (int)(name.Length * 3.30);

                if (MiniMap.GetEnd)
                {
                    DrawShadowText(
                        Render,
                        new Vector2(pos.X - ExrtaPos, pos.Y - 31),
                        name,
                        MiniMap.GetHeroColor,
                        TextSize,
                        "Arial Black");
                }

                DrawShadowText(
                    Render,
                    pos - IconExtra,
                    "⚪",
                    MiniMap.GetHeroColor,
                    IconSize,
                    "Arial Black"); //❄//★//✫//❋//❉//✸//⍟//⊛/◊//•//⊗//°//✪

                if (MiniMap.GetTeleport)
                {
                    DrawShadowText(
                    Render,
                    pos - TPTextExtra,
                    "TP",
                    MiniMap.GetHeroColor,
                    TPTextSize,
                    "Arial Black");
                }
            }
        }
    }
}
