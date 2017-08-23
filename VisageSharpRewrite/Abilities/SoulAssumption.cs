using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using System.Linq;

namespace VisageSharpRewrite.Abilities
{
    public class SoulAssumption
    {
        private readonly Ability ability;

        //private readonly DotaTexture abilityIcon;

        private uint level
        {
            get
            {
                return this.ability.Level;
            }
        }

        private Vector2 iconSize;

        public SoulAssumption(Ability ability)
        {
            this.ability = ability;
            //this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/storm_spirit_static_remnant");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public bool HasMaxCharges(Hero me)
        {
            var soulAssumption = me.Modifiers.Where(x => x.Name == "modifier_visage_soul_assumption").FirstOrDefault();
            if (soulAssumption == null) return false;
            return soulAssumption.StackCount == 2 + level;
        }

        public bool CanbeCastedOn(Hero target, bool hasLens)
        {
            if (target == null) return false;
            return Variables.Hero.Distance2D(target) <= this.ability.CastRange + (hasLens ? 200 : 0) + 100 
                    && !target.IsMagicImmune() && this.ability.CanBeCasted();
        }

        public bool isLearned()
        {
            return this.ability.Level > 0;
        }

        public double Damage(Hero target, bool hasLens)
        {
            double dmg = 0;
            if (target == null) return 0;
            if (this.ability == null) return 0;
            var soulAssumption = Variables.Hero.Modifiers.Where(x => x.Name == "modifier_visage_soul_assumption").FirstOrDefault();
            if (soulAssumption == null) return 10;
            var stackCount = soulAssumption.StackCount;
            var magicResist = target.MagicDamageResist;
            var magicDmg = 20 + stackCount * 110;
            var intAmp = Variables.Hero.Intelligence / 16 * 0.01;
            dmg =  magicDmg * (1 - magicResist) * (1 + intAmp + (hasLens ? 0.05 : 0));
            return dmg;
        }

        public void Use(Hero target)
        {
            if (Utils.SleepCheck("soul assumption"))
            {
                SwitchTread();
                this.ability.UseAbility(target);
                Utils.Sleep(100, "soul assumption");
            }
        }

        public void SwitchTread()
        {
            if (Variables.PowerTreadsSwitcher != null && Variables.PowerTreadsSwitcher.IsValid
                && Variables.Hero.Health > 300)
            {
                Variables.PowerTreadsSwitcher.SwitchTo(
                    Ensage.Attribute.Intelligence,
                    Variables.PowerTreadsSwitcher.PowerTreads.ActiveAttribute,
                    false);
            }
        }

    }
}
