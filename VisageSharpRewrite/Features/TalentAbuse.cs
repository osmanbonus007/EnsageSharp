using Ensage;
using Ensage.Common;
using System;
using System.Linq;

namespace VisageSharpRewrite.Features
{
    public class TalentAbuse
    {
        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        public TalentAbuse()
        {

        }

        public void Execute()
        {
            var talent10_l = me.Spellbook.Spells.First(x => x.Name == "special_bonus_gold_income_50");
            var talent10_r = me.Spellbook.Spells.First(x => x.Name == "special_bonus_exp_boost_30");

            var talent15_l = me.Spellbook.Spells.First(x => x.Name == "special_bonus_attack_damage_50");
            var talent15_r = me.Spellbook.Spells.First(x => x.Name == "special_bonus_cast_range_100");

            var talent20_l = me.Spellbook.Spells.First(x => x.Name == "special_bonus_hp_300");
            var talent20_r = me.Spellbook.Spells.First(x => x.Name == "special_bonus_respawn_reduction_40");

            var talent25_l = me.Spellbook.Spells.First(x => x.Name == "special_bonus_spell_amplify_20");
            var talent25_r = me.Spellbook.Spells.First(x => x.Name == "special_bonus_unique_visage_2");


            if(me.AbilityPoints > 0)
            {
                if (Utils.SleepCheck("lvlup"))
                {
                    //foreach (var s in me.Spellbook.Spells)
                    //{ 
                    //    Console.WriteLine("spells " + s.Name);
                    //}
                    Console.WriteLine("spells " + talent10_l.Name);
                    Console.WriteLine("spells " + me.Spellbook.Spells.Where(x => x.Name.Contains("special_bonus")).FirstOrDefault().Name);
                    Player.UpgradeAbility(me, me.Spellbook.Spells.Where(x => x.Name.Contains("special_bonus")).FirstOrDefault());
                    Utils.Sleep(500, "lvlup");
                }
            }
        }
    }
}
