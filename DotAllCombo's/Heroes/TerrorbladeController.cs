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

	internal class TerrorbladeController : Variables, IHeroController
	{
		private Item _soulring, _arcane, _blink, _shiva, _dagon, _mjollnir, _mom, _halberd, _abyssal, _ethereal, _cheese, _satanic, _medall, _mail, _orchid, _bkb, _phase, _sheep, _manta;
		private Ability _q;
	    private Ability _w;
	    private Ability _e;
	    private Ability _r;

	    private readonly Menu _skills = new Menu("Skills", "Skills");
		private readonly Menu _items = new Menu("Items", "Items");
		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			//spell
			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_e = Me.Spellbook.SpellE;
			_r = Me.Spellbook.SpellR;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			var s = ObjectManager.GetEntities<Hero>()
		   .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsIllusion && x.Distance2D(Me) <= 1500).ToList();
			if (Active)
            {
                E = Toolset.ClosestToMouse(Me);
                if (E == null) return;
				if (E.IsAlive && !E.IsInvul())
				{
					if (Me.IsAlive && Me.Distance2D(E) <= 1900)
					{
						// item 
						_satanic = Me.FindItem("item_satanic");
						_shiva = Me.FindItem("item_shivas_guard");
						_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
						_arcane = Me.FindItem("item_arcane_boots");
						_mom = Me.FindItem("item_mask_of_madness");
						_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
						_ethereal = Me.FindItem("item_ethereal_blade");
						_blink = Me.FindItem("item_blink");
						_soulring = Me.FindItem("item_soul_ring");
						_cheese = Me.FindItem("item_cheese");
						_halberd = Me.FindItem("item_heavens_halberd");
						_abyssal = Me.FindItem("item_abyssal_blade");
						_mjollnir = Me.FindItem("item_mjollnir");
						_manta = Me.FindItem("item_manta");
						_mail = Me.FindItem("item_blade_mail");
						_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
						_bkb = Me.FindItem("item_black_king_bar");
						_phase = Me.FindItem("item_phase_boots");
						_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
						var linkens = E.IsLinkensProtected();

						if (
							(_r.CanBeCasted()
							 || Me.Health >= (Me.MaximumHealth * 0.4))
							&& _blink.CanBeCasted()
							&& Me.Position.Distance2D(E) > 300
							&& Me.Position.Distance2D(E) < 1180
							&& Utils.SleepCheck("blink"))
						{
							_blink.UseAbility(E.Position);
							Utils.Sleep(250, "blink");
						}
						if (
							_q.CanBeCasted()
							&& ((_dagon != null && !_dagon.CanBeCasted())
								|| (_r.CanBeCasted()
									&& Me.Health >= (Me.MaximumHealth * 0.3))
								|| (!_r.CanBeCasted() && Me.Health <= (Me.MaximumHealth * 0.3)))
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
							&& Me.Position.Distance2D(E) < _q.GetCastRange() &&
							Utils.SleepCheck("Q"))
						{
							_q.UseAbility();
							Utils.Sleep(150, "Q");
						}
						if (_e.CanBeCasted()
							&& !_q.CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
							&& Me.Position.Distance2D(E) < Me.GetAttackRange()
							&& Utils.SleepCheck("_e"))
						{
							_e.UseAbility();
							Utils.Sleep(150, "_e");
						}
						if (_w.CanBeCasted()
							&& !_q.CanBeCasted()
							&& (_e == null || !_e.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name))
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
							&& Me.Position.Distance2D(E) < Me.GetAttackRange()
							&& Utils.SleepCheck("W"))
						{
							_w.UseAbility();
							Utils.Sleep(150, "W");
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
						var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
						if (_phase != null
							&& _phase.CanBeCasted()
							&& Utils.SleepCheck("phase")
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_phase.Name)
							&& !_blink.CanBeCasted()
							&& Me.Distance2D(E) >= Me.GetAttackRange() + 20)
						{
							_phase.UseAbility();
							Utils.Sleep(200, "phase");
						}
						if ( // Dagon
							_dagon != null
							&& (E.Health <= (E.MaximumHealth * 0.4)
								|| !_r.CanBeCasted())
							&& _dagon.CanBeCasted()
							&& Me.CanCast()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled("item_dagon")
							&& !E.IsMagicImmune() &&
							Utils.SleepCheck("dagon"))
						{
							_dagon.UseAbility(E);
							Utils.Sleep(150, "dagon");
						} // Dagon Item end
						if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)) &&
							_manta.CanBeCasted() && Me.IsSilenced() && Utils.SleepCheck("manta"))
						{
							_manta.UseAbility();
							Utils.Sleep(400, "manta");
						}
						if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)) &&
							_manta.CanBeCasted() && (E.Position.Distance2D(Me.Position) <= Me.GetAttackRange() + Me.HullRadius) 
							&& (_e==null || !_e.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name))
							&& Utils.SleepCheck("manta"))
						{
							_manta.UseAbility();
							Utils.Sleep(150, "manta");
						}
						if ( // orchid
							_orchid != null
							&& _orchid.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsLinkensProtected()
							&& !E.IsMagicImmune()
							&& Me.Distance2D(E) <= Me.GetAttackRange()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
							&& !stoneModif
							&& Utils.SleepCheck("orchid")
							)
						{
							_orchid.UseAbility(E);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (_ethereal != null
							&& _ethereal.CanBeCasted()
							&& Me.Distance2D(E) <= 700
							&& Me.Distance2D(E) <= 400
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
							&& Utils.SleepCheck("ethereal"))
						{
							_ethereal.UseAbility(E);
							Utils.Sleep(100, "ethereal");
						}
						var q = (_q != null && _q.Cooldown <= 0 && _q.ManaCost < Me.Mana);
						var w = (_w != null && _w.Cooldown <= 0 && _w.ManaCost < Me.Mana);
						var e = (_e != null && _e.Cooldown <= 0 && _e.ManaCost < Me.Mana);
						var r = (_r != null && _r.Cooldown <= 0 && _r.ManaCost < Me.Mana && _r.Level <= 2);
						var d = (_dagon != null && _dagon.Cooldown <= 0 && _dagon.ManaCost < Me.Mana);


						if ( // SoulRing Item 
							_soulring != null &&
							Me.Health >= (Me.MaximumHealth * 0.3)
							&& (q || w || e || d)
							&& _soulring.CanBeCasted()
							&& Utils.SleepCheck("soulring"))
						{
							_soulring.UseAbility();
							Utils.Sleep(150, "soulring");
						} // SoulRing Item end

						if ( // Arcane Boots Item
							_arcane != null
							&& (q || w || e || d)
							&& _arcane.CanBeCasted()
							&& Utils.SleepCheck("arcane"))
						{
							_arcane.UseAbility();
							Utils.Sleep(150, "arcane");
						} // Arcane Boots Item end

						if ( // Shiva Item
							_shiva != null
							&& _shiva.CanBeCasted()
							&& Me.CanCast()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
							&& !E.IsMagicImmune()
							&& (_shiva.CanBeCasted()
								&& Utils.SleepCheck("shiva")
								&& Me.Distance2D(E) <= 600)
							)
						{
							_shiva.UseAbility();
							Utils.Sleep(250, "shiva");
						} // Shiva Item end
						if (_mail != null && _mail.CanBeCasted() && (s.Count(x => x.Distance2D(Me) <= 650) >=
																   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
						{
							_mail.UseAbility();
							Utils.Sleep(100, "mail");
						}
						if ( // Medall
							_medall != null
							&& _medall.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsMagicImmune()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
							&& Utils.SleepCheck("Medall")
							&& Me.Distance2D(E) <= Me.GetAttackRange() + Me.HullRadius
							)
						{
							_medall.UseAbility(E);
							Utils.Sleep(250, "Medall");
						} // Medall Item end

						if ( // MOM
							_mom != null
							&& _mom.CanBeCasted()
							&& Me.CanCast()
							&& Utils.SleepCheck("mom")
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
							&& Me.Distance2D(E) <= Me.GetAttackRange() + Me.HullRadius
							)
						{
							_mom.UseAbility();
							Utils.Sleep(250, "mom");
						} // MOM Item end
						
						if ( // Abyssal Blade
							_abyssal != null
							&& _abyssal.CanBeCasted()
							&& Me.CanCast()
							&& !linkens
							&& Utils.SleepCheck("abyssal")
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
							&& Me.Distance2D(E) <= 400
							)
						{
							_abyssal.UseAbility(E);
							Utils.Sleep(250, "abyssal");
						} // Abyssal Item end

						if ( // Hellbard
							_halberd != null
							&& _halberd.CanBeCasted()
							&& Me.CanCast()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
							&& !E.IsMagicImmune()
							&& Utils.SleepCheck("halberd")
							&& Me.Distance2D(E) <= 700
							)
						{
							_halberd.UseAbility(E);
							Utils.Sleep(250, "halberd");
						} // Hellbard Item end

						if ( // Mjollnir
							_mjollnir != null
							&& _mjollnir.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsMagicImmune()
							&& Utils.SleepCheck("mjollnir")
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
							&& Me.Distance2D(E) <= 900
							)
						{
							_mjollnir.UseAbility(Me);
							Utils.Sleep(250, "mjollnir");
						} // Mjollnir Item end

						if ( // Satanic 
							_satanic != null
							&& Me.Health <= (Me.MaximumHealth * 0.4)
							&& (!_r.CanBeCasted() || Me.IsSilenced()
								|| E.Health <= (E.MaximumHealth * 0.4))
							&& _satanic.CanBeCasted()
							&& Me.Distance2D(E) <= Me.GetAttackRange() + Me.HullRadius
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
							&& Utils.SleepCheck("Satanic")
							)
						{
							_satanic.UseAbility();
							Utils.Sleep(350, "Satanic");
						} // Satanic Item end
						if ( // cheese
							_cheese != null
							&& _cheese.CanBeCasted()
							&& Me.Health <= (Me.MaximumHealth * 0.3)
							&& (!_r.CanBeCasted()
								|| !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name))
							&& Me.Distance2D(E) <= Me.GetAttackRange() + Me.HullRadius
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
							&& Utils.SleepCheck("cheese")
							)
						{
							_cheese.UseAbility();
							Utils.Sleep(200, "cheese");
						} // cheese Item end
						if (_bkb != null && _bkb.CanBeCasted() && (s.Count(x => x.Distance2D(Me) <= 650) >=
																 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("bkb"))
						{
							_bkb.UseAbility();
							Utils.Sleep(100, "bkb");
						}

						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
					var illu = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassId == ClassId.CDOTA_Unit_Hero_Terrorblade && x.IsIllusion)
										&& x.IsAlive && x.IsControllable).ToList();
					if (illu.Count > 0)
					{
						foreach (var v in illu)
						{
							if (
						   v.Distance2D(E) <= v.GetAttackRange() + v.HullRadius + 24 && !E.IsAttackImmune()
						   && v.NetworkActivity != NetworkActivity.Attack && v.CanAttack() && Utils.SleepCheck(v.Handle.ToString())
						   )
							{
								v.Attack(E);
								Utils.Sleep(270, v.Handle.ToString());
							}
							if (
							(!v.CanAttack() || v.Distance2D(E) >= 0) && !v.IsAttacking()
							&& v.NetworkActivity != NetworkActivity.Attack &&
							v.Distance2D(E) <= 1200 && Utils.SleepCheck(v.Handle.ToString())
							)
							{
								v.Move(E.Predict(400));
								Utils.Sleep(400, v.Handle.ToString());
							}
						}
					}
				}
			}
			if (Menu.Item("ult").GetValue<bool>())
			{
				if (Me == null || !Me.IsAlive) return;
				if (_r == null || !_r.CanBeCasted()) return;
				var ult = ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.Team != Me.Team
					&& !x.IsIllusion
					&& x.Distance2D(Me) <= _r.GetCastRange() + Me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				var illu = ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.IsIllusion
					&& x.Distance2D(Me) <= _r.GetCastRange() + Me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				var ally = ObjectManager.GetEntities<Hero>()
					.Where(x =>
					x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.Team == Me.Team
					&& !x.IsIllusion
					&& x.Distance2D(Me) <= _r.GetCastRange() + Me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				if (ult != null && Menu.Item("ultEnem").GetValue<bool>())
				{
					var linkens = ult.IsLinkensProtected();
					if (!linkens
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						&& ult.Health > (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
						&& Me.Health < (Me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
					{
						_r.UseAbility(ult);
						Utils.Sleep(500, "R");
					}
				}
				if (ult == null || ult.Health < (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value))
				{
					if (illu != null && Menu.Item("ultIllu").GetValue<bool>())
					{
						if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
							&& illu.Health > (illu.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
							&& Me.Health < (Me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
						{
							_r.UseAbility(illu);
							Utils.Sleep(500, "R");
						}
					}
				}
				if (ult == null || ult.Health < (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value) || illu == null || illu.Health < (illu.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value))
				{
					if (ally != null && Menu.Item("ultAlly").GetValue<bool>())
					{
						if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
							&& ally.Health > (ally.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
							&& Me.Health < (Me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
						{
							_r.UseAbility(ally);
							Utils.Sleep(500, "R");
						}
					}
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1a");

			Print.LogMessage.Success("The self-righteous shall choke on their sanctimony.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Key Combo").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(_skills);
			Menu.AddSubMenu(_items);
			_skills.AddItem(new MenuItem("Skills", "Skills:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{   {"terrorblade_reflection",true},
				{"terrorblade_conjure_image",true},
				{"terrorblade_metamorphosis",true},
				{"terrorblade_sunder",true}
			})));
			_items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_phase_boots", true},
				{"item_cheese", true},
				{"item_blade_mail",true},
				{"item_black_king_bar",true},
				{"item_mjollnir",true},
				{"item_satanic", true},
				{"item_heavens_halberd",true},
				{"item_sheepstick",true},
				{"item_manta", true}
			})));
			_items.AddItem(new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_mask_of_madness", true},
				{"item_dagon",true},
				{"item_orchid",true},
				{"item_bloodthorn", true},
				{"item_ethereal_blade",true},
				{"item_shivas_guard",true},
				{"item_abyssal_blade", true},
				{"item_medallion_of_courage", true},
				{"item_solar_crest", true}
			})));

			Menu.AddItem(new MenuItem("ult", "Auto Ult").SetValue(true));

			Menu.AddItem(new MenuItem("ultEnem", "Use Ult in Enemy").SetValue(true));

			Menu.AddItem(new MenuItem("heal", "Min target Healt % to Ult").SetValue(new Slider(50, 1)));

			Menu.AddItem(new MenuItem("healme", "Min Me Healt % to Ult").SetValue(new Slider(30, 1)));
			Menu.AddItem(new MenuItem("ultIllu", "Use Ult in Illusion if is no suitable enemy").SetValue(true));
			Menu.AddItem(new MenuItem("ultAlly", "Use Ult in Ally if is no suitable enemy and Illusion").SetValue(false));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{

		}
	}
}
