using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using SharpDX;
	using Service.Debug;

	internal class BrewmasterController : Variables, IHeroController
    {
        private readonly Menu _menuItems = new Menu("Items", "Items");
        private readonly Menu _menuSkills = new Menu("Skills: ", "Skills: ");
        private Ability _q, _w, _r;
        private Item _blink, _bkb, _orchid, _necronomicon, _urn, _medal, _shiva, _manta;
        private Vector3 _mousepos;

        public void Combo()
		{
			_blink = Me.FindItem("item_blink");
			_bkb = Me.FindItem("item_black_king_bar");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_necronomicon = Me.FindItem("item_necronomicon");
			_urn = Me.FindItem("item_urn_of_shadows");
			_medal = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
			_manta = Me.FindItem("item_manta");
			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;
			//manta (when silenced)
			if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)) &&
				_manta.CanBeCasted() && Me.IsSilenced() && Utils.SleepCheck("manta"))
			{
				_manta.UseAbility();
				Utils.Sleep(400, "manta");
			}
			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
			{
				if (Me.CanCast())
				{
					_mousepos = Game.MousePosition;
                    E = Toolset.ClosestToMouse(Me);
                    if (Me.Distance2D(_mousepos) <= 1200)
					{
						//drunken haze (main combo)
						if ((_w != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(_w.Name)) &&
							_w.CanBeCasted() &&
							((E.Position.Distance2D(Me.Position) < 850) &&
							 (E.Position.Distance2D(Me.Position) > 300)) && _r.CanBeCasted() &&
							Utils.SleepCheck("W"))
						{
							_w.UseAbility(E);
							Utils.Sleep(150, "W");
						}
						//drunken haze (if can't cast ult) --->Сюда добавить переменную отвечающую за ручное выключение ульты из комбо && если ульт выключен
						if ((_w != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(_w.Name)) &&
							_w.CanBeCasted() && E.Position.Distance2D(Me.Position) < 850 &&
							(!_r.CanBeCasted() || (E.Health < (E.MaximumHealth * 0.50)) ||
							 !(Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(_r.Name))) && !E.HasModifier("modifier_brewmaster_drunken_haze") &&
							(E.Health > (E.MaximumHealth * 0.07)) && Utils.SleepCheck("W"))
						{
							_w.UseAbility(E);
							Utils.Sleep(150, "W");
						}
						//black king bar
						if ((_bkb != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name)) &&
							_bkb.CanBeCasted() && Utils.SleepCheck("bkb"))
						{
							_bkb.UseAbility();
							Utils.Sleep(150, "bkb");
						}
						//blink
						if ((_blink != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)) &&
							_blink.CanBeCasted() && E.Position.Distance2D(Me.Position) <= 1150 &&
							E.Position.Distance2D(Me.Position) > 300 && Utils.SleepCheck("blink"))
						{
							_blink.UseAbility(E.Position);
							//blink.UseAbility(Me.Distance2D(mousepos) < 1200 ? mousepos : new Vector3(Me.NetworkPosition.X + 1150 * (float)Math.Cos(Me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), Me.NetworkPosition.Y + 1150 * (float)Math.Sin(Me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), 100), false);
							Utils.Sleep(150, "blink");
						}
						//orchid
						if ((_orchid != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)) &&
							_orchid.CanBeCasted() && (E.Position.Distance2D(Me.Position) < 300) &&
							Utils.SleepCheck("orchid"))
						{
							_orchid.UseAbility(E);
							Utils.Sleep(150, "orchid");
						}
						//necronomicon
						if (_necronomicon != null && _necronomicon.CanBeCasted() &&
							(E.Health > (E.MaximumHealth * 0.20)) &&
							(E.Position.Distance2D(Me.Position) < 400) && Utils.SleepCheck("necronomicon"))
						{
							_necronomicon.UseAbility();
							Utils.Sleep(150, "necronomicon");
						}
						//thunder clap
						if ((_q != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(_q.Name)) &&
							_q.CanBeCasted() && (E.Position.Distance2D(Me.Position) < 300) && Utils.SleepCheck("Q"))
						{
							_q.UseAbility();
							Utils.Sleep(150, "Q");
						}
						//urn of shadow
						if ((_urn != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name)) &&
							_urn.CanBeCasted() && Utils.SleepCheck("urn"))
						{
							_urn.UseAbility(E);
							Utils.Sleep(150, "urn");
						}
						//medal
						if ((_medal != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medal.Name)) &&
							_medal.CanBeCasted() && (E.Position.Distance2D(Me.Position) < 300) &&
							Utils.SleepCheck("medal"))
						{
							_medal.UseAbility(E);
							Utils.Sleep(150, "medal");
						}
						//shiva
						if ((_shiva != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)) &&
							_shiva.CanBeCasted() && (E.Position.Distance2D(Me.Position) <= 800) &&
							Utils.SleepCheck("shiva"))
						{
							_shiva.UseAbility();
							Utils.Sleep(150, "shiva");
						}
						//manta
						if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)) &&
							_manta.CanBeCasted() && (E.Position.Distance2D(Me.Position) <= 450) &&
							Utils.SleepCheck("manta"))
						{
							_manta.UseAbility();
							Utils.Sleep(150, "manta");
						}
						//ULTIMATE: R
						if ((_r != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(_r.Name)) &&
							_r.CanBeCasted() && (E.Position.Distance2D(Me.Position) < 500) &&
							(E.Health > (E.MaximumHealth * 0.35)) && !_q.CanBeCasted() && !_orchid.CanBeCasted() &&
							!_necronomicon.CanBeCasted() && !_urn.CanBeCasted() && !_medal.CanBeCasted() &&
							!_shiva.CanBeCasted() && !_manta.CanBeCasted() && Utils.SleepCheck("R"))
						{
							_r.UseAbility();
							Utils.Sleep(1000, "R");
						}
						//Moving+Attaking
						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
				}
			}
			var targets =
				ObjectManager.GetEntities<Hero>()
					.Where(
						enemy =>
							enemy.Team == Me.GetEnemyTeam() && !enemy.IsIllusion() && enemy.IsVisible && enemy.IsAlive &&
							enemy.Health > 0)
					.ToList();


			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
			{
				if (E.IsAlive && !E.IsInvul() &&
					(Game.MousePosition.Distance2D(E) <= 1000 || E.Distance2D(Me) <= 600))
				{
					var checkDrunken = E.HasModifier("modifier_brewmaster_drunken_haze");
					var checkClap = E.HasModifier("modifier_brewmaster_thunder_clap");


					var necronomicons =
						ObjectManager.GetEntities<Creep>().Where(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Creep)
																  && x.IsAlive && x.IsControllable);
					if (necronomicons == null)
					{
						return;
					}
					foreach (var v in necronomicons)
					{
						var archer =
							ObjectManager.GetEntities<Unit>()
								.Where(
									unit =>
										unit.Name == "npc_dota_necronomicon_archer" && unit.IsAlive &&
										unit.IsControllable)
								.ToList();
						if (archer != null && E.Position.Distance2D(v.Position) <= 650 &&
							v.Spellbook.SpellQ.CanBeCasted() &&
							Utils.SleepCheck(v.Handle.ToString()))

						{
							v.Spellbook.SpellQ.UseAbility(E);
							Utils.Sleep(300, v.Handle.ToString());
						}

						if (E.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(E);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}

					//Necronomicon Warrior
					var necrowarrior =
						ObjectManager.GetEntities<Creep>().Where(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Creep)
																  && x.IsAlive && x.IsControllable);
					if (necrowarrior == null)
					{
						return;
					}
					foreach (var v in necrowarrior)
					{
					    if (!(E.Position.Distance2D(v.Position) < 1550) || !Utils.SleepCheck(v.Handle.ToString())) continue;
					    v.Attack(E);
					    Utils.Sleep(700, v.Handle.ToString());
					}
					//Illusions	
					var illus =
						ObjectManager.GetEntities<Hero>()
							.Where(x => x.IsAlive && x.IsControllable && x.Team == Me.Team && x.IsIllusion)
							.ToList();
					if (illus == null)
					{
						return;
					}
					foreach (Hero illusion in illus.TakeWhile(illusion => Utils.SleepCheck("illu")))
					{
						illusion.Attack(E);
						Utils.Sleep(1000, "illu");
					}

					var primalstorm =
						ObjectManager.GetEntities<Unit>()
							.Where(x => x.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalStorm && x.IsAlive);
					var primalearth =
						ObjectManager.GetEntities<Unit>()
							.Where(x => (x.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalEarth && x.IsAlive));
					var primalfire =
						ObjectManager.GetEntities<Unit>()
							.Where(x => (x.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalFire && x.IsAlive));
					if (primalearth == null)
					{
						return;
					}
					foreach (var v in primalearth)
					{
						if (E.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellQ.CanBeCasted() &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellQ.UseAbility(E);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (E.Position.Distance2D(v.Position) < 300 && v.Spellbook.SpellR.CanBeCasted() &&
							!checkClap &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellR.UseAbility();
							Utils.Sleep(400, v.Handle.ToString());
						}

						if (E.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(E);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}

					if (primalstorm == null)
					{
						return;
					}


				    var enumerable = primalstorm as Unit[] ?? primalstorm.ToArray();
				    foreach (var v in enumerable)
					{
						if (E.Position.Distance2D(v.Position) < 500 && v.Spellbook.SpellQ.CanBeCasted() &&
							(Menu.Item("Primalstorm: Use Dispel Magic").GetValue<bool>()) &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellQ.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellQ.UseAbility(E.Position);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (E.Position.Distance2D(v.Position) < 900 && v.Spellbook.SpellE.CanBeCasted() &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellE.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellE.UseAbility();
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (E.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellR.CanBeCasted() &&
							!checkDrunken &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellR.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellR.UseAbility(E);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (E.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(E);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}
					// 2 Skill

					foreach (var target1 in targets)
					{
						if ((target1.Health > (target1.MaximumHealth * 0.85)) || ((target1.IsChanneling())))
						{
							foreach (var v in enumerable)
							{
								_w = v.Spellbook.SpellW;
								if (v.Spellbook.SpellW.CanBeCasted() &&
									((target1.Position.Distance2D(v.Position) < 600) || (target1.IsChanneling())) &&
									(target1.Position.Distance2D(v.Position) < 1550) &&
									((Menu.Item("Primalstorm: Use Cyclone").GetValue<bool>()) ||
									 (target1.IsChanneling())) &&
									((target1.Position != E.Position) || (target1.IsChanneling())) &&
									Utils.SleepCheck("ulti"))
								{
									v.Spellbook.SpellW.UseAbility(target1);
									Utils.Sleep(700, "ulti");
								}
							}
						}
					}
					//

					if (primalfire == null)
					{
						return;
					}
					foreach (var v in primalfire)
					{
						if (E.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(E);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Need to Creet!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(_menuItems);
			_menuItems.AddItem(new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_blink", true},
			    {"item_black_king_bar", true},
			    {"item_orchid", true}, {"item_bloodthorn", true},
			    {"item_necronomicon", true},
			    {"item_solar_crest", true},
			    {"item_urn_of_shadows", true},
			    {"item_medallion_of_courage", true},
			    {"item_shivas_guard", true},
			    {"item_manta", true}
			})));
			Menu.AddSubMenu(_menuSkills);
			_menuSkills.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"brewmaster_thunder_clap", true},
			    {"brewmaster_drunken_haze", true},
			    {"brewmaster_primal_split", true}
			})));
			Console.WriteLine(">Brewmaster by VickTheRock loaded!");
			Menu.AddItem(
				new MenuItem("Primalstorm: Use Cyclone", "Primalstorm: Use Cyclone").SetValue(false)
					.SetTooltip(
						"If disabled, casts Cyclone only in targets, which channeling abilitys like: tp's, blackhole's, deathward's and e.t.c."));
			Menu.AddItem(
				new MenuItem("Primalstorm: Use Dispel Magic", "Primalstorm: Use Dispel Magic").SetValue(false)
					.SetTooltip("If enabled, always safe mana for Cyclon."));
			Menu.AddItem(
				new MenuItem("Save mana for Cyclone", "Save mana for Cyclone").SetValue(false)
					.SetTooltip(
						"Do not cast Dispel Magic, Drunken Haze or Invisability if after cast there will be no mana for Cyclone."));
			
		}

		public void OnCloseEvent()
		{
			
		}
	}
}