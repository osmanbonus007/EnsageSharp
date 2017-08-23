using System;
using System.Linq;
using System.ComponentModel;

using Ensage;

using SharpDX;

using BeAwarePlus.Data;
using BeAwarePlus.GlobalList;
using BeAwarePlus.Menus;

namespace BeAwarePlus.Renderer
{
    internal class OnWorld
    {
        private MenuManager MenuManager { get; }

        private GlobalWorld GlobalWorld { get; }

        private ParticleToTexture ParticleToTexture { get; }

        public OnWorld(
            MenuManager menumanager,
            GlobalWorld globalworld,
            ParticleToTexture particletotexture)
        {
            MenuManager = menumanager;
            GlobalWorld = globalworld;
            ParticleToTexture = particletotexture;

            if (MenuManager.OnWorldItem.Value)
            {
                Drawing.OnDraw += OnDraw;
            }

            MenuManager.OnWorldItem.PropertyChanged += OnWorldItemChanged;
        }

        private void OnWorldItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (MenuManager.OnWorldItem.Value)
            {
                Drawing.OnDraw += OnDraw;
            }
            else
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        public void Dispose()
        {
            MenuManager.OnWorldItem.PropertyChanged -= OnWorldItemChanged;

            if (MenuManager.OnWorldItem.Value)
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void OnDraw(EventArgs args)
        {
            foreach (var World in GlobalWorld.WorldList.ToList())
            {
                var pos = Drawing.WorldToScreen(World.GetWorldPos);

                if (pos.IsZero)
                {
                    continue;
                }

                Drawing.DrawRect(
                    new Vector2(pos.X + 18, pos.Y - 35),
                    new Vector2(64, 128),
                    Drawing.GetTexture("materials/ensage_ui/other/beawareplus_screen.vmat"));

                Drawing.DrawRect(
                    new Vector2(pos.X + 25, pos.Y - 20),
                    new Vector2(50, 50),
                    Drawing.GetTexture("materials/ensage_ui/heroes_round/" + World.GetHeroTexturName + ".vmat"));

                Drawing.DrawRect(
                    new Vector2(pos.X + 34, pos.Y + 40),
                    new Vector2(35, 35),
                    Drawing.GetTexture("materials/ensage_ui/modifier_textures/round/" + World.GetAbilityTexturName + ".vmat"));
            }
        }
    }
}
