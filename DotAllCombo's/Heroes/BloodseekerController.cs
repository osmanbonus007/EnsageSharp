using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{

	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;

	using Service.Debug;

	internal class BloodseekerController : Variables, IHeroController
	{
		private Ability _q, _w, _r;
		private Item _urn, _dagon, _soul, _phase, _cheese, _cyclone, _halberd, _ethereal, _arcane,
            _mjollnir, _orchid, _abyssal, _stick, _force, _mom, _shiva, _mail, _bkb, _satanic, _medall, _blink, _sheep;
		

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;

            E = Toolset.ClosestToMouse(Me);
            if (E == null)
				return;
			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_shiva = Me.FindItem("item_shivas_guard");
			_ethereal = Me.FindItem("item_ethereal_blade");
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_blink = Me.FindItem("item_blink");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_cyclone = Me.FindItem("item_cyclone");
			_force = Me.FindItem("item_force_staff");
			_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
			_cheese = Me.FindItem("item_cheese");
			_soul = Me.FindItem("item_soul_ring");
			_arcane = Me.FindItem("item_arcane_boots");
			_stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
			_phase = Me.FindItem("item_phase_boots");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();

		    var stoneModif = E.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
			var modifR = E.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture");
			var modifQ = E.HasModifier("modifier_bloodseeker_bloodrage");
			if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
			if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
			{
				if (_cyclone != null && _cyclone.CanBeCasted() && _w.CanBeCasted()
					   && Me.Distance2D(E) <= _cyclone.GetCastRange() + 300
					   && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name)
					   && _w.CanBeCasted()
					   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					   && Utils.SleepCheck(Me.Handle.ToString()))
				{
					_cyclone.UseAbility(E);
					Utils.Sleep(500, Me.Handle.ToString());
				}
				if (
					   _q != null && _q.CanBeCasted() && Me.Distance2D(E) <= 700
					   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					   && !modifQ
					   && Utils.SleepCheck("Q")
					   )
				{
					_q.UseAbility(Me);
					Utils.Sleep(200, "Q");
				}
				if (
					  _w != null && _w.CanBeCasted() && Me.Distance2D(E) <= 700
					  &&
					  (!_cyclone.CanBeCasted() || _cyclone == null ||
					   !Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name))
					  && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					  && Utils.SleepCheck("W")
					  )
				{
					_w.UseAbility(E.Predict(300));
					Utils.Sleep(200, "W");
				}

				if (
					_force != null
					&& _force.CanBeCasted()
					&& Me.Distance2D(E) < 800
					&& modifR
					&& E.IsSilenced()
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_force.Name)
					&& Utils.SleepCheck("force"))
				{
					_force.UseAbility(E);
					Utils.Sleep(240, "force");
				}
				if (_cyclone == null || !_cyclone.CanBeCasted() || !Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name))
				{

					if (
						_r != null && _r.CanBeCasted() && Me.Distance2D(E) <= 900
						&& !E.HasModifier("modifier_bloodseeker_rupture")
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						&& Utils.SleepCheck("R")
						)
					{
						_r.UseAbility(E);
						Utils.Sleep(500, "R");
					}
					if ( // MOM
						_mom != null
						&& _mom.CanBeCasted()
						&& Me.CanCast()
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
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
					if ( // Arcane Boots Item
						_arcane != null
						&& Me.Mana <= _r.ManaCost
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
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
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
							|| (E.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
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
					if (_phase != null
						&& _phase.CanBeCasted()
						&& Utils.SleepCheck("phase")
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_phase.Name)
						&& !_blink.CanBeCasted()
						&& Me.Distance2D(E) >= Me.AttackRange + 20)
					{
						_phase.UseAbility();
						Utils.Sleep(200, "phase");
					}
					if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
					{
						_urn.UseAbility(E);
						Utils.Sleep(240, "urn");
					}
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
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
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
					
				}
			}
		} // Combo

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Blood!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"bloodseeker_bloodrage", true},
				    {"bloodseeker_rupture", true},
				    {"bloodseeker_blood_bath", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
                    {"item_dagon", true},
                    {"item_ethereal_blade", true},
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    { "item_bloodthorn", true},
				    {"item_urn_of_shadows", true},
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
			       {"item_soul_ring", true},
			       {"item_arcane_boots", true},
			       {"item_magic_stick", true},
			       {"item_magic_wand", true},
			       {"item_mjollnir", true},
			       {"item_satanic", true},
			       {"item_phase_boots", true}
			   })));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
