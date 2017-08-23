using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using VisageSharpRewrite.Abilities;
using VisageSharpRewrite.Utilities;

namespace VisageSharpRewrite.Features
{
    public class Combo
    {

        private SoulAssumption soulAssumption
        {
            get
            {
                return Variables.soulAssumption;
            }
        }

        private GraveChill graveChill
        {
            get
            {
                return Variables.graveChill;
            }
        }

        private FamiliarControl familiarControl
        {
            get
            {
                return Variables.familiarControl;
            }
        }

        private ItemUsage itemUsage;

        private ParticleEffect meToTargetParticleEffect;

        private bool hasLens;

        private AutoNuke autoNuke;

        private Hero me;

        private Dictionary<float, Orbwalker> orbwalkerDictionary = new Dictionary<float, Orbwalker>();


        public Combo()
        {
            hasLens = false;
            this.autoNuke = new AutoNuke();
            this.itemUsage = new ItemUsage();
        }

        public void Update(Hero me)
        {
            this.hasLens = me.Inventory.Items.Any(x => x.Name == "item_aether_lens");
        }

        public void FamiliarOrbwalk(List<Unit> familiars, Hero Target)
        {
            try
            {
                if (familiars == null) return;
                if (Target == null) return;
                if (familiars.All(x => !x.CanMove())) return;
                Orbwalker orbwalker;
                foreach (var f in familiars)
                {
                    if (!orbwalkerDictionary.TryGetValue(f.Handle, out orbwalker))
                    {
                        orbwalker = new Orbwalker(f);
                        orbwalkerDictionary.Add(f.Handle, orbwalker);
                    }

                    orbwalker.OrbwalkOn(Target);
                }
            }
            catch
            {
                return;
            }

        }

        public void Execute(Hero me, Hero target, List<Unit> familiars)
        {

            if (target == null)
            {
                if (Utils.SleepCheck("MousePosition"))
                {
                    me.Move(Game.MousePosition);
                    Utils.Sleep(150, "MousePosition");
                }

            }
            if (!me.IsAlive) return;
            if (target == null) return;
            Update(me);
            itemUsage.OffensiveItem(target);
            //
            //all within attack range
            FamiliarOrbwalk(familiars, target);
            if (familiars.Any(f => f.Distance2D(target) <= f.AttackRange + 100))
            {
                if (Utils.SleepCheck("stone"))
                {
                    foreach (var f in familiars)
                    {
                        if (familiarControl.FamiliarCanStoneEnemies(target, f))
                        {
                            familiarControl.UseStone(f);
                        }
                        else
                        {                       
                            /*
                            if (familiarControl.NotMuchDmgLeft(f))
                            {
                                f.Move(f.Spellbook.SpellQ.GetPrediction(target));
                            }
                            else {
                                //familiarControl.FamiliarOrbwalk(f, target);
                                //f.Attack(target);
                            }
                            */
                        }
                    }
                    Utils.Sleep(300, "stone");
                }
                // familiar attack
            }
            else {
                //Console.WriteLine("now move to new target");
                /*
                if (Utils.SleepCheck("move"))
                {
                    foreach (var f in familiars)
                    {
                        if (f.Distance2D(target) > f.AttackRange + 100)
                        {
                            if (f.CanMove() && !f.IsAttacking())
                            {
                                f.Move(f.Spellbook.SpellQ.GetPrediction(target));
                            }
                        }
                    }
                    Utils.Sleep(300, "move");
                }
                */
            }

            
            //grave chill
            if (graveChill.CanBeCastedOn(target, hasLens))
            {
                if (Utils.SleepCheck("gravechill"))
                {
                    graveChill.UseOn(target);
                    Utils.Sleep(200, "gravechill");
                }
            }
            else
            {
                // go towards target
                if (me.CanMove())
                {

                    if (Utils.SleepCheck("move"))
                    {
                        //me.Move(target.Position);
                        Utils.Sleep(100, "move");
                    }
                }   
                             
                //Orbwalk
                if (Utils.SleepCheck("orbwalk"))
                {
                    try
                    {
                        if (Orbwalking.AttackOnCooldown()) //target != null is to avoid an null exception case in Orbwalk
                        {
                            Orbwalking.Orbwalk(target, 0, 0, false, true);
                        }
                        else
                        {
                            Orbwalking.Attack(target, true);
                        }
                        Utils.Sleep(200, "orbwalk");
                    }
                    catch
                    {
                        return;
                    }
                    
                }
                

                //soulAssumption
                autoNuke.KillSteal(me);
                // max dmg on target
                if (soulAssumption.HasMaxCharges(me) && soulAssumption.CanbeCastedOn(target, hasLens))
                {
                    if (Utils.SleepCheck("soulassumption"))
                    {
                        soulAssumption.Use(target);
                        Utils.Sleep(200, "soulassumption");
                    }
                }
            }
        }

        public void DrawTarget(Hero target)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 110, Convert.ToSingle(Drawing.Height * 0.7));
            var name = "materials/ensage_ui/heroes_horizontal/" + target.Name.Replace("npc_dota_hero_", "") + ".vmat";
            var size = new Vector2(50, 50);
            Drawing.DrawRect(startPos, size + new Vector2(13, -6),
                Drawing.GetTexture(name));
            Drawing.DrawRect(startPos, size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);
        }

        public void DrawParticleEffect(Hero target)
        {
            if (target == null) return;
            if (meToTargetParticleEffect == null)
            {
                meToTargetParticleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);     //target inditcator
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(Variables.Hero.Position.X, Variables.Hero.Position.Y, Variables.Hero.Position.Z));             //start point XYZ
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));                                                    // 1 means the particle is visible
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(target.Position.X, target.Position.Y, target.Position.Z)); //end point XYZ
            }
            else //updating positions
            {
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(Variables.Hero.Position.X, Variables.Hero.Position.Y, Variables.Hero.Position.Z));
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(target.Position.X, target.Position.Y, target.Position.Z));
            }
        }

        public void PlayerExecution(Hero target)
        {
            if((target == null || !target.IsAlive || !target.IsValid) && meToTargetParticleEffect != null)
            {
                meToTargetParticleEffect.Dispose();
                meToTargetParticleEffect = null;
            }
        }

        public void DisableParticleEffect()
        {
            if (meToTargetParticleEffect != null)
            {
                meToTargetParticleEffect.Dispose();
                meToTargetParticleEffect = null;
            }
        }


    }
}
