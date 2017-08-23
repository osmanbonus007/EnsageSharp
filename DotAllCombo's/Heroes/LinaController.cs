using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
	using SharpDX;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using Service.Debug;

	internal class LinaController : Variables, IHeroController
	{
		private Ability _q, _w, _r;
		private readonly Menu _skills = new Menu("Skills", "Skills");
		private readonly Menu _items = new Menu("Items", "Items");
		private readonly Menu _ult = new Menu("AutoUlt", "AutoUlt");


		private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost, _force, _cyclone;

		private int[] _rDmg;

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.2");

			Print.LogMessage.Success("One little spark and before you know it, the whole world is burning.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


			_skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lina_dragon_slave", true},
				{"lina_light_strike_array", true},
				{"lina_laguna_blade", true}
			})));
			_items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_cyclone", true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},
				{"item_rod_of_atos", true},
				{"item_sheepstick", true},
				{"item_dagon", true},
				{"item_arcane_boots", true},
				{"item_blink", true},
				{"item_shivas_guard", true},
				{"item_soul_ring", true},
				{"item_ghost", true},
				{"item_cheese", true}
			})));
			_ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lina_laguna_blade", true}
			})));
			_items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_force_staff", true},
				{"item_cyclone", true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
				{"item_rod_of_atos", true},
			})));
			Menu.AddSubMenu(_skills);
			Menu.AddSubMenu(_items);
			Menu.AddSubMenu(_ult);
		}
		public void Combo()
        {
            E = Toolset.ClosestToMouse(Me);
            if (E == null)
				return;
			if (!Menu.Item("enabled").IsActive())
				return;
			//spell
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

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

			_force = Me.FindItem("item_force_staff");

			_cyclone = Me.FindItem("item_cyclone");

			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");

			_atos = Me.FindItem("item_rod_of_atos");
			//Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
			var modifHex = E.Modifiers.Where(y => y.Name == "modifier_sheepstick_debuff")
						.DefaultIfEmpty(null)
						.FirstOrDefault();
			var modifEther = E.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
			var eulModifier = E.Modifiers.Where(y => y.Name == "modifier_eul_cyclone").DefaultIfEmpty(null).FirstOrDefault();

			var eulModif = E.HasModifier("modifier_eul_cyclone");
			var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
			if (Me.IsAlive)
			{
				if (Active && E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade)
				{
					if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900 && !eulModif)
					{
						Orbwalking.Orbwalk(E, 0, 1600, true, true);
					}
					float angle = Me.FindAngleBetween(E.Position, true);
					Vector3 pos = new Vector3((float)(E.Position.X - 500 * Math.Cos(angle)), (float)(E.Position.Y - 500 * Math.Sin(angle)), 0);
					if (
						_blink != null
						&& Me.CanCast()
						&& _blink.CanBeCasted()
						&& Me.Distance2D(E) >= Me.GetAttackRange()+Me.HullRadius
						&& Me.Distance2D(pos) <= 1180
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						_blink.UseAbility(pos);
						Utils.Sleep(250, "blink");
					}
					if (
						_cyclone != null
						&& _cyclone.CanBeCasted()
						&& Me.Distance2D(E) <= 1100
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name)
						&& _w.CanBeCasted()
						&& Utils.SleepCheck("cyclone"))
					{
						_cyclone.UseAbility(E);
						Utils.Sleep(300, "cyclone");
					}

					Vector3 start = E.NetworkActivity == NetworkActivity.Move ? new Vector3((float)((_w.GetCastDelay(Me, E, true) + 0.5) * Math.Cos(E.RotationRad) * E.MovementSpeed + E.Position.X),
												(float)((_w.GetCastDelay(Me, E, true) + 0.5) * Math.Sin(E.RotationRad) * E.MovementSpeed + E.NetworkPosition.Y), E.NetworkPosition.Z) : E.NetworkPosition;
					if (_w != null && _w.CanBeCasted()
					&& Me.Distance2D(start) <= _w.GetCastRange() + Me.HullRadius

					&& Utils.SleepCheck("w")
					&& (eulModifier != null && eulModifier.RemainingTime <= _w.GetCastDelay(Me, E, true) + 0.5
					|| modifHex != null && modifHex.RemainingTime <= _w.GetCastDelay(Me, E, true) + 0.5
					|| (_sheep == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name) || _sheep.Cooldown > 0)
					&& (_cyclone == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name) || _cyclone.Cooldown < 20 && _cyclone.Cooldown > 0)))
					{
						_w.UseAbility(start);
						Utils.Sleep(150, "w");
					}
					if (_cyclone == null || !_cyclone.CanBeCasted() || !_w.CanBeCasted() ||
						!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name))
					{
						if (eulModif) return;

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
						if ( // atos Blade
							_atos != null
							&& _atos.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsLinkensProtected()
							&& !E.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
							&& Me.Distance2D(E) <= 2000
							&& Utils.SleepCheck("atos")
							)
						{
							_atos.UseAbility(E);

							Utils.Sleep(250, "atos");
						} // atos Item end

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
						if (!_orchid.CanBeCasted() || _orchid == null ||
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
										&& Me.Distance2D(E) < _q.GetCastRange() + Me.HullRadius
										&& !stoneModif
										&& !E.IsMagicImmune()
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
										&& !_q.CanBeCasted()
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
										&& Me.Health >= (Me.MaximumHealth * 0.4)
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
											 && Me.Health <= (Me.MaximumHealth * 0.7))
											|| Me.Health <= (Me.MaximumHealth * 0.3))
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

									if ( // Dagon
										Me.CanCast()
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
					}
				}
				A();
			}
		}


		public void OnCloseEvent()
		{

		}

		private void A()
		{

			if (Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name) && _r.CanBeCasted())
			{
				var enemies =
				 ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsIllusion).ToList();


                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };
                foreach (var v in enemies)
				{
					if (v == null)
						return;

					_rDmg = new[] { 450, 650, 850 };


					var leans = Me.FindItem("item_aether_lens");
					var agh = (_rDmg[_r.Level - 1]);
					double damage = (_rDmg[_r.Level - 1] * (1 - v.MagicDamageResist));
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor(_rDmg[_r.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					if (leans != null) damage = damage * 1.08;

					if (!Me.AghanimState() && !v.IsLinkensProtected())
					{
						if (Me.HasModifier("modifier_item_aether_lens")) damage = damage * 1.08;
						if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage * 0.5;
						if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage * 1.3;
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
                                damage = damage * soul[demon.Spellbook.Spell2.Level];
                        }
                        var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
						damage = damage * spellamplymult;

						if ( // vail
							_vail != null
							&& _vail.CanBeCasted()
							&& Me.Distance2D(v) <= _r.GetCastRange() + Me.HullRadius
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
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
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
							&& v.Health < damage
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							_r.UseAbility(v);
							Utils.Sleep(150, v.Handle.ToString());
							return;
						}
					}
					if (Me.AghanimState() && !v.IsLinkensProtected())
					{
						if (_r != null && v != null && _r.CanBeCasted()
							&& !v.HasModifier("modifier_tusk_snowball_movement")
							&& !v.HasModifier("modifier_snowball_movement_friendly")
							&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
							&& !v.HasModifier("modifier_ember_spirit_flame_guard")
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&&
							!v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
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
							&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name)
							&& v.Health <= (agh - v.HealthRegeneration * _r.ChannelTime)
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							_r.UseAbility(v);
							Utils.Sleep(150, v.Handle.ToString());
							return;
						}
					}
					if (v.IsLinkensProtected() &&
						(Me.IsVisibleToEnemies || Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)))
					{
						if (_force != null && _force.CanBeCasted() && Me.Distance2D(v) < _force.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_force.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							_force.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(v) < _orchid.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							_orchid.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (_atos != null && _atos.CanBeCasted() && Me.Distance2D(v) < _atos.GetCastRange() - 400 &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_atos.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							_atos.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (_dagon != null && _dagon.CanBeCasted() && Me.Distance2D(v) < _dagon.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon") &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							_dagon.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (_cyclone != null && _cyclone.CanBeCasted() &&
								 Me.Distance2D(v) < _cyclone.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							_cyclone.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
					}
				}
			}
		}
	}
}
