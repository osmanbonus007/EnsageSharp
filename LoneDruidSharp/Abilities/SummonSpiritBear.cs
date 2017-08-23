using Ensage;
using Ensage.Common;
using SharpDX;

namespace LoneDruidSharpRewrite.Abilities
{
    public class SummonSpiritBear
    {
        private readonly Ability ability;

        private readonly DotaTexture abilityIcon;

        private bool attacked;

        private Vector2 iconSize;

        public SummonSpiritBear(Ability ability)
        {
            this.ability = ability;
            this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/lone_druid_spirit_bear");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }



    }
}
