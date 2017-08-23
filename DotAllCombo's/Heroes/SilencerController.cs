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

    internal class SilencerController : Variables, IHeroController
    {
        private Ability _q, _w, _e, _r;
        private readonly Menu _skills = new Menu("Skills", "Skills");
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _ult = new Menu("AutoUlt", "AutoUlt");
        

        private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost, _force, _cyclone;

        public void Combo()
        {
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

			//spell
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

			_e = Me.Spellbook.SpellE;

			_r = Me.Spellbook.SpellR;

			// Item
			_ethereal = Me.FindItem("item_ethereal_blade");

			_sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");

			_vail = Me.FindItem("item_veil_of_discord");

			_cheese = Me.FindItem("item_cheese");

			_ghost = Me.FindItem("item_ghost");

			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");

			_atos = Me.FindItem("item_rod_of_atos");

			_soul = Me.FindItem("item_soul_ring");

			_arcane = Me.FindItem("item_arcane_boots");

			_blink = Me.FindItem("item_blink");

			_shiva = Me.FindItem("item_shivas_guard");

			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            
			var modifEther = E.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != Me.GetEnemyTeam() && !x.IsIllusion);


			if (Active && Me.IsAlive && E.IsAlive && Utils.SleepCheck("activated"))
			{
				var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
				if (E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade)
				{
					if (
						_q != null
						&& _q.CanBeCasted()
						&& Me.CanCast()
						&& Me.Distance2D(E) < 1400
						&& !stoneModif
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						_q.UseAbility(E.Position);
						Utils.Sleep(200, "Q");
					}
					if ( // atos Blade
						_atos != null
						&& _atos.CanBeCasted()
						&& Me.CanCast()
						&& !E.IsLinkensProtected()
						&& !E.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_atos.Name)
						&& Me.Distance2D(E) <= 2000
						&& Utils.SleepCheck("atos")
						)
					{
						_atos.UseAbility(E);

						Utils.Sleep(250 + Game.Ping, "atos");
					} // atos Item end

					if (
						_blink != null
						&& _q.CanBeCasted()
						&& Me.CanCast()
						&& _blink.CanBeCasted()
						&& Me.Distance2D(E) > 1000
						&& !stoneModif
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						_blink.UseAbility(E.Position);

						Utils.Sleep(250, "blink");
					}
					if (
						_e != null
						&& _e.CanBeCasted()
						&& Me.CanCast()
						&& !E.IsLinkensProtected()
						&& Me.Position.Distance2D(E) < 1400
						&& !stoneModif
						&& Utils.SleepCheck("E"))
					{
						_e.UseAbility(E);
						Utils.Sleep(200, "E");
					}
					if (!_e.CanBeCasted() || _e == null)
					{
						if ( // orchid
							_orchid != null
							&& _orchid.CanBeCasted()
							&& Me.CanCast()
							&& !E.IsLinkensProtected()
							&& !E.IsMagicImmune()
							&& Me.Distance2D(E) <= 1400
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
							&& !stoneModif
							&& Utils.SleepCheck("orchid")
							)
						{
							_orchid.UseAbility(E);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!_orchid.CanBeCasted() || _orchid == null ||
							!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
						{
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
							if (!_vail.CanBeCasted() || _vail == null ||
								!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_vail.Name))
							{
								if ( // ethereal
									_ethereal != null
									&& _ethereal.CanBeCasted()
									&& Me.CanCast()
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
								if (!_ethereal.CanBeCasted() || _ethereal == null ||
									!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
								{
									
									if (
										_r != null
										&& _r.CanBeCasted()
										&& Me.CanCast()
										&& (v.Count(x => x.Distance2D(Me) <= 700) >=
											(Menu.Item("Heel").GetValue<Slider>().Value))
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
										&& Utils.SleepCheck("R"))
									{
										_r.UseAbility();
										Utils.Sleep(330, "R");
									}

									if ( // SoulRing Item 
										_soul != null
										&& _soul.CanBeCasted()
										&& Me.CanCast()
                                        && Me.Health <= (Me.MaximumHealth * 0.5)
                                        && Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
										)
									{
										_soul.UseAbility();
									} // SoulRing Item end

									if ( // Arcane Boots Item
										_arcane != null
										&& _arcane.CanBeCasted()
										&& Me.CanCast()
										&& Me.Mana <= _r.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_arcane.Name)
										)
									{
										_arcane.UseAbility();
									} // Arcane Boots Item end

									if ( //Ghost
										_ghost != null
										&& _ghost.CanBeCasted()
										&& Me.CanCast()
										&& ((Me.Position.Distance2D(E) < 300
											 && Me.Health <= (Me.MaximumHealth * 0.7))
											|| Me.Health <= (Me.MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										_ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if ( // Shiva Item
										_shiva != null
										&& _shiva.CanBeCasted()
										&& Me.CanCast()
										&& !E.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
										&& Me.Distance2D(E) <= 600
										)

									{
										_shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end


									if ( // sheep
										_sheep != null
										&& _sheep.CanBeCasted()
										&& Me.CanCast()
										&& !E.IsLinkensProtected()
										&& !E.IsMagicImmune()
										&& Me.Distance2D(E) <= 1400
										&& !stoneModif
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
										&& Utils.SleepCheck("sheep")
										)
									{
										_sheep.UseAbility(E);
										Utils.Sleep(250, "sheep");
									} // sheep Item end

									if ( // Dagon
										Me.CanCast()
										&& _dagon != null
										&& (_ethereal == null
											|| (modifEther
												|| _ethereal.Cooldown < 17))
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

									if (
										// cheese
										_cheese != null
										&& _cheese.CanBeCasted()
										&& Me.Health <= (Me.MaximumHealth * 0.3)
										&& Me.Distance2D(E) <= 700
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
										&& Utils.SleepCheck("cheese")
										)
									{
										_cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
								}
							}
						}
					}
				}
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
				else if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900
										&& !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
				{
					Orbwalking.Orbwalk(E, 0, 1600, false, true);
				}
				Utils.Sleep(200, "activated");
			}
			A();
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("What's wrong, Queenie? Cat got your tongue?");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


		    _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"silencer_last_word", true},
			    {"silencer_global_silence", true},
			    {"silencer_curse_of_the_silent", true},
			    {"silencer_glaives_of_wisdom", true}
			})));
			_items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_orchid", true}, {"item_bloodthorn", true},
			    {"item_ethereal_blade", true},
			    {"item_veil_of_discord", true},
			    {"item_rod_of_atos", true},
			    {"item_sheepstick", true},
			    {"item_arcane_boots", true},
			    {"item_blink", true},
			    {"item_soul_ring", true},
			    {"item_ghost", true},
			    {"item_cheese", true}
			})));
			_ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"silencer_global_silence", true}
			})));
			_ult.AddItem(new MenuItem("Heel", "Min targets to AutoUlt").SetValue(new Slider(2, 1, 5)));
			_items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_force_staff", true},
			    {"item_cyclone", true},
			    {"item_rod_of_atos", true},
			    {"item_dagon", true}
			})));
			Menu.AddSubMenu(_skills);
			Menu.AddSubMenu(_items);
			Menu.AddSubMenu(_ult);
		}

		public void OnCloseEvent()
		{
			
		}

		public void A()
		{
            if (!Me.IsAlive)return;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsIllusion).ToList();
            if (v.Count <= 0) return;
            if (_ult.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name))
			{
				if (
					_r != null
					&& _r.CanBeCasted()
					&& Me.CanCast()
					&& (v.Count(x => x.Distance2D(Me) <= 700) >=
						(Menu.Item("Heel").GetValue<Slider>().Value))
					&& Utils.SleepCheck("R"))
				{
					_r.UseAbility();
					Utils.Sleep(330, "R");
				}
			}
			_force = Me.FindItem("item_force_staff");
			_cyclone = Me.FindItem("item_cyclone");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
           
			foreach (var e in v)
			{
				if (e.IsLinkensProtected() && (Me.IsVisibleToEnemies || Active))
				{
					if (_force != null && _force.CanBeCasted() && Me.Distance2D(e) < _force.GetCastRange() &&
						Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_force.Name) &&
						Utils.SleepCheck(e.Handle.ToString()))
					{
						_force.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (_cyclone != null && _cyclone.CanBeCasted() && Me.Distance2D(e) < _cyclone.GetCastRange() &&
							 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name) &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						_cyclone.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(e) < _orchid.GetCastRange() &&
							 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						_orchid.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (_dagon != null && _dagon.CanBeCasted() && Me.Distance2D(e) < _dagon.GetCastRange() &&
							 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon") &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						_dagon.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
				}
			}
		}
	}
}