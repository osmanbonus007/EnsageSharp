using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using System.Collections.Generic;
	using System.Linq;
    using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;
    using Service.Debug;
	

	internal class PuckController : Variables, IHeroController
	{
		private Ability _q, _w, _e, _d, _r;
		private Item _blink, _dagon, _ethereal, _shiva, _orchid, _sheep, _vail;
		//private int stage;

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
            E = Toolset.ClosestToMouse(Me);


            if (Me == null || E == null)
				return;
			// skills
				_q = Me.Spellbook.SpellQ;
				_w = Me.Spellbook.SpellW;
				_e = Me.Spellbook.SpellE;
				_d = Me.Spellbook.SpellD;
				_r = Me.Spellbook.SpellR;
			// itens
			_blink = Me.FindItem("item_blink");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_ethereal = Me.FindItem("item_ethereal_blade");
			_shiva = Me.FindItem("item_shivas_guard");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_vail = Me.FindItem("item_veil_of_discord");
			_sheep = Me.FindItem("item_sheepstick");

			//Starting Combo
            
			var isInAdvantage = (E.HasModifier("modifier_item_blade_mail_reflect") ||
									E.HasModifier("modifier_item_lotus_orb_active") ||
									E.HasModifier("modifier_nyx_assassin_spiked_carapace") ||
									E.HasModifier("modifier_templar_assassin_refraction_damage") ||
									E.HasModifier("modifier_ursa_enrage") ||
									E.HasModifier("modifier_abaddon_borrowed_time") ||
									(E.HasModifier("modifier_dazzle_shallow_grave")));
			if (Game.IsKeyDown(Menu.Item("Full").GetValue<KeyBind>().Key) && Utils.SleepCheck("combo"))
			{
				if (Me.CanCast() && !Me.IsChanneling() && Me.Distance2D(E) <= 1200 &&
				 E.IsVisible && E.IsAlive && !E.IsMagicImmune() &&
				 !isInAdvantage)
				{
                    if (Me.CanCast())
                    {
                        var v =
                      ObjectManager.GetEntities<Hero>()
                          .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                          .ToList();
                        var X = Me.Position.X;
                        var y = Me.Position.Y;
                        var pos = new Vector3(X, y, Me.Position.Z);
                        if (Me.Position.X < 0)
                        {
                            X = Me.Position.X + 100;
                        }
                        else
                        {
                            X = Me.Position.X - 100;
                        }
                        if (Me.Position.Y < 0)
                        {
                            y = Me.Position.Y + 100;
                        }
                        else
                        {
                            y = Me.Position.Y - 100;
                        }
                        uint elsecount = 0;
                        if (_blink != null
                            && _blink.CanBeCasted()
                            && Me.Distance2D(E.Position) > 300
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                        {
                            _blink.UseAbility(E.Position);
                        }
                        elsecount++;
                        if ( // vail
                            _vail != null
                            && _vail.CanBeCasted()
                            && Me.CanCast()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
                            && !E.IsMagicImmune()
                            && Utils.SleepCheck("vail")
                            && Me.Distance2D(E) <= 1500
                            )
                        {
                            _vail.UseAbility(E.Position);
                        }
                        else elsecount++;
                        if (_orchid != null && _orchid.CanBeCasted() &&
                                Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
                        {
                            _orchid.UseAbility(E);
                        }
                        else elsecount++;
                        if (_sheep != null && _sheep.CanBeCasted() &&
                                      Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
                        {
                            _sheep.UseAbility(E);
                        }
                        else elsecount++;
                        if (_ethereal != null
                            && _ethereal.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
                        {
                            _ethereal.UseAbility(E);
                        }
                        else elsecount++;
                        if (_q != null
                            && _q.CanBeCasted()
                            && (_blink == null
                            || !_blink.CanBeCasted())
                            && Me.Distance2D(E) < _q.GetCastRange()
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name))
                        {
                            _q.UseAbility(pos);
                        }
                        else elsecount++;

                        if (_w != null && _w.CanBeCasted() && Me.Distance2D(E) < 390 &&
                                Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
                        {
                            _w.UseAbility();
                        }
                        else elsecount++;
                        if (// Dagon
                             elsecount == 7 &&
                                Me.CanCast()
                                && _dagon != null
                                && (_ethereal == null
                                || (E.HasModifier("modifier_item_ethereal_blade_slow")
                                || _ethereal.Cooldown < 17))
                                && !E.IsLinkensProtected()
                                && _dagon.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && !E.IsMagicImmune()
                                && Utils.SleepCheck("dagon")
                                )
                        {
                            _dagon.UseAbility(E);
                            Utils.Sleep(200, "dagon");
                        }
                        else elsecount++;  // Dagon Item end
                        if (elsecount == 8 && _shiva != null
                                && _shiva.CanBeCasted()
                                && Me.Distance2D(E) <= 600
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
                        {
                            _shiva.UseAbility();
                        }
                        else elsecount++;
                        if (elsecount == 9 && _r != null && _r.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
                                                                 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
                            Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name) && Utils.SleepCheck("R"))
                        {
                            _r.UseAbility(E.Position);
                            Utils.Sleep(100, "R");
                        }
                    }
					if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
					{
						Orbwalking.Orbwalk(E, 0, 1600, true, true);
					}
				}
				Utils.Sleep(250, "combo");
			}
			//Escape-combo
			if (Game.IsKeyDown(Menu.Item("Escape").GetValue<KeyBind>().Key) && Me.Distance2D(E) <= 1200 &&
				E.IsVisible && E.IsAlive && !E.IsMagicImmune() &&
				Utils.SleepCheck("combo2"))
			{
			   

                if (Me.CanCast())
				{
					var x = Me.Position.X;
					var y = Me.Position.Y;
					var pos = new Vector3(x, y, Me.Position.Z);
                    if (Me.Position.X < 0)
                    {
                        x = Me.Position.X + 100;
                    }
                    else
                    {
                        x = Me.Position.X - 100;
                    }
                    if (Me.Position.Y < 0)
                    {
                        y = Me.Position.Y + 100;
                    }
                    else
                    {
                        y = Me.Position.Y - 100;
                    }
                    uint elsecount = 0;
                    if (_blink != null 
                        && _blink.CanBeCasted() 
                        && Me.Distance2D(E.Position) > 300 
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                    {
                        _blink.UseAbility(E.Position);
                    }
                    elsecount++;
                    if ( // vail
						_vail != null
						&& _vail.CanBeCasted()
						&& Me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
						&& !E.IsMagicImmune()
						&& Utils.SleepCheck("vail")
						&& Me.Distance2D(E) <= 1500
						)
					{
						_vail.UseAbility(E.Position);
                    }
                    else elsecount++;
                    if (_orchid != null && _orchid.CanBeCasted() &&
                            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
                    {
                        _orchid.UseAbility(E);
                    }
                    else elsecount++;
                    if (_sheep != null && _sheep.CanBeCasted() &&
                                  Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
                    {
                        _sheep.UseAbility(E);
                    }
                    else elsecount++;
                    if (_ethereal != null
						&& _ethereal.CanBeCasted()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
					{
						_ethereal.UseAbility(E);
					}
                    else elsecount++;
                        if (_q != null 
							&& _q.CanBeCasted() 
							&& ( _blink == null 
							|| !_blink.CanBeCasted())
							&& Me.Distance2D(E) < _q.GetCastRange() 
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name))
						{
							_q.UseAbility(pos);
                    }
                    else elsecount++;

                    if (_w != null && _w.CanBeCasted() && Me.Distance2D(E) < 390 &&
							Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
						{
							_w.UseAbility();
                    }
                    else elsecount++;
                    if (// Dagon
                         elsecount == 7 &&
                            Me.CanCast()
                            && _dagon != null
                            && (_ethereal == null
                            || (E.HasModifier("modifier_item_ethereal_blade_slow")
                            || _ethereal.Cooldown < 17))
                            && !E.IsLinkensProtected()
                            && _dagon.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                            && !E.IsMagicImmune()
                            && Utils.SleepCheck("dagon")
                            )
                        {
                            _dagon.UseAbility(E);
                            Utils.Sleep(200, "dagon");
                    }
                    else elsecount++;  // Dagon Item end
                    if (_shiva != null
							&& _shiva.CanBeCasted()
							&& Me.Distance2D(E) <= 600
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
						{
							_shiva.UseAbility();
                    }
                    else elsecount++;
                    if (elsecount ==9 && _e != null
                            && _e.CanBeCasted()
                            && _d != null
                            && _d.CanBeCasted()
                            &&
                            ((_sheep == null || !_sheep.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
                             && (_shiva == null || !_shiva.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
                             && (_dagon == null || !_dagon.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                             && (_ethereal == null || !_ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
                             && (_orchid == null || !_orchid.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
                             && (_blink == null || !_blink.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
                             && (_w == null || !_w.CanBeCasted() || Me.Distance2D(E) >= 400 || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
                            && Utils.SleepCheck("1")))
                    {
                        _e.UseAbility();
                        Utils.Sleep(250, "1");
                    }
                    if (
							_d != null
							&& _d.CanBeCasted()
							&& (_sheep == null || !_sheep.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
							&& (_shiva == null || !_shiva.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
							&& (_dagon == null || !_dagon.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							&& (_ethereal == null || !_ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
							&& (_orchid == null || !_orchid.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
							&& (_blink == null || !_blink.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
							&& (_w == null || !_w.CanBeCasted() || Me.Distance2D(E) >= 400 || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
							&& Utils.SleepCheck("12"))
						{
							var baseDota =
								ObjectManager.GetEntities<Unit>()
									.Where(unit => unit.Name == "npc_dota_base" && unit.Team == Me.Team)
									.ToList();

							if (baseDota != null)
							{
								for (int i = 0; i < baseDota.Count(); ++i)
								{
									if (baseDota[i].Distance2D(Me) >= 1200)
									{
										_d.UseAbility();
									}
								}
							}
							Utils.Sleep(200, "12");
                    }
				}
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
				Utils.Sleep(300, "combo2");
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("Slon|Modif by Vick", "0.1");

			Print.LogMessage.Success("I find myself strangely drawn to this odd configuration of activity.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Full", "Full Combo(Please use only if you have blink)").SetValue(new KeyBind('A', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("Escape", "Escape Combo(Please use only if you have blink)").SetValue(new KeyBind('D', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("Heel", "Min targets to Ult(Full Combo)").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"puck_dream_coil", true},
					{"puck_ethereal_jaunt", true},
					{"puck_phase_shift", true},
					{"puck_waning_rift", true},
					{"puck_illusory_orb", true}
				})));

			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_veil_of_discord", true},
					{"item_blink", true},
					{"item_dagon", true},
					{"item_cyclone", true},
					{"item_ethereal_blade", true},
					{"item_shivas_guard", true},
					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_sheepstick", true}
				})));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}