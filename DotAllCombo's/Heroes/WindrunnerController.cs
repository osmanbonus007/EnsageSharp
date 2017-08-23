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

	internal class WindrunnerController : Variables, IHeroController
	{
		private Ability _q, _e, _r;

		private Item _urn, _orchid, _ethereal, _dagon, _halberd, _blink, _mjollnir, _abyssal, _mom, _shiva, _mail, _bkb, _iron, _satanic, _medall;

		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_q = Me.Spellbook.SpellQ;
		    _e = Me.Spellbook.SpellE;
			_r = Me.Spellbook.SpellR;
			_iron = Me.FindItem("item_branches"); 
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_ethereal = Me.FindItem("item_ethereal_blade");
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_blink = Me.FindItem("item_blink");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
            E = Toolset.ClosestToMouse(Me);
            if (E == null)
				return;
			if (Active)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
			if (Active && Me.Distance2D(E) <= 1400 && E.IsAlive && !Me.IsInvisible())
			{
                float angleblink = Me.FindAngleBetween(E.Position, true);
                if (_iron != null)
				{
					float angle = Me.FindAngleBetween(E.Position, true);
					Vector3 pos = new Vector3((float)(E.Position.X - 290 * Math.Cos(angle)), (float)(E.Position.Y - 290 * Math.Sin(angle)), 0);
                   


                    if (
						_q != null && _q.CanBeCasted() && Me.Distance2D(E) <= 1300
						&& Me.CanAttack()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility(E);
						Utils.Sleep(100, "Q");
					}
					if (
						_iron != null
						&& _blink != null
						&& _blink.Cooldown > 11
						&& _iron.CanBeCasted()
						&& Me.Distance2D(E) <= 300
						&& Me.CanAttack()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_branches")
						&& Utils.SleepCheck("iron")
						)
					{
						_iron.UseAbility(pos);
						Utils.Sleep(10000, "iron");
					}
                  
                }
                
                var blinkpos = _iron !=null 
                    ? new Vector3((float)(E.Position.X + 200 * Math.Cos(angleblink)), (float)(E.Position.Y + 200 * Math.Sin(angleblink)), 0) 
                    : new Vector3((float)(E.Position.X - 200 * Math.Cos(angleblink)), (float)(E.Position.Y - 200 * Math.Sin(angleblink)), 0);
                
                if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& Me.Distance2D(blinkpos) < 1190
					&& ((_iron == null && Me.Distance2D(E) > Me.GetAttackRange())
					|| (_iron != null && _iron.CanBeCasted() && !_q.CanBeCasted()))
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					_blink.UseAbility(blinkpos);
					Utils.Sleep(250, "blink");
				}
				
				if (
					_r != null && _r.CanBeCasted() && Me.Distance2D(E) <= 800
					&& Me.CanAttack()
					&& (E.IsStunned()
					||E.IsHexed())
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
					&& Utils.SleepCheck("R")
					)
				{
					_r.UseAbility(E);
					Utils.Sleep(100, "R");
				}
				if (
					_e != null && _e.CanBeCasted() 
					&& Me.Distance2D(E) <= 900
					&& Me.Distance2D(E) >= Me.GetAttackRange()+Me.HullRadius
					&& Me.CanAttack()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
					&& Utils.SleepCheck("E")
					)
				{
					_e.UseAbility();
					Utils.Sleep(100, "E");
				}

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
				if ( // orchid
					_orchid != null
					&& _orchid.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Me.Distance2D(E) <= Me.AttackRange + 40
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
					&& Utils.SleepCheck("orchid")
					)
				{
					_orchid.UseAbility(E);
					Utils.Sleep(250, "orchid");
				} // orchid Item end

				if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
					&& !E.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					_shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}

				if (_ethereal != null && _ethereal.CanBeCasted()
					&& Me.Distance2D(E) <= 700 && Me.Distance2D(E) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
				{
					_ethereal.UseAbility(E);
					Utils.Sleep(100, "ethereal");
				}

				if (_dagon != null
					&& _dagon.CanBeCasted()
					&& Me.Distance2D(E) <= 500
					&& Utils.SleepCheck("dagon"))
				{
					_dagon.UseAbility(E);
					Utils.Sleep(100, "dagon");
				}
				if ( // Abyssal Blade
					_abyssal != null
					&& _abyssal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsStunned()
					&& !E.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
					&& Me.Distance2D(E) <= 400
					)
				{
					_abyssal.UseAbility(E);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
				{
					_urn.UseAbility(E);
					Utils.Sleep(240, "urn");
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
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Ooh, now I get to shoot stuff!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"windrunner_powershot", true},
				    {"windrunner_shackleshot", true},
				    {"windrunner_windrun", true},
				    {"windrunner_focusfire", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_branches", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    {"item_bloodthorn", true},
				    {"item_blink", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("UltHealth", "Min Health Down To Ult").SetValue(new Slider(450, 200, 1000)));
			
		}

		public void OnCloseEvent()
		{
			
		}
	}
}