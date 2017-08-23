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

    internal class RikiController : Variables, IHeroController
    {
        private Ability _q, _w, _r;
        private Item _urn, _dagon, _diff, _mjollnir, _orchid, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _medall, _blink;
        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success("Into the shadows.");
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
            
            Menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"riki_smoke_screen", true},
                    {"riki_blink_strike", true},
                    {"riki_tricks_of_the_trade", true}
                })));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_blink", true},
                    {"item_diffusal_blade", true},
                    {"item_diffusal_blade_2", true},
                    {"item_orchid", true},
                    {"item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
                    {"item_abyssal_blade", true},
                    {"item_shivas_guard", true},
                    {"item_blade_mail", true},
                    {"item_black_king_bar", true},
                    {"item_satanic", true},
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true}
                })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Ult", "Min targets to Ultimate").SetValue(new Slider(3, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

        public void Combo()
        {
            
            Me = ObjectManager.LocalHero;
			
            if (!Menu.Item("enabled").IsActive()) return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

            _q = Me.Spellbook.SpellQ;
            _w = Me.Spellbook.SpellW;
            _r = Me.Spellbook.SpellR;
            _shiva = Me.FindItem("item_shivas_guard");
            _mom = Me.FindItem("item_mask_of_madness");
            _diff = Me.FindItem("item_diffusal_blade")?? Me.FindItem("item_diffusal_blade_2");
            _urn = Me.FindItem("item_urn_of_shadows");
            _dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            _mjollnir = Me.FindItem("item_mjollnir");
            _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
            _abyssal = Me.FindItem("item_abyssal_blade");
            _mail = Me.FindItem("item_blade_mail");
            _bkb = Me.FindItem("item_black_king_bar");
            _satanic = Me.FindItem("item_satanic");
            _blink = Me.FindItem("item_blink");
            _medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");

            var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
            
            var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();

            if (_r.IsInAbilityPhase || Me.HasModifier("modifier_riki_tricks_of_the_trade_phase")) return;
            if (Active && E.IsAlive)
            {

				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
            if (Active && Me.Distance2D(E) <= 1400 && E.IsAlive && (!Me.IsInvisible() || Me.IsVisibleToEnemies || E.Health <= (E.MaximumHealth * 0.7)))
            {
                if (stoneModif) return;
                    if (
                        _q != null
                        && _q.CanBeCasted()
                        && Me.Distance2D(E) <= 300
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                        && Utils.SleepCheck("Q")
                        )
                    {
                        _q.UseAbility(Prediction.InFront(E, 80));
                        Utils.Sleep(200, "Q");
					}
				if (
                        _w != null && _w.CanBeCasted()
                        && Me.Distance2D(E) <= _w.GetCastRange()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                        && Utils.SleepCheck("W")
                        )
                    {
                        _w.UseAbility(E);
                        Utils.Sleep(200, "W");
                    }
                    if (
                        _blink != null
                        && Me.CanCast()
                        && _blink.CanBeCasted()
                        && Me.Distance2D(E) < 1180
                        && Me.Distance2D(E) > 300
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                        {
                            _blink.UseAbility(E.Position);
                            Utils.Sleep(250, "blink");
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
				if (_diff != null
						&& _diff.CanBeCasted()
						&& _diff.CurrentCharges > 0
						&& Me.Distance2D(E) <= 400
						&& !E.HasModifier("modifier_item_diffusal_blade_slow")
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_diff.Name) &&
						Utils.SleepCheck("diff"))
                    {
					_diff.UseAbility(E);
					Utils.Sleep(4000, "diff");
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
                        if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 300 &&
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
                        if (_mail != null && _mail.CanBeCasted() 
                        && ((v.Count(x => x.Distance2D(Me) <= 650) >= (Menu.Item("Heelm").GetValue<Slider>().Value)) 
                        || Me.HasModifier("modifier_skywrath_mystic_flare_aura_effect")) &&
                            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
                        {
                            _mail.UseAbility();
                            Utils.Sleep(100, "mail");
                        }
                        if (_bkb != null && _bkb.CanBeCasted() && ((v.Count(x => x.Distance2D(Me) <= 650) >=
                                                                 (Menu.Item("Heel").GetValue<Slider>().Value))
                        || Me.HasModifier("modifier_skywrath_mystic_flare_aura_effect")) &&
                            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("bkb"))
                        {
                            _bkb.UseAbility();
                            Utils.Sleep(100, "bkb");
                        }
                    if (_r != null && _r.CanBeCasted()
                        && (v.Count(x => x.Distance2D(Me) <= 500)
                        >= (Menu.Item("Ult").GetValue<Slider>().Value))
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                        && Utils.SleepCheck("R"))
                    {
                        _r.UseAbility();
                        Utils.Sleep(100, "R");
                    }
                }
			
		}

        public void OnCloseEvent()
        {

        }
		
	}
}



