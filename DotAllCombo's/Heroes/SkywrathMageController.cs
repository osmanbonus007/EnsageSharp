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


	internal class SkywrathMageController : Variables, IHeroController
	{
		private readonly Menu _skills = new Menu("Skills", "Skills");
		private readonly Menu _items = new Menu("Items", "Items");
		private readonly Menu _ult = new Menu("AutoAbility", "AutoAbility");
		private readonly Menu _healh = new Menu("Healh", "Max Enemy Healh % to Ult");


		private Ability _q, _w, _e, _r;

		private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost, _force, _cyclone;
		public void OnLoadEvent()
		{

			AssemblyExtensions.InitAssembly("VickTheRock", "1.0");

			Print.LogMessage.Success("I am sworn to turn the tide where ere I can.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("keyQ", "Spam Q key").SetValue(new KeyBind('Q', KeyBindType.Press)));


			_skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_arcane_bolt", true},
				{"skywrath_mage_concussive_shot", true},
				{"skywrath_mage_ancient_seal", true},
				{"skywrath_mage_mystic_flare", true}
			})));
			_items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon",true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},
				{"item_rod_of_atos", true},
				{"item_sheepstick", true},
				{"item_arcane_boots", true},
				{"item_shivas_guard",true},
				{"item_blink", true},
				{"item_soul_ring", true},
				{"item_ghost", true},
				{"item_cheese", true}
			})));
			_ult.AddItem(new MenuItem("autoUlt", "AutoAbility").SetValue(true));
			_ult.AddItem(new MenuItem("AutoAbility", "AutoAbility").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_concussive_shot", true},
				{"skywrath_mage_ancient_seal", true},
				{"skywrath_mage_mystic_flare", true},
				{"item_rod_of_atos", true},
				{"item_cyclone", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},

			})));
			_items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_force_staff", true},
				{"item_cyclone", true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
				{"item_rod_of_atos", true},
				{"item_dagon", true}
			})));
			_healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(false));
			Menu.AddSubMenu(_skills);
			Menu.AddSubMenu(_items);
			Menu.AddSubMenu(_ult);
			Menu.AddSubMenu(_healh);
		} // OnLoadEvent

		public void OnCloseEvent()
		{
			E = null;
		}

		/* Доп. функции скрипта
		-----------------------------------------------------------------------------*/


		public void Combo()
		{
            E = Toolset.ClosestToMouse(Me);
            if (E.HasModifier("modifier_abaddon_borrowed_time")
			|| E.HasModifier("modifier_item_blade_mail_reflect")
			|| E.IsMagicImmune())
			{
				var enemies = ObjectManager.GetEntities<Hero>()
						.Where(x => x.IsAlive && x.Team != Me.Team && !x.IsIllusion && !x.IsMagicImmune()
						&& (!x.HasModifier("modifier_abaddon_borrowed_time")
						|| !x.HasModifier("modifier_item_blade_mail_reflect"))
						&& x.Distance2D(E) > 200).ToList();
				E = GetClosestToTarget(enemies, E);
				if (Utils.SleepCheck("spam"))
				{
					Utils.Sleep(5000, "spam");
				}
			}
			if (E == null) return;
			//spell
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

			_e = Me.Spellbook.SpellE;

			_r = Me.Spellbook.SpellR;
			// Item
			_ethereal = Me.FindItem("item_ethereal_blade");

			_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");

			_vail = Me.FindItem("item_veil_of_discord");

			_cheese = Me.FindItem("item_cheese");

			_ghost = Me.FindItem("item_ghost");

			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");

			_atos = Me.FindItem("item_rod_of_atos");

			_soul = Me.FindItem("item_soul_ring");

			_arcane = Me.FindItem("item_arcane_boots");

			_blink = Me.FindItem("item_blink");

			_shiva = Me.FindItem("item_shivas_guard");

			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);


			Push = Game.IsKeyDown(Menu.Item("keyQ").GetValue<KeyBind>().Key);

			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
			A();
			if (Push && _q != null && _q.CanBeCasted())
			{
				if (
					_q != null
					&& _q.CanBeCasted()
					&& (E.IsLinkensProtected()
					|| !E.IsLinkensProtected())
					&& Me.CanCast()
					&& Me.Distance2D(E) < _q.GetCastRange() + Me.HullRadius + 24
					&& Utils.SleepCheck("Q")
					)
				{
					_q.UseAbility(E);
					Utils.Sleep(200, "Q");
				}
			}
			if (Active && Me.IsAlive && E.IsAlive && Utils.SleepCheck("activated"))
			{
				if (stoneModif) return;
				//var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
				if (E.IsVisible && Me.Distance2D(E) <= 2300)
				{
					var distance = Me.IsVisibleToEnemies ? 1400 : _e.GetCastRange() + Me.HullRadius;
					if (
						_q != null
						&& _q.CanBeCasted()
						&& Me.CanCast()
						&& E.IsLinkensProtected()
						&& !E.IsMagicImmune()
						&& Me.Distance2D(E) < _q.GetCastRange() + Me.HullRadius
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility(E);
						Utils.Sleep(200, "Q");
					}

					if (
						_e != null
						&& _e.CanBeCasted()
						&& Me.CanCast()
						&& !E.IsLinkensProtected()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
						&& Me.Position.Distance2D(E) < _e.GetCastRange() + Me.HullRadius + 500
						&& Utils.SleepCheck("E"))
					{
						_e.UseAbility(E);
						Utils.Sleep(200, "E");
					}
					if (
					  _q != null
					  && _q.CanBeCasted()
					  && Me.CanCast()
					  && !E.IsMagicImmune()
					  && Me.Distance2D(E) < distance
					  && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					  && Utils.SleepCheck("Q")
					  )
					{
						_q.UseAbility(E);
						Utils.Sleep(200, "Q");
					}
					if ( // sheep
						_sheep != null
						&& _sheep.CanBeCasted()
						&& Me.CanCast()
						&& !E.IsLinkensProtected()
						&& !E.IsMagicImmune()
						&& Me.Distance2D(E) <= 1400
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
						&& Utils.SleepCheck("sheep")
						)
					{
						_sheep.UseAbility(E);
						Utils.Sleep(250, "sheep");
					} // sheep Item end
					if (_e == null || !_e.CanBeCasted() || Me.IsSilenced() || Me.Position.Distance2D(E) > _e.GetCastRange() + Me.HullRadius || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name))
					{
						if (
						   _q != null
						   && _q.CanBeCasted()
						   && Me.CanCast()
						   && !E.IsMagicImmune()
						   && Me.Distance2D(E) < 1400
						   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						   && Utils.SleepCheck("Q")
					   )
						{
							_q.UseAbility(E);
							Utils.Sleep(200, "Q");
						}
						if ( // atos Blade
							_atos != null
							&& _atos.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsLinkensProtected()
							&& !E.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
							&& Me.Distance2D(E) <= distance
							&& Utils.SleepCheck("atos")
							)
						{
							_atos.UseAbility(E);

							Utils.Sleep(250 + Game.Ping, "atos");
						} // atos Item end
						if (
							_w != null
							&& E.IsVisible
							&& _w.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsMagicImmune()
							&& Me.Distance2D(E) < distance
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							&& Utils.SleepCheck("W"))
						{
							_w.UseAbility();
							Utils.Sleep(300, "W");
						}
						float angle = Me.FindAngleBetween(E.Position, true);
						Vector3 pos = new Vector3((float)(E.Position.X - 500 * Math.Cos(angle)), (float)(E.Position.Y - 500 * Math.Sin(angle)), 0);
						if (
							_blink != null
							&& _q.CanBeCasted()
							&& Me.CanCast()
							&& _blink.CanBeCasted()
							&& Me.Distance2D(E) >= 490
							&& Me.Distance2D(pos) <= 1180
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
							&& Utils.SleepCheck("blink")
							)
						{
							_blink.UseAbility(pos);
							Utils.Sleep(250, "blink");
						}
						if ( // orchid
							_orchid != null
							&& _orchid.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsLinkensProtected()
							&& !E.IsMagicImmune()
							&& Me.Distance2D(E) <= distance
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
							&& Utils.SleepCheck("orchid")
							)
						{
							_orchid.UseAbility(E);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!_orchid.CanBeCasted() || _orchid == null || Me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
						{
							if ( // vail
								_vail != null
							   && _vail.CanBeCasted()
							   && Me.CanCast()
							   && !E.IsMagicImmune()
							   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
							   && Me.Distance2D(E) <= distance
							   && Utils.SleepCheck("vail")
							   )
							{
								_vail.UseAbility(E.Position);
								Utils.Sleep(250, "vail");
							} // orchid Item end
							if (_vail == null || !_vail.CanBeCasted() || Me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name))
							{
								if (// ethereal
									   _ethereal != null
									   && _ethereal.CanBeCasted()
									   && Me.CanCast()
									   && !E.IsLinkensProtected()
									   && !E.IsMagicImmune()
									   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
									   && Utils.SleepCheck("ethereal")
									  )
								{
									_ethereal.UseAbility(E);
									Utils.Sleep(200, "ethereal");
								} // ethereal Item end
								if (_ethereal == null || !_ethereal.CanBeCasted() || Me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
								{
									if (
										 _q != null
										&& _q.CanBeCasted()
										&& Me.CanCast()
										&& Me.Distance2D(E) < 1400
										 && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
										&& Utils.SleepCheck("Q")
										)
									{
										_q.UseAbility(E);
										Utils.Sleep(200, "Q");
									}


									if (
									   _r != null
									   && _r.CanBeCasted()
									   && Me.CanCast()
									   && !E.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && E.MovementSpeed <= 220
									   && Me.Position.Distance2D(E) < 1200
									   && E.Health >= (E.MaximumHealth / 100 * Menu.Item("Healh").GetValue<Slider>().Value)
										  && !Me.HasModifier("modifier_pugna_nether_ward_aura")
										  && !E.HasModifier("modifier_item_blade_mail_reflect")
									   && !E.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && !E.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
									   && !E.HasModifier("modifier_puck_phase_shift")
									   && !E.HasModifier("modifier_eul_cyclone")
										  && !E.HasModifier("modifier_dazzle_shallow_grave")
									   && !E.HasModifier("modifier_brewmaster_storm_cyclone")
									   && !E.HasModifier("modifier_spirit_breaker_charge_of_darkness")
									   && !E.HasModifier("modifier_shadow_demon_disruption")
									   && !E.HasModifier("modifier_tusk_snowball_movement")
									   && !E.IsMagicImmune()
									   && (E.FindSpell("abaddon_borrowed_time").Cooldown > 0 && !E.HasModifier("modifier_abaddon_borrowed_time_damage_redirect"))
									   && (E.FindItem("item_cyclone") != null && E.FindItem("item_cyclone").Cooldown > 0
									   || (E.FindItem("item_cyclone") == null || E.IsStunned() || E.IsHexed() || E.IsRooted()))
									   && (E.FindItem("item_force_staff") != null && E.FindItem("item_force_staff").Cooldown > 0
									   || (E.FindItem("item_force_staff") == null || E.IsStunned() || E.IsHexed() || E.IsRooted()))

									   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
									   && Utils.SleepCheck("R"))
									{
										_r.UseAbility(Prediction.InFront(E, 100));
										Utils.Sleep(330, "R");
									}

									if (// SoulRing Item 
										_soul != null
										&& _soul.CanBeCasted()
										&& Me.CanCast()
										&& Me.Health >= (Me.MaximumHealth * 0.5)
										&& Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
										)
									{
										_soul.UseAbility();
									} // SoulRing Item end

									if (// Arcane Boots Item
										_arcane != null
										&& _arcane.CanBeCasted()
										&& Me.CanCast()
										&& Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_arcane.Name)
										)
									{
										_arcane.UseAbility();
									} // Arcane Boots Item end

									if (//Ghost
										_ghost != null
										&& _ghost.CanBeCasted()
										&& Me.CanCast()
										&& ((Me.Position.Distance2D(E) < 300
										&& Me.Health <= (Me.MaximumHealth * 0.7))
										|| Me.Health <= (Me.MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										_ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if (// Shiva Item
										_shiva != null
										&& _shiva.CanBeCasted()
										&& Me.CanCast()
										&& !E.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
										&& Me.Distance2D(E) <= 600
										)

									{
										_shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end

									if (// Dagon
										Me.CanCast()
										&& _dagon != null
										&& (_ethereal == null
										|| (E.HasModifier("modifier_item_ethereal_blade_slow")
										|| _ethereal.Cooldown < 17))
										&& !E.IsLinkensProtected()
										&& _dagon.CanBeCasted()
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
										&& !E.IsMagicImmune()
										&& Utils.SleepCheck("dagon")
									   )
									{
										_dagon.UseAbility(E);
										Utils.Sleep(200, "dagon");
									} // Dagon Item end

									if (
										 // cheese
										 _cheese != null
										 && _cheese.CanBeCasted()
										 && Me.Health <= (Me.MaximumHealth * 0.3)
										 && Me.Distance2D(E) <= 700
										 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
										 && Utils.SleepCheck("cheese")
									 )
									{
										_cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
								}
							}
						}
						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
				}
				Utils.Sleep(100, "activated");
			}
		} // Combo


		private Hero GetClosestToTarget(List<Hero> units, Hero z)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(z) > v.Distance2D(z)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		public void A()
		{
			Me = ObjectManager.LocalHero;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.IsValid && x.Team != Me.Team && !x.IsIllusion).ToList();

			var ecount = v.Count();
			if (ecount == 0) return;
			if (Menu.Item("autoUlt").GetValue<bool>() && Me.IsAlive)
			{
				if (!Me.IsAlive) return;
				_force = Me.FindItem("item_force_staff");
				_cyclone = Me.FindItem("item_cyclone");
				_e = Me.Spellbook.SpellE;
				for (int i = 0; i < ecount; ++i)
				{
					var reflect = v[i].HasModifier("modifier_item_blade_mail_reflect");
					 
					if (Me.HasModifier("modifier_spirit_breaker_charge_of_darkness_vision"))
					{

						if (v[i].ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker)
						{
							float angle = Me.FindAngleBetween(v[i].Position, true);
							Vector3 pos = new Vector3((float)(Me.Position.X + 100 * Math.Cos(angle)),
								(float)(Me.Position.Y + 100 * Math.Sin(angle)), 0);

							if (_w != null && _w.CanBeCasted() && Me.Distance2D(v[i]) <= 900 + Game.Ping
								&& !v[i].IsMagicImmune()
								&& Utils.SleepCheck(v[i].Handle.ToString())
								&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							   )
							{ 
								_w.UseAbility();
								Utils.Sleep(150, v[i].Handle.ToString());
							}

						if (_atos != null && _r != null && _r.CanBeCasted() && _atos.CanBeCasted()
							   && Me.Distance2D(v[i]) <= 900 + Game.Ping
							   && !v[i].IsMagicImmune()
								&& Utils.SleepCheck(v[i].Handle.ToString())
							   && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
							   )
							{ 
								_atos.UseAbility(v[i]);
								Utils.Sleep(150, v[i].Handle.ToString());
							}
							if (_r != null && _r.CanBeCasted() && Me.Distance2D(v[i]) <= 700 + Game.Ping
							    && !v[i].HasModifier("modifier_item_blade_mail_reflect")
							    && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
							    && !v[i].HasModifier("modifier_dazzle_shallow_grave")
							    && !v[i].IsMagicImmune()
							    && Utils.SleepCheck(v[i].Handle.ToString())
							    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_r.Name)
								)
							{
								_r.UseAbility(pos);
								Utils.Sleep(150, v[i].Handle.ToString());
							}
							
							if (_cyclone != null && !_r.CanBeCasted()
							    && _cyclone.CanBeCasted()
							    && Me.Distance2D(v[i]) <= 500 + Game.Ping
							    && Utils.SleepCheck(v[i].Handle.ToString())
							    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name)
								)
							{
								_cyclone.UseAbility(Me);
								Utils.Sleep(150, v[i].Handle.ToString());
							}

						}

					}
					if (_cyclone != null && reflect && _cyclone.CanBeCasted() &&
						v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect") &&
						Me.Distance2D(v[i]) < _cyclone.GetCastRange()
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						_cyclone.UseAbility(Me);
					if (_r != null && _r.CanBeCasted() && Me.Distance2D(v[i]) <= _r.GetCastRange() + 100
						&& !Me.HasModifier("modifier_pugna_nether_ward_aura")
						&&
						(v[i].HasModifier("modifier_meepo_earthbind")
						 || v[i].HasModifier("modifier_pudge_dismember")
						 || v[i].HasModifier("modifier_naga_siren_ensnare")
						 || v[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
						 || (v[i].HasModifier("modifier_legion_commander_duel")
						  && !v[i].AghanimState())
						 || v[i].HasModifier("modifier_kunkka_torrent")
						 || v[i].HasModifier("modifier_ice_blast")
						 || v[i].HasModifier("modifier_crystal_maiden_crystal_nova")
						 || v[i].HasModifier("modifier_enigma_black_hole_pull")
						 || v[i].HasModifier("modifier_ember_spirit_searing_chains")
						 || v[i].HasModifier("modifier_dark_troll_warlord_ensnare")
						 || v[i].HasModifier("modifier_crystal_maiden_frostbite")
						 || (v[i].FindSpell("rattletrap_power_cogs") != null &&
						 v[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
						 || v[i].HasModifier("modifier_axe_berserkers_call")
						 || v[i].HasModifier("modifier_bane_fiends_grip")
						 || (v[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
						 && v[i].ClassId != ClassId.CDOTA_Unit_Hero_FacelessVoid)
						 || v[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
						 || (v[i].FindSpell("witch_doctor_death_ward") != null
						 && v[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
						 || (v[i].FindSpell("crystal_maiden_crystal_nova") != null
						 && v[i].FindSpell("crystal_maiden_crystal_nova").IsInAbilityPhase)
						 || v[i].IsStunned())
						 && (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
						 || (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
						 || (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && (!v[i].HasModifier("modifier_medusa_stone_gaze_stone")
						 && !v[i].HasModifier("modifier_faceless_void_time_walk")
						 && !v[i].HasModifier("modifier_item_monkey_king_bar")
						 && !v[i].HasModifier("modifier_rattletrap_battery_assault")
						 && !v[i].HasModifier("modifier_item_blade_mail_reflect")
						 && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						 && !v[i].HasModifier("modifier_pudge_meat_hook")
						 && !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
						 && !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						 && !v[i].HasModifier("modifier_puck_phase_shift")
						 && !v[i].HasModifier("modifier_eul_cyclone")
						 && !v[i].HasModifier("modifier_invoker_tornado")
						 && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						 && !v[i].HasModifier("modifier_dazzle_shallow_grave")
						 && !v[i].HasModifier("modifier_mirana_leap")
						 && !v[i].HasModifier("modifier_abaddon_borrowed_time")
						 && !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
						 && !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
						 && !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
						 && !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
						 && !v[i].HasModifier("modifier_shadow_demon_disruption")
						 && !v[i].HasModifier("modifier_tusk_snowball_movement")
						 && !v[i].HasModifier("modifier_invoker_tornado")
						 && ((v[i].FindSpell("abaddon_borrowed_time") != null
							  && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
							 || v[i].FindSpell("abaddon_borrowed_time") == null)
						 && v[i].Health >= (v[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
						 && !v[i].IsMagicImmune())
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{ 
						_r.UseAbility(Prediction.InFront(v[i], 70));
						Utils.Sleep(250, v[i].Handle.ToString());
					}

				if (_atos != null && _r != null && _r.CanBeCasted() && _atos.CanBeCasted()
					    && !v[i].IsLinkensProtected()
					    && Me.Distance2D(v[i]) <= 1200
					    && v[i].MagicDamageResist <= 0.1
					    && !v[i].IsMagicImmune()
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
					    && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{
						_atos.UseAbility(v[i]);
						Utils.Sleep(250, v[i].Handle.ToString());
					}

					if (_r != null && _r.CanBeCasted() && Me.Distance2D(v[i]) <= _r.GetCastRange() + 100
					    && !Me.HasModifier("modifier_pugna_nether_ward_aura")
					    && v[i].MovementSpeed <= 240
					    && v[i].MagicDamageResist <= 0.1
					    && !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
					    && !v[i].HasModifier("modifier_item_blade_mail_reflect")
					    && !v[i].HasModifier("modifier_sniper_headshot")
					    && !v[i].HasModifier("modifier_leshrac_lightning_storm_slow")
					    && !v[i].HasModifier("modifier_razor_unstablecurrent_slow")
					    && !v[i].HasModifier("modifier_pudge_meat_hook")
					    && !v[i].HasModifier("modifier_tusk_snowball_movement")
					    && !v[i].HasModifier("modifier_faceless_void_time_walk")
					    && !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
					    && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
					    && !v[i].HasModifier("modifier_puck_phase_shift")
					    && !v[i].HasModifier("modifier_abaddon_borrowed_time")
					    && !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
					    && !v[i].HasModifier("modifier_eul_cyclone")
					    && !v[i].HasModifier("modifier_dazzle_shallow_grave")
					    && !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
					    && !v[i].HasModifier("modifier_mirana_leap")
					    && !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
					    && !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
					    && !v[i].HasModifier("modifier_shadow_demon_disruption")
					    && ((v[i].FindSpell("abaddon_borrowed_time") != null
					         && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
					        || v[i].FindSpell("abaddon_borrowed_time") == null)
					    && (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
					        || (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
					        || (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && v[i].Health >= (v[i].MaximumHealth/100*(Menu.Item("Healh").GetValue<Slider>().Value))
					    && !v[i].IsMagicImmune()
					    && Utils.SleepCheck(v[i].Handle.ToString())
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						)
					{
						_r.UseAbility(Prediction.InFront(v[i], 90));
						Utils.Sleep(250, v[i].Handle.ToString());
					}
						

					if (_r != null && _r.CanBeCasted() && Me.Distance2D(v[i]) <= _r.GetCastRange() + 100
					    && !Me.HasModifier("modifier_pugna_nether_ward_aura")
					    && v[i].MovementSpeed <= 240
					    && (!Active || !_e.CanBeCasted())
					    && v[i].Health >= (v[i].MaximumHealth/100*(Menu.Item("Healh").GetValue<Slider>().Value))
					    && !v[i].HasModifier("modifier_item_blade_mail_reflect")
					    && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
					    && !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
					    && !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
					    && !v[i].HasModifier("modifier_puck_phase_shift")
					    && !v[i].HasModifier("modifier_eul_cyclone")
					    && !v[i].HasModifier("modifier_invoker_tornado")
					    && !v[i].HasModifier("modifier_dazzle_shallow_grave")
					    && !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
					    && !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
					    && !v[i].HasModifier("modifier_shadow_demon_disruption")
					    && !v[i].HasModifier("modifier_faceless_void_time_walk")
					    && !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
					    && !v[i].HasModifier("modifier_huskar_life_break_charge")
					    && !v[i].HasModifier("modifier_mirana_leap")
					    && !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
					    && !v[i].HasModifier("modifier_tusk_snowball_movement")
					    && !v[i].IsMagicImmune()
					    && !v[i].HasModifier("modifier_abaddon_borrowed_time")
					    && ((v[i].FindSpell("abaddon_borrowed_time") != null
					         && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
					        || v[i].FindSpell("abaddon_borrowed_time") == null)
					    && (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
					        || (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
					        || (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_r.Name)
					    && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{
						_r.UseAbility(Prediction.InFront(v[i], 90));
						Utils.Sleep(250, v[i].Handle.ToString());
					}

					if (_w != null && _w.CanBeCasted() && Me.Distance2D(v[i]) <= 1400
					    && ((v[i].MovementSpeed <= 255
					         && !v[i].HasModifier("modifier_phantom_assassin_stiflingdagger"))
					        || (v[i].Distance2D(Me) <= Me.HullRadius + 24
					            && v[i].NetworkActivity == NetworkActivity.Attack)
					        || v[i].MagicDamageResist <= 0.07)
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					    && Utils.SleepCheck(v[i].Handle.ToString())
					    && !v[i].IsMagicImmune()
						)
					{
						_w.UseAbility();
						Utils.Sleep(250, v[i].Handle.ToString());
					}

					if (_vail != null && v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& _vail.CanBeCasted()
						&& !v[i].IsMagicImmune()
						&& Me.Distance2D(v[i]) <= 1200
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						_vail.UseAbility(v[i].Position);
					if (_e != null && !_e.CanBeCasted() && !v[i].IsStunned() && !v[i].IsHexed() && !v[i].IsRooted() && (_orchid != null && _orchid.CanBeCasted() || _sheep != null && _sheep.CanBeCasted()))
						_e = _orchid ?? _sheep;
					if (_e != null && v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
					    && _e.CanBeCasted()
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_e.Name)
					    && (v[i].FindItem("item_manta") != null && v[i].FindItem("item_manta").Cooldown > 0
					        || (v[i].FindItem("item_manta") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && Me.Distance2D(v[i]) <= 900
					    && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{
						_e.UseAbility(v[i]);
						Utils.Sleep(250, v[i].Handle.ToString());
					}

				if (_ethereal != null &&
					    v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
					    && !v[i].HasModifier("modifier_legion_commander_duel")
					    && _ethereal.CanBeCasted()
					    && _e.CanBeCasted()
					    && Me.Distance2D(v[i]) <= _ethereal.GetCastRange() + Me.HullRadius
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
					    && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{
						_ethereal.UseAbility(v[i]);
						Utils.Sleep(250, v[i].Handle.ToString());
					}

					if (_e != null && _e.CanBeCasted() && Me.Distance2D(v[i]) <= _e.GetCastRange()
					    && !v[i].IsLinkensProtected()
					    &&
					    (v[i].HasModifier("modifier_meepo_earthbind")
					     || v[i].HasModifier("modifier_pudge_dismember")
					     || v[i].HasModifier("modifier_naga_siren_ensnare")
					     || v[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
					     || v[i].HasModifier("modifier_legion_commander_duel")
					     || v[i].HasModifier("modifier_kunkka_torrent")
					     || v[i].HasModifier("modifier_ice_blast")
					     || v[i].HasModifier("modifier_enigma_black_hole_pull")
					     || v[i].HasModifier("modifier_ember_spirit_searing_chains")
					     || v[i].HasModifier("modifier_dark_troll_warlord_ensnare")
					     || v[i].HasModifier("modifier_crystal_maiden_crystal_nova")
					     || v[i].HasModifier("modifier_axe_berserkers_call")
					     || v[i].HasModifier("modifier_bane_fiends_grip")
					     || v[i].HasModifier("modifier_rubick_telekinesis")
					     || v[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
					     || v[i].HasModifier("modifier_winter_wyvern_cold_embrace")
					     || v[i].HasModifier("modifier_shadow_shaman_shackles")
					     || (v[i].FindSpell("magnataur_reverse_polarity") != null
					         && v[i].FindSpell("magnataur_reverse_polarity").IsInAbilityPhase)
					     || (v[i].FindItem("item_blink") != null && v[i].FindItem("item_blink").Cooldown > 11)
					     || (v[i].FindSpell("queenofpain_blink") != null
					         && v[i].FindSpell("queenofpain_blink").IsInAbilityPhase)
					     || (v[i].FindSpell("antimage_blink") != null && v[i].FindSpell("antimage_blink").IsInAbilityPhase)
					     || (v[i].FindSpell("antimage_mana_void") != null
					         && v[i].FindSpell("antimage_mana_void").IsInAbilityPhase)
					     || (v[i].FindSpell("legion_commander_duel") != null
					         && v[i].FindSpell("legion_commander_duel").Cooldown <= 0)
					     || (v[i].FindSpell("doom_bringer_doom") != null
					         && v[i].FindSpell("doom_bringer_doom").IsInAbilityPhase)
					     || (v[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
					         && v[i].ClassId != ClassId.CDOTA_Unit_Hero_FacelessVoid)
					     || (v[i].FindSpell("witch_doctor_death_ward") != null &&
					         v[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
					     || (v[i].FindSpell("rattletrap_power_cogs") != null &&
					         v[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
					     || (v[i].FindSpell("tidehunter_ravage") != null &&
					         v[i].FindSpell("tidehunter_ravage").IsInAbilityPhase)
					     || (v[i].FindSpell("axe_berserkers_call") != null &&
					         v[i].FindSpell("axe_berserkers_call").IsInAbilityPhase)
					     || (v[i].FindSpell("brewmaster_primal_split") != null &&
					         v[i].FindSpell("brewmaster_primal_split").IsInAbilityPhase)
					     || (v[i].FindSpell("omniknight_guardian_angel") != null &&
					         v[i].FindSpell("omniknight_guardian_angel").IsInAbilityPhase)
					     || (v[i].FindSpell("queenofpain_sonic_wave") != null &&
					         v[i].FindSpell("queenofpain_sonic_wave").IsInAbilityPhase)
					     || (v[i].FindSpell("sandking_epicenter") != null &&
					         v[i].FindSpell("sandking_epicenter").IsInAbilityPhase)
					     || (v[i].FindSpell("slardar_slithereen_crush") != null &&
					         v[i].FindSpell("slardar_slithereen_crush").IsInAbilityPhase)
					     || (v[i].FindSpell("tiny_toss") != null && v[i].FindSpell("tiny_toss").IsInAbilityPhase)
					     || (v[i].FindSpell("tusk_walrus_punch") != null &&
					         v[i].FindSpell("tusk_walrus_punch").IsInAbilityPhase)
					     || (v[i].FindSpell("faceless_void_time_walk") != null &&
					         v[i].FindSpell("faceless_void_time_walk").IsInAbilityPhase)
					     || (v[i].FindSpell("faceless_void_chronosphere") != null
					         && v[i].FindSpell("faceless_void_chronosphere").IsInAbilityPhase)
					     || (v[i].FindSpell("disruptor_static_storm") != null
					         && v[i].FindSpell("disruptor_static_storm").Cooldown <= 0)
					     || (v[i].FindSpell("lion_finger_of_death") != null
					         && v[i].FindSpell("lion_finger_of_death").Cooldown <= 0)
					     || (v[i].FindSpell("luna_eclipse") != null && v[i].FindSpell("luna_eclipse").Cooldown <= 0)
					     || (v[i].FindSpell("lina_laguna_blade") != null && v[i].FindSpell("lina_laguna_blade").Cooldown <= 0)
					     || (v[i].FindSpell("doom_bringer_doom") != null && v[i].FindSpell("doom_bringer_doom").Cooldown <= 0)
					     || (v[i].FindSpell("life_stealer_rage") != null && v[i].FindSpell("life_stealer_rage").Cooldown <= 0
					         && Me.Level >= 7)
						    )
					    && (v[i].FindItem("item_manta") != null && v[i].FindItem("item_manta").Cooldown > 0
					        || (v[i].FindItem("item_manta") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
					    && !v[i].IsMagicImmune()
					    && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(_e.Name)
					    && !v[i].HasModifier("modifier_medusa_stone_gaze_stone")
					    && Utils.SleepCheck(v[i].Handle.ToString())
						)
					{
						_e.UseAbility(v[i]);
						Utils.Sleep(250, v[i].Handle.ToString());
					}
					
					if (v[i].IsLinkensProtected() && (Me.IsVisibleToEnemies || Active) && Utils.SleepCheck(v[i].Handle.ToString()))
					{
						if (_force != null && _force.CanBeCasted() && Me.Distance2D(v[i]) < _force.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_force.Name))
							_force.UseAbility(v[i]);
						else if (_cyclone != null && _cyclone.CanBeCasted() && Me.Distance2D(v[i]) < _cyclone.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name))
							_cyclone.UseAbility(v[i]);
						else if (_atos != null && _atos.CanBeCasted() && Me.Distance2D(v[i]) < _atos.GetCastRange() - 400 &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_atos.Name))
							_atos.UseAbility(v[i]);
						else if (_dagon != null && _dagon.CanBeCasted() && Me.Distance2D(v[i]) < _dagon.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							_dagon.UseAbility(v[i]);
						else if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(v[i]) < _orchid.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
							_orchid.UseAbility(v[i]);
						Utils.Sleep(350, v[i].Handle.ToString());
					}
				}
			}
		} // SkywrathMage class
	}
}