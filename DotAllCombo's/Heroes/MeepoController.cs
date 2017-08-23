using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using SharpDX;
	using SharpDX.Direct3D9;
	using System.Threading.Tasks;
	using Ensage.Common.Menu;
	using Service.Debug;

    internal class MeepoController : Variables, IHeroController
    {
		public Hero InitMeepo;

		public bool Activated, PoofKey, SafePoof, PoofAutoMode, SliderCountUnit, Dodge = true;

#pragma warning disable CS0649 // Field 'MeepoController.Travel' is never assigned to, and will always have its default value null
		public Item Blink, Travel, Shiva, Sheep, Medall, Dagon, Cheese, Ethereal, Vail, Atos, Orchid, Abyssal;
#pragma warning restore CS0649 // Field 'MeepoController.Travel' is never assigned to, and will always have its default value null
		public Font Txt;
		public Font Not;
#pragma warning disable CS0649 // Field 'MeepoController.meepos' is never assigned to, and will always have its default value null
		public List<Hero> Meepos;
#pragma warning restore CS0649 // Field 'MeepoController.meepos' is never assigned to, and will always have its default value null
		public readonly Menu Skills = new Menu("All Poof", "PoofMeepo");
		//public readonly Menu farm = new Menu("FarmMode", "FarmMode");
		

		public void Combo()
		{
			if (Me == null || Me.ClassId != ClassId.CDOTA_Unit_Hero_Meepo || !Game.IsInGame) return;
            if(!Me.IsAlive) return;

            Activated = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
            PoofKey = Game.IsKeyDown(Menu.Item("poofKey").GetValue<KeyBind>().Key);
            PoofAutoMode = Menu.Item("poofAutoMod").GetValue<KeyBind>().Active;
            SafePoof = Menu.Item("poofSafe").IsActive();
            Dodge = Menu.Item("Dodge").GetValue<KeyBind>().Active;
            var checkObj = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                        || x.HasInventory
                        || x.ClassId == ClassId.CDOTA_Unit_SpiritBear) && x.IsAlive && x.Team != Me.Team && x.IsValid).ToList();
            var meepos = ObjectManager.GetEntities<Hero>().Where(x => x.IsControllable && x.IsAlive && x.ClassId == ClassId.CDOTA_Unit_Hero_Meepo).ToList();





            var fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain).ToList();
            //blink = Me.FindItem("item_blink");



            E = ObjectManager.GetEntities<Hero>()
                        .Where(x => x.IsAlive && x.Team != Me.Team && !x.IsIllusion)
                        .OrderBy(x =>
                {
                    var firstOrDefault = meepos.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault();
                    if (firstOrDefault != null)
                        return GetDistance2D(x.Position,
                            firstOrDefault.Position);
                    return 0;
                })
                        .FirstOrDefault();


            /**************************************************DODGE*************************************************************/

            var f = ObjectManager.GetEntities<Hero>()
                        .Where(x => x.IsAlive && x.Team == Me.Team && !x.IsIllusion && x.IsControllable && x.ClassId == ClassId.CDOTA_Unit_Hero_Meepo)
                        .OrderBy(x =>
                {
                    var firstOrDefault = fount.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault();
                    if (firstOrDefault != null)
                        return GetDistance2D(x.Position,
                            firstOrDefault.Position);
                    return 0;
                })
                        .FirstOrDefault();
		    var meeposCount = meepos.Count();
            var q = new Ability[meeposCount];
            for (var i = 0; i < meeposCount; ++i) q[i] = meepos[i].Spellbook.SpellQ;
            var w = new Ability[meeposCount];
            for (var i = 0; i < meeposCount; ++i) w[i] = meepos[i].Spellbook.SpellW;
            if (Dodge && Me.IsAlive)
            {
                var baseDota =
                  ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_base" && unit.Team != Me.Team).ToList();
                if (baseDota != null)
                {
                    for (var t = 0; t < baseDota.Count(); ++t)
                    {
                        for (var i = 0; i < meeposCount; ++i)
                        {
                            var angle = meepos[i].FindAngleBetween(baseDota[t].Position, true);
                            var pos = new Vector3((float)(baseDota[t].Position.X - 710 * Math.Cos(angle)), (float)(baseDota[t].Position.Y - 710 * Math.Sin(angle)), 0);
                            if (!(meepos[i].Distance2D(baseDota[t]) <= 700) ||
                                meepos[i].HasModifier("modifier_bloodseeker_rupture") ||
                                !Utils.SleepCheck(meepos[i].Handle + "MoveDodge")) continue;
                            meepos[i].Move(pos);
                            Utils.Sleep(120, meepos[i].Handle + "MoveDodge");
                            //	Console.WriteLine("Name: " + baseDota[t].Name);
                            //	Console.WriteLine("Speed: " + baseDota[t].Speed);
                            //	Console.WriteLine("ClassId: " + baseDota[t].ClassId);
                            //	Console.WriteLine("Handle: " + baseDota[t].Handle);
                            //	Console.WriteLine("UnitState: " + baseDota[t].UnitState);
                        }
                    }
                }

                var thinker =
                   ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_thinker" && unit.Team != Me.Team).ToList();
                if (thinker != null)
                {
                    for (var i = 0; i < thinker.Count(); ++i)
                    {
                        for (var j = 0; j < meeposCount; ++j)
                        {
                            var angle = meepos[j].FindAngleBetween(thinker[i].Position, true);
                            var pos = new Vector3((float)(thinker[i].Position.X - 360 * Math.Cos(angle)), (float)(thinker[i].Position.Y - 360 * Math.Sin(angle)), 0);
                            if (!(meepos[j].Distance2D(thinker[i]) <= 350) ||
                                meepos[j].HasModifier("modifier_bloodseeker_rupture")) continue;
                            if (!Utils.SleepCheck(meepos[j].Handle + "MoveDodge")) continue;
                            meepos[j].Move(pos);
                            Utils.Sleep(350, meepos[j].Handle + "MoveDodge");
                        }
                    }
                }
                foreach (var v in meepos)
                {
                    if (Utils.SleepCheck(v.Handle + "_move") && v.Health <= v.MaximumHealth / 100 * Menu.Item("healh").GetValue<Slider>().Value
                        && !v.HasModifier("modifier_bloodseeker_rupture")
                        && v.Distance2D(fount.First().Position) >= 1000
                        )
                    {
                        v.Move(fount.First().Position);
                        Utils.Sleep(300, v.Handle + "_move");
                    }
                    if (!Activated) continue;
                    var angle = v.FindAngleBetween(fount.First().Position, true);
                    var pos = new Vector3((float)(fount.First().Position.X - 500 * Math.Cos(angle)), (float)(fount.First().Position.Y - 500 * Math.Sin(angle)), 0);

                    if (
                        v.Health >= v.MaximumHealth * 0.58
                        && v.Distance2D(fount.First()) <= 400
                        && Me.Team == Team.Radiant
                        && Utils.SleepCheck(v.Handle + "RadMove")
                    )
                    {
                        v.Move(pos);
                        Utils.Sleep(400, v.Handle + "RadMove");
                    }
                    if (!(v.Health >= v.MaximumHealth * 0.58) || !(v.Distance2D(fount.First()) <= 400) ||
                        Me.Team != Team.Dire || !Utils.SleepCheck(v.Handle + "DireMove")) continue;
                    v.Move(pos);
                    Utils.Sleep(400, v.Handle + "DireMove");
                }

                for (var i = 0; i < meeposCount; ++i)
                {
                    Travel = meepos[i].FindItem("item_travel_boots") ?? meepos[i].FindItem("item_travel_boots_2");
                    if (w[i] != null
                        && w[i].CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * Menu.Item("healh").GetValue<Slider>().Value
                        && meepos[i].Handle != f.Handle
                        && meepos[i].Distance2D(f) >= 700
                        && E == null
                        && meepos[i].Distance2D(fount.First().Position) >= 1500
                        && Utils.SleepCheck(meepos[i].Handle + "W"))
                    {
                        w[i].UseAbility(f);
                        Utils.Sleep(1000, meepos[i].Handle + "W");
                    }
                    else if (
                        Travel != null
                        && Travel.CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * Menu.Item("healh").GetValue<Slider>().Value
                       && (!w[i].CanBeCasted()
                       || meepos[i].Position.Distance2D(f) >= 1000
                       || (w[i].CanBeCasted()
                       && f.Distance2D(fount.First()) >= 1500))
                       || (meepos[i].IsSilenced()
                       || meepos[i].MovementSpeed <= 280)
                       && meepos[i].Distance2D(fount.First().Position) >= 1500
                       && E == null
                       && Utils.SleepCheck(meepos[i].Handle + "travel"))
                    {
                        Travel.UseAbility(fount.First().Position);
                        Utils.Sleep(1000, meepos[i].Handle + "travel");
                    }
                    if (meepos[i].HasModifier("modifier_bloodseeker_rupture"))
                    {

                        if (w[i] != null
                            && w[i].CanBeCasted()
                            && meepos[i].Handle != f.Handle
                            && Utils.SleepCheck(meepos[i].Handle + "W"))
                        {
                            w[i].UseAbility(f);
                            Utils.Sleep(500, meepos[i].Handle + "W");
                        }
                        else if (Travel != null && Travel.CanBeCasted()
                                 && !w[i].CanBeCasted()
                                 && meepos[i].Distance2D(fount.First().Position) >= 1200
                                 && Utils.SleepCheck(meepos[i].Handle + "travel"))
                        {
                            Travel.UseAbility(fount.First().Position);
                            Utils.Sleep(1000, meepos[i].Handle + "travel");
                        }
                    }
                    if (E != null
                        && q[i] != null
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * Menu.Item("healh").GetValue<Slider>().Value
                        && q[i].CanBeCasted()
                        && E.Modifiers.Any(y => y.Name != "modifier_meepo_earthbind")
                        && !E.IsMagicImmune()
                        && meepos[i].Distance2D(E) <= q[i].GetCastRange() - 50
                        && Utils.SleepCheck(meepos[i].Handle + "_net_casting"))
                    {
                        q[i].CastSkillShot(E);
                        Utils.Sleep(q[i].GetCastDelay(meepos[i], E, true) + 500, meepos[i].Handle + "_net_casting");
                    }
                    else if (!q[i].CanBeCasted() && meepos[i].Health <= meepos[i].MaximumHealth / 100 * Menu.Item("healh").GetValue<Slider>().Value)
                    {
                        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                        for (var j = 0; j < meeposCount; ++j)
                        {
                            if (E == null || q[j] == null || meepos[i].Handle == meepos[j].Handle ||
                                !(meepos[j].Position.Distance2D(E) < q[i].GetCastRange()) ||
                                !E.Modifiers.Any(y => y.Name != "modifier_meepo_earthbind") ||
                                !(meepos[j].Position.Distance2D(meepos[i]) < q[j].GetCastRange()) ||
                                E.IsMagicImmune() || !Utils.SleepCheck(meepos[i].Handle + "_net_casting")) continue;
                            q[j].CastSkillShot(E);
                            Utils.Sleep(q[j].GetCastDelay(meepos[j], E, true) + 1500, meepos[i].Handle + "_net_casting");
                            break;
                        }
                    }
                    if (E != null
                        && w[i] != null
                        && w[i].CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * Menu.Item("healh").GetValue<Slider>().Value
                        && meepos[i].Handle != f.Handle && meepos[i].Distance2D(f) >= 700
                        && (meepos[i].Distance2D(E) >= (E.AttackRange + 60)
                        || meepos[i].MovementSpeed <= 290)
                        && (q == null || (!q[i].CanBeCasted()
                        || E.HasModifier("modifier_meepo_earthbind")
                        || !E.IsMagicImmune()) || meepos[i].Distance2D(E) >= 1000)
                        && meepos[i].Distance2D(fount.First().Position) >= 1100
                        && Utils.SleepCheck(meepos[i].Handle + "W"))
                    {
                        w[i].UseAbility(f);
                        Utils.Sleep(1000, meepos[i].Handle + "W");
                    }
                    else if (
                            E != null
                            && Travel != null
                            && Travel.CanBeCasted()
                            && meepos[i].Health <= meepos[i].MaximumHealth
                            / 100 * Menu.Item("healh").GetValue<Slider>().Value
                            && (!w[i].CanBeCasted()
                            || meepos[i].Position.Distance2D(f) >= 1000
                            || (w[i].CanBeCasted()
                            && f.Distance2D(fount.First()) >= 2000))
                            && (meepos[i].Distance2D(E) >= (E.AttackRange + 60)
                            || (meepos[i].IsSilenced()
                            || meepos[i].MovementSpeed <= 290))
                            && meepos[i].Distance2D(fount.First().Position) >= 1100
                            && Utils.SleepCheck(meepos[i].Handle + "travel"))
                    {
                        Travel.UseAbility(fount.First().Position);
                        Utils.Sleep(1000, meepos[i].Handle + "travel");
                    }
                }
            }
            /**************************************************DODGE*************************************************************/
            /***************************************************POOF*************************************************************/
            if (PoofKey)
            {
                for (var i = 0; i < meeposCount; ++i)
                {
                    for (var j = 0; j < checkObj.Count(); ++j)
                    {
                        if (w[i] == null ||
                            ((!(meepos[i].Distance2D(checkObj[j]) <= 365) || !SafePoof) && (SafePoof)) ||
                            !w[i].CanBeCasted() || (meepos[i].Health < meepos[i].MaximumHealth
                                                    / 100 * Menu.Item("healh").GetValue<Slider>().Value && Dodge) ||
                            !Utils.SleepCheck(meepos[i].Handle + "Wpos")) continue;
                        w[i].UseAbility(meepos[i]);
                        Utils.Sleep(250, meepos[i].Handle + "Wpos");
                    }
                }
            }



            if (PoofAutoMode)
            {
                for (var i = 0; i < meeposCount; i++)
                {
                    var nCreeps = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral) && x.Team != Me.Team && x.IsSpawned && x.IsAlive).Where(x => x.Distance2D(meepos[i]) <= 345).ToList().Count();

                    SliderCountUnit = nCreeps >= (Skills.Item("poofCount").GetValue<Slider>().Value);

                    if (!SliderCountUnit || w[i] == null || !w[i].CanBeCasted() || !meepos[i].CanCast() ||
                        !(meepos[i].Health >= meepos[i].MaximumHealth
                          / 100 * Menu.Item("healh").GetValue<Slider>().Value - 0.05) ||
                        !(meepos[i].Mana >= (meepos[i].MaximumMana
                                             / 100 * Menu.Item("mana").GetValue<Slider>().Value)) ||
                        !Utils.SleepCheck(meepos[i].Handle + "Wpos")) continue;
                    w[i].UseAbility(meepos[i]);
                    Utils.Sleep(250, meepos[i].Handle + "Wpos");
                }
            }
            /***************************************************POOF*************************************************************/
            /**************************************************COMBO*************************************************************/
		    if (!Activated) return;
		    {
		        for (var i = 0; i < meeposCount; ++i)
		        {
		            E = ClosestToMouse(meepos[i]);

		            if (E == null) return;
		            InitMeepo = GetClosestToTarget(meepos, E);


		            if (
		                w[i] != null
		                && meepos[i].CanCast()
		                && (
		                    meepos[i].Handle != f.Handle && f.HasModifier("modifier_fountain_aura_buff")
		                    || meepos[i].Handle == f.Handle && !f.HasModifier("modifier_fountain_aura_buff")
		                )
		                && meepos.Count(x => x.Distance2D(meepos[i]) <= 1000) > 1
		                && meepos[i].Health >= meepos[i].MaximumHealth * 0.8
		                && w[i].CanBeCasted()
		                && InitMeepo.Distance2D(E) <= 350
		                && Utils.SleepCheck(meepos[i].Handle + "poof")
		            )
		            {
		                w[i].UseAbility(E.Position);
		                Utils.Sleep(250, meepos[i].Handle + "poof");
		            }

		            if (Me.HasModifier("modifier_fountain_aura_buff"))
		            {
		                if (
		                    Me.Spellbook.SpellW != null
		                    && Me.Spellbook.SpellW.CanBeCasted()
		                    && Me.Health >= Me.MaximumHealth * 0.8
		                    && meepos.Count(x => x.Distance2D(Me) <= 1000) > 1
		                    && InitMeepo.Distance2D(E) <= 350
		                    && Utils.SleepCheck(Me.Handle + "pooff")
		                )
		                {
		                    Me.Spellbook.SpellW.UseAbility(E.Position);
		                    Utils.Sleep(250, Me.Handle + "pooff");
		                }
		            }
		            //	
		            /*int[] cool;
					var core = Me.FindItem("item_octarine_core");
					if (core !=null)
						cool = new int[4] { 20, 16, 12, 8 };
					else
						cool = new int[4] { 15, 12, 9, 6 };*/

		            Orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
		            Blink = meepos[i].FindItem("item_blink");
		            Sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");


		            if ( // sheep
		                Sheep != null
		                && Sheep.CanBeCasted()
		                && Me.CanCast()
		                && !E.IsLinkensProtected()
		                && !E.IsMagicImmune()
		                && Me.Distance2D(E) <= 900
		                && meepos[i].Distance2D(E) <= 350
		                && Utils.SleepCheck("sheep")
		            )
		            {
		                Sheep.UseAbility(E);
		                Utils.Sleep(250, "sheep");
		            } // sheep Item end

		            if ( // Medall
		                Medall != null
		                && Medall.CanBeCasted()
		                && Utils.SleepCheck("Medall")
		                && meepos[i].Distance2D(E) <= 300
		                && Me.Distance2D(E) <= 700
		            )
		            {
		                Medall.UseAbility(E);
		                Utils.Sleep(250, "Medall");
		            } // Medall Item end
		            if ( // orchid
		                Orchid != null
		                && Orchid.CanBeCasted()
		                && Me.CanCast()
		                && !E.IsLinkensProtected()
		                && !E.IsMagicImmune()
		                && meepos[i].Distance2D(E) <= 300
		                && Me.Distance2D(E) <= 900
		                && Utils.SleepCheck("orchid")
		            )
		            {
		                Orchid.UseAbility(E);
		                Utils.Sleep(250, "orchid");
		            } // orchid Item end
		            if (Utils.SleepCheck("Q")
		                && !E.HasModifier("modifier_meepo_earthbind")
		                && (((!Blink.CanBeCasted()
		                      || Blink == null)
		                     && meepos[i].Distance2D(E) <= q[i].GetCastRange())
		                    || (Blink.CanBeCasted()
		                        && meepos[i].Distance2D(E) <= 350))
		            )
		            {
		                if (q[i] != null
		                    && (meepos[i].Health >= meepos[i].MaximumHealth
		                        / 100 * Menu.Item("healh").GetValue<Slider>().Value
		                        || !Dodge)
		                    && q[i].CanBeCasted()
		                    && !E.IsMagicImmune()
		                    && !meepos[i].IsChanneling()
		                    && meepos[i].Distance2D(E) <= q[i].GetCastRange()
		                    && Utils.SleepCheck(meepos[i].Handle + "_net_casting"))
		                {
		                    q[i].CastSkillShot(E);
		                    Utils.Sleep(q[i].GetCastDelay(meepos[i], E, true) + 1500, meepos[i].Handle + "_net_casting");
		                    Utils.Sleep(1500, "Q");
		                }
		            }

		            if (
		                Blink != null
		                && Me.CanCast()
		                && Blink.CanBeCasted()
		                && Me.Distance2D(E) >= 350
		                && Me.Distance2D(E) <= 1150
		            )
		            {
		                if (Blink.CanBeCasted()
		                    && !Menu.Item("blinkDelay").IsActive()
		                    && meepos[i].Health >= meepos[i].MaximumHealth / 100 * Menu.Item("healh").GetValue<Slider>().Value
		                    && Utils.SleepCheck("13"))
		                {
		                    Blink.UseAbility(E.Position);
		                    Utils.Sleep(200, "13");
		                }

		                Task.Delay(1340 - (int)Game.Ping).ContinueWith(_ =>
		                {
		                    if (!Blink.CanBeCasted() || !Menu.Item("blinkDelay").IsActive() ||
		                        !Utils.SleepCheck("12")) return;
		                    Blink.UseAbility(E.Position);
		                    Utils.Sleep(200, "12");
		                });
		                for (var j = 0; j < meeposCount; ++j)
		                {
		                    if (w[j] == null || meepos[j].Handle == Me.Handle || !meepos[j].CanCast() ||
		                        (((f.Handle == meepos[j].Handle || !f.HasModifier("modifier_fountain_aura_buff")) &&
		                          f.HasModifier("modifier_fountain_aura_buff"))) ||
		                        meepos[j].Health < meepos[j].MaximumHealth / 100 *
		                        Menu.Item("healh").GetValue<Slider>().Value || E.IsMagicImmune() || !w[j].CanBeCasted() ||
		                        !Utils.SleepCheck(meepos[j].Handle + "poof")) continue;
		                    w[j].UseAbility(E.Position);
		                    Utils.Sleep(250, meepos[j].Handle + "poof");
		                }
		            }
		            if (
		                meepos[i].Distance2D(E) <= 200 && (!meepos[i].IsAttackImmune() || !E.IsAttackImmune())
		                && meepos[i].NetworkActivity != NetworkActivity.Attack && meepos[i].CanAttack()
		                && meepos[i].Health >= meepos[i].MaximumHealth / 100 * Menu.Item("healh").GetValue<Slider>().Value
		                && !meepos[i].IsChanneling() 
		                && Utils.SleepCheck(meepos[i].Handle + "Attack")
		            )
		            {
		                meepos[i].Attack(E);
		                Utils.Sleep(180, meepos[i].Handle + "Attack");
		            }
		            else if (((
		                         (!meepos[i].CanAttack()
		                          || meepos[i].Distance2D(E) >= 0)
		                         && meepos[i].NetworkActivity != NetworkActivity.Attack
		                         && meepos[i].Distance2D(E) <= 1000))
		                     && ((meepos[i].Handle != Me.Handle
		                          && (Blink != null && Blink.CanBeCasted()
		                              && Me.Distance2D(E) <= 350)
		                          || (meepos[i].Handle == Me.Handle
		                              && !Blink.CanBeCasted()))
		                         || Blink == null)
		                     && meepos[i].Health >= meepos[i].MaximumHealth / 100 * Menu.Item("healh").GetValue<Slider>().Value
		                     && !meepos[i].IsChanneling()
		                     && Utils.SleepCheck(meepos[i].Handle + "Move"))
		            {
		                meepos[i].Move(E.Predict(450));
		                Utils.Sleep(250, meepos[i].Handle + "Move");
		            }


		        }

		        Vail = Me.FindItem("item_veil_of_discord");
		        Shiva = Me.FindItem("item_shivas_guard");
		        Medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
		        Atos = Me.FindItem("item_rod_of_atos");
		        Cheese = Me.FindItem("item_cheese");
		        Abyssal = Me.FindItem("item_abyssal_blade");
		        Dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
		        Ethereal = Me.FindItem("item_ethereal_blade");

		        E = Toolset.ClosestToMouse(Me);
		        if (E == null) return;
		        if ( // ethereal
		            Ethereal != null
		            && Ethereal.CanBeCasted()
		            && Me.CanCast()
		            && !E.IsLinkensProtected()
		            && !E.IsMagicImmune()
		            && Utils.SleepCheck("ethereal")
		        )
		        {
		            Ethereal.UseAbility(E);
		            Utils.Sleep(200, "ethereal");
		        } // ethereal Item end
		        if (// Dagon
		            Me.CanCast()
		            && Dagon != null
		            && (Ethereal == null
		                || (E.HasModifier("modifier_item_ethereal_blade_slow")
		                    || Ethereal.Cooldown < 17))
		            && !E.IsLinkensProtected()
		            && Dagon.CanBeCasted()
		            && !E.IsMagicImmune()
		            && Utils.SleepCheck("dagon")
		        )
		        {
		            Dagon.UseAbility(E);
		            Utils.Sleep(200, "dagon");
		        } // Dagon Item end
		        if ( // vail
		            Vail != null
		            && Vail.CanBeCasted()
		            && Me.CanCast()
		            && !E.IsMagicImmune()
		            && Me.Distance2D(E) <= 1100
		            && Utils.SleepCheck("vail")
		        )
		        {
		            Vail.UseAbility(E.Position);
		            Utils.Sleep(250, "vail");
		        } // orchid Item end
		        if (// Shiva Item
		            Shiva != null
		            && Shiva.CanBeCasted()
		            && Me.CanCast()
		            && !E.IsMagicImmune()
		            && Utils.SleepCheck("shiva")
		            && Me.Distance2D(E) <= 600
		        )
		        {
		            Shiva.UseAbility();
		            Utils.Sleep(250, "shiva");
		        } // Shiva Item end
		        if (
		            // cheese
		            Cheese != null
		            && Cheese.CanBeCasted()
		            && Me.Health <= (Me.MaximumHealth * 0.3)
		            && Me.Distance2D(E) <= 700
		            && Utils.SleepCheck("cheese")
		        )
		        {
		            Cheese.UseAbility();
		            Utils.Sleep(200, "cheese");
		        } // cheese Item end

		        if ( // atos Blade
		            Atos != null
		            && Atos.CanBeCasted()
		            && Me.CanCast()
		            && !E.IsLinkensProtected()
		            && !E.IsMagicImmune()
		            && Me.Distance2D(E) <= 2000
		            && Utils.SleepCheck("atos")
		        )
		        {
		            Atos.UseAbility(E);
		            Utils.Sleep(250, "atos");
		        } // atos Item end
		        if (Abyssal == null || !Abyssal.CanBeCasted() || !Me.CanCast() || E.IsStunned() || E.IsHexed() ||
		            !Utils.SleepCheck("abyssal") || !(Me.Distance2D(E) <= 300)) return;
		        Abyssal.UseAbility(E);
		        Utils.Sleep(250, "abyssal");
		    }
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("<font face='verdana' color='#f80000'>Probably gonna dig a grave or two before this is done.</font>");

			Console.WriteLine("Meepo combo loaded!");
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("Dodge", "Dodge meepo's").SetValue(new KeyBind('T', KeyBindType.Toggle)));
			Menu.AddItem(new MenuItem("blinkDelay", "Use Blink delay before all poof meepo the enemy").SetValue(true));
			Menu.AddItem(new MenuItem("healh", "Min Healh to Move Fount").SetValue(new Slider(58, 10)));
			Skills.AddItem(new MenuItem("poofSafe", "Use poof if ability radius suitable targets.").SetValue(true));
			Skills.AddItem(new MenuItem("poofKey", "All Poof Key").SetValue(new KeyBind('F', KeyBindType.Press)));
			Skills.AddItem(new MenuItem("poofAutoMod", "AutoPoofFarm").SetValue(new KeyBind('J', KeyBindType.Toggle)));
			Skills.AddItem(new MenuItem("poofCount", "Min units to Poof").SetValue(new Slider(3, 1, 10)));
			Skills.AddItem(new MenuItem("mana", "Min Mana % to Poof").SetValue(new Slider(35, 10)));
			Menu.AddSubMenu(Skills);
			
			Txt = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Tahoma",
					Height = 12,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.Default
				});

			Not = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Monospace",
					Height = 35,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.ClearType
				});

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
		}
		
		public Hero ClosestToMouse(Hero source, float range = 90000)
		{
			var mousePosition = Game.MousePosition;
			var enemyHeroes =
				ObjectManager.GetEntities<Hero>()
					.Where(
						x =>
							x.Team != Me.Team && !x.IsIllusion && x.IsAlive && x.IsVisible
							&& x.Distance2D(mousePosition) <= range);
			Hero[] closestHero = { null };
			foreach (var enemyHero in enemyHeroes.Where(enemyHero => closestHero[0] == null || closestHero[0].Distance2D(mousePosition) > enemyHero.Distance2D(mousePosition)))
			{
				closestHero[0] = enemyHero;
			}
			return closestHero[0];
		}


		public Hero GetClosestToTarget(List<Hero> units, Hero z)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(z) > v.Distance2D(z)))
			{
				closestHero = v;
			}
			return closestHero;
		}

        private double GetDistance2D(Vector3 a, Vector3 b)
		{
			return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
		}


        private void CurrentDomain_DomainUnload(object sender, EventArgs args)
		{
			Txt.Dispose();
			Not.Dispose();
		}


		public void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			if (player == null || player.Team == Team.Observer || Me == null)
				return;
			if (Activated)
			{
				Txt.DrawText(null, "Combo meepo's active", 1200, 12, Color.Green);
			}

			if (!Dodge)
			{
				Txt.DrawText(null, "Warning! Dodge unActive", 1200, 22, Color.DarkRed);
			}

			if (PoofAutoMode)
			{
				Txt.DrawText(null, "Auto Poof On", 1200, 30, Color.Green);
			}

			if (!PoofAutoMode)
			{
				Txt.DrawText(null, "Auto Poof Off", 1200, 30, Color.DarkRed);
			}

			/*
			if (farm)
			{
				txt.DrawText(null, "Farm Meepo On", 1200, 32, Color.Green);
			}

			if (!farm)
			{
				txt.DrawText(null, "Farm Meepo Off", 1200, 32, Color.DarkRed);
			}
			if (push)
			{
				txt.DrawText(null, "Push Meepo On", 1200, 42, Color.Green);
			}

			if (!push)
			{
				txt.DrawText(null, "Push Meepo Off", 1200, 42, Color.DarkRed);
			}
			модификатор зевса
			аутопуф - комбо
			*/
		}

        private void Drawing_OnPostReset(EventArgs args)
		{
			Txt.OnResetDevice();
			Not.OnResetDevice();
		}

        private void Drawing_OnPreReset(EventArgs args)
		{
			Txt.OnLostDevice();
			Not.OnLostDevice();
		}
	}
}