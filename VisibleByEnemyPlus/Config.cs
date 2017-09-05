using System;

using SharpDX;

using Ensage.SDK.Menu;
using Ensage.Common.Menu;

namespace VisibleByEnemyPlus
{
    internal class Config : IDisposable
    {
        public MenuFactory Factory { get; }

        public MenuItem<bool> AlliedHeroesItem { get; }

        public MenuItem<bool> WardsItem { get; }

        public MenuItem<bool> MinesItem { get; }

        public MenuItem<bool> ShrinesItem { get; }

        public MenuItem<bool> ShrinesDrawItem { get; }

        public MenuItem<bool> NeutralsItem { get; }

        public MenuItem<bool> UnitsItem { get; }

        public MenuItem<bool> BuildingsItem { get; }

        public MenuItem<StringList> EffectTypeItem { get; }

        public MenuItem<Slider> RedItem { get; set; }

        public MenuItem<Slider> GreenItem { get; }

        public MenuItem<Slider> BlueItem { get; }

        public MenuItem<Slider> AlphaItem { get; }

        private bool Disposed { get; set; }

        public Config()
        {
            Factory = MenuFactory.CreateWithTexture("VisibleByEnemyPlus", "visiblebyenemyplus");
            Factory.Target.SetFontColor(Color.Aqua);

            EffectTypeItem = Factory.Item("Effect Type", new StringList(EffectsName) { SelectedIndex = 0 });

            RedItem = Factory.Item("Red", new Slider(255, 0, 255));
            GreenItem = Factory.Item("Green", new Slider(255, 0, 255));
            BlueItem = Factory.Item("Blue", new Slider(255, 0, 255));
            AlphaItem = Factory.Item("Alpha", new Slider(255, 0, 255));

            if (EffectTypeItem.Value.SelectedIndex == 0)
            {
                RedItem.Item.SetFontColor(Color.Black);
                GreenItem.Item.SetFontColor(Color.Black);
                BlueItem.Item.SetFontColor(Color.Black);
                AlphaItem.Item.SetFontColor(Color.Black);
            }
            else
            {
                RedItem.Item.SetFontColor(new Color(RedItem, 0, 0, 255));
                GreenItem.Item.SetFontColor(new Color(0, GreenItem, 0, 255));
                BlueItem.Item.SetFontColor(new Color(0, 0, BlueItem, 255));
                AlphaItem.Item.SetFontColor(new Color(185, 176, 163, AlphaItem));
            }

            AlliedHeroesItem = Factory.Item("Allied Heroes", true);
            WardsItem = Factory.Item("Wards", true);
            MinesItem = Factory.Item("Mines", true);
            ShrinesItem = Factory.Item("Shrines", true);
            ShrinesDrawItem = Factory.Item("Shrines Draw On Minimap", true);
            NeutralsItem = Factory.Item("Neutrals", true);
            UnitsItem = Factory.Item("Units", true);
            BuildingsItem = Factory.Item("Buildings", true);
        }

        private string[] EffectsName { get; } =
        {
            "Default",
            "Default MOD",
            "VBE",
            "Omniknight",
            "Assault",
            "Arrow",
            "Mark",
            "Glyph",
            "Coin",
            "Lightning",
            "Energy Orb",
            "Pentagon",
            "Axis",
            "Beam Jagged",
            "Beam Rainbow",
            "Walnut Statue",
            "Thin Thick",
            "Ring Wave",
            "Visible"
        };

        public string[] Effects { get; } =
        {
            "particles/items_fx/aura_shivas.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy.vpcf",
            "materials/ensage_ui/particles/vbe.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_omniknight.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_assault.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_arrow.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_mark.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_glyph.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_coin.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_lightning.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_energy_orb.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_pentagon.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_axis.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_beam_jagged.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_beam_rainbow.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_walnut_statue.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_thin_thick.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_ring_wave.vpcf",
            "materials/ensage_ui/particles/visiblebyenemy_visible.vpcf"
        };

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