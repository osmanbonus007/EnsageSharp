using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System.Collections.Generic;
using System.Linq;


namespace VisageSharpRewrite.Utilities
{
    public class HelmOfDominatorControl
    {

        public HelmOfDominatorControl()
        {

        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        public void Execute(Hero Target)
        {
            this.DominatedUnit = ObjectManager.GetEntities<Unit>().Where(x => x.IsControllableByPlayer(ObjectManager.LocalPlayer)).ToList();
            
            if (this.DominatedUnit == null) return;
            UnitsOrbwalk(Target);
            if (Utils.SleepCheck("unitcast"))
            {
                foreach (var unit in this.DominatedUnit)
                {
                    SpellCast(unit, Target);
                }
                Utils.Sleep(200, "unitcast");
            }
            
        }


        private List<Unit> DominatedUnit;

        private Dictionary<float, Orbwalker> orbwalkerDictionary = new Dictionary<float, Orbwalker>();

        private void UnitsOrbwalk(Hero Target)
        {
            if (this.DominatedUnit == null) return;
            if (Target == null) return;
            if (this.DominatedUnit.All(x => !x.CanMove())) return;
            Orbwalker orbwalker;
            foreach(var unit in this.DominatedUnit)
            {
                if(!orbwalkerDictionary.TryGetValue(unit.Handle, out orbwalker))
                {
                    orbwalker = new Orbwalker(unit);
                    orbwalkerDictionary.Add(unit.Handle, orbwalker);
                }
            }
        }

        private void SpellCast(Unit unit, Hero Target)
        {
            if (unit.Name.Equals("npc_dota_neutral_centaur_khan"))
            {
                if(unit.Distance2D(Target) <= 150 && unit.Spellbook.Spell1.CanBeCasted() && !Target.IsRooted() && !Target.IsStunned() && unit.Mana >= unit.Spellbook.Spell1.ManaCost)
                {
                    unit.Spellbook.Spell1.UseAbility();
                    return;
                }
            }
            if (unit.Name.Contains("neutral_polar"))
            {
                if (unit.Distance2D(Target) <= 200 && unit.Spellbook.Spell1.CanBeCasted() && unit.Mana >= unit.Spellbook.Spell1.ManaCost)
                {
                    unit.Spellbook.Spell1.UseAbility();
                    return;
                }
            }
            if (unit.Name.Contains("neutral_satyr"))
            {
                if (unit.Distance2D(Target) <= unit.Spellbook.SpellQ.CastRange && unit.Spellbook.SpellQ.CanBeCasted() && unit.Mana >= unit.Spellbook.Spell1.ManaCost)
                {
                    unit.Spellbook.SpellQ.UseAbility(Target);
                    return;
                }
            }
            if (unit.Name.Contains("neutral_mud_golem"))
            {
                if (unit.Distance2D(Target) <= unit.Spellbook.Spell1.CastRange && unit.Spellbook.Spell1.CanBeCasted() && unit.Mana >= unit.Spellbook.Spell1.ManaCost)
                {
                    unit.Spellbook.Spell1.UseAbility(Target);
                    return;
                }
            }
            
            if (unit.Name.Contains("dark_troll_warlord"))
            {
                if (unit.Distance2D(Target) <= unit.Spellbook.Spell1.CastRange && unit.Spellbook.Spell1.CanBeCasted() && unit.Mana >= unit.Spellbook.Spell1.ManaCost)
                {
                    unit.Spellbook.Spell1.UseAbility(Target);
                    return;
                }
            }
        }
    }
}
