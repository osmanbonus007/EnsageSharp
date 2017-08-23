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

	internal class AxeController : Variables, IHeroController
	{
		private Ability _q, _w, _r;
	    private int[] _damage;

        private Item _urn, _dagon, _mjollnir, _abyssal, _mom, _armlet, _shiva, _mail, _bkb, _satanic, _medall, _blink, _lotusorb;
		private double _rDmg;
		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			if (!Menu.Item("enabled").IsActive())
				return;
			_q = Me.Spellbook.SpellQ;
			_w = Me.Spellbook.SpellW;
			_r = Me.Spellbook.SpellR;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					.ToList();
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			_mjollnir = Me.FindItem("item_mjollnir");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_lotusorb = Me.FindItem("item_lotus_orb");
			_mail = Me.FindItem("item_blade_mail");
			_armlet = Me.FindItem("item_armlet");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_blink = Me.FindItem("item_blink");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;
			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");

			if(stoneModif)return;
			if (Active && Me.Distance2D(E) <= 2000 && E.IsAlive && !Me.IsInvisible())
			{
				if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& Me.Distance2D(E) < 1180
					&& Me.Distance2D(E) > 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("_blink")
					)
				{
					_blink.UseAbility(E.Position);
					Utils.Sleep(250, "_blink");
				}
				if (
					_q != null && _q.CanBeCasted()
					&& Me.Distance2D(E) <= _q.GetCastRange() + Me.HullRadius
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Utils.SleepCheck("_q")
					)
				{
					_q.UseAbility();
					Utils.Sleep(200, "_q");
				}
				if ( // MOM
					_mom != null
					&& _mom.CanBeCasted()
					&& Me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
					&& Utils.SleepCheck("_mom")
					&& Me.Distance2D(E) <= 700
					)
				{
					_mom.UseAbility();
					Utils.Sleep(250, "_mom");
				}
				if (
					_w != null && _w.CanBeCasted() && Me.Distance2D(E) <= _w.GetCastRange()+Me.HullRadius
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
					&& !E.HasModifier("modifier_axe_battle_hunger")
					&& Utils.SleepCheck("_w")
					)
				{
					_w.UseAbility(E);
					Utils.Sleep(200, "_w");
				}
				if (_lotusorb != null && _lotusorb.CanBeCasted() &&
				    Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_lotusorb.Name)
					&& (v.Count(x => x.Distance2D(Me) <= 650) >= (Menu.Item("Heelm").GetValue<Slider>().Value) && Utils.SleepCheck("_lotusorb"))
				   )
				{
					_lotusorb.UseAbility(Me);
					Utils.Sleep(250, "_lotusorb");
				}
				if ( // Mjollnir
					_mjollnir != null
					&& _mjollnir.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
					&& Utils.SleepCheck("_mjollnir")
					&& Me.Distance2D(E) <= 900
					)
				{
					_mjollnir.UseAbility(Me);
					Utils.Sleep(250, "_mjollnir");
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
				if ( // Abyssal Blade
					_abyssal != null
					&& _abyssal.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsStunned()
					&& !E.IsHexed()
					&& Utils.SleepCheck("_abyssal")
					&& Me.Distance2D(E) <= 400
					)
				{
					_abyssal.UseAbility(E);
					Utils.Sleep(250, "_abyssal");
				} // Abyssal Item end
				if (_armlet != null && !_armlet.IsToggled &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_armlet.Name) &&
					Utils.SleepCheck("_armlet"))
				{
					_armlet.ToggleAbility();
					Utils.Sleep(300, "_armlet");
				}

				if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
					&& !E.IsMagicImmune() && Utils.SleepCheck("_shiva"))
				{
					_shiva.UseAbility();
					Utils.Sleep(100, "_shiva");
				}

				if ( // Dagon
					Me.CanCast()
					&& _dagon != null
					&& !E.IsLinkensProtected()
					&& _dagon.CanBeCasted()
					&& !E.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
					&& Utils.SleepCheck("_dagon")
					)
				{
					_dagon.UseAbility(E);
					Utils.Sleep(200, "_dagon");
				} // Dagon Item end


				if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("_urn"))
				{
					_urn.UseAbility(E);
					Utils.Sleep(240, "_urn");
				}
				if ( // Satanic 
					_satanic != null &&
					Me.Health <= (Me.MaximumHealth * 0.3) &&
					_satanic.CanBeCasted() &&
					Me.Distance2D(E) <= Me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
					&& Utils.SleepCheck("_satanic")
					)
				{
					_satanic.UseAbility();
					Utils.Sleep(240, "_satanic");
				} // Satanic Item end
				if (_mail != null && _mail.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
														   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("_mail"))
				{
					_mail.UseAbility();
					Utils.Sleep(100, "_mail");
				}
				if (_bkb != null && _bkb.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Utils.SleepCheck("_bkb"))
				{
					_bkb.UseAbility();
					Utils.Sleep(100, "_bkb");
				}
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
				if (_q != null && _q.IsInAbilityPhase && v.Count(x => x.Distance2D(Me) <= _q.GetCastRange()+Me.HullRadius+23) == 0 && Utils.SleepCheck("Phase"))
				{
					Me.Stop();
					Utils.Sleep(100, "Phase");
				}
			}
            if (Menu.Item("kill").IsActive())
            {
                if (_r == null || !Me.IsAlive) return;
                var count = v.Count();
                if (count <= 0) return;
                
                for (int i = 0; i < count; ++i)
                {
                    if (_w != null && _w.CanBeCasted() && Menu.Item("HUNGER").IsActive())
                    {
                        if (!v[i].HasModifier("modifier_axe_battle_hunger")
                            && Me.Distance2D(v[i]) <= _w.GetCastRange() + Me.HullRadius
                            && Me.Mana >= _r.ManaCost + 180
                            && Utils.SleepCheck(Me.Handle.ToString()))
                        {
                            _w.UseAbility(v[i]);
                            Utils.Sleep(400, Me.Handle.ToString());
                        }
                    }
                    if (!_r.CanBeCasted()) return;
                    _damage = Me.AghanimState() ? new[] { 0, 300, 425, 550 } : new[] { 0, 250, 325, 400 };
                    _rDmg = ((_damage[_r.Level]));
                    
                    if (_r.IsInAbilityPhase && v.Where(x => Me.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24).OrderBy(z => z.Health).First().Health > _rDmg && v[i].Distance2D(Me) <= _r.GetCastRange() + Me.HullRadius + 24 && Utils.SleepCheck(v[i].Handle.ToString()))
                    {
                        Me.Stop();
                        Utils.Sleep(100, v[i].Handle.ToString());
                    }


                    if (v[i].IsFullMagicSpellResist()) return;
                  
                    if (_blink != null && _blink.CanBeCasted() && _r != null && _r.CanBeCasted() && Menu.Item("_blink").IsActive())
                    {
                        if (Me.Distance2D(v[i]) > _r.GetCastRange() + Me.HullRadius + 24 && v[i].Health < _rDmg && Utils.SleepCheck(v[i].Handle.ToString()))
                        {
                            _blink.UseAbility(v[i].Position);
                            Utils.Sleep(150, v[i].Handle.ToString());
                        }
                    }
                    var bonusRange = Menu.Item("killRange").IsActive() ? Menu.Item("Blade").GetValue<Slider>().Value : 0;
                    if (v[i].Health <= _rDmg && v[i].Distance2D(Me) <= _r.GetCastRange() + Me.HullRadius+24 + bonusRange && Utils.SleepCheck(v[i].Handle.ToString()))
                    {
                        _r.UseAbility(v[i]);
                        Utils.Sleep(150, v[i].Handle.ToString());
                    }
                    
                }
            }
        }

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("I came a long way to see you die.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"axe_berserkers_call", true},
					{"axe_battle_hunger", true},
					{"axe_culling_blade", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_dagon", true},
					{"item_blink", true},
					{"item_armlet", true},
					{"item_lotus_orb", true},
					{"item_heavens_halberd", true},
					{"item_urn_of_shadows", true},
					{"item_veil_of_discord", true},
					{"item_abyssal_blade", true},
					{"item_shivas_guard", true},
					{"item_blade_mail", true},
					{"item_black_king_bar", true},
					{"item_satanic", true},
					{"item_medallion_of_courage", true},
					{"item_solar_crest", true}
				})));

			Menu.AddItem(new MenuItem("kill", "AutoUsage").SetValue(true));
			Menu.AddItem(new MenuItem("HUNGER", "Use Auto spells: BATTLE HUNGER(_w)").SetValue(true));

			Menu.AddItem(new MenuItem("_blink", "Use Auto _blink and CULLING BLADE(_r)").SetValue(true));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail|Lotus").SetValue(new Slider(2, 1, 5)));

            Menu.AddItem(new MenuItem("killRange", "Use Bonus Ult Range").SetValue(true)).SetTooltip("Move to enemy");
            Menu.AddItem(new MenuItem("Blade", "Bonus Ult Range").SetValue(new Slider(200, 1, 500))).SetTooltip("Move to enemy");
        }
       
        public void OnCloseEvent()
		{
			
		}
	}
}
