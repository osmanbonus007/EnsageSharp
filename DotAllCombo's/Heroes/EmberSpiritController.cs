using DotaAllCombo.Extensions;
using SharpDX;

namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;


	using Service.Debug;

	internal class EmberSpiritController : Variables, IHeroController
	{
		private Ability _q, _w, _e, _r, _d;

		private Item _urn, _dagon, _ghost, _soul, _atos, _vail, _sheep, _cheese, _stick, _arcane,
			_halberd, _mjollnir, _ethereal, _orchid, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _medall, _blink;

		private bool _wKey;
		private bool _oneUlt;

		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Push = Game.IsKeyDown(Menu.Item("keyEscape").GetValue<KeyBind>().Key);
			_wKey = Game.IsKeyDown(Menu.Item("wKey").GetValue<KeyBind>().Key);
			_oneUlt = Menu.Item("oneult").IsActive();
			if (!Menu.Item("enabled").IsActive())
				return;

            E = Toolset.ClosestToMouse(Me);
            if (Push)
			{
				Unit fount = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);
				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.IsValid && unit.IsAlive && unit.Team == Me.Team && unit.Name == "npc_dota_ember_spirit_remnant").ToList();

				if (fount != null)
				{
					float angle = Me.FindAngleBetween(fount.Position, true);
					Vector3 pos = new Vector3((float)(Me.Position.X + _r.GetCastRange() * Math.Cos(angle)),
						(float)(Me.Position.Y + _r.GetCastRange() * Math.Sin(angle)), 0);

					if (remnant.Count(x => x.Distance2D(Me) <= 10000) == 0)
					{
						if (_r != null && _r.CanBeCasted()
							&& Me.FindModifier("modifier_ember_spirit_fire_remnant_charge_counter").StackCount >= 1
							&& Utils.SleepCheck("z"))
						{
							_r.UseAbility(pos);
							Utils.Sleep(1000, "z");
						}
					}
					if (remnant.Count(x => x.Distance2D(Me) <= 10000) > 0)
					{
						if (_d != null && _d.CanBeCasted())
						{
							for (int i = 0; i < remnant.Count; ++i)
							{
								var kill =
									remnant[i].Modifiers.Where(y => y.Name == "modifier_ember_spirit_fire_remnant_thinker")
										.DefaultIfEmpty(null)
										.FirstOrDefault();

								if (kill != null
									&& kill.RemainingTime < 44
									&& Utils.SleepCheck("Rem"))
								{
									_d.UseAbility(fount.Position);
									Utils.Sleep(300, "Rem");
								}
							}
						}
					}
				}
			}
		
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

			_e = Me.Spellbook.SpellE;

			_r = Me.Spellbook.SpellR;

			_d = Me.Spellbook.SpellD;

			_ethereal = Me.FindItem("item_ethereal_blade");
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon =
				Me.Inventory.Items.FirstOrDefault(
					item =>
						item.Name.Contains("item_dagon"));
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_blink = Me.FindItem("item_blink");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			if (E == null) return;
			_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
			_vail = Me.FindItem("item_veil_of_discord");
			_cheese = Me.FindItem("item_cheese");
			_ghost = Me.FindItem("item_ghost");
			_atos = Me.FindItem("item_rod_of_atos");
			_soul = Me.FindItem("item_soul_ring");
			_arcane = Me.FindItem("item_arcane_boots");
			_stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
			_shiva = Me.FindItem("item_shivas_guard");


			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");



			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();


			
			if ((Active || _wKey) && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}

			if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Toolset.invUnit(E))
			{
				if (stoneModif) return;
				if ( // MOM
					_mom != null
					&& _mom.CanBeCasted()
					&& Me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
					&& Utils.SleepCheck("mom")
					&& Me.Distance2D(E) <= 700
					)
				{
					_mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Hellbard
					_halberd != null
					&& _halberd.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& (E.NetworkActivity == NetworkActivity.Attack
						|| E.NetworkActivity == NetworkActivity.Crit
						|| E.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& Me.Distance2D(E) <= 700
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
					)
				{
					_halberd.UseAbility(E);
					Utils.Sleep(250, "halberd");
				}
				if ( //Ghost
					_ghost != null
					&& _ghost.CanBeCasted()
					&& Me.CanCast()
					&& ((Me.Position.Distance2D(E) < 300
						 && Me.Health <= (Me.MaximumHealth * 0.7))
						|| Me.Health <= (Me.MaximumHealth * 0.3))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
					&& Utils.SleepCheck("Ghost"))
				{
					_ghost.UseAbility();
					Utils.Sleep(250, "Ghost");
				}
				if ( // Arcane Boots Item
					_arcane != null
					&& Me.Mana <= _w.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_arcane.Name)
					&& _arcane.CanBeCasted()
					&& Utils.SleepCheck("arcane")
					)
				{
					_arcane.UseAbility();
					Utils.Sleep(250, "arcane");
				} // Arcane Boots Item end
				if ( // Mjollnir
					_mjollnir != null
					&& _mjollnir.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& Me.Distance2D(E) <= 900
					)
				{
					_mjollnir.UseAbility(Me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if (
					// cheese
					_cheese != null
					&& _cheese.CanBeCasted()
					&& Me.Health <= (Me.MaximumHealth * 0.3)
					&& Me.Distance2D(E) <= 700
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
					&& Utils.SleepCheck("cheese")
					)
				{
					_cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // Medall
					_medall != null
					&& _medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
					&& Me.Distance2D(E) <= 700
					)
				{
					_medall.UseAbility(E);
					Utils.Sleep(250, "Medall");
				} // Medall Item end
				if (
					_w != null && _w.CanBeCasted() && Me.Distance2D(E) <= 700
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& Utils.SleepCheck("W")
					)
				{
					_w.UseAbility(E.Predict(300));
					Utils.Sleep(200, "W");
				}
				if ( // Q Skill
					_q != null
					&& _q.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Me.Distance2D(E) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					_q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					_w != null
					&& _w.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& Utils.SleepCheck("W")
					)
				{
					_w.UseAbility(E.Position);
					Utils.Sleep(200, "W");
				} // W Skill end
				if ( // E Skill
					_e != null
					&& _e.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Utils.SleepCheck("E")
					&& Me.Distance2D(E) <= 400
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
					)
				{
					_e.UseAbility();
					Utils.Sleep(350, "E");
				} // E Skill end
				if ( //R Skill
					_r != null
					&& !_oneUlt
					&& _r.CanBeCasted()
					&& Me.CanCast()
					&& Me.Distance2D(E) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					_r.UseAbility(E.Predict(700));
					Utils.Sleep(110, "R");
				} // R Skill end
				if ( //R Skill
					_r != null
					&& _oneUlt
					&& _r.CanBeCasted()
					&& Me.CanCast()
					&& Me.Distance2D(E) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					_r.UseAbility(E.Predict(700));
					Utils.Sleep(5000, "R");
				} // R Skill end
				if ( // sheep
					_sheep != null
					&& _sheep.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Me.Distance2D(E) <= 1400
					&& !stoneModif
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
					&& Utils.SleepCheck("sheep")
					)
				{
					_sheep.UseAbility(E);
					Utils.Sleep(250, "sheep");
				} // sheep Item end
				if ( // Abyssal Blade
					_abyssal != null
					&& _abyssal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsStunned()
					&& !E.IsHexed()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
					&& Utils.SleepCheck("abyssal")
					&& Me.Distance2D(E) <= 400
					)
				{
					_abyssal.UseAbility(E);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 900
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) &&
					Utils.SleepCheck("orchid"))
				{
					_orchid.UseAbility(E);
					Utils.Sleep(100, "orchid");
				}

				if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
					&& !E.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					_shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // ethereal
					_ethereal != null
					&& _ethereal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
					&& Utils.SleepCheck("ethereal")
					)
				{
					_ethereal.UseAbility(E);
					Utils.Sleep(200, "ethereal");
				} // ethereal Item end
				if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& Me.Distance2D(E) >= 450
					&& Me.Distance2D(E) <= 1150
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					_blink.UseAbility(E.Position);
					Utils.Sleep(250, "blink");
				}

				if ( // SoulRing Item 
					_soul != null
					&& _soul.CanBeCasted()
					&& Me.CanCast()
					&& Me.Health >= (Me.MaximumHealth * 0.5)
					&& Me.Mana <= _r.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
					)
				{
					_soul.UseAbility();
				} // SoulRing Item end
				if ( // Dagon
					Me.CanCast()
					&& _dagon != null
					&& (_ethereal == null
						|| (E.HasModifier("modifier_item_ethereal_blade_slow")
							|| _ethereal.Cooldown < 17))
					&& !E.IsLinkensProtected()
					&& _dagon.CanBeCasted()
					&& !E.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					_dagon.UseAbility(E);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // atos Blade
					_atos != null
					&& _atos.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
					&& Me.Distance2D(E) <= 2000
					&& Utils.SleepCheck("atos")
					)
				{
					_atos.UseAbility(E);

					Utils.Sleep(250, "atos");
				} // atos Item end
				if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
				{
					_urn.UseAbility(E);
					Utils.Sleep(240, "urn");
				}
				if ( // vail
					_vail != null
					&& _vail.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
					&& Me.Distance2D(E) <= 1500
					&& Utils.SleepCheck("vail")
					)
				{
					_vail.UseAbility(E.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end
				  /*	if (
					  force != null
					  && force.CanBeCasted()
					  && Me.Distance2D(e) < force.GetCastRange()
					  && Utils.SleepCheck(e.Handle.ToString()))
				  {
					  force.UseAbility(e);
					  Utils.Sleep(500, e.Handle.ToString());
				  }
			  */
				if (
					_stick != null
					&& _stick.CanBeCasted()
					&& _stick.CurrentCharges != 0
					&& Me.Distance2D(E) <= 700
					&& (Me.Health <= (Me.MaximumHealth * 0.5)
						|| Me.Mana <= (Me.MaximumMana * 0.5))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_stick.Name))
				{
					_stick.UseAbility();
					Utils.Sleep(200, "mana_items");
				}
				if ( // Satanic 
					_satanic != null &&
					Me.Health <= (Me.MaximumHealth * 0.3) &&
					_satanic.CanBeCasted() &&
					Me.Distance2D(E) <= Me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					_satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				} // Satanic Item end
				if (_mail != null && _mail.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
														   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
				{
					_mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				if (_bkb != null && _bkb.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("bkb"))
				{
					_bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}

				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();

				if (remnant.Count <= 0)
					return;
				for (int i = 0; i < remnant.Count; ++i)
				{
					if (//D Skill
					   remnant != null
					   && _d.CanBeCasted()
					   && Me.CanCast()
					   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_d.Name)
					   && remnant[i].Distance2D(E) <= 500
					   && Utils.SleepCheck("D")
					   )
					{
						_d.UseAbility(E.Position);
						Utils.Sleep(400, "D");
					}
				}
			}
			if (_wKey && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive)
			{
				if ( // Q Skill
					_q != null
					&& _q.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Me.Distance2D(E) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					_q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					_w != null
					&& _w.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& Utils.SleepCheck("W")
					)
				{
					_w.UseAbility(E.Position);
					Utils.Sleep(200, "W");
				} // W Skill end
			}
			else if (_wKey && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Toolset.invUnit(E))
			{
				if ( // MOM
					_mom != null
					&& _mom.CanBeCasted()
					&& Me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
					&& Utils.SleepCheck("mom")
					&& Me.Distance2D(E) <= 700
					)
				{
					_mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Hellbard
					_halberd != null
					&& _halberd.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& (E.NetworkActivity == NetworkActivity.Attack
						|| E.NetworkActivity == NetworkActivity.Crit
						|| E.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& Me.Distance2D(E) <= 700
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
					)
				{
					_halberd.UseAbility(E);
					Utils.Sleep(250, "halberd");
				}
				if ( //Ghost
					_ghost != null
					&& _ghost.CanBeCasted()
					&& Me.CanCast()
					&& ((Me.Position.Distance2D(E) < 300
						 && Me.Health <= (Me.MaximumHealth * 0.7))
						|| Me.Health <= (Me.MaximumHealth * 0.3))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
					&& Utils.SleepCheck("Ghost"))
				{
					_ghost.UseAbility();
					Utils.Sleep(250, "Ghost");
				}
				if ( // Arcane Boots Item
					_arcane != null
					&& Me.Mana <= _w.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_arcane.Name)
					&& _arcane.CanBeCasted()
					&& Utils.SleepCheck("arcane")
					)
				{
					_arcane.UseAbility();
					Utils.Sleep(250, "arcane");
				} // Arcane Boots Item end
				if ( // Mjollnir
					_mjollnir != null
					&& _mjollnir.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& Me.Distance2D(E) <= 900
					)
				{
					_mjollnir.UseAbility(Me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if (
					// cheese
					_cheese != null
					&& _cheese.CanBeCasted()
					&& Me.Health <= (Me.MaximumHealth * 0.3)
					&& Me.Distance2D(E) <= 700
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
					&& Utils.SleepCheck("cheese")
					)
				{
					_cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // Medall
					_medall != null
					&& _medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
					&& Me.Distance2D(E) <= 700
					)
				{
					_medall.UseAbility(E);
					Utils.Sleep(250, "Medall");
				} // Medall Item end

				if ( // Q Skill
					_q != null
					&& _q.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Me.Distance2D(E) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					_q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					_w != null
					&& _w.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& Utils.SleepCheck("W")
					)
				{
					_w.UseAbility(E.Position);
					Utils.Sleep(200, "W");
				} // W Skill end

				if ( // sheep
					_sheep != null
					&& _sheep.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Me.Distance2D(E) <= 1400
					&& !stoneModif
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
					&& Utils.SleepCheck("sheep")
					)
				{
					_sheep.UseAbility(E);
					Utils.Sleep(250, "sheep");
				} // sheep Item end
				if ( // Abyssal Blade
					_abyssal != null
					&& _abyssal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsStunned()
					&& !E.IsHexed()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
					&& Utils.SleepCheck("abyssal")
					&& Me.Distance2D(E) <= 400
					)
				{
					_abyssal.UseAbility(E);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 900
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) &&
					Utils.SleepCheck("orchid"))
				{
					_orchid.UseAbility(E);
					Utils.Sleep(100, "orchid");
				}

				if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
					&& !E.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					_shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // ethereal
					_ethereal != null
					&& _ethereal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
					&& Utils.SleepCheck("ethereal")
					)
				{
					_ethereal.UseAbility(E);
					Utils.Sleep(200, "ethereal");
				} // ethereal Item end
				if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& Me.Distance2D(E) >= 450
					&& Me.Distance2D(E) <= 1150
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					_blink.UseAbility(E.Position);
					Utils.Sleep(250, "blink");
				}

				if ( // SoulRing Item 
					_soul != null
					&& _soul.CanBeCasted()
					&& Me.CanCast()
					&& Me.Health >= (Me.MaximumHealth * 0.5)
					&& Me.Mana <= _r.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
					)
				{
					_soul.UseAbility();
				} // SoulRing Item end
				if ( // Dagon
					Me.CanCast()
					&& _dagon != null
					&& (_ethereal == null
						|| (E.HasModifier("modifier_item_ethereal_blade_slow")
							|| _ethereal.Cooldown < 17))
					&& !E.IsLinkensProtected()
					&& _dagon.CanBeCasted()
					&& !E.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					_dagon.UseAbility(E);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // atos Blade
					_atos != null
					&& _atos.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
					&& Me.Distance2D(E) <= 2000
					&& Utils.SleepCheck("atos")
					)
				{
					_atos.UseAbility(E);

					Utils.Sleep(250, "atos");
				} // atos Item end
				if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
				{
					_urn.UseAbility(E);
					Utils.Sleep(240, "urn");
				}
				if ( // vail
					_vail != null
					&& _vail.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
					&& Me.Distance2D(E) <= 1500
					&& Utils.SleepCheck("vail")
					)
				{
					_vail.UseAbility(E.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end

				if (
					_stick != null
					&& _stick.CanBeCasted()
					&& _stick.CurrentCharges != 0
					&& Me.Distance2D(E) <= 700
					&& (Me.Health <= (Me.MaximumHealth * 0.5)
						|| Me.Mana <= (Me.MaximumMana * 0.5))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_stick.Name))
				{
					_stick.UseAbility();
					Utils.Sleep(200, "mana_items");
				}
				if ( // Satanic 
					_satanic != null &&
					Me.Health <= (Me.MaximumHealth * 0.3) &&
					_satanic.CanBeCasted() &&
					Me.Distance2D(E) <= Me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					_satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				}

			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("War's flames blaze again!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(new MenuItem("keyEscape", "Escape key").SetValue(new KeyBind('S', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("wKey", "W key").SetValue(new KeyBind('W', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"ember_spirit_activate_fire_remnant", true},
					{"ember_spirit_fire_remnant", true},
					{"ember_spirit_flame_guard", true},
					{"ember_spirit_searing_chains", true},
					{"ember_spirit_sleight_of_fist", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_ethereal_blade", true},
					{"item_blink", true},
					{"item_heavens_halberd", true},
					{"item_orchid", true},
					{ "item_bloodthorn", true},
					{"item_urn_of_shadows", true},
					{"item_veil_of_discord", true},
					{"item_abyssal_blade", true},
					{"item_shivas_guard", true},
					{"item_blade_mail", true},
					{"item_black_king_bar", true},
					{"item_medallion_of_courage", true},
					{"item_solar_crest", true}
				})));
			Menu.AddItem(
				new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_mask_of_madness", true},
					{"item_cyclone", true},
					{"item_force_staff", true},
					{"item_sheepstick", true},
					{"item_cheese", true},
					{"item_ghost", true},
					{"item_rod_of_atos", true},
					{"item_soul_ring", true},
					{"item_arcane_boots", true},
					{"item_magic_stick", true},
					{"item_magic_wand", true},
					{"item_mjollnir", true},
					{"item_satanic", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("oneult", "Use One ult").SetValue(false));
		}

		public void OnCloseEvent()
		{

		}
	}
}