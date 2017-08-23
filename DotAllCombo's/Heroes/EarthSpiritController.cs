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
	using System.Threading.Tasks;
	using Service.Debug;

	internal class EarthSpiritController : Variables, IHeroController
	{
		public void Combo()
		{
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			CastQ = Game.IsKeyDown(Menu.Item("qKey").GetValue<KeyBind>().Key);
			CastW = Game.IsKeyDown(Menu.Item("wKey").GetValue<KeyBind>().Key);
			CastE = Game.IsKeyDown(Menu.Item("eKey").GetValue<KeyBind>().Key);
            _autoUlt = Menu.Item("oneult").IsActive();
            if (!Menu.Item("enabled").IsActive())
                return;
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

			_q = Me.FindSpell("earth_spirit_boulder_smash");
			_e = Me.FindSpell("earth_spirit_geomagnetic_grip");
			_w = Me.FindSpell("earth_spirit_rolling_boulder");
			Me.FindSpell("earth_spirit_petrify");
			_r = Me.FindSpell("earth_spirit_magnetize");
			_d = Me.FindSpell("earth_spirit_stone_caller");

			_wmod = Me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");

            _ethereal = Me.FindItem("item_ethereal_blade");
            _urn = Me.FindItem("item_urn_of_shadows");
            _dagon =
                Me.Inventory.Items.FirstOrDefault(
                    item =>
                        item.Name.Contains("item_dagon"));
            _halberd = Me.FindItem("item_heavens_halberd");
            _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
            _abyssal = Me.FindItem("item_abyssal_blade");
            _mail = Me.FindItem("item_blade_mail");
            _bkb = Me.FindItem("item_black_king_bar");
            _blink = Me.FindItem("item_blink");
            _medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
            _sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
            _vail = Me.FindItem("item_veil_of_discord");
            _cheese = Me.FindItem("item_cheese");
            _ghost = Me.FindItem("item_ghost");
            _atos = Me.FindItem("item_rod_of_atos");
            _soul = Me.FindItem("item_soul_ring");
            _arcane = Me.FindItem("item_arcane_boots");
            _stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
            _shiva = Me.FindItem("item_shivas_guard");
            var stoneModif = E.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            var charge = Me.Modifiers.FirstOrDefault(y => y.Name == "modifier_earth_spirit_stone_caller_charge_counter");

            var remnant = ObjectManager.GetEntities<Unit>().Where(x => x.ClassId == ClassId.CDOTA_Unit_Earth_Spirit_Stone && x.Team == Me.Team && x.IsValid).ToList();
            var remnantCount = remnant.Count;


            if (Active && Me.Distance2D(E) <= 1300 && E.IsAlive && !Me.IsInvisible() && Utils.SleepCheck("Combo"))
            {
				if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& remnant.Count(x => x.Distance2D(Me) >= 350) == 0
					&& Me.Distance2D(E) >= 450
					&& Me.Distance2D(E) <= 1150
					&& !_wmod
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					_blink.UseAbility(E.Position);
					Utils.Sleep(250, "blink");
				}

				if (remnant.Count(x => x.Distance2D(Me) <= 1200) == 0)
                {
                if (
                    _d.CanBeCasted()
                    && _q != null
                    && _q.CanBeCasted()
                    && !_wmod
                    && ((_blink == null
                    || !_blink.CanBeCasted()
                    || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                    || (_blink != null && _blink.CanBeCasted() && Me.Distance2D(E) <= 450))
                    && Me.Distance2D(E) <= _e.GetCastRange() - 50
                    && Utils.SleepCheck("Rem")
                    )
                        {
                            _d.UseAbility(Prediction.InFront(Me, 50));
                            Utils.Sleep(500, "Rem");
                        }
				if (
                    _d.CanBeCasted()
                    && _q != null
                    && !_q.CanBeCasted()
                    && _e.CanBeCasted()
                    && Me.Distance2D(E)<=_e.GetCastRange()
                    && !_wmod
                    && ((_blink == null
                    || !_blink.CanBeCasted()
                    || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                    || (_blink != null && _blink.CanBeCasted() && Me.Distance2D(E) <= 450))
                    && Utils.SleepCheck("Rem")
                    )
						{
							_d.UseAbility(Prediction.InFront(E, 0));
							Utils.Sleep(500, "Rem");
						}
                }
                if (//Q Skill
                    _w != null
                    && (!_q.CanBeCasted()
                    || _q == null)
                    && !_e.CanBeCasted()
                    && _w.CanBeCasted()
                    && Me.Distance2D(E) <= _e.GetCastRange() - 50
                    && Me.CanCast()
                    && Utils.SleepCheck(Me.Handle + "remnantW")
                    )
                {
                    _w.CastSkillShot(E);
                    Utils.Sleep(250, Me.Handle + "remnantW");
                }
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) >= 1)
                {

                    for (int i = 0; i < remnantCount; ++i)
                    {

                        var r = remnant[i];
                        if (
                            _d != null && _d.CanBeCasted()
                            && ((_q != null && _q.CanBeCasted())
                            || (_w != null && _w.CanBeCasted()))
                            && !_wmod
                            && remnant.Count(x => x.Distance2D(Me) <= 350) == 0
                            && ((_blink == null
                            || !_blink.CanBeCasted()
                            || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                            || (_blink != null && Me.Distance2D(E) <= 350 && _blink.CanBeCasted()))
                            )
                        {
                            if (Me.Distance2D(E) <= _e.GetCastRange() - 50
                                && Utils.SleepCheck("Rem"))
                            {
                                _d.UseAbility(Prediction.InFront(Me, 50));
                                Utils.Sleep(600, "Rem");
                            }
                        }
                        if (
                            Me.Distance2D(r) >=  210
                            && remnant.Count(x => x.Distance2D(Me) <= 350) >= 1
                            && _q.CanBeCasted()
                            && Utils.SleepCheck("RemMove"))
                        {
                            Me.Move(r.Position);
                            Utils.Sleep(250, "RemMove");
                        }
                        if (//Q Skill
                           _q != null
                           && _q.CanBeCasted()
                           && Me.CanCast()
                           && Me.Distance2D(E) <= _e.GetCastRange() - 50
                           && Me.Distance2D(r) <= 210
                           && Utils.SleepCheck(r.Handle + "remnantQ")
                           )
                        {
                            _q.CastSkillShot(E);
                            Utils.Sleep(250, r.Handle + "remnantQ");
                        }
                        else
                        if (//W Skill
                           _w != null
                           && _w.CanBeCasted()
                           && !_q.CanBeCasted()
                           && Me.Distance2D(E) <= _e.GetCastRange()
                           && Utils.SleepCheck(Me.Handle + "remnantW")
                           )
                        {
                            _w.CastSkillShot(E);
                            Utils.Sleep(250, Me.Handle + "remnantW");
                        }
                        if (r != null
                           && _e != null
                           && _e.CanBeCasted()
                           && Me.CanCast()
                           && Me.Distance2D(r) < _e.GetCastRange()
                           && Me.Distance2D(E) <= _e.GetCastRange()
                           )
                        {
                            if (//E Skill
                                E.Distance2D(r) <= 200
                                && Utils.SleepCheck(r.Handle + "remnantE")
                               )
                            {
                                _e.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                            if (//E Skill
                              Me.Distance2D(E) <= 200
                              && E.Distance2D(r) > 0
                              && Me.Distance2D(r) >= E.Distance2D(r)
                              && Utils.SleepCheck(r.Handle + "remnantE")
                              )
                            {
                                _e.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                        }
                    }
                }
                

                if (//W Skill
                       _w != null
                       && charge.StackCount == 0
                       && _w.CanBeCasted()
                       && Me.Distance2D(E) <= 800
                       && Me.CanCast()
                       && Utils.SleepCheck(Me.Handle + "remnantW")
                       )
                {
                    _w.CastSkillShot(E);
                    Utils.Sleep(250, Me.Handle + "remnantW");
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
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
                    && Me.Distance2D(E) <= 700
                    )
                {
                    _medall.UseAbility(E);
                    Utils.Sleep(250, "Medall");
                } // Medall Item end

                if ( //R Skill
                    _r != null
                    && _r.CanBeCasted()
                    && Me.CanCast()
                    && Me.Distance2D(E) <= 200
                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                    && Utils.SleepCheck("R")
                    )
                {
                    _r.UseAbility();
                    Utils.Sleep(200, "R");
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
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
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

                var v =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.Distance2D(Me) <= 700)
                        .ToList();
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
                Utils.Sleep(50, "Combo");
            }
        }
		private  void Others(EventArgs args)
		{

			CastQ = Game.IsKeyDown(Menu.Item("qKey").GetValue<KeyBind>().Key);
			CastW = Game.IsKeyDown(Menu.Item("wKey").GetValue<KeyBind>().Key);
			CastE = Game.IsKeyDown(Menu.Item("eKey").GetValue<KeyBind>().Key);
			_autoUlt = Menu.Item("oneult").IsActive();
            if (!Menu.Item("enabled").IsActive())
                return;

            if (E == null) return;

            _d = Me.Spellbook.SpellD;
            _q = Me.Spellbook.SpellQ;
            _e = Me.Spellbook.SpellE;
            _w = Me.Spellbook.SpellW;
		    _r = Me.Spellbook.SpellR;


            var magnetizemod = E.Modifiers.Where(y => y.Name == "modifier_earth_spirit_magnetize").DefaultIfEmpty(null).FirstOrDefault();

            if (_autoUlt && magnetizemod != null && magnetizemod.RemainingTime <= 0.2 + Game.Ping && Me.Distance2D(E) <= _d.GetCastRange() && Utils.SleepCheck("Rem"))
            {
                _d.UseAbility(E.Position);
                Utils.Sleep(1000, "Rem");
            }
            var remnant = ObjectManager.GetEntities<Unit>().Where(x => x.ClassId == ClassId.CDOTA_Unit_Earth_Spirit_Stone && x.Team == Me.Team
                                       && x.Distance2D(Me) <= 1700 && x.IsAlive && x.IsValid).ToList();
            var remnantCount = remnant.Count;
            
            if (CastQ && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
            {
                _wmod = Me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) == 0)
                {
                    if (
                    _d.CanBeCasted()
                    && _q.CanBeCasted()
                    && !_wmod
                    && ((_blink == null
                    || !_blink.CanBeCasted()
                    || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                    || (_blink != null && Me.Distance2D(E) <= 450 && _blink.CanBeCasted()))
                    )
                    {
                        if (Me.Distance2D(E) <= _e.GetCastRange() - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            
                            _d.UseAbility(Prediction.InFront(Me, 50));
                            Utils.Sleep(600, "Rem");
                        }
                    }
                }
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) >= 1)
                {
                    for (int i = 0; i < remnantCount; ++i)
                    {
                        var r = remnant[i];
                        if (
                            _d != null && _d.CanBeCasted()
                            && ((_q != null && _q.CanBeCasted())
                            || (_w != null && _w.CanBeCasted()))
                            && !_wmod
                            && Me.Distance2D(r) >= 350
                            && ((_blink == null
                            || !_blink.CanBeCasted()
                            || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                            || (_blink != null && Me.Distance2D(E) <= 450 && _blink.CanBeCasted()))
                            )
                        {
                            if (Me.Distance2D(E) <= _e.GetCastRange() - 50
                                && Utils.SleepCheck("Rem"))
                            {
                                _d.UseAbility(Prediction.InFront(Me, 50));
                                Utils.Sleep(600, "Rem");
                            }
                        }
                        if (
                           Me.Distance2D(r) >= 200
                           && Me.Distance2D(r) <= 350
                           && _q.CanBeCasted()
                           && Utils.SleepCheck("RemMove"))
                        {
                            Me.Move(r.Position);
                            Utils.Sleep(300, "RemMove");
                        }
                        if (//Q Skill
                          r != null
                          && _q != null
                          && _q.CanBeCasted()
                          && Me.CanCast()
                          && Me.Distance2D(E) <= _e.GetCastRange() - 50
                          && r.Distance2D(Me) <= 210
                          && Utils.SleepCheck(r.Handle + "remnantQ")
                          )
                        {
                            _q.CastSkillShot(E);
                            Utils.Sleep(250, r.Handle + "remnantQ");
                        }
                    }
                }
            }
            if (CastW)
            {
                _wmod = Me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
                Task.Delay(350).ContinueWith(_ =>
                {
                    if (remnant.Count(x => x.Distance2D(Me) <= 1200) == 0)
                    {
                        if (
                            _d.CanBeCasted()
                            && _wmod
                            && Me.Distance2D(E) >= 600
                            && Utils.SleepCheck("nextAction")
                            )
                        {
                            _d.UseAbility(Prediction.InFront(Me, 170));
                            Utils.Sleep(1800 + _d.FindCastPoint(), "nextAction");
                        }
                    }
                });
                if (//W Skill
                    _w != null
                    && _w.CanBeCasted()
                    && Game.MousePosition.Distance2D(E) <= 500
                    && Me.Distance2D(E) <= _w.GetCastRange() - 200
                    && Utils.SleepCheck(Me.Handle + "remnantW")
                    )
                {
                    _w.CastSkillShot(E);
                    Utils.Sleep(250, Me.Handle + "remnantW");
                }
                else if (//W Skill
                    _w != null
                    && _w.CanBeCasted()
                    && Game.MousePosition.Distance2D(E) >= 500
                    && Utils.SleepCheck(Me.Handle + "remnantW")
                    )
                {
                    _w.UseAbility(Game.MousePosition);
                    Utils.Sleep(250, Me.Handle + "remnantW");
                }
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) >= 1)
                {
                    for (var i = 0; i < remnantCount; ++i)
                    {
                        var i1 = i;
                        Task.Delay(350).ContinueWith(_ =>
                        {

                            var r = remnant[i1];
                            if (r != null && Me.Distance2D(r) >= 200)
                            {
                                if (
                                    _d.CanBeCasted()
                                    && _wmod
                                    && Me.Distance2D(E) >= 600

                                    && Utils.SleepCheck("nextAction")
                                    )
                                {
                                    _d.UseAbility(Prediction.InFront(Me, 170));
                                    Utils.Sleep(1800 + _d.FindCastPoint(), "nextAction");
                                }
                            }
                        });
                    }
                }
            }
            if (CastE && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
            {
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) == 0)
                {
                    if (
                    _d.CanBeCasted()
                    && _e.CanBeCasted()
                    )
                    {
                        if (Me.Distance2D(E) <= _e.GetCastRange() - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            _d?.UseAbility(E.Position);
                            Utils.Sleep(1000, "Rem");
                        }
                    }
                }
                if (remnant.Count(x => x.Distance2D(Me) <= 1200) >= 1)
                {
                    for (int i = 0; i < remnantCount; ++i)
                    {
                        var r = remnant[i];

                        if (r.Distance2D(E) >= 300)
                        {
                            if (
                                _d.CanBeCasted()
                                && (_e != null && _e.CanBeCasted())
                                   && !r.HasModifier("modifier_earth_spirit_boulder_smash")
                                   && !r.HasModifier("modifier_earth_spirit_geomagnetic_grip")
                                   )
                            {
                                if (Me.Distance2D(E) <= _e.GetCastRange() - 50
                                    && Utils.SleepCheck("Rem"))
                                {
                                    _d.UseAbility(E.Position);
                                    Utils.Sleep(1000, "Rem");
                                }
                            }
                        }
                        if (r != null
                           && _e != null
                           && _e.CanBeCasted()
                           && Me.CanCast()
                           && Me.Distance2D(r) < _e.GetCastRange()
                           && Me.Distance2D(E) <= _e.GetCastRange()
                           )
                        {
                            if (//E Skill
                                E.Distance2D(r) <= 200
                                && Utils.SleepCheck(r.Handle + "remnantE")
                               )
                            {
                                _e.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                            if (//E Skill
                              Me.Distance2D(E) <= 200
                              && E.Distance2D(r) > 0
                              && Me.Distance2D(r) >= E.Distance2D(r)
                              && Utils.SleepCheck(r.Handle + "remnantE")
                              )
                            {
                                _e.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                        }
                    }
                }
            }
        }
	
		public void OnCloseEvent()
		{
			Game.OnUpdate -= Others;
		}
		private static Ability _q, _w, _e, _r, _d;
	    private static bool _wmod;

        private static Item _urn, _dagon, _ghost, _soul, _atos, _vail, _sheep, _cheese, _stick, _arcane, _halberd, _ethereal, _orchid,
			_abyssal, _shiva, _mail, _bkb, _medall, _blink;
		
		private static bool _autoUlt;

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");
			Print.LogMessage.Success("This beginning marks their end!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true)); 
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("qKey", "Q Spell").SetValue(new KeyBind('Q', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("wKey", "W Spell").SetValue(new KeyBind('W', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("eKey", "E Spell").SetValue(new KeyBind('E', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    { "earth_spirit_magnetize", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_ethereal_blade", true},
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    {"item_dagon", true},
                    {"item_urn_of_shadows", true},
				    {"item_veil_of_discord", true},
				    {"item_abyssal_blade", true},
				    {"item_bloodthorn", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true}
				})));
			Menu.AddItem(
				new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true},
                    {"item_shivas_guard", true},
				    {"item_sheepstick", true},
				    {"item_cheese", true},
				    {"item_ghost", true},
				    {"item_rod_of_atos", true},
				    {"item_soul_ring", true},
				    {"item_arcane_boots", true},
				    {"item_magic_stick", true},
				    {"item_magic_wand", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("oneult", "Use AutoUpdate Ultimate Remnant").SetValue(true));
			Game.OnUpdate += Others;
		}
	}
}