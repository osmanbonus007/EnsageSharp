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

	internal class SandKingController : Variables, IHeroController
	{

		private Ability _q, _r;

		private Item _urn,
			_dagon,
		    _mjollnir,
			_orchid,
			_abyssal,
			_mom,
			_shiva,
			_mail,
			_bkb,
			_ethereal,
			_glimmer,
			_vail,
			_satanic,
			_medall,
			_blink;

		

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

			_q = Me.Spellbook.SpellQ;
			_r = Me.Spellbook.SpellR;
			_shiva = Me.FindItem("item_shivas_guard");
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			_mjollnir = Me.FindItem("item_mjollnir");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_ethereal = Me.FindItem("item_ethereal_blade");
			_glimmer = Me.FindItem("item_glimmer_cape");
			_vail = Me.FindItem("item_veil_of_discord");
			_satanic = Me.FindItem("item_satanic");
			_blink = Me.FindItem("item_blink");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");


			if (Me.IsChanneling() || _r.IsInAbilityPhase || _r.IsChanneling) return;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var modifInv = Me.IsInvisible();
			if (Active && Utils.SleepCheck("Combo"))
			{
				if (Me.HasModifier("modifier_sandking_sand_storm")) return;
				float angle = Me.FindAngleBetween(E.Position, true);

				Vector3 pos = new Vector3((float)(E.Position.X - (_q.GetCastRange() - 100) * Math.Cos(angle)),
					(float)(E.Position.Y - (_q.GetCastRange() - 100) * Math.Sin(angle)), 0);
				uint elsecount = 1;
				if (elsecount == 1 && (_blink != null && _blink.CanBeCasted() && Me.Distance2D(pos) <= 1100 || _blink == null && Me.Distance2D(E) <= _q.GetCastRange() - 50))
				{
					if (
						_r != null && _r.CanBeCasted()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						&& Utils.SleepCheck("R")
						)
					{
						_r.UseAbility();
						Utils.Sleep(200, "R");
						Utils.Sleep(300, "Combo");
					}
				}

				if (!Utils.SleepCheck("Combo") || Me.IsChanneling() || _r.IsChanneling || _r.IsInAbilityPhase) return;

				if (!Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name) || !_r.CanBeCasted())
                {
                    if (
                        _blink != null
                        && _blink.CanBeCasted()
                        && Me.Distance2D(E) >= (_q.CanBeCasted() ? _q.GetCastRange() : 450)
                        && Me.Distance2D(pos) <= 1190
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        _blink.UseAbility(pos);
                        Utils.Sleep(250, "blink");
                    }
                    if (
						_blink != null
						&& _blink.CanBeCasted()
						&& Me.Distance2D(E) < 1180
						&& Me.Distance2D(E) > (_q.CanBeCasted() ? _q.GetCastRange() : 450)
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						_blink.UseAbility(E.Position);
						Utils.Sleep(250, "blink");
					}

					if (
					_q != null && _q.CanBeCasted() && Me.Distance2D(E) <= _q.GetCastRange() + 300
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Utils.SleepCheck("Q")
					)
					{
						_q.UseAbility(E);
						Utils.Sleep(200, "Q");
					}
				}
				if (Me.Distance2D(E) <= 2000 && E != null && E.IsAlive && !modifInv && !Me.IsChanneling() && (!_r.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)))
				{
					if (_ethereal != null && _ethereal.CanBeCasted() && Me.Distance2D(E) <= 700 &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
					{
						_ethereal.UseAbility(E);
						Utils.Sleep(100, "ethereal");
					}

					if (_vail != null && _vail.CanBeCasted() && Me.Distance2D(E) <= 1100 &&
					   Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name) && Utils.SleepCheck("vail"))
					{
						_vail.UseAbility(E.Position);
						Utils.Sleep(130, "vail");
					}


					if ( // Abyssal Blade
					_abyssal != null
					&& _abyssal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsStunned()
					&& !E.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& Me.Distance2D(E) <= 400
					)
					{
						_abyssal.UseAbility(E);
						Utils.Sleep(250, "abyssal");
					} // Abyssal Item end
					if (_glimmer != null
						&& _glimmer.CanBeCasted()
						&& Me.Distance2D(E) <= 300
						&& Utils.SleepCheck("glimmer"))
					{
						_glimmer.UseAbility(Me);
						Utils.Sleep(200, "glimmer");
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

					if ( // Dagon
						Me.CanCast()
						&& _dagon != null
						&& !E.IsLinkensProtected()
						&& _dagon.CanBeCasted()
						&& !E.IsMagicImmune()
						&& !stoneModif
						&& Utils.SleepCheck("dagon")
						)
					{
						_dagon.UseAbility(E);
						Utils.Sleep(200, "dagon");
					} // Dagon Item end


					if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
					{
						_urn.UseAbility(E);
						Utils.Sleep(240, "urn");
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
					if (Menu.Item("logic").IsActive())
					{
						if (_mail != null && _mail.CanBeCasted() && Toolset.HasStun(E) && !E.IsStunned() &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
						{
							_mail.UseAbility();
							Utils.Sleep(100, "mail");
						}
						if (_bkb != null && _bkb.CanBeCasted() && Toolset.HasStun(E) && !E.IsStunned() &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("bkb"))
						{
							_bkb.UseAbility();
							Utils.Sleep(100, "bkb");
						}
					}
				}
				if (Me.IsChanneling() || _r.IsChanneling || _r.IsInAbilityPhase) return;
				elsecount++;
				if (elsecount == 2 && E != null && E.IsAlive)
				{

					if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900 && !Me.IsChanneling())
					{
						Orbwalking.Orbwalk(E, 0, 1600, true, true);
					}
				}
				Utils.Sleep(200, "Combo");
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("I am king now of all I survey.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"sandking_burrowstrike", true},
				    //{"sandking_sand_storm", true},
				    {"sandking_epicenter", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
				    {"item_urn_of_shadows", true},
				    {"item_veil_of_discord", true},
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
			Menu.AddItem(new MenuItem("logic", "UseBKB if e have stun").SetValue(true));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}