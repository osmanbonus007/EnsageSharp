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

	internal class JuggernautController : Variables, IHeroController
	{
		private static Ability _w, _r;

		private Item _urn, _ethereal, _dagon, _halberd, _mjollnir, _orchid, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _medall, _manta;
		private readonly Menu _ult = new Menu("Ult", "Ult");

		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			if (Menu.Item("keyW").GetValue<KeyBind>().Active)
			{
				var healingWard = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Additive)
					&& x.IsAlive && x.IsControllable && x.Team == Me.Team).ToList();
				if (healingWard.Count >= 1)
				{
					for (int i = 0; i < healingWard.Count(); ++i)
					{
						if (Me.Position.Distance2D(healingWard[i].Position) > 5 && Utils.SleepCheck(healingWard[i].Handle.ToString()))
						{
							healingWard[i].Move(Me.Predict(310));
							Utils.Sleep(50, healingWard[i].Handle.ToString());
						}
					}
				}
			}
		    _w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;
			
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_ethereal = Me.FindItem("item_ethereal_blade");
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
			_manta = Me.FindItem("item_manta");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
            E = Toolset.ClosestToMouse(Me);

            if (E == null) return;
			if (Active && Me.Distance2D(E) <= 1400 && Me.HasModifier("modifier_juggernaut_blade_fury") && Utils.SleepCheck("move"))
			{
				Me.Move(Prediction.InFront(E, 170));
				Utils.Sleep(150, "move");
			}
			if (Active && Me.Distance2D(E) <= 1400)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && !Me.HasModifier("modifier_juggernaut_blade_fury"))
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
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
				if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 900 &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) && Utils.SleepCheck("orchid"))
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
				if (_r != null && _r.CanBeCasted() && Me.Distance2D(E) <= 600 && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name))
				{
					var creep = ObjectManager.GetEntities<Creep>().Where(x => x.IsAlive && x.Team != Me.Team && x.IsSpawned).ToList();

					for (int i = 0; i < creep.Count(); i++)
					{
						if (creep.Count(x => x.Distance2D(Me) <= Menu.Item("Heel").GetValue<Slider>().Value) <=
															 (Menu.Item("Healh").GetValue<Slider>().Value)
						&& Utils.SleepCheck("R")
						)
						{
							_r.UseAbility(E);
							Utils.Sleep(200, "R");
						}
					}
					if (creep == null)
					{
						if (
							Utils.SleepCheck("R")
												)
						{
							_r.UseAbility(E);
							Utils.Sleep(200, "R");
						}
					}
				}
				if (
					  _w != null && _w.CanBeCasted() && Me.Distance2D(E) <= E.AttackRange + E.HullRadius+24
					  && Me.Health <= (Me.MaximumHealth * 0.4)
					  && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					  && Utils.SleepCheck("W")
					  )
				{
					_w.UseAbility(Me.Position);
					Utils.Sleep(200, "W");
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

			Print.LogMessage.Success("Put Me in the vanguard.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("keyW", "Controll Ward Key").SetValue(new KeyBind('J', KeyBindType.Toggle)));
			_ult.AddItem(new MenuItem("Heal", "Min Radius Distance Creeps to ult").SetValue(new Slider(300, 10, 425)));
			_ult.AddItem(new MenuItem("Healh", "Max Count Creeps to ult").SetValue(new Slider(3, 1, 10)));
			Menu.AddSubMenu(_ult);
		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"juggernaut_blade_fury", true},
				    {"juggernaut_healing_ward", true},
				    {"juggernaut_omni_slash", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    { "item_bloodthorn", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
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