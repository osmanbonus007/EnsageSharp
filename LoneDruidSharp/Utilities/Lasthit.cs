using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System;
using System.Linq;

namespace LoneDruidSharpRewrite.Utilities
{
    class Lasthit
    {
        public Unit Bear { get; set; }

        public Unit Me { get; set; }

        public Move myMove;

        public Move bearMove;

        public Sleeper bearAttackSleeper;

        public Sleeper myAttackSleeper;

        public Unit _LowestHpCreep { get; set; }

        public Unit _creepTarget { get; set; }

        public int autoAttackMode { get; set; }

        public bool onlyBear;

        public Lasthit()
        {
            
            this.autoAttackMode = 2;
            
            //this.onlyBear = Variable.OnlyBearLastHitActive;
            
            
            this.myAttackSleeper = new Sleeper();
            this.bearAttackSleeper = new Sleeper();
        }

        public void Update()
        {
            this.Bear = Variable.Bear;
            this.Me = Variable.Hero;
            this.onlyBear = Variable.OnlyBearLastHitActive;
            this.myMove = new Move(Me);
            this.bearMove = new Move(Bear);

        }

        public void getLowestHpCreep(Unit source, int range)
        {
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

        public void getKillableCreep(Unit creep, bool onlyBear)
        {
            var percent = creep.Health / creep.MaximumHealth * 100;
            var dmgThreshold = onlyBear ? GetDamangeOnUnit(Bear, creep, 0) : GetDamangeOnUnit(Bear, creep, 0) + GetDamangeOnUnit(Me, creep, 0);
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

        private static double GetDmanageOnTargetFromSource(Unit src, Unit target, double bonusdmg)
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

        public double GetDamangeOnUnit(Unit src, Unit unit, double bonusdamage)
        {
            var hasQuellingBlade = src.Inventory.Items.ToList().Any(x => x.Name == "item_iron_talon" || x.Name == "item_quelling_blade");
            double modif = 1;
            double magicdamage = 0;
            double physDamage = src.MinimumDamage + src.BonusDamage;
            if (hasQuellingBlade && unit.Team != Me.Team)
            {
                if (src.IsRanged)
                {
                    physDamage = src.MinimumDamage * 1.15 + src.BonusDamage;
                }
                else
                {
                    physDamage = src.MinimumDamage * 1.4 + src.BonusDamage;
                }
            }
            var damageMp = 1 - 0.06 * unit.Armor / (1 + 0.06 * Math.Abs(unit.Armor));
            magicdamage = magicdamage * (1 - unit.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage) * damageMp + magicdamage) * modif;
            if (unit.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                unit.ClassId == ClassId.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }
            return realDamage;
        }

        public void OnlyBearLastHitExecute()
        {
            this.Update();
            this.setAutoAttackMode();
            if (!Variable.OnlyBearLastHitActive)
            {
                return;
            }
            if (!this.anyCreepsAround(this.Bear))
            {
                return;
            }
            //Hero Control
            
            if(this.Bear.Distance2D(this.Me) > 1100)
            {
                if (Utils.SleepCheck("Move"))
                {
                    this.Me.Move(this.Bear.Position);
                    Utils.Sleep(100, "Move");
                }
            }
            else
            {
                if (Utils.SleepCheck("Hold"))
                {
                    this.Me.Hold();
                    Utils.Sleep(100, "Hold");
                }
            }           
            //Bear Control       
            this.getLowestHpCreep(Bear, 1000);
            if (this._LowestHpCreep == null) return;
            this.getKillableCreep(this._LowestHpCreep, true);
            if (this._creepTarget == null) return;


            if (this._creepTarget.IsValid && this._creepTarget.IsVisible && this._creepTarget.IsAlive)
            {
                var damageThreshold = GetDamangeOnUnit(this.Bear, _creepTarget, 0);
                var numOfMeleeOnKillable = MeleeCreepsAttackKillableTarget(_creepTarget);
                var numOfRangedOnKillable = RangedCreepsAttackKillableTarget(_creepTarget);
                if (numOfMeleeOnKillable + numOfRangedOnKillable != 0)
                {
                    if (_creepTarget.Distance2D(Bear) <= Bear.AttackRange)
                    {   //bear can attack
                        if (_creepTarget.Health < damageThreshold)
                        {   // if attack below killable dmg, instant attack it
                            if (Bear.CanAttack())
                            {
                                Bear.Attack(_creepTarget);
                            }
                        }else if(_creepTarget.Health < damageThreshold * 2.5 && _creepTarget.Health > damageThreshold && Utils.SleepCheck("A-Stop"))
                        {  // Attack-Hold
                            Bear.Attack(_creepTarget);
                            Bear.Hold();
                            Utils.Sleep(100, "A-Stop");             
                        }
                    }
                    else
                    {
                        if (Utils.SleepCheck("Follow"))
                        {
                            Bear.Follow(_LowestHpCreep);
                            Utils.Sleep(100, "Follow");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("no one is attacking");
                    //no one is hitting that creeps, go ahead and kill it
                    if (Bear.CanAttack() && _creepTarget.Health <= 1.5 * damageThreshold)
                    {
                        Bear.Attack(_creepTarget);
                        this.bearAttackSleeper.Sleep(100);
                    }
                }
            }
        }

        public void CombinedLastHitExecute()
        {
            this.Update();
            this.setAutoAttackMode();
            if (!Variable.CombinedLastHitActive)
            {
                return;
            }
            if (!this.anyCreepsAround(this.Me))
            {
                return;
            }
            double meleedmg;
            double rangeddmg;
            int rangedCreepProjectilesInAir;

            this.getLowestHpCreep(Bear, 1000);
            if (this._LowestHpCreep == null) return;
            this.getKillableCreep(this._LowestHpCreep, false);
            if (this._creepTarget == null) return;
            var MeleeCreep = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                                                                        && _x.Distance2D(Me) <= 1000
                                                                                        && _x.Name.Equals("npc_dota_creep_badguys_melee")).FirstOrDefault();
            if (MeleeCreep == null)
            {
                meleedmg = 0;
            }
            else
            {
                meleedmg = GetDmanageOnTargetFromSource(MeleeCreep, _creepTarget, 0);
            }

            bool anyOneHitBear = ObjectManager.TrackingProjectiles.Any(x => x.Target.Name.Equals(Bear.Name));
            //Console.WriteLine("someone hitting bear" + anyOneHitBear);
            var RangedCreep = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                                                               && _x.Name.Equals("npc_dota_creep_badguys_ranged")).FirstOrDefault();
            if (RangedCreep == null)
            {
                rangeddmg = 0;
            }
            else
            {
                rangeddmg = GetDmanageOnTargetFromSource(RangedCreep, _creepTarget, 0);
            }
            var RangedCreepProjectilesToKillableCreeps = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name.Equals("npc_dota_creep_badguys_ranged") && x.Target.Name == _creepTarget.Name);
            if (RangedCreepProjectilesToKillableCreeps == null)
            {
                rangedCreepProjectilesInAir = 0;
            }
            else
            {
                rangedCreepProjectilesInAir = RangedCreepProjectilesToKillableCreeps.Count();
            }
            if (this._creepTarget.IsValid && this._creepTarget.IsVisible && this._creepTarget.IsAlive)
            {
                var getDamageFromBear = GetDamangeOnUnit(Bear, _creepTarget, 0);
                var getDamageFromMe = GetDamangeOnUnit(this.Me, _creepTarget, 0);
                //Console.WriteLine("bear Dmg " + getDamageFromBear);
                //Console.WriteLine("my dmg " + getDamageFromMe);
                var numOfMeleeOnKillable = MeleeCreepsAttackKillableTarget(_creepTarget);
                var numOfRangedOnKillable = RangedCreepsAttackKillableTarget(_creepTarget);
                if (numOfMeleeOnKillable + numOfRangedOnKillable != 0)
                {                  
                    if ((_creepTarget.Health < (Bear.Distance2D(_creepTarget) < 150 ? getDamageFromBear : 0) + getDamageFromMe + (numOfMeleeOnKillable * 10) + numOfRangedOnKillable * rangeddmg && rangedCreepProjectilesInAir >= numOfRangedOnKillable - 1 && Me.Distance2D(_creepTarget) >= 400)
                        || (_creepTarget.Health < (Bear.Distance2D(_creepTarget) < 150 ? getDamageFromBear : 0) + getDamageFromMe + (numOfMeleeOnKillable >= 2 ? 20 : 0) && Me.Distance2D(_creepTarget) > 200 && Me.Distance2D(_creepTarget) <= 400)
                        || (_creepTarget.Health < (Bear.Distance2D(_creepTarget) < 150 ? getDamageFromBear : 0) + getDamageFromMe - 5)
                        )
                    {
                        if (!Me.IsAttacking())
                        {
                            Me.Attack(_creepTarget);
                        }
                        if (Bear != null)
                        {
                            if (!Bear.IsAttacking())
                            {
                                Bear.Attack(_creepTarget);
                            }
                        }
                    }
                    else if (_creepTarget.Health < 2 * (getDamageFromBear + getDamageFromMe) && _creepTarget.Health > getDamageFromBear + getDamageFromMe)
                    {
                        if (Utils.SleepCheck("A-stop"))
                        {
                            Me.Attack(_creepTarget);
                            Me.Hold();
                            if (Bear != null)
                            {
                                Bear.Hold();
                                Bear.Attack(_creepTarget);
                            }
                            Utils.Sleep(50, "A-stop");
                        }
                    }
                    else
                    {
                        if (Bear != null)
                        {
                            if (Utils.SleepCheck("Follow"))
                            {
                                Bear.Move(this._LowestHpCreep.Position);
                                Utils.Sleep(100, "Follow");
                            }
                        }
                    }
                }
                else
                {
                }
            }


        }

        public void setAutoAttackMode()
        {
            if ((Variable.OnlyBearLastHitActive || Variable.CombinedLastHitActive) && this.autoAttackMode == 2)
            {
                this.autoAttackMode = 0;
                Game.ExecuteCommand("dota_player_units_auto_attack_mode " + this.autoAttackMode);
            }
        }

        public void resetAutoAttackMode()
        {
            if (this.autoAttackMode == 0)
            {
                this.autoAttackMode = 2;
                Game.ExecuteCommand("dota_player_units_auto_attack_mode " + this.autoAttackMode);
            }
        }

        public bool anyCreepsAround(Unit src)
        {
            var creepsaroundme =  ObjectManager.GetEntities<Unit>().Any(_x => ((_x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                               _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege)
                                                              && src.Distance2D(_x) <= 1000
                                                              && _x.Team != src.Team
                                                              )); //enemy creeps around me
            //Console.WriteLine("any?" + creepsaroundme);
            return creepsaroundme;
        }

        public int MeleeCreepsAttackKillableTarget(Unit creep)
        {
            //melee creeps name = npc_dota_creep_badguys_melee
            //ranged creps name = npc_dota_creep_badguys_ranged
            var allMeleeCreepsAttackingMe = ObjectManager.GetEntities<Unit>().Where(_x =>
                                                                                    ((_x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                                                    _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege)
                                                                                    && creep.Distance2D(_x) <= 150
                                                                                    && _x.Team != creep.Team
                                                                                    && _x.IsAlive
                                                                                    && _x.GetTurnTime(creep) == 0)
                                                                                    && _x.Name.Equals("npc_dota_creep_badguys_melee"));
            if (allMeleeCreepsAttackingMe == null) return 0;


            return allMeleeCreepsAttackingMe.Count(); ;
        }

        public int RangedCreepsAttackKillableTarget(Unit creep)
        {
            //melee creeps name = npc_dota_creep_badguys_melee
            //ranged creeps name = npc_dota_creep_badguys_ranged
            var allRangedCreepsAttackingMe = ObjectManager.GetEntities<Unit>().Where(_x =>
                                                                                   ((_x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                                                   _x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege)
                                                                                   && creep.Distance2D(_x) <= 650
                                                                                   && _x.Team != creep.Team
                                                                                   && _x.IsAlive
                                                                                   && _x.GetTurnTime(creep) == 0)
                                                                                   && _x.Name.Equals("npc_dota_creep_badguys_ranged"));
            if (allRangedCreepsAttackingMe == null) return 0;

            return allRangedCreepsAttackingMe.Count();
        }
    }
}
