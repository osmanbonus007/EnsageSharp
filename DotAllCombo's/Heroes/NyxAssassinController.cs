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
    using SharpDX;

    internal class NyxAssassinController : Variables, IHeroController
    {
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _skills = new Menu("Skills: ", "Skills: ");
        private Ability _q, _w, _r;
        private Item _sheep, _vail, _soul, _abyssal, _mjollnir, _orchid, _arcane, _blink, _shiva, _dagon, _ethereal, _cheese, _halberd, _satanic, _mom, _medall;
        

        public void Combo()
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
			if (Active && Me.IsAlive)
            {
                E = Toolset.ClosestToMouse(Me);
                if (E == null) return;
				_q = Me.Spellbook.SpellQ;

				_w = Me.Spellbook.SpellW;

				_r = Me.Spellbook.SpellR;

				// item 
				_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");

				_satanic = Me.FindItem("item_satanic");

				_shiva = Me.FindItem("item_shivas_guard");

				_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

				_arcane = Me.FindItem("item_arcane_boots");

                _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");

                _mom = Me.FindItem("item_mask_of_madness");

				_vail = Me.FindItem("item_veil_of_discord");

				_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");

				_ethereal = Me.FindItem("item_ethereal_blade");

				_blink = Me.FindItem("item_blink");

				_soul = Me.FindItem("item_soul_ring");

				_cheese = Me.FindItem("item_cheese");

				_halberd = Me.FindItem("item_heavens_halberd");

				_abyssal = Me.FindItem("item_abyssal_blade");

				_mjollnir = Me.FindItem("item_mjollnir");
				var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
				var linkens = E.IsLinkensProtected();
				var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
				if (E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade)
                {
					if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
					{
						Orbwalking.Orbwalk(E, 0, 1600, true, true);
					}
					if (
					   _r != null
					   && _r.CanBeCasted()
					   && !Me.HasModifier("modifier_nyx_assassin_vendetta")
					   && Me.Distance2D(E) <= 1400
					   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
					   && Utils.SleepCheck("R")
					   )
					{
						_r.UseAbility();
						Utils.Sleep(200, "R");
					}

					if (_r != null && (_r.IsInAbilityPhase || Me.HasModifier("modifier_nyx_assassin_vendetta") || _r.IsChanneling)) return;
					if (_r == null || !_r.CanBeCasted() && !Me.HasModifier("modifier_nyx_assassin_vendetta")
						|| !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name))
                    {
                        if (stoneModif) return;
                        var angle = Me.FindAngleBetween(E.Position, true);
                        var pos = new Vector3((float)(E.Position.X - 100 * Math.Cos(angle)), (float)(E.Position.Y - 100 * Math.Sin(angle)), 0);
                        if (
                            _blink != null
                            && _q.CanBeCasted()
                            && Me.CanCast()
                            && _blink.CanBeCasted()
                            && Me.Distance2D(E) >= 300
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
                           && Me.Distance2D(E) <= Me.AttackRange+Me.HullRadius+50
                           && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
                           && Utils.SleepCheck("orchid")
                           )
                        {
                            _orchid.UseAbility(E);
                            Utils.Sleep(250, "orchid");
                        } // orchid Item end
                        if ( // vail
                            _vail != null
                            && _vail.CanBeCasted()
                            && Me.CanCast()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
                            && !E.IsMagicImmune()
                            && Utils.SleepCheck("vail")
                            && Me.Distance2D(E) <= 1500
                            )
                        {
                            _vail.UseAbility(E.Position);
                            Utils.Sleep(250, "vail");
                        }

                        if ( // ethereal
                            _ethereal != null &&
                            _ethereal.CanBeCasted()
                            && (!_vail.CanBeCasted()
                                || _vail == null)
                            && Me.CanCast() &&
                            !linkens &&
                            !E.IsMagicImmune() &&
                            Utils.SleepCheck("ethereal")
                            )
                        {
                            _ethereal.UseAbility(E);
                            Utils.Sleep(150, "ethereal");
                        }
						
                        if ((_vail == null || !_vail.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)) && (_ethereal == null || !_ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)))
                        {
                            if ( // sheep
                                _sheep != null
                                && _sheep.CanBeCasted()
                                && Me.CanCast()
                                && !E.IsLinkensProtected()
                                && !E.IsMagicImmune()
                                && !E.IsStunned()
                                && !E.IsHexed()
                                && Me.Distance2D(E) <= 1400
                                && !stoneModif
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
                                && Utils.SleepCheck("sheep")
                                )
                            {
                                _sheep.UseAbility(E);
                                Utils.Sleep(250, "sheep");
                            } // sheep Item end
                            if (
                                _q!=null
								&& _q.CanBeCasted()
                                && _q.Cooldown<=0
                                && Me.Mana>= _q.ManaCost
                                && !E.IsStunned()
                                && !E.IsHexed()
								&& Me.Distance2D(E) <= _q.GetCastRange() +Me.HullRadius
								&& !E.IsMagicImmune()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                && Utils.SleepCheck("Q"))
                            {
                                _q.CastSkillShot(E);
                                Utils.Sleep(100, "Q");
                            }
                            if (
                                _w != null
                                && _w.CanBeCasted()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
								&& E.Mana >= (E.MaximumMana * 0.2)
								&& Me.Position.Distance2D(E.Position) < _w.GetCastRange()
                                && Utils.SleepCheck("W"))
                            {
                                _w.UseAbility(E);
                                Utils.Sleep(100, "W");
                            }

                            if ( // SoulRing Item 
                                _soul != null &&
                                Me.Health >= (Me.MaximumHealth * 0.5) &&
                                Me.Mana <= _r.ManaCost &&
                                _soul.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
                                && Utils.SleepCheck("soul"))
                            {
                                _soul.UseAbility();
                                Utils.Sleep(100, "soul");
                            } // SoulRing Item end

                            if ( // Arcane Boots Item
                                _arcane != null &&
                                Me.Mana <= _q.ManaCost &&
                                _arcane.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_arcane.Name) 
                                && Utils.SleepCheck("arcane"))
                            {
                                _arcane.UseAbility();
                                Utils.Sleep(100, "arcane");
                            } // Arcane Boots Item end

                            if ( // Shiva Item
                                _shiva != null &&
                                _shiva.CanBeCasted() &&
                                Me.CanCast() &&
                                !E.IsMagicImmune() &&
                                Utils.SleepCheck("shiva") &&
                                Me.Distance2D(E) <= 600
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                                )
                            {
                                _shiva.UseAbility();
                                Utils.Sleep(250, "shiva");
                            } // Shiva Item end

                            if ( // Medall
                                _medall != null &&
                                _medall.CanBeCasted() &&
                                Me.CanCast() &&
                                !E.IsMagicImmune() &&
                                Utils.SleepCheck("Medall")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
                                && Me.Distance2D(E) <= 500
                                )
                            {
                                _medall.UseAbility(E);
                                Utils.Sleep(250, "Medall");
                            } // Medall Item end

                            if ( // MOM
                                _mom != null &&
                                _mom.CanBeCasted() &&
                                Me.CanCast() &&
                                Utils.SleepCheck("mom") &&
                                Me.Distance2D(E) <= 700
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
                                )
                            {
                                _mom.UseAbility();
                                Utils.Sleep(250, "mom");
                            } // MOM Item end

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
                            if ( // Abyssal Blade
                                _abyssal != null &&
                                _abyssal.CanBeCasted() &&
                                Me.CanCast() &&
                                !E.IsMagicImmune() &&
                                Utils.SleepCheck("abyssal") &&
                                Me.Distance2D(E) <= 400
                                && !E.IsStunned()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
                                )
                            {
                                _abyssal.UseAbility(E);
                                Utils.Sleep(250, "abyssal");
                            } // Abyssal Item end

                            if ( // Hellbard
                                _halberd != null &&
                                _halberd.CanBeCasted() &&
                                Me.CanCast() &&
                                !E.IsMagicImmune() &&
                                Utils.SleepCheck("halberd") &&
                                Me.Distance2D(E) <= 700
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
                                )
                            {
                                _halberd.UseAbility(E);
                                Utils.Sleep(250, "halberd");
                            } // Hellbard Item end

                            if ( // Dagon
                                Me.CanCast()
                                && _dagon != null
                                && (_ethereal == null
                                    || (E.HasModifier("modifier_item_ethereal_blade_slow")
                                        || _ethereal.Cooldown < 18))
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
							
                            if ( // Mjollnir
                                _mjollnir != null &&
                                _mjollnir.CanBeCasted() &&
                                Me.CanCast() &&
                                !E.IsMagicImmune() &&
                                Utils.SleepCheck("mjollnir")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
                                && Me.Distance2D(E) <= 900
                                )
                            {
                                _mjollnir.UseAbility(Me);
                                Utils.Sleep(250, "mjollnir");
                            } // Mjollnir Item end
							
                            if ( // Satanic 
                                _satanic != null &&
                                Me.Health <= (Me.MaximumHealth * 0.3) &&
                                _satanic.CanBeCasted() &&
                                Me.Distance2D(E) <= 700 &&
                                Utils.SleepCheck("Satanic")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
                                )
                            {
                                _satanic.UseAbility();
                                Utils.Sleep(350, "Satanic");
                            } // Satanic Item end
                        }
                    }
                }
            }
            if (!Menu.Item("Kill").GetValue<bool>() || !Me.IsAlive ||
                (!Me.IsVisibleToEnemies && Me.IsInvisible())) return;
            var enemies =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsIllusion).ToList();
            double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };

            if (enemies.Count <= 0) return;
            foreach (var v in enemies)
            {
                if (v == null) return;

                var wM = new[] { 3.5, 4, 4.5, 5 };
                var wDmg = Me.TotalIntelligence*wM[_w.Level - 1];

                var damageW = Math.Floor(wDmg*(1 - v.MagicDamageResist));

                var lens = Me.HasModifier("modifier_item_aether_lens");
                var spellamplymult = 1 + (Me.TotalIntelligence/16/100);
                if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                {
                    damageW =
                        Math.Floor(wDmg*
                                   (1 - (0.10 + v.Spellbook.Spell3.Level*0.04))*(1 - v.MagicDamageResist));
                }

                if (lens) damageW = damageW*1.08;
                if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageW = damageW*0.5;
                if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageW = damageW*1.3;
                if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                {
                    var blood =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                    if (blood != null)
                        damageW = damageW * bloodrage[blood.Spellbook.Spell1.Level];
                    else
                        damageW = damageW * 1.4;
                }


                if (v.HasModifier("modifier_chen_penitence"))
                {
                    var chen =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                    if (chen != null)
                        damageW = damageW * penitence[chen.Spellbook.Spell1.Level];
                }


                if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                {
                    var demon =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                    if (demon != null)
                        damageW = damageW * soul[demon.Spellbook.Spell2.Level];
                }
                damageW = damageW*spellamplymult;

                if (damageW > v.Mana)
                    damageW = v.Mana;


                if ( // vail
                    _vail != null
                    && _vail.CanBeCasted()
                    && _w.CanBeCasted()
                    && v.Health <= damageW * 1.25
                    && v.Health >= damageW
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
                var etherealdamage = (int) (((Me.TotalIntelligence*2) + 75));
                if ( // vail
                    _ethereal != null
                    && _ethereal.CanBeCasted()
                    && _w != null
                    && _w.CanBeCasted()
                    && v.Health <= etherealdamage + damageW*1.4
                    && v.Health >= damageW
                    && Me.CanCast()
                    && !v.HasModifier("modifier_item_ethereal_blade_slow")
                    && !v.IsMagicImmune()
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                    && Me.Distance2D(v) <= _ethereal.GetCastRange() + 50
                    && Utils.SleepCheck("ethereal")
                )
                {
                    _ethereal.UseAbility(v);
                    Utils.Sleep(250, "ethereal");
                }

                if (_w != null && v != null && _w.CanBeCasted()
                    && Me.AghanimState()
                    ? !(Me.Distance2D(v) <= 1050)
                    : !(Me.Distance2D(v) <= _w.GetCastRange() + 50) ||
                      v.HasModifier("modifier_tusk_snowball_movement") ||
                      v.HasModifier("modifier_snowball_movement_friendly") ||
                      v.HasModifier("modifier_templar_assassin_refraction_absorb") ||
                      v.HasModifier("modifier_ember_spirit_flame_guard") ||
                      v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability") ||
                      v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison") ||
                      v.HasModifier("modifier_puck_phase_shift") || v.HasModifier("modifier_eul_cyclone") ||
                      v.HasModifier("modifier_dazzle_shallow_grave") ||
                      v.HasModifier("modifier_shadow_demon_disruption") ||
                      v.HasModifier("modifier_necrolyte_reapers_scythe") ||
                      v.HasModifier("modifier_necrolyte_reapers_scythe") ||
                      v.HasModifier("modifier_storm_spirit_ball_lightning") ||
                      v.HasModifier("modifier_ember_spirit_fire_remnant") ||
                      v.HasModifier("modifier_nyx_assassin_spiked_carapace") ||
                      v.HasModifier("modifier_phantom_lancer_doppelwalk_phase") ||
                      v.FindSpell("abaddon_borrowed_time").CanBeCasted() ||
                      v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect") || v.IsMagicImmune() ||
                      !(v.Health < damageW) || !Utils.SleepCheck(v.Handle.ToString())) continue;
                _w.UseAbility(v);
                Utils.Sleep(150, v.Handle.ToString());
                return;
            }
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("My purpose is clear, my targets doomed.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddSubMenu(_items);
            _items.AddItem(new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_orchid", true},
                {"item_bloodthorn", true},
                {"item_ethereal_blade", true},
                {"item_veil_of_discord", true},
                {"item_rod_of_atos", true},
                {"item_sheepstick", true},
                {"item_arcane_boots", true},
                {"item_dagon", true},
                {"item_blink", true},
                {"item_soul_ring", true},
                {"item_medallion_of_courage", true},
                {"item_mask_of_madness", true},
                {"item_abyssal_blade", true},
                {"item_mjollnir", true},
                {"item_satanic", true},
                {"item_solar_crest", true},
                {"item_ghost", true},
                {"item_cheese", true}
            })));
            Menu.AddSubMenu(_skills);
            _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"nyx_assassin_impale", true},
                {"nyx_assassin_mana_burn", true},
                {"nyx_assassin_vendetta", true}
            })));
			Menu.AddItem(new MenuItem("Kill", "KillSteal W").SetValue(true)); 
        }

        public void OnCloseEvent()
        {

        }
    }
}
