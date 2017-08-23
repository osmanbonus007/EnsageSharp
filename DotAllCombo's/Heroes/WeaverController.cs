using System.Globalization;
using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
	using System.Threading.Tasks;
	using SharpDX;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using Service.Debug;

	internal class WeaverController : Variables, IHeroController
	{
		private Ability _q, _w, _r;
        private float _health;
        private Item _urn, _orchid, _ethereal, _dagon, _halberd, _blink, _mjollnir, _abyssal, _mom, _shiva, _mail, _bkb, _satanic, _medall;
		
		private readonly Menu _ultMe = new Menu("Options Ultimate Me", "Ultimate on an Me.");
		private readonly Menu _ultAlly = new Menu("Options Ultimate Ally", "Ultimate on an Ally.");

		public void Combo()
		{
			if (!Menu.Item("enabled").GetValue<bool>()) return;

			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			_q = Me.Spellbook.SpellQ;
			_r = Me.Spellbook.SpellR;
			_w = Me.Spellbook.SpellW;

			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_ethereal = Me.FindItem("item_ethereal_blade");
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_blink = Me.FindItem("item_blink");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_satanic = Me.FindItem("item_satanic");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			_shiva = Me.FindItem("item_shivas_guard");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

			if (Active && Me.IsInvisible())
			{
				if (Me.Distance2D(E) <= 150 && Utils.SleepCheck("Attack") && Me.NetworkActivity != NetworkActivity.Attack)
				{
					Me.Attack(E);
					Utils.Sleep(150, "Attack");
				}
				else if (Me.Distance2D(E) <= 2400 && Me.Distance2D(E) >= 130 && Me.NetworkActivity != NetworkActivity.Attack && Utils.SleepCheck("Move"))
				{
					Me.Move(E.Position);
					Utils.Sleep(150, "Move");
				}
			}
			else if (Active && Me.Distance2D(E) <= 1400 && E.IsAlive && !Me.IsInvisible())
			{
				if (Me.Distance2D(E) >= 400)
				{
					if (
						_w != null && _w.CanBeCasted() && Me.Distance2D(E) <= 1100
						&& Me.CanAttack()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
						&& Utils.SleepCheck("W")
						)
					{
						_w.UseAbility();
						Utils.Sleep(100, "W");
					}
				}
				if (_w != null && _w.IsInAbilityPhase || Me.HasModifier("modifier_weaver_shukuchi")) return;

				if (Menu.Item("orbwalk").GetValue<bool>())
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
				if (
					_blink != null
					&& Me.CanCast()
					&& _blink.CanBeCasted()
					&& Me.Distance2D(E) < 1190
					&& Me.Distance2D(E) > Me.AttackRange - 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					_blink.UseAbility(E.Position);
					Utils.Sleep(250, "blink");
				}
				if (
					_q != null && _q.CanBeCasted() && Me.Distance2D(E) <= Me.AttackRange + 300
					&& Me.CanAttack()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					_q.UseAbility(E.Position);
					Utils.Sleep(100, "Q");
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
				if ( // orchid
					_orchid != null
					&& _orchid.CanBeCasted()
					&& Me.CanCast()
					&& !E.IsLinkensProtected()
					&& !E.IsMagicImmune()
					&& Me.Distance2D(E) <= Me.AttackRange + 40
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
					&& Utils.SleepCheck("orchid")
					)
				{
					_orchid.UseAbility(E);
					Utils.Sleep(250, "orchid");
				} // orchid Item end

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
			}
			OnTimedEvent();
		}


		public void OnCloseEvent()
		{
            Drawing.OnDraw -= DrawQDamage;
        }

		private void OnTimedEvent()
		{
			if (Game.IsPaused || _r == null) return;
			if (_r != null)
			{
				if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name))
				{
					//Console.WriteLine("PING"+(int)Game.Ping);
					float now = Me.Health;
					Task.Delay(4000 - (int)Game.Ping).ContinueWith(_ =>
					{
						
						float back4 = Me.Health;
						if ((Menu.Item("ultMode2").GetValue<bool>() && (now - back4) >= Menu.Item("MomentDownHealth2").GetValue<Slider>().Value
								|| (Menu.Item("ultMode1").GetValue<bool>() && (((int)Me.MaximumHealth / (4 / (now - back4))) / 1000000) >= ((double)Menu.Item("MomentDownHealth1").GetValue<Slider>().Value / 100))) && _r.CanBeCasted())
						{
							if (_r.CanBeCasted() && Utils.SleepCheck("R"))
							{
								if (!Me.AghanimState())
									_r.UseAbility();
								else
									_r.UseAbility(Me);
								Utils.Sleep(250, "R");
							}
						}
					});

					var ally = ObjectManager.GetEntities<Hero>()
										  .Where(x => x.Team == Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.Equals(Me)).ToList();
					if (Menu.Item("ult").GetValue<bool>() && Me.AghanimState())
					{
						foreach (var a in ally)
						{
							float allyHealth = a.Health;
							Task.Delay(4000 - (int)Game.Ping).ContinueWith(_ =>
							{

								float backAlly4 = a.Health;
								if ((Menu.Item("ultMode2Ally").GetValue<bool>() && (allyHealth - backAlly4) >= Menu.Item("MomentAllyDownHealth2").GetValue<Slider>().Value
										|| (Menu.Item("ultMode1Ally").GetValue<bool>() && (((int)a.MaximumHealth / (4 / (allyHealth - backAlly4))) / 1000000) >= ((double)Menu.Item("MomentAllyDownHealth1").GetValue<Slider>().Value / 100))) && Me.Distance2D(a)<= 1000 + Me.HullRadius)
								{
									if (_r.CanBeCasted() && Utils.SleepCheck("RAlly"))
									{
										_r.UseAbility(a);
										Utils.Sleep(250, "RAlly");
									}
								}
							});
						}
					}
				}
			}
		}
		private bool OnScreen(Vector3 v)
		{
			return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
		}
        private void DrawQDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || !Me.IsAlive || Game.IsWatchingGame)
            {
                return;
            }
            if (Menu.Item("ultDraw").GetValue<bool>())
            {
                float now = Me.Health;
                Task.Delay(4000 - (int)Game.Ping / 1000).ContinueWith(_ =>
                {
                    float back4 = Me.Health;
                    _health = (now - back4);
                    if (_health < 0)
                        _health = 0;
                });
                var screenPos = HUDInfo.GetHPbarPosition(Me);
                if (!OnScreen(Me.Position)) return;
                //TODO test
                var text = Math.Floor(_health).ToString(CultureInfo.InvariantCulture);
                var size = new Vector2(18, 18);
                var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                var position = new Vector2(screenPos.X - textSize.X - 1, screenPos.Y + 1);
                Drawing.DrawText(
                    text,
                    position,
                    size,
                    (Color.LawnGreen),
                    FontFlags.AntiAlias);
                Drawing.DrawText(
                    text,
                    new Vector2(screenPos.X - textSize.X - 0, screenPos.Y + 0),
                    size,
                    (Color.Black),
                    FontFlags.AntiAlias);
            }
        }
        public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("The threads of fate are mine to weave.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"weaver_shukuchi", true},
					{"weaver_the_swarm", true},
					{"weaver_time_lapse", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_mask_of_madness", true},
					{"item_heavens_halberd", true},
					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_blink", true},
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
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));

            _ultMe.AddItem(new MenuItem("ultDraw", "Show Me Lost Health").SetValue(true));
			_ultAlly.AddItem(new MenuItem("ult", "Use ult in Ally").SetValue(true)).SetTooltip("You need have AghanimS!");
			_ultMe.AddItem(new MenuItem("ultMode1", "Use 1 Mode(Number % of lost health)").SetValue(true));
			_ultMe.AddItem(new MenuItem("MomentDownHealth1", "Min Health % Down To Ult").SetValue(new Slider(35, 5))).SetTooltip("Minimal damage % in my max health which I absorb values 4 seconds before using the Ultimate.");

			_ultMe.AddItem(new MenuItem("ultMode2", "Use 2 Mode(Number of lost health)").SetValue(true));
			_ultMe.AddItem(new MenuItem("MomentDownHealth2", "Min Health Down To Ult").SetValue(new Slider(450, 200, 2000))).SetTooltip("Minimal damage which I absorb values 4 seconds before using the Ultimate.");
			_ultAlly.AddItem(new MenuItem("ultMode1Ally", "Use 1 Mode(% Number of lost health)").SetValue(true));
			_ultAlly.AddItem(
				new MenuItem("MomentAllyDownHealth1", "Min Health % Ally Down To Ult").SetValue(new Slider(35, 5))).SetTooltip("Damage % which absorb ally in 4 seconds)");
			_ultAlly.AddItem(new MenuItem("ultMode2Ally", "Use 2 Mode(Number of lost health)").SetValue(true));
			_ultAlly.AddItem(new MenuItem("MomentAllyDownHealth2", "Min Ally count Health Down To Ult").SetValue(new Slider(750, 300, 2000)))
				.SetTooltip("Damage which absorb ally in 4 seconds)");

			Menu.AddSubMenu(_ultMe);
			Menu.AddSubMenu(_ultAlly);
            Drawing.OnDraw += DrawQDamage;
        }
	}
}

