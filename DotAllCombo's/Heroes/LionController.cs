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
	using Service.Debug;

	internal class LionController : Variables, IHeroController
	{
		private Ability _q, _w, _e, _r;
		private readonly Menu _skills = new Menu("Skills", "Skills");
		private readonly Menu _items = new Menu("Items", "Items");
		private readonly Menu _ult = new Menu("AutoUlt", "AutoUlt");


		private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost;

		private int[] _rDmg;

		public void Combo()
        {
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

			//spell
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

			_e = Me.Spellbook.SpellE;

			_r = Me.Spellbook.SpellR;

			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
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



			var modifEther = E.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");

			if (Active && Me.IsAlive && E.IsAlive)
			{
				var hexMod = E.Modifiers.Where(y => y.Name == "modifier_lion_voodoo" || y.Name == "modifier_sheepstick_debuff" || y.Name == "modifier_lion_impale").DefaultIfEmpty(null).FirstOrDefault();
				var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
				if (E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade && !E.IsLinkensProtected() )
				{

					if ( // atos Blade
						_atos != null
						&& _atos.CanBeCasted()
						&& Me.CanCast()
						&& !E.IsLinkensProtected()
						&& !E.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
						&& Me.Distance2D(E) <= 1500
						&& Utils.SleepCheck("atos")
						)
					{
						_atos.UseAbility(E);

						Utils.Sleep(250, "atos");
					} // atos Item end
					if (
						_w != null
						&& _w.CanBeCasted()
						&& !E.IsHexed()
						&& Me.CanCast()
						&& !E.IsStunned()
						&& Me.Distance2D(E) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
						&& Utils.SleepCheck("W"))
					{
						_w.UseAbility(E);
						Utils.Sleep(300, "W");
					}
					if (
						_blink != null
						&& Me.CanCast()
						&& _blink.CanBeCasted()
						&& Me.Distance2D(E) > 1000
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						_blink.UseAbility(E.Position);
						Utils.Sleep(250, "blink");
					}
					
					if(_w == null || !_w.CanBeCasted() ||
					!Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
					{
						if ( // orchid
						   _orchid != null
						   && _orchid.CanBeCasted()
						   && Me.CanCast()
						   && !E.IsLinkensProtected()
						   && !E.IsMagicImmune()
						   && Me.Distance2D(E) <= 1400
						   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
						   && !stoneModif
						   && Utils.SleepCheck("orchid")
						   )
						{
							_orchid.UseAbility(E);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (_orchid == null || !_orchid.CanBeCasted() ||
						    !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
						{
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
							if (!_vail.CanBeCasted() || _vail == null ||
							    !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name))
							{
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
								if (!_ethereal.CanBeCasted() || _ethereal == null ||
								    !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
								{
									if (
										_q != null
										&& _q.CanBeCasted()
										&& Me.CanCast()
										&& Me.Distance2D(E) < 1400
										&& !stoneModif
										&& (hexMod == null || hexMod.RemainingTime <= 0.1 + Game.Ping)
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
										&& !_w.CanBeCasted()
										&& Me.CanCast()
										&& Me.Position.Distance2D(E) < 1200
										&& !stoneModif
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
										&& Utils.SleepCheck("R"))
									{
										_r.UseAbility(E);
										Utils.Sleep(330, "R");
									}
									if ( // SoulRing Item 
										_soul != null
										&& _soul.CanBeCasted()
										&& Me.CanCast()
										&& Me.Health >= (Me.MaximumHealth*0.4)
										&& Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
										)
									{
										_soul.UseAbility();
									} // SoulRing Item end

									if ( // Arcane Boots Item
										_arcane != null
										&& _arcane.CanBeCasted()
										&& Me.CanCast()
										&& Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_arcane.Name)
										)
									{
										_arcane.UseAbility();
									} // Arcane Boots Item end

									if ( //Ghost
										_ghost != null
										&& _ghost.CanBeCasted()
										&& Me.CanCast()
										&& ((Me.Position.Distance2D(E) < 300
										     && Me.Health <= (Me.MaximumHealth*0.7))
										    || Me.Health <= (Me.MaximumHealth*0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										_ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if ( // Shiva Item
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

									if ( // sheep
										_sheep != null
										&& _sheep.CanBeCasted()
										&& Me.CanCast()
										&& !E.IsLinkensProtected()
										&& !E.IsMagicImmune()
										&& Me.Distance2D(E) <= 1400
										&& !stoneModif
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
										&& Utils.SleepCheck("sheep")
										)
									{
										_sheep.UseAbility(E);
										Utils.Sleep(250, "sheep");
									} // sheep Item end

									if ( // Dagon
										Me.CanCast()
										&& !_w.CanBeCasted()
										&& _dagon != null
										&& (_ethereal == null
										    || (modifEther
										        || _ethereal.Cooldown < 17))
										&& !E.IsLinkensProtected()
										&& _dagon.CanBeCasted()
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
										&& !E.IsMagicImmune()
										&& !stoneModif
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
										&& Me.Health <= (Me.MaximumHealth*0.3)
										&& Me.Distance2D(E) <= 700
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
										&& Utils.SleepCheck("cheese")
										)
									{
										_cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
									if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
									{
										Orbwalking.Orbwalk(E, 0, 1600, true, true);
									}
								}
							}
						}
					}
				}
			}
			if (Me != null && Me.IsAlive && E.Distance2D(Me) <= 1000)
			{
				A();
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("To destroy the darkness in itself!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


			_skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_voodoo", true},
				{"lion_impale", true},
				{"lion_finger_of_death", true}
			})));
			_items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon", true},
				{"item_orchid", true},
				{ "item_bloodthorn", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},
				{"item_rod_of_atos", true},
				{"item_sheepstick", true},
				{"item_arcane_boots", true},
				{"item_blink", true},
				{"item_soul_ring", true},
				{"item_ghost", true},
				{"item_cheese", true}
			})));
			_ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_finger_of_death", true}
			})));
			_items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_mana_drain", true}
			})));
			Menu.AddSubMenu(_skills);
			Menu.AddSubMenu(_items);
			Menu.AddSubMenu(_ult);
		}

		public void OnCloseEvent()
		{

		}

		public void A()
		{
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsIllusion).ToList();
			if (enemies.Count <= 0 || Me == null || !Me.IsAlive && !_r.CanBeCasted()) return;

			if (Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name))
			{

                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
                foreach (var v in enemies)
				{
					_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
					_atos = Me.FindItem("item_rod_of_atos");

					_rDmg = Me.AghanimState() ? new[] {725, 875, 1025} : new[] {600, 725, 850};
					

					var lens = Me.HasModifier("modifier_item_aether_lens");
					var damage = Math.Floor(_rDmg[_r.Level - 1] * (1 - v.MagicDamageResist));
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor(_rDmg[_r.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					if (v.NetworkName == "CDOTA_Unit_Hero_Tusk" &&
						v.Spellbook.SpellW.CooldownLength - 3 > v.Spellbook.SpellQ.Cooldown)
						damage = 0;
					if (lens) damage = damage * 1.08;
					var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
					if (rum) damage = damage * 0.5;
					var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
					if (mom) damage = damage * 1.3;

					var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            damage = damage * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            damage = damage * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            damage = damage * penitence[chen.Spellbook.Spell1.Level];
                    }


                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            damage = damage * souls[demon.Spellbook.Spell2.Level];
                    }
                    damage = damage * spellamplymult;

					if ( // vail
						_vail != null
						&& _vail.CanBeCasted()
						&& Me.Distance2D(v)<=_r.GetCastRange()+Me.HullRadius
						&& _r.CanBeCasted()
						&& v.Health <= damage * 1.25
						&& v.Health >= damage
						&& Me.CanCast()
						&& !v.HasModifier("modifier_item_veil_of_discord_debuff")
						&& !v.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
						&& Me.Distance2D(v) <= _w.GetCastRange()
						&& Utils.SleepCheck("vail")
						)
					{
						_vail.UseAbility(v.Position);
						Utils.Sleep(250, "vail");
					}
					int etherealdamage = (int)(((Me.TotalIntelligence * 2) + 75));
					if ( // vail
					  _ethereal != null
					  && _ethereal.CanBeCasted()
					  && _r != null
					  && _r.CanBeCasted()
					  && v.Health <= etherealdamage + (damage * 1.4)
					  && v.Health >= damage
					  && Me.CanCast()
					  && !v.HasModifier("modifier_item_ethereal_blade_slow")
					  && !v.IsMagicImmune()
					  && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
					  && Me.Distance2D(v) <= _ethereal.GetCastRange() + Me.HullRadius
					  && Utils.SleepCheck("ethereal")
					  )
					{
						_ethereal.UseAbility(v);
						Utils.Sleep(250, "ethereal");
					}


					if (_r != null && v != null && _r.CanBeCasted()
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& v.Health <= (damage - 40)
						&& Me.Distance2D(v) <= _r.GetCastRange() + 50
						&& Utils.SleepCheck(v.Handle.ToString())
						)
					{
						_r.UseAbility(v);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					if (_w != null && v != null && _w.CanBeCasted() && Me.Distance2D(v) <= _w.GetCastRange() + 30
						&& !v.IsLinkensProtected()
						&&
						(
							v.HasModifier("modifier_meepo_earthbind")
							|| v.HasModifier("modifier_pudge_dismember")
							|| v.HasModifier("modifier_naga_siren_ensnare")
							|| v.HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
							|| v.HasModifier("modifier_legion_commander_duel")
							|| v.HasModifier("modifier_kunkka_torrent")
							|| v.HasModifier("modifier_ice_blast")
							|| v.HasModifier("modifier_enigma_black_hole_pull")
							|| v.HasModifier("modifier_ember_spirit_searing_chains")
							|| v.HasModifier("modifier_dark_troll_warlord_ensnare")
							|| v.HasModifier("modifier_crystal_maiden_frostbite")
							|| v.HasModifier("modifier_axe_berserkers_call")
							|| v.HasModifier("modifier_bane_fiends_grip")
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur &&
							v.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur &&
							v.FindSpell("magnataur_skewer").IsInAbilityPhase
							|| v.FindItem("item_blink").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_QueenOfPain &&
							v.FindSpell("queenofpain_blink").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage && v.FindSpell("antimage_blink").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage &&
							v.FindSpell("antimage_mana_void").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_DoomBringer &&
							v.FindSpell("doom_bringer_doom").IsInAbilityPhase
							|| v.HasModifier("modifier_rubick_telekinesis")
							|| v.HasModifier("modifier_item_blink_dagger")
							|| v.HasModifier("modifier_storm_spirit_electric_vortex_pull")
							|| v.HasModifier("modifier_winter_wyvern_cold_embrace")
							|| v.HasModifier("modifier_winter_wyvern_winters_curse")
							|| v.HasModifier("modifier_shadow_shaman_shackles")
							||
							v.HasModifier("modifier_faceless_void_chronosphere_freeze") &&
							v.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_WitchDoctor &&
							v.FindSpell("witch_doctor_death_ward").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_Rattletrap &&
							v.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
							||
							v.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter &&
							v.FindSpell("tidehunter_ravage").IsInAbilityPhase
							&& !v.IsMagicImmune()
							)
						&& !v.HasModifier("modifier_medusa_stone_gaze_stone")
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						_w.UseAbility(v);
						Utils.Sleep(250, v.Handle.ToString());
					}
					if (v.IsLinkensProtected() &&
						(Me.IsVisibleToEnemies || Active))
					{
						if (_e != null && _e.CanBeCasted() && Me.Distance2D(v) < _e.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_e.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							_e.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
					}
				}
			}
		}
	}
}