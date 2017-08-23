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

	internal class ChaosKnightController : Variables, IHeroController
	{
		private Ability _q, _w, _r;

		private Item _urn, _dagon, _halberd, _mjollnir, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _blink, _armlet, _medall, _manta;

        

		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;


			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_armlet = Me.FindItem("item_armlet");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_blink = Me.FindItem("item_blink");
			_satanic = Me.FindItem("item_satanic");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
			_manta = Me.FindItem("item_manta");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
            E = Toolset.ClosestToMouse(Me);
            if (E == null)
				return;
			var a = ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team == Me.Team && x.IsAlive && x.IsVisible && x.IsIllusion && x.IsControllable).ToList();
			var illy = a.Count;
			if (illy >= 1)
			{
				for (int i = 0; i < illy; ++i)
				{
					if (a[i].Distance2D(E) <= a[i].GetAttackRange() + a[i].HullRadius && !E.IsAttackImmune()
						&& !a[i].IsAttacking() && a[i].CanAttack() && Utils.SleepCheck(a[i].Handle + "Attack")
						)
					{
						a[i].Attack(E);
						Utils.Sleep(330, a[i].Handle + "Attack");
					}
					else if (a[i].Distance2D(E) <= 1000)
					{
						if ((!a[i].CanAttack() || a[i].Distance2D(E) >= 0)
						   && !a[i].IsAttacking()
						   && a[i].CanMove() && Utils.SleepCheck(a[i].Handle + "Move")
						   )
						{
							a[i].Move(Prediction.InFront(E, 100));
							Utils.Sleep(400, a[i].Handle + "Move");
						}
					}
				}
			}
			if (Active)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
		    if (!Active || !(Me.Distance2D(E) <= 1400) || E == null || !E.IsAlive || Toolset.InvUnit(Me)) return;
		    {
		        var angle = Me.FindAngleBetween(E.Position, true);
		        var pos = new Vector3((float) (E.Position.X - 70*Math.Cos(angle)), (float) (E.Position.Y - 70*Math.Sin(angle)),
		            0);
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
		        if (
		            _q != null
		            && _q.CanBeCasted()
		            && Me.Distance2D(E) <= 900
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
		            && Utils.SleepCheck("Q")
		        )
		        {
		            _q.UseAbility(E);
		            Utils.Sleep(200, "Q");
		        }
		        if (_armlet != null
		            && !_armlet.IsToggled
		            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_armlet.Name) &&
		            Utils.SleepCheck("armlet"))
		        {
		            _armlet.ToggleAbility();
		            Utils.Sleep(300, "armlet");
		        }
		        if ((_manta != null
		             && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name))
		            && _manta.CanBeCasted() && Me.IsSilenced() && Utils.SleepCheck("manta"))
		        {
		            _manta.UseAbility();
		            Utils.Sleep(400, "manta");
		        }
		        if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name))
		            && _manta.CanBeCasted() && (E.Position.Distance2D(Me.Position) <= Me.GetAttackRange() + Me.HullRadius)
		            && Utils.SleepCheck("manta"))
		        {
		            _manta.UseAbility();
		            Utils.Sleep(150, "manta");
		        }
		        if (
		            _r != null
		            && _r.CanBeCasted()
		            && Me.Distance2D(E) <= 300
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
		            && (_armlet == null || _armlet.IsToggled)
		            && Utils.SleepCheck("R")
		        )
		        {
		            _r.UseAbility();
		            Utils.Sleep(200, "R");
		        }
		        if (
		            _w != null
		            && _w.CanBeCasted()
		            && (Me.Distance2D(E) <= _w.GetCastRange() + 300
		                && (Me.Distance2D(E) >= Me.GetAttackRange()
		                    || Me.Distance2D(E) <= Me.GetAttackRange()
		                    && E.NetworkActivity == NetworkActivity.Attack)
		            )
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
		            && Utils.SleepCheck("W")
		        )
		        {
		            _w.UseAbility(E);
		            Utils.Sleep(200, "W");
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

		        if (_shiva != null
		            && _shiva.CanBeCasted()
		            && Me.Distance2D(E) <= 600
		            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
		            && !E.IsMagicImmune()
		            && Utils.SleepCheck("Shiva"))
		        {
		            _shiva.UseAbility();
		            Utils.Sleep(100, "Shiva");
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
		        if (_urn != null
		            && _urn.CanBeCasted()
		            && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
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
		            Me.Health <= (Me.MaximumHealth*0.3) &&
		            _satanic.CanBeCasted() &&
		            Me.Distance2D(E) <= Me.GetAttackRange() + Me.HullRadius
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

			Print.LogMessage.Success("Rogue Knight at your service!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"chaos_knight_chaos_bolt", true},
				    {"chaos_knight_reality_rift", true},
				    {"chaos_knight_phantasm", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_blink", true},
				    {"item_armlet", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true},
				    {"item_manta", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}