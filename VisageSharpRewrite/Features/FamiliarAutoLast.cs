using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using VisageSharpRewrite.Abilities;

namespace VisageSharpRewrite.Features
{
    public class FamiliarAutoLast
    {

        public Unit _LowestHpCreep { get; set; }

        public Unit _creepTarget { get; set; }

        public int autoAttackMode { get; set; }

        private bool looked;

        private FamiliarControl familiarControl
        {
            get
            {
                return Variables.familiarControl;
            }
        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        public FamiliarAutoLast()
        {
            //this.familiarControl = Variables.familiarControl;
            this.autoAttackMode = 2;
        }

        public void Update()
        {
            //this.familiarControl = Variables.familiarControl;
        }

        public void Execute(List<Unit> familiars)
        {
            //Update();

            this.setAutoAttackMode();
            if (familiars == null) return;

            //Auto Stone if under low attack dmg or health being low  
            foreach (var f in familiars)
            {
                if (Variables.familiarControl.FamiliarHasToStone(f))
                {
                    if (Utils.SleepCheck("stone"))
                    {

                        Variables.familiarControl.UseStone(f);
                        Utils.Sleep(100, "stone");
                    }
                }

            }

            if (familiarControl.AnyEnemyNearFamiliar(familiars, 600))
            {
                familiarControl.RetreatToTowerOrFountain(familiars);
                var AnyEnemyNearMe = ObjectManager.GetEntities<Hero>().Any(x => x.IsAlive && x.Team != Variables.Hero.Team
                                                        && x.Distance2D(Variables.Hero) <= 1000);
                if (!Variables.ComboOn && !AnyEnemyNearMe)
                {
                    Game.ExecuteCommand("dota_camera_set_lookatpos " + familiars.FirstOrDefault().Position.X + " " + familiars.FirstOrDefault().Position.Y);
                    looked = true;
                }

                if (looked)
                {
                    DelayAction.Add(1500, () =>
                    {
                        Game.ExecuteCommand("dota_camera_set_lookatpos " + Variables.Hero.Position.X + " " + Variables.Hero.Position.Y);
                    });
                    looked = false;
                }
                //disable auto last hit
                Variables.MenuManager.AutoFamiliarLastHitMenu.SetValue(new KeyBind(Variables.MenuManager.AutoFamiliarLastHitMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                return;
            }


            //Console.WriteLine("familiar pos " + familiar.FirstOrDefault().Position);
            // If there is enemy nearby
            var AnyoneAttackingMe = ObjectManager.TrackingProjectiles.Any(x => x.Target.Name.Equals("npc_dota_visage_familiar1") || x.Target.Name.Equals("npc_dota_visage_familiar2") || x.Target.Name.Equals("npc_dota_visage_familiar3"));
            //Console.WriteLine("anyone attacking me " + AnyoneAttackingMe);
            //if no ally creeps nearby, go follow the nearst ally creeps
            var closestAllyCreep = ObjectManager.GetEntities<Unit>().Where(_x =>
                                                                          _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                                                          && _x.IsAlive && _x.Team == me.Team && _x.IsMoving
                                                                          && (_x.Name.Equals("npc_dota_creep_badguys_melee")) || _x.Name.Equals("npc_dota_creep_goodguys_melee")).
                                                                          OrderBy(x => familiars.Sum(y => x.Distance2D(y)))
                                                                          .FirstOrDefault();
            if (Utils.SleepCheck("move"))
            {
                if (!familiarControl.AnyAllyCreepsAroundFamiliar(familiars))
                {
                    //Console.WriteLine("cloest ally creeps " + closestAllyCreep.Name);
                    if (closestAllyCreep == null)
                    {
                        foreach (var f in familiars)
                        {
                            //if (Utils.SleepCheck("move"))
                            //{
                                f.Follow(me);
                                //Console.WriteLine("f position " + f.Position);
                                //Console.WriteLine("cloest Ally creep is " + closestAllyCreep.Position);
                                //Utils.Sleep(100, "move");
                            //}
                        }
                    }
                    else {
                        //Console.WriteLine("familiars have to move");
                        foreach (var f in familiars)
                        {
                            //if (Utils.SleepCheck("move"))
                            //{
                                f.Follow(closestAllyCreep);
                                //Console.WriteLine("f position " + f.Position);
                                //Console.WriteLine("cloest Ally creep is " + closestAllyCreep.Position);
                            //    Utils.Sleep(100, "move");
                            //}
                        }
                    }
                    Utils.Sleep(100, "move");
                    return;
                }
            }


            //else
            //{
            if (AnyoneAttackingMe)
            {
                //go the the cloestallycreeps
                //var closestAllyCreep = ObjectManager.GetEntities<Unit>().Where(_x =>
                //                                                     _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                //                                                     && _x.IsAlive && _x.Team == me.Team
                //                                                     && _x.Name.Equals("npc_dota_creep_goodguys_ranged"))
                //                                                     .
                //                                                     OrderBy(x => x.Distance2D(familiars.FirstOrDefault())
                //                                                    ).FirstOrDefault();
                //Console.WriteLine("cloest allu creeps " + closestAllyCreep.Name);
                if (closestAllyCreep == null) return;
                if (Utils.SleepCheck("move"))
                {
                    foreach (var f in familiars)
                    {
                        f.Follow(me);
                    }
                    Utils.Sleep(1000, "move");
                }
                return;
            }
            //else
            //{
            if (familiarControl.AnyAllyCreepsAroundFamiliar(familiars))
            {
                // has enemy creeps around
                //AutoLastHit Mode
                getLowestHpCreep(familiars.FirstOrDefault(), 1000);
                if (this._LowestHpCreep == null) return;
                getKillableCreep(familiars.FirstOrDefault(), this._LowestHpCreep);
                if (this._creepTarget == null) return;
                if (this._creepTarget.IsValid && this._creepTarget.IsVisible && this._creepTarget.IsAlive)
                {
                    var damageThreshold = GetDmanageOnTargetFromSource(familiars.FirstOrDefault(), _creepTarget, 0);
                    var numOfMeleeOnKillable = NumOfMeleeCreepsAttackingMe(_creepTarget);
                    var numOfRangedOnKillable = NumOfRangedCreepsAttackingMe(_creepTarget);
                    if (numOfMeleeOnKillable + numOfRangedOnKillable != 0)
                    {
                        var AttackableFamiliar = familiars.Where(x => x.CanAttack()

                        && x.Modifiers.Any(y => y.Name == "modifier_visage_summon_familiars_damage_charge")
                                                                 && x.IsAlive
                                                                 );
                        var AttackableFamilarInRange = familiars.Where(x => x.CanAttack()
                                                                 && x.Modifiers.Any(y => y.Name == "modifier_visage_summon_familiars_damage_charge")
                                                                 && x.Distance2D(_creepTarget) <= x.AttackRange
                                                                 );

                        if (AttackableFamiliar == null) return;
                        if (AttackableFamiliar.All<Unit>(f => _creepTarget.Distance2D(f) <= f.AttackRange && f.CanAttack()))
                        {
                            var familiarDmg = AttackableFamilarInRange.Sum(f => GetDmanageOnTargetFromSource(f, _creepTarget, 0));
                            //Console.WriteLine("familiar dmg is " + familiarDmg);
                            if (_creepTarget.Health < familiarDmg)
                            {
                                foreach (var f in AttackableFamiliar)
                                {
                                    if (!f.IsAttacking())
                                    {
                                        if (Utils.SleepCheck("familiarAttack"))
                                        {
                                            f.Attack(_creepTarget);
                                            Utils.Sleep(200, "familiarAttack");
                                        }
                                    }
                                }
                            }
                            else if (_creepTarget.Health < familiarDmg * 2 && _creepTarget.Health > familiarDmg)
                            //attack-hold
                            {
                                if (Utils.SleepCheck("familiarAttack"))
                                {
                                    foreach (var f in AttackableFamiliar)
                                    {
                                        f.Hold();
                                        f.Attack(_creepTarget);
                                    }
                                    Utils.Sleep(100, "familiarAttack");
                                }
                            }
                            else
                            {
                                if (AttackableFamiliar.Any<Unit>(x => x.Distance2D(_creepTarget) > x.AttackRange) && _creepTarget.ClassId != ClassId.CDOTA_BaseNPC_Creep_Siege)
                                {
                                    if (Utils.SleepCheck("familiarmove"))
                                    {
                                        foreach (var f in AttackableFamiliar)
                                        {
                                            f.Move(this._LowestHpCreep.Position);
                                        }
                                        Utils.Sleep(100, "familiarmove");
                                    }

                                }
                            }
                        }
                        else // not in range
                        {
                            if (Utils.SleepCheck("move") && _creepTarget.ClassId != ClassId.CDOTA_BaseNPC_Creep_Siege)
                            {
                                foreach (var f in familiars)
                                {
                                    f.Move(_creepTarget.Position);
                                }
                                Utils.Sleep(200, "move");
                            }
                        }

                    }
                    else
                    {
                        var AttackableFamilarInRange = familiars.Where(x => x.CanAttack()
                                                                && x.Modifiers.Any(y => y.Name == "modifier_visage_summon_familiars_damage_charge")
                                                                && x.Distance2D(_creepTarget) <= x.AttackRange
                                                                );
                        var familiarDmg = AttackableFamilarInRange.Sum(f => GetDmanageOnTargetFromSource(f, _creepTarget, 0));
                        if (Utils.SleepCheck("attack") && _creepTarget.Health < familiarDmg * 1.5)
                        {
                            foreach (var f in familiars)
                            {
                                if (_creepTarget.ClassId != ClassId.CDOTA_BaseNPC_Creep_Siege)
                                {
                                    f.Attack(_creepTarget);

                                }
                                Utils.Sleep(200, "attack");
                            }
                        }
                    }
                }
            }
            //}



        }

        private void getLowestHpCreep(Unit source, int range)
        {
            if (source == null)
            {
                this._LowestHpCreep = null;
                return;
            }
            var lowestHp =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassId == ClassId.CDOTA_BaseNPC_Tower ||
                                 x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Creep
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Additive
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Barracks
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Building
                                 || x.ClassId == ClassId.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                                && x.Team != source.Team && x.Distance2D(source) < range)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
            //Console.WriteLine("lowestHp creep is " + lowestHp.Name);
            this._LowestHpCreep = lowestHp;
        }

        private void getKillableCreep(Unit src, Unit creep)
        {
            var percent = creep.Health / creep.MaximumHealth * 100;
            var dmgThreshold = GetDmanageOnTargetFromSource(src, creep, 0);
            //Console.WriteLine("dmg threshold is " + dmgThreshold);
            if (creep.Health < dmgThreshold * 10 &&
                (percent < 75 || creep.Health < dmgThreshold)
                )
            {
                this._creepTarget = creep;
            }
            else
            {
                this._creepTarget = null;
            }
        }

        private double GetDmanageOnTargetFromSource(Unit src, Unit target, double bonusdmg)
        {
            double realDamage = 0;
            double physDamage = src.MinimumDamage + src.BonusDamage;
            if (src == null)
            {
                return realDamage;
            }

            if (target.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
               target.ClassId == ClassId.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }

            var damageMp = 1 - 0.06 * target.Armor / (1 + 0.06 * Math.Abs(target.Armor));
            realDamage = (bonusdmg + physDamage) * damageMp;
            return realDamage;
        }

        private int NumOfMeleeCreepsAttackingMe(Unit me)
        {
            int num = 0;
            //melee creeps name = npc_dota_creep_badguys_melee
            //ranged creps name = npc_dota_creep_badguys_ranged
            try
            {
                var allMeleeCreepsAttackingMe = ObjectManager.GetEntities<Unit>().Where(_x =>
                                                                                    (_x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                                                                    && me.Distance2D(_x) <= 150
                                                                                    && _x.Team != me.Team
                                                                                    && _x.IsAlive
                                                                                    && _x.GetTurnTime(me) == 0)
                                                                                    && (_x.Name.Equals("npc_dota_creep_badguys_melee")
                                                                                    || (_x.Name.Equals("npc_dota_creep_goodguys_melee"))));
                if (allMeleeCreepsAttackingMe == null) return num;
                num = allMeleeCreepsAttackingMe.Count();
            }
            catch
            {

            }
            return num;
        }

        private int NumOfRangedCreepsAttackingMe(Unit me)
        {
            int num = 0;
            //melee creeps name = npc_dota_creep_badguys_melee
            //ranged creeps name = npc_dota_creep_badguys_ranged
            try
            {
                var allRangedCreepsAttackingMe = ObjectManager.GetEntities<Unit>().Where(_x =>
                                                                                    (_x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                                                                    && me.Distance2D(_x) <= 650
                                                                                    && _x.Team != me.Team
                                                                                    && _x.IsAlive
                                                                                    && _x.GetTurnTime(me) == 0)
                                                                                    && (_x.Name.Equals("npc_dota_creep_badguys_ranged")
                                                                                        || _x.Name.Equals("npc_dota_creep_goodguys_ranged")));
                if (allRangedCreepsAttackingMe == null) return num;
                num = allRangedCreepsAttackingMe.Count();
            }
            catch
            {

            }
            return num;
        }

        public void setAutoAttackMode()
        {
            if (Variables.InAutoLasthiMode && this.autoAttackMode == 2)
            {
                this.autoAttackMode = 0;
                Game.ExecuteCommand("dota_player_units_auto_attack_mode " + this.autoAttackMode);
            }
        }

        public void PlayerExecution()
        {
            resetAutoAttackMode();
        }

        private void resetAutoAttackMode()
        {
            if (this.autoAttackMode == 0)
            {
                this.autoAttackMode = 2;
                Game.ExecuteCommand("dota_player_units_auto_attack_mode " + this.autoAttackMode);
            }
        }

        public void OnDraw()
        {

        }


    }
}
