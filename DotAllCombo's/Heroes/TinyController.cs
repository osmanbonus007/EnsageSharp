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

	internal class TinyController : Variables, IHeroController
	{
		private Ability _q, _w;

		private Item _urn, _ethereal, _dagon, _halberd, _vail, _mjollnir, _orchid, _abyssal, _shiva, _mail, _bkb, _satanic, _medall, _blink;

        

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;

            E = Toolset.ClosestToMouse(Me);
            if (E == null)
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;

			_blink = Me.FindItem("item_blink");
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
			_vail = Me.FindItem("item_veil_of_discord");
            
			var modifEther = E.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
			var stoneModif = E.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var isInvisible =
				Me.IsInvisible();
			if (Active)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
			if (Active && Me.Distance2D(E) <= 1400 && E.IsAlive && !isInvisible)
			{
                float angle = Me.FindAngleBetween(E.Position, true);
                Vector3 pos = new Vector3((float)(E.Position.X - 100 * Math.Cos(angle)), (float)(E.Position.Y - 100 * Math.Sin(angle)), 0);
                if (
                    _blink != null
                    && _q.CanBeCasted()
                    && Me.CanCast()
                    && _blink.CanBeCasted()
                    && Me.Distance2D(E) >= Toolset.AttackRange+150
                    && Me.Distance2D(pos) <= 1180
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                    && Utils.SleepCheck("blink")
                    )
                {
                    _blink.UseAbility(pos);
                    Utils.Sleep(250, "blink");
                }
                if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 900 &&
					   Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) && Utils.SleepCheck("orchid"))
				{
					_orchid.UseAbility(E);
					Utils.Sleep(100, "orchid");
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
				if (_ethereal != null && _ethereal.CanBeCasted()
					   && Me.Distance2D(E) <= 700 && Me.Distance2D(E) <= 400
					   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name) &&
					   Utils.SleepCheck("ethereal"))
				{
					_ethereal.UseAbility(E);
					Utils.Sleep(100, "ethereal");
				}

				if ( // Dagon
					Me.CanCast()
					&& _dagon != null
					&& (_ethereal == null
						|| (modifEther
							|| _ethereal.Cooldown < 17))
					&& !E.IsLinkensProtected()
					&& _dagon.CanBeCasted()
					&& Me.Distance2D(E) <= 1400
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                    && !E.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					_dagon.UseAbility(E);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if (
					_q != null && _q.CanBeCasted() && Me.Distance2D(E) <= 1500
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					_q.UseAbility(E.Predict(400));
					Utils.Sleep(200, "Q");
				}
				if (
					_w != null && _w.CanBeCasted() && Me.Distance2D(E) <= 1500
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& Utils.SleepCheck("W")
					)
				{
					_w.UseAbility(E);
					Utils.Sleep(200, "W");
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


				if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
					&& !E.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					_shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
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

			Print.LogMessage.Success("The road ahead looks rocky, but that's all right with Me.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"tiny_avalanche", true},
				    {"tiny_toss", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    { "item_bloodthorn", true},
				    {"item_mjollnir", true},
                    {"item_dagon", true},
                    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_veil_of_discord", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
