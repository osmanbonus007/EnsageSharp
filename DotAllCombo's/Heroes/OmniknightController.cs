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

	internal class OmniknightController : Variables, IHeroController
	{
		private Ability _q, _w, _r;

#pragma warning disable CS0649 // Field 'OmniknightController.urn' is never assigned to, and will always have its default value null
		private Item _urn, _ethereal, _dagon, _halberd, _mjollnir, _orchid, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, 
#pragma warning restore CS0649 // Field 'OmniknightController.urn' is never assigned to, and will always have its default value null
            _medall, _glimmer, _manta, _pipe, _guardian, _sphere;

        

		private Menu _items = new Menu("Items", "Items");
		private Menu _heal = new Menu("Heal", "Heal Items Settings");
		private Menu _ult = new Menu("AutoUlt", "AutoUlt");

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;

			_mom = Me.FindItem("item_mask_of_madness");
			_glimmer = Me.FindItem("item_glimmer_cape");
			_manta = Me.FindItem("item_manta");
			_pipe = Me.FindItem("item_pipe");
			_guardian = Me.FindItem("item_guardian_greaves") ?? Me.FindItem("item_mekansm");
			_sphere = Me.FindItem("item_sphere");
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
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					.ToList();
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;
			if (Active && Me.Distance2D(E) <= 1400 && E.IsAlive && !Me.IsInvisible())
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
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
					&& Me.Distance2D(E) <= 700
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
				var ally = ObjectManager.GetEntities<Hero>()
										.Where(x => x.Team == Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();

				var countAlly = ally.Count();
				var countV = v.Count();
				for (int i = 0; i < countAlly; ++i)
				{
					if (
							_q != null && _q.CanBeCasted() 
							&& !Me.IsMagicImmune()
							&& Me.Health <= (Me.MaximumHealth * 0.6)
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
							&& Utils.SleepCheck("Q")
							)
					{
						_q.UseAbility(Me);
						Utils.Sleep(200, "Q");
					}
					if (
							_q != null && _q.CanBeCasted()
							&& Me.Distance2D(E) <= 255
							&& !Me.IsMagicImmune()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
							&& Utils.SleepCheck("Q")
							)
					{
						_q.UseAbility(Me);
						Utils.Sleep(200, "Q");
					}
					
						if (
							_w != null 
							&& _w.CanBeCasted() 
							&& Me.Distance2D(E) <=400
							&& !_q.CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							&& Utils.SleepCheck("W")
							)
					{
						_w.UseAbility(Me);
						Utils.Sleep(200, "W");
					}
					for (int z = 0; z < countV; ++z)
					{

						if (
							_q != null && _q.CanBeCasted() 
							&& Me.Distance2D(ally[i]) <= _q.GetCastRange() + 50
							&& !ally[i].IsMagicImmune()
							&& ally[i].Health <= (ally[i].MaximumHealth * 0.6)
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
							&& Utils.SleepCheck("Q")
							)
						{
							_q.UseAbility(ally[i]);
							Utils.Sleep(200, "Q");
						}
						else
						if (
							_w != null && _w.CanBeCasted() 
							&& !_q.CanBeCasted()
							&& Me.Distance2D(ally[i]) <= _w.GetCastRange() + 50
							&& ally[i].Health <= (ally[i].MaximumHealth * 0.6) 
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							&& Utils.SleepCheck("Wq")
							)
						{
							_w.UseAbility(ally[i]);
							Utils.Sleep(200, "Wq");
						}
						if (
							_w != null 
							&& _w.CanBeCasted() 
							&& Me.Distance2D(ally[i]) <= _w.GetCastRange() + 50
							&& !ally[i].IsMagicImmune()
							&& ((ally[i].Distance2D(v[z]) <= ally[i].AttackRange + ally[i].HullRadius + 10)
							|| (ally[i].Distance2D(v[z]) <= v[i].AttackRange + ally[i].HullRadius + 10))
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							&& Utils.SleepCheck("Ww")
							)
						{
							_w.UseAbility(ally[i]);
							Utils.Sleep(200, "Ww");
						}
						if (
							_q != null && _q.CanBeCasted() && Me.Distance2D(ally[i]) <= _q.GetCastRange() + 50
							&& !ally[i].IsMagicImmune()
							&& ally[i].Distance2D(v[z]) <= 250 + ally[i].HullRadius - 10
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
							&& Utils.SleepCheck("Q")
							)
						{
							_q.UseAbility(ally[i]);
							Utils.Sleep(200, "Q");
						}
						if (
							_r != null && _r.CanBeCasted()
							&& Me.Distance2D(ally[i]) <= _r.GetCastRange() + 50
							&& (v.Count(x => x.Distance2D(Me) <= _r.GetCastRange()) >= (Menu.Item("UltCountTarget").GetValue<Slider>().Value))
							&& (ally.Count(x => x.Distance2D(Me) <= _r.GetCastRange()) >= (Menu.Item("UltCountAlly").GetValue<Slider>().Value))
							&& ally[i].Health <= (ally[i].MaximumHealth / 100 * (Menu.Item("HealhUlt").GetValue<Slider>().Value))
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
							&& Utils.SleepCheck("R")
							)
						{
							_r.UseAbility();
							Utils.Sleep(200, "R");
						}
						if (
							_guardian != null && _guardian.CanBeCasted()
							&& Me.Distance2D(ally[i]) <= _guardian.GetCastRange()
							&& (v.Count(x => x.Distance2D(Me) <= _guardian.GetCastRange()) >= (Menu.Item("healsetTarget").GetValue<Slider>().Value))
							&& (ally.Count(x => x.Distance2D(Me) <= _guardian.GetCastRange()) >= (Menu.Item("healsetAlly").GetValue<Slider>().Value))
							&& ally[i].Health <= (ally[i].MaximumHealth / 100 * (Menu.Item("HealhHeal").GetValue<Slider>().Value))
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(_guardian.Name)
							&& Utils.SleepCheck("guardian")
							)
						{
							_guardian.UseAbility();
							Utils.Sleep(200, "guardian");
						}
						if (
							_pipe != null && _pipe.CanBeCasted()
							&& Me.Distance2D(ally[i]) <= _pipe.GetCastRange()
							&& (v.Count(x => x.Distance2D(Me) <= _pipe.GetCastRange()) >= (Menu.Item("pipesetTarget").GetValue<Slider>().Value))
							&& (ally.Count(x => x.Distance2D(Me) <= _pipe.GetCastRange()) >= (Menu.Item("pipesetAlly").GetValue<Slider>().Value))
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(_pipe.Name)
							&& Utils.SleepCheck("pipe")
							)
						{
							_pipe.UseAbility();
							Utils.Sleep(200, "pipe");
						}

						if (
							_sphere != null && _sphere.CanBeCasted() && Me.Distance2D(ally[i]) <= _sphere.GetCastRange() + 50
							&& !ally[i].IsMagicImmune()
							&& ((ally[i].Distance2D(v[z]) <= ally[i].AttackRange + ally[i].HullRadius + 10)
							|| (ally[i].Distance2D(v[z]) <= v[i].AttackRange + ally[i].HullRadius + 10)
							|| ally[i].Health <= (Me.MaximumHealth * 0.5))
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(_sphere.Name)
							&& Utils.SleepCheck("sphere")
							)
						{
							_sphere.UseAbility(ally[i]);
							Utils.Sleep(200, "sphere");
						}
						if (
							_glimmer != null && _glimmer.CanBeCasted() && Me.Distance2D(ally[i]) <= _glimmer.GetCastRange() + 50
							&& ally[i].Health <= (Me.MaximumHealth * 0.5)
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(_glimmer.Name)
							&& Utils.SleepCheck("glimmer")
							)
						{
							_glimmer.UseAbility(ally[i]);
							Utils.Sleep(200, "glimmer");
						}
						if (
							_manta != null && _manta.CanBeCasted()
							&& (Me.Distance2D(v[z]) <= Me.AttackRange + Me.HullRadius + 10)
							|| (Me.Distance2D(v[z]) <= v[i].AttackRange + Me.HullRadius + 10)
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(_manta.Name)
							&& Utils.SleepCheck("manta")
							)
						{
							_manta.UseAbility();
							Utils.Sleep(200, "manta");
						}
					}
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Where piety fails, my hammer falls.");

		    Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"omniknight_guardian_angel", true},
				    {"omniknight_purification", true},
				    {"omniknight_repel", true}
				})));
			_items.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
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
			_items.AddItem(
				new MenuItem("ItemsS", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_manta", true},
				    {"item_mekansm", true},
				    {"item_pipe", true},
				    {"item_guardian_greaves", true},
				    {"item_sphere", true},
				    {"item_glimmer_cape", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min Target's to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min Target's to BladeMail").SetValue(new Slider(2, 1, 5)));
			_ult.AddItem(new MenuItem("UltCountTarget", "Min Target's to ToUlt").SetValue(new Slider(2, 1, 5)));
			_ult.AddItem(new MenuItem("HealhUlt", "Min healh Ally % to Ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			_ult.AddItem(new MenuItem("UltCountAlly", "Min Ally to ToUlt").SetValue(new Slider(2, 1, 5)));
			_heal.AddItem(new MenuItem("pipesetTarget", "Min Target's to Pipe").SetValue(new Slider(2, 1, 5)));
			_heal.AddItem(new MenuItem("pipesetAlly", "Min Ally to Pipe").SetValue(new Slider(2, 1, 5)));
			_heal.AddItem(new MenuItem("healsetTarget", "Min Target's to Meka|Guardian").SetValue(new Slider(2, 1, 5)));
			_heal.AddItem(new MenuItem("healsetAlly", "Min Ally to Meka|Guardian").SetValue(new Slider(2, 1, 5)));
			_heal.AddItem(new MenuItem("HealhHeal", "Min healh % to Heal").SetValue(new Slider(35, 10, 70))); // x/ 10%
			Menu.AddSubMenu(_items);
			Menu.AddSubMenu(_heal);
			Menu.AddSubMenu(_ult);
		}

		public void OnCloseEvent()
		{
			
		}
	}
}