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

	internal class BountyHunterController : Variables, IHeroController
    {
        private Ability _q, _r;

        private Item _urn, _ethereal, _dagon, _halberd, _mjollnir, _orchid, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _medall; 
        
		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Push = Menu.Item("KeyR").GetValue<KeyBind>().Active;
			_q = Me.Spellbook.SpellQ;
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
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			
			if (Active)
            {
                E = Toolset.ClosestToMouse(Me);
                if (E == null)
					return;
				var track = E.HasModifier("modifier_bounty_hunter_track");
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
				if (Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
				{
					if (
						_q != null && _q.CanBeCasted() && Me.Distance2D(E) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility(E);
						Utils.Sleep(200, "Q");
					}
					if (
						_r != null && _r.CanBeCasted() && Me.Distance2D(E) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
						&& !track
						&& !Me.IsChanneling()
						&& Utils.SleepCheck("R")
						)
					{
						_r.UseAbility(E);
						Utils.Sleep(200, "R");
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
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
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
					if (_mail != null && _mail.CanBeCasted() && (enemies.Count(x => x.Distance2D(Me) <= 650) >=
															   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
					{
						_mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					if (_bkb != null && _bkb.CanBeCasted() && (enemies.Count(x => x.Distance2D(Me) <= 650) >=
															 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("bkb"))
					{
						_bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}
				}
			}

			if (Push && Me.IsAlive && _r != null && _r.CanBeCasted())
				if (!Me.HasModifier("modifier_bounty_hunter_wind_walk") || Me.IsVisibleToEnemies)
					foreach (var v in enemies)
					{
						var checkMod = v.Modifiers.Where(y => y.Name == "modifier_bounty_hunter_track").DefaultIfEmpty(null).FirstOrDefault();
						var invItem = v.FindItem("item_glimmer_cape") ?? v.FindItem("item_invis_sword") ?? v.FindItem("item_silver_edge") ?? v.FindItem("item_glimmer_cape");
						if (
							((v.ClassId == ClassId.CDOTA_Unit_Hero_Riki || v.ClassId == ClassId.CDOTA_Unit_Hero_Broodmother
							|| v.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz || v.ClassId == ClassId.CDOTA_Unit_Hero_Invoker
							|| v.ClassId == ClassId.CDOTA_Unit_Hero_SandKing || v.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin
							|| v.ClassId == ClassId.CDOTA_Unit_Hero_Treant || v.ClassId == ClassId.CDOTA_Unit_Hero_PhantomLancer
							)
							|| (
								v.Modifiers.Any(x =>
								(x.Name == "modifier_riki_permanent_invisibility"
								|| x.Name == "modifier_mirana_moonlight_shadow"
								|| x.Name == "modifier_treant_natures_guise"
								|| x.Name == "modifier_weaver_shukuchi"
								|| x.Name == "modifier_broodmother_spin_web_invisible_applier"
								|| x.Name == "modifier_item_invisibility_edge_windwalk"
								|| x.Name == "modifier_rune_invis"
								|| x.Name == "modifier_clinkz_wind_walk"
								|| x.Name == "modifier_item_shadow_amulet_fade"
								|| x.Name == "modifier_item_silver_edge_windwalk"
								|| x.Name == "modifier_item_edge_windwalk"
								|| x.Name == "modifier_nyx_assassin_vendetta"
								|| x.Name == "modifier_invisible"
								|| x.Name == "modifier_invoker_ghost_walk_enemy")))
							|| (invItem != null && invItem.Cooldown <= 0)
							|| v.Health <= (v.MaximumHealth * 0.5))
							&& Me.Distance2D(v) <= _r.GetCastRange() + Me.HullRadius
							&& (!v.HasModifier("modifier_bounty_hunter_track") || checkMod != null && checkMod.RemainingTime <= 2)
							&& Utils.SleepCheck("R"))
						{
							_r.UseAbility(v);
							Utils.Sleep(300, "R");
						}
					}
	}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Need to Creet!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("KeyR", "Use Auto Track").SetValue(new KeyBind('R', KeyBindType.Toggle)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"bounty_hunter_track", true},
				    {"bounty_hunter_shuriken_toss", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_mjollnir", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
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