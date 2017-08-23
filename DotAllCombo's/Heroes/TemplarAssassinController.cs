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

	internal class TemplarAssassinController : Variables, IHeroController
	{
		private Ability _q, _w, _r;
		private Item _urn, _dagon, _phase, _cheese, _halberd, _ethereal,
					_mjollnir, _orchid, _abyssal, _stick, _mom, _shiva, _mail, _bkb, _satanic, _medall, _blink, _sheep, _manta, _pike;
		
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Well that's it. The secret is out!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"templar_assassin_meld", true},
				    {"templar_assassin_refraction", true},
				    {"templar_assassin_psionic_trap", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_ethereal_blade", true},
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
				    {"item_bloodthorn", true},
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
			       {"item_hurricane_pike", true},
			       {"item_mask_of_madness", true},
			       {"item_sheepstick", true},
			       {"item_cheese", true},
			       {"item_magic_stick", true},
			       {"item_magic_wand", true},
			       {"item_manta", true},
			       {"item_mjollnir", true},
			       {"item_satanic", true},
			       {"item_phase_boots", true}
			   })));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("piKe", "Min targets healh use hurricane pike").SetValue(new Slider(35, 20)));
			Menu.AddItem(new MenuItem("piKeMe", "Min Me healh use hurricane pike").SetValue(new Slider(45, 20)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}
		public void Combo()
		{
			if (Menu.Item("enabled").IsActive() && Utils.SleepCheck("combo"))
			{

                E = Toolset.ClosestToMouse(Me);
                if (E == null)
					return;
				_q = Me.Spellbook.SpellQ;
				_w = Me.Spellbook.SpellW;
				_r = Me.Spellbook.SpellR;
				Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

				_shiva = Me.FindItem("item_shivas_guard");
				_pike = Me.FindItem("item_hurricane_pike");
				var dragon = Me.FindItem("item_dragon_lance");
				
				_ethereal = Me.FindItem("item_ethereal_blade");
				_mom = Me.FindItem("item_mask_of_madness");
				_urn = Me.FindItem("item_urn_of_shadows");
				_dagon = Me.Inventory.Items.FirstOrDefault(
						item => item.Name.Contains("item_dagon"));
				_halberd = Me.FindItem("item_heavens_halberd");
				_mjollnir = Me.FindItem("item_mjollnir");
				_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
				_abyssal = Me.FindItem("item_abyssal_blade");
				_mail = Me.FindItem("item_blade_mail");
				_manta = Me.FindItem("item_manta");
				_bkb = Me.FindItem("item_black_king_bar");
				_satanic = Me.FindItem("item_satanic");
				_blink = Me.FindItem("item_blink");
				_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
				_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
				_cheese = Me.FindItem("item_cheese");
				_stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
				_phase = Me.FindItem("item_phase_boots");

				var meld = Me.Modifiers.ToList().Exists(y => y.Name == "modifier_templar_assassin_meld");
				var pikeMod = Me.Modifiers.ToList().Exists(y => y.Name == "modifier_item_hurricane_pike_range");
			    Toolset.Range();
			    var stoneModif = E.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
				var v =
					ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
						.ToList();

				var pikeRange = Me.Modifiers.FirstOrDefault(y => y.Name == "modifier_item_hurricane_pike_range");
				if (pikeRange != null)
				{
					if (
						_q != null
						&& _q.CanBeCasted()
						&& !meld
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility();
						Utils.Sleep(200, "Q");
					}
					if (
						_w != null
						&& _q != null
						&& pikeRange.StackCount <= 3
						&& !_q.CanBeCasted()
						&& _w.CanBeCasted()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
						&& Utils.SleepCheck("W")
						)
					{
						_w.UseAbility();
						Me.Attack(E);
						Utils.Sleep(200, "W");
					}
					if (Menu.Item("orbwalk").GetValue<bool>())
					{
						Orbwalking.Orbwalk(E, 0, 7000, true, true);
					}
				}
				if (pikeRange != null && pikeRange.StackCount > 0) return;
				if (pikeMod) return;
				if (Active)
				{
					if (meld)
					{
						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
				}

				if (meld) return;

				if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
				{
					if (_r != null
						&& (_pike == null
						|| !_pike.CanBeCasted())
                        && Me.Distance2D(E)>=Toolset.AttackRange+5
						&& _r.CanBeCasted()
						&& !meld
						&& Utils.SleepCheck("R")
						&& !E.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow")
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name))
					{
						_r.UseAbility(Prediction.InFront(E, 140));
						Utils.Sleep(150, "R");
					}
					if (
						_q != null && _q.CanBeCasted() 
						&& Me.Distance2D(E) <= Toolset.AttackRange+300
						&& !meld
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility();
						Utils.Sleep(200, "Q");
					}
					float angle = Me.FindAngleBetween(E.Position, true);
					Vector3 pos = new Vector3((float)(E.Position.X + 100 * Math.Cos(angle)), (float)(E.Position.Y + 100 * Math.Sin(angle)), 0);
					if (
						_blink != null
						&& Me.CanCast()
						&& _blink.CanBeCasted()
						&& Me.Distance2D(E) >= Toolset.AttackRange
						&& Me.Distance2D(pos) <= 1190
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						_blink.UseAbility(pos);
						Utils.Sleep(250, "blink");
					}
					if (
						_w != null && _w.CanBeCasted() && Me.Distance2D(E) <= Toolset.AttackRange-10
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
						&& Utils.SleepCheck("W")
						)
					{
						_w.UseAbility();
						Me.Attack(E);
						Utils.Sleep(200, "W");
					}
					if ( // MOM
						_mom != null
						&& _mom.CanBeCasted()
						&& Me.CanCast()
						&& !meld
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
						&& !meld
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
						)
					{
						_halberd.UseAbility(E);
						Utils.Sleep(250, "halberd");
					}
					if ( // Mjollnir
						_mjollnir != null
						&& _mjollnir.CanBeCasted()
						&& Me.CanCast()
						&& !meld
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
						&& !meld
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
						&& !meld
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
						&& !meld
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
						&& !meld
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
					if (_orchid != null 
						&& _orchid.CanBeCasted()
						&& !meld
						&& Me.Distance2D(E) <= 600
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) &&
						Utils.SleepCheck("orchid"))
					{
						_orchid.UseAbility(E);
						Utils.Sleep(100, "orchid");
					}

					if (_shiva != null 
						&& _shiva.CanBeCasted() 
						&& Me.Distance2D(E) <= 600
						&& !meld
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
						&& !meld
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


					if ( // Dagon
						Me.CanCast()
						&& _dagon != null
						&& (_ethereal == null
						|| (E.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
						|| _ethereal.Cooldown < 17))
						&& !E.IsLinkensProtected()
						&& _dagon.CanBeCasted()
						&& !meld
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
						&& !meld
						&& Utils.SleepCheck("phase")
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_phase.Name)
						&& !_blink.CanBeCasted()
						&& Me.Distance2D(E) >= Me.AttackRange + 20)
					{
						_phase.UseAbility();
						Utils.Sleep(200, "phase");
					}
					if (_urn != null 
						&& _urn.CanBeCasted() 
						&& _urn.CurrentCharges > 0 
						&& Me.Distance2D(E) <= 400
						&& !meld
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) 
						&& Utils.SleepCheck("urn"))
					{
						_urn.UseAbility(E);
						Utils.Sleep(240, "urn");
					}
					if (
						_stick != null
						&& _stick.CanBeCasted()
						&& _stick.CurrentCharges != 0
						&& Me.Distance2D(E) <= 700
						&& !meld
						&& (Me.Health <= (Me.MaximumHealth * 0.5)
							|| Me.Mana <= (Me.MaximumMana * 0.5))
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_stick.Name))
					{
						_stick.UseAbility();
						Utils.Sleep(200, "mana_items");
					}
					if (_manta != null
						&& _manta.CanBeCasted()
						&& !meld
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)
						&& Me.Distance2D(E) <= Toolset.AttackRange
						&& Utils.SleepCheck("manta"))
					{
						_manta.UseAbility();
						Utils.Sleep(100, "manta");
					}
					if ( // Satanic 
						_satanic != null &&
						Me.Health <= (Me.MaximumHealth * 0.3) &&
						_satanic.CanBeCasted()
						&& !meld
						&& Me.Distance2D(E) <= Me.AttackRange + 50
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
						&& Utils.SleepCheck("satanic")
						)
					{
						_satanic.UseAbility();
						Utils.Sleep(240, "satanic");
					} // Satanic Item end
					if (_mail != null 
						&& _mail.CanBeCasted() 
						&& (v.Count(x => x.Distance2D(Me) <= 650) 
						>= (Menu.Item("Heelm").GetValue<Slider>().Value))
						&& !meld
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) 
						&& Utils.SleepCheck("mail"))
					{
						_mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					if (_bkb != null 
						&& _bkb.CanBeCasted()
						&& !meld
						&& (v.Count(x => x.Distance2D(Me) <= 650) 
						>= (Menu.Item("Heel").GetValue<Slider>().Value)) &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) 
						&& Utils.SleepCheck("bkb"))
					{
						_bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}
					
					if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive)
					{
						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
					if (_pike != null 
						&& _pike.CanBeCasted() 
						&& Me.IsAttacking()
						&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_pike.Name)
						&& (E.Health <= (E.MaximumHealth / 100 * Menu.Item("piKe").GetValue<Slider>().Value)
						||  Me.Health <= (Me.MaximumHealth / 100 * Menu.Item("piKeMe").GetValue<Slider>().Value))
                        && (_w == null
						|| !_w.CanBeCasted())
						&& !meld
						&& Me.Distance2D(E) <= 450 
						&& Utils.SleepCheck("pike"))
					{
						_pike.UseAbility(E);
						if (((_pike != null && _pike.CanBeCasted()) || IsCasted(_pike)) && _r.CanBeCasted() && !Me.Modifiers.ToList().Exists(y => y.Name == "modifier_templar_assassin_meld") && Me.Distance2D(E.NetworkPosition) <= 400 && Me.CanCast() && !Me.IsSilenced() && !Me.IsHexed())
						{
							var a1 = Me.Position.ToVector2().FindAngleBetween(E.Position.ToVector2(), true);
							var p1 = new Vector3(
								(E.Position.X + 520 * (float)Math.Cos(a1)),
								(E.Position.Y + 520 * (float)Math.Sin(a1)),
								100);
							_r.UseAbility(p1);
						}
						Utils.Sleep(100, "pike");
					}
					var traps = ObjectManager.GetEntities<Unit>().Where(x => x.Name == "npc_dota_templar_assassin_psionic_trap" && x.Team == Me.Team
									  && x.Distance2D(Me) <= 1700 && x.IsAlive && x.IsValid).ToList();
					foreach (var q in traps)
					{
						if (!HurPikeActived() && E.NetworkPosition.Distance2D(q.Position) < 390 && q.Spellbook.SpellQ.CanBeCasted() && Utils.SleepCheck("traps") && !E.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow"))
						{
							q.Spellbook.SpellQ.UseAbility();
							Utils.Sleep(150, "traps");
						}
					}
				}
				Utils.Sleep(50, "combo");
			}
		}
		
		private static bool IsCasted(Ability ability)
		{
			return ability.Level > 0 && ability.CooldownLength > 0 && Math.Ceiling(ability.CooldownLength).Equals(Math.Ceiling(ability.Cooldown));
		}
		public void OnCloseEvent()
		{

		}
		private bool HurPikeActived()
		{
			return (_pike != null && (_pike.CooldownLength - _pike.Cooldown) < 4.5 && _pike.Cooldown > 0);
		}
	}
}