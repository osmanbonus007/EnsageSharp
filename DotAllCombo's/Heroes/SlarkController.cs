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
	using SharpDX.Direct3D9;
    using Service.Debug;
	using System.Threading.Tasks;
	internal class SlarkController : Variables, IHeroController
    {
		private Ability _q, _w, _r;
        protected bool Trying;
		private Item _urn, _ethereal, _dagon, _halberd, _mjollnir, _abyssal, _mom, _mail, _bkb, _satanic, _blink, _armlet, _medall, _orchid;
		private Font _text;
		private Line _line;

		public void OnLoadEvent()
		{
            AssemblyExtensions.InitAssembly("Mhoska/Modif by Vick", "0.1b");

            Print.LogMessage.Success("If I'd known I'd end up here, I'd have stayed in Dark Reef Prison.");
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Key Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
			Menu.AddItem(new MenuItem("healUlt", "Use Ult if Me Health % <").SetValue(new Slider(25, 15, 80)));
			Menu.AddItem(
			new MenuItem("Skills", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"slark_shadow_dance", true},
			    {"slark_pounce", true},
			    {"slark_dark_pact", true}
			})));
			Menu.AddItem(
				new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_blink", true},
				    {"item_orchid", true},
				    {"item_bloodthorn", true},
				    {"item_armlet", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_abyssal_blade", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			_text = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Segoe UI",
					Height = 17,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.ClearType
				});

			_line = new Line(Drawing.Direct3DDevice9);
		}

		public async void Combo()
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame) return;
			
			if (!Menu.Item("enabled").IsActive()) return;

            E = Toolset.ClosestToMouse(Me);

            if (E == null) return;
			_q = Me.Spellbook.SpellQ;

			_w = Me.Spellbook.SpellW;

			_r = Me.Spellbook.SpellR;
			_mom = Me.FindItem("item_mask_of_madness");
			_urn = Me.FindItem("item_urn_of_shadows");
			_dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			_ethereal = Me.FindItem("item_ethereal_blade");
			_halberd = Me.FindItem("item_heavens_halberd");
			_mjollnir = Me.FindItem("item_mjollnir");
			_armlet = Me.FindItem("item_armlet");
			_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			_abyssal = Me.FindItem("item_abyssal_blade");
			_mail = Me.FindItem("item_blade_mail");
			_bkb = Me.FindItem("item_black_king_bar");
			_blink = Me.FindItem("item_blink");
			_satanic = Me.FindItem("item_satanic");
			_medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
			var invisModif = Toolset.invUnit(Me);

			

			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			if (invisModif) return;
		    if (!Active) return;
		    {
		        if (E != null && (!E.IsValid || !E.IsVisible || !E.IsAlive))
		        {
		            E = null;
		        }
		        if (E == null || !E.IsAlive || E.IsInvul() || E.IsIllusion) return;
		        if (!Me.CanCast() || Me.IsChanneling()) return;
		        var angle = Me.FindAngleBetween(E.Position, true);
		        var pos = new Vector3((float)(E.Position.X - 100 * Math.Cos(angle)), (float)(E.Position.Y - 100 * Math.Sin(angle)), 0);

		        if (_blink != null
		            && _blink.Cooldown <= 0
		            && Me.Distance2D(pos) <= 1180
		            && Me.Distance2D(E) >= 400 &&
		            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
		        {
		            _blink.UseAbility(pos);
		        }
		        if (_w != null && _w.CanBeCasted()
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
		        {
		            if (E.NetworkActivity == NetworkActivity.Move)
		            {
		                var vectorOfMovement = new Vector2((float)Math.Cos(E.RotationRad) * E.MovementSpeed, (float)Math.Sin(E.RotationRad) * E.MovementSpeed);
		                var hitPosition = Interception(E.Position, vectorOfMovement, Me.Position, 933.33f);
		                var hitPosMod = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(Me, hitPosition)), vectorOfMovement.Y * (TimeToTurn(Me, hitPosition)), 0);
		                var hitPosMod2 = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(Me, hitPosMod)), vectorOfMovement.Y * (TimeToTurn(Me, hitPosMod)), 0);

		                if (GetDistance2D(Me, hitPosMod2) > (755 + E.HullRadius))
		                {
		                    return;
		                }
		                if (IsFacing(Me, hitPosMod2))
		                {
		                    _w.UseAbility();
		                    Trying = true;
		                    await Task.Delay(400); //Avoid trying to W multiple times.
		                    Trying = false;
		                }
		                else
		                {
		                    Me.Move((hitPosMod2 - Me.Position) * 50 / (float)GetDistance2D(hitPosMod2, Me) + Me.Position);
		                }
		            }
		            else
		            {
		                if (GetDistance2D(Me, E) > (755 + E.HullRadius))
		                {
		                    return;
		                }
		                if (IsFacing(Me, E))
		                {
		                    _w.UseAbility();
		                    Trying = true;
		                    await Task.Delay(400);
		                    Trying = false;
		                }
		                else
		                {
		                    Me.Move((E.Position - Me.Position) * 50 / (float)GetDistance2D(E, Me) + Me.Position);
		                }
		            }
		        }
		        if (_w != null && (!_w.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)))
		        {
		            if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
		            {
		                Orbwalking.Orbwalk(E, 0, 1600, true, true);
		            }
		        }
		        if (_q != null 
		            && _q.CanBeCasted()
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
		            && Me.Distance2D(E) <= 325
		            && Utils.SleepCheck("Q"))
		        {
		            _q.UseAbility();
		            Utils.Sleep(200, "Q");
		        }
		        if (_orchid != null 
		            && _orchid.CanBeCasted() 
		            && Me.Distance2D(E) <= 400
		            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name) 
		            && Utils.SleepCheck("orchid"))
		        {
		            _orchid.UseAbility(E);
		            Utils.Sleep(100, "orchid");
		        }
		        if (_r != null 
		            && _r.IsValid 
		            && _r.CanBeCasted() 
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
		            && Me.Health <= Me.MaximumHealth / 100 * Menu.Item("healUlt").GetValue<Slider>().Value 
		            && Utils.SleepCheck("R"))
		        {
		            _r.UseAbility();
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
		        if (_armlet != null && !_armlet.IsToggled &&
		            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_armlet.Name) &&
		            Utils.SleepCheck("armlet"))
		        {
		            _armlet.ToggleAbility();
		            Utils.Sleep(300, "armlet");
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
		        var v = ObjectManager.GetEntities<Hero>().Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
		            .ToList();
		        if (_mail != null && _mail.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
		                                                     (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
		            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mail.Name) && Utils.SleepCheck("mail"))
		        {
		            _mail.UseAbility();
		            Utils.Sleep(100, "mail");
		        }
		        if (_bkb == null || !_bkb.CanBeCasted() ||
		            (v.Count(x => x.Distance2D(Me) <= 650) < (Menu.Item("Heel").GetValue<Slider>().Value)) ||
		            !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) || !Utils.SleepCheck("bkb")) return;
		        _bkb.UseAbility();
		        Utils.Sleep(100, "bkb");
		    }
		}
		public void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			_line.GLLines = true;
			_line.Antialias = false;
			_line.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			_line.Begin();
			_line.Draw(vLine, color);
			_line.End();
		}

		public void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
		

		private  double GetDistance2D(dynamic a, dynamic b)
		{
			if (!(a is Unit || a is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", nameof(a));
			if (!(b is Unit || b is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", nameof(b));
			if (a is Unit) a = a.Position;
			if (b is Unit) b = b.Position;

			return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
		}

		private static Vector3 Interception(Vector3 x, Vector2 y, Vector3 z, float s)
		{
			var x1 = x.X - z.X;
			var y1 = x.Y - z.Y;

			var hs = y.X * y.X + y.Y * y.Y - s * s;
			var h1 = x1 * y.X + y1 * y.Y;
			float t;

		    const double tolerance = 0;
		    if (Math.Abs(hs) < tolerance)
			{
				t = -(x1 * x1 + y1 * y1) / 2 * h1;
			}
			else
			{
				var mp = -h1 / hs;
				var d = mp * mp - (x1 * x1 + y1 * y1) / hs;

				var root = (float)Math.Sqrt(d);

				var t1 = mp + root;
				var t2 = mp - root;

				var tMin = Math.Min(t1, t2);
				var tMax = Math.Max(t1, t2);

				t = tMin > 0 ? tMin : tMax;
			}
			return new Vector3(x.X + t * y.X, x.Y + t * y.Y, x.Z);
		}

		private static bool IsFacing(Entity startUnit, dynamic enemy)
		{
			if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
			if (enemy is Unit) enemy = enemy.Position;

			float deltaY = startUnit.Position.Y - enemy.Y;
			float deltaX = startUnit.Position.X - enemy.X;
			var angle = (float)(Math.Atan2(deltaY, deltaX));

			var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
			var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

			return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.1;
		}

		private static float TimeToTurn(Entity startUnit, dynamic enemy)
		{
			if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
			if (enemy is Unit) enemy = enemy.Position;

			const double turnRate = 0.5; //Game.FindKeyValues(string.Format("{0}/MovementTurnRate", StartUnit.Name), KeyValueSource.Hero).FloatValue; // (Only works in lobby)

			float deltaY = startUnit.Position.Y - enemy.Y;
			float deltaX = startUnit.Position.X - enemy.X;
			var angle = (float)(Math.Atan2(deltaY, deltaX));

			var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
			var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

			var calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

			if (calc < 0.1 && calc > -0.1) return 0;

			return (float)(calc * (0.03 / turnRate));
		}
		

		private void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			_text.Dispose();
			_line.Dispose();
		}
		
		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
		}
	}
}