

using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
	using Ensage.Common;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Timers;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;

    internal class PudgeController : Variables, IHeroController
	{
		public Ability Q, W, R;
		public Item Urn, Ethereal, Dagon, Glimmer, Vail, Orchid, Atos, Leans, Shiva, Mail, Sheep, Abyssal, Bkb, Lotus;
		public float EMoveSpeed , MinRangeHook;
		public Timer Time;
		public readonly Menu Skills = new Menu("Skills", "Skills");
		public readonly Menu Items = new Menu("Items", "Items");
		public float CastRange;
		public Vector3 HookPosition, CastPos;
		public double TimeTurn;

		public void OnLoadEvent()
		{
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(Skills);
			Menu.AddSubMenu(Items);
			Skills.AddItem(new MenuItem("Skills", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{   {"pudge_meat_hook",true},
				{"pudge_rot",true},
				{"pudge_dismember",true}
			})));
			Items.AddItem(new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_abyssal_blade",true},
				{"item_sheepstick",true},
				{"item_bloodthorn", true},
				{"item_dagon",true},
				{"item_lotus_orb", true},
				{"item_orchid",true},
				{"item_urn_of_shadows",true},
				{"item_ethereal_blade",true},
				{"item_veil_of_discord",true},
				{"item_rod_of_atos",true},
				{"item_shivas_guard",true},
				{"item_blade_mail",true},
				{"item_black_king_bar",true}
			})));
			Menu.AddItem(new MenuItem("z", "Toggle Cancelling Hook Time").SetValue(new Slider(100, 200, 280)));
			Menu.AddItem(new MenuItem("x", "Cancelling Hook").SetValue(new Slider(70, 1, 75)));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
			Time = new Timer(Menu.Item("z").GetValue<Slider>().Value);
			Time.Elapsed += CancelHook;
		} // OnLoad
		public void OnCloseEvent()
		{
			Time.Elapsed -= CancelHook;
			Time = null;
		} // OnClose


		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive() || Game.IsChatOpen || Time.Enabled)
				return;
			Me = ObjectManager.LocalHero;

			Q = Me.Spellbook.SpellQ;
			W = Me.Spellbook.SpellW;
			R = Me.Spellbook.SpellR;
			
			Leans = Me.FindItem("item_aether_lens");
			Urn = Me.FindItem("item_urn_of_shadows");
			Dagon = Me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			Ethereal = Me.FindItem("item_ethereal_blade");
			Shiva = Me.FindItem("item_shivas_guard");
			Glimmer = Me.FindItem("item_glimmer_cape");
			Vail = Me.FindItem("item_veil_of_discord");
			Orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
			Abyssal = Me.FindItem("item_abyssal_blade");
			Atos = Me.FindItem("item_rod_of_atos");
			Bkb = Me.FindItem("item_black_king_bar");
			Mail = Me.FindItem("item_blade_mail");
			Lotus = Me.FindItem("item_lotus_orb");
			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key);
			var v = ObjectManager.GetEntities<Hero>().Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune()).ToList();

			if (Active && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
			{
				if (Me.HasModifier("modifier_pudge_rot") && v.Count(x => x.Distance2D(Me) <= W.GetCastRange()+Me.HullRadius) == 0)
				{
					W.ToggleAbility();
					Time.Start();
				}
				else if(!Me.HasModifier("modifier_pudge_rot") && v.Count(x => x.Distance2D(Me) <= W.GetCastRange() + Me.HullRadius) > 0)
				{
					W.ToggleAbility();
					Time.Start();
				}
			}

            E = Toolset.ClosestToMouse(Me);
            if (E == null || !Me.IsAlive) return;
			Sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
			if (R.IsInAbilityPhase || R.IsChanneling || Me.IsChanneling()) return;
			if (Active)
			{
				MinRangeHook = E.HullRadius + 27;
				CastRange = Leans != null ? (Q.CastRange + 200 + E.HullRadius) : (Q.CastRange + E.HullRadius);
				EMoveSpeed = E.HasModifier("modifier_spirit_breaker_charge_of_darkness") ? 550 + ((int)E.Spellbook.Spell1.Level * 50) : E.MovementSpeed;

				Vector2 vector = new Vector2((float)Math.Cos(E.RotationRad) * EMoveSpeed, (float)Math.Sin(E.RotationRad) * EMoveSpeed);
				Vector3 start = new Vector3((float)((0.3 + (Game.Ping / 1000)) * Math.Cos(E.RotationRad) * EMoveSpeed + E.Position.X),
											(float)((0.3 + (Game.Ping / 1000)) * Math.Sin(E.RotationRad) * EMoveSpeed + E.NetworkPosition.Y), E.NetworkPosition.Z);
				Vector3 specialPosition = new Vector3((float)(MinRangeHook * Math.Cos(E.RotationRad) + E.NetworkPosition.X),
													(float)(MinRangeHook * Math.Sin(E.RotationRad) + E.NetworkPosition.Y),
													E.NetworkPosition.Z);
				HookPosition = Interception(start, vector, Me.Position, 1600);
				if (
					Atos != null && Atos.CanBeCasted() && Me.CanCast() && !E.IsMagicImmune() && !E.HasModifier("modifier_spirit_breaker_charge_of_darkness")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Atos.Name) && Me.Distance2D(E) <= 1500 && Utils.SleepCheck("a")
					)
				{
					Atos.UseAbility(E);
					Utils.Sleep(250, "a");
				}
				else if (Q.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
				{
					if (E.NetworkActivity == NetworkActivity.Move || E.HasModifier("modifier_spirit_breaker_charge_of_darkness"))
					{
						for (double i = 0.03; i <= 0.135; i += 0.03)
						{
							Vector3 estimated = new Vector3((float)(i * Math.Cos(E.RotationRad) * EMoveSpeed + HookPosition.X),
															(float)(i * Math.Sin(E.RotationRad) * EMoveSpeed + HookPosition.Y), E.NetworkPosition.Z);
							if (GetTimeToTurn(estimated) <= i)
							{
								HookPosition = estimated;
								TimeTurn = i;
								break;
							}
						}
						CastPos = (HookPosition - Me.Position) * ((Q.GetCastRange()+Me.HullRadius) / HookPosition.Distance2D(Me.Position)) + Me.Position;
						if (Me.Position.Distance2D(HookPosition) < CastRange)
						{
							Q.UseAbility(CastPos); Time.Interval = 150 + TimeTurn * 1000;
							Time.Start();
						}
					}
					else
					{
						CastPos = (specialPosition - Me.Position) * ((Q.GetCastRange() + Me.HullRadius) / specialPosition.Distance2D(Me.Position)) + Me.Position;
						if (Me.Position.Distance2D(E.NetworkPosition) < CastRange)
						{
							Q.UseAbility(CastPos);
							Time.Start();
						}
					}
				}
				else
				{
					if (R.IsInAbilityPhase || R.IsChanneling)
						return;
					uint countElse = 0;
					countElse += 1;
					if (Vail != null && Vail.CanBeCasted() && Me.Distance2D(E) <= 1100 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Vail.Name) && Utils.SleepCheck("vail"))
					{
						Vail.UseAbility(E.Position);
						Utils.Sleep(130, "vail");
					}
					else countElse += 1;
					if (Orchid != null && Orchid.CanBeCasted() && Me.Distance2D(E) <= 900 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Orchid.Name) && Utils.SleepCheck("orchid"))
					{
						Orchid.UseAbility(E);
						Utils.Sleep(100, "orchid");
					}
					else countElse += 1;
					if (Shiva != null && Shiva.CanBeCasted() && Me.Distance2D(E) <= 600 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name) && !E.IsMagicImmune() && Utils.SleepCheck("Shiva"))
					{
						Shiva.UseAbility();
						Utils.Sleep(100, "Shiva");
					}
					else countElse += 1;
					if (Ethereal != null && Ethereal.CanBeCasted() && Me.Distance2D(E) <= 700 && Me.Distance2D(E) <= 400 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Ethereal.Name) && Utils.SleepCheck("ethereal"))
					{
						Ethereal.UseAbility(E);
						Utils.Sleep(100, "ethereal");
					}
					else countElse += 1;
					if (Dagon != null && Dagon.CanBeCasted() && Me.Distance2D(E) <= Dagon.GetCastRange() && Utils.SleepCheck("dagon"))
					{
						Dagon.UseAbility(E);
						Utils.Sleep(100, "dagon");
					}
					else countElse += 1;
					if (Urn != null && Urn.CanBeCasted() && Urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Urn.Name) && Utils.SleepCheck("urn"))
					{
						Urn.UseAbility(E);
						Utils.Sleep(240, "urn");
					}
					else countElse += 1;
					if (Glimmer != null && Glimmer.CanBeCasted() && Me.Distance2D(E) <= 300 && Utils.SleepCheck("glimmer"))
					{
						Glimmer.UseAbility(Me);
						Utils.Sleep(200, "glimmer");
					}
					else countElse += 1;
					if (Mail != null && Mail.CanBeCasted() && v.Count(x => x.Distance2D(Me) <= 650) >=
															 (Menu.Item("Heel").GetValue<Slider>().Value)
															 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Mail.Name)
															 && Utils.SleepCheck("mail"))
					{
						Mail.UseAbility();
						Console.WriteLine(countElse.ToString());
						Utils.Sleep(100, "mail");
					}
					else countElse += 1;
					if (Bkb != null && Bkb.CanBeCasted() && v.Count(x => x.Distance2D(Me) <= 650) >=
															 (Menu.Item("Heel").GetValue<Slider>().Value)
															 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Bkb.Name)
															 && Utils.SleepCheck("bkb"))
					{
						Bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}

					else countElse += 1;
					if (Lotus != null && Lotus.CanBeCasted() && v.Count(x => x.Distance2D(Me) <= 650) >= 2
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Lotus.Name)
						&& Utils.SleepCheck("lotus"))
					{
						Lotus.UseAbility(Me);
						Utils.Sleep(100, "lotus");
					}
					else countElse += 1;
					if (countElse == 11 && R != null && R.CanBeCasted() && Me.Distance2D(E) <= R.GetCastRange() + 150 && (!Urn.CanBeCasted() || Urn.CurrentCharges <= 0) && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& Utils.SleepCheck("R")
						)
					{
						R.UseAbility(E);
						Utils.Sleep(150, "R");
					}
					else countElse += 1;
					if (Abyssal != null && !R.CanBeCasted() && Abyssal.CanBeCasted() && !E.IsStunned() && !E.IsHexed()
						&& Me.Distance2D(E) <= 300 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Abyssal.Name) && Utils.SleepCheck("abyssal"))
					{
						Abyssal.UseAbility(E);
						Utils.Sleep(200, "abyssal");
					}
					else countElse += 1;
					if (Sheep != null && !R.CanBeCasted() && Sheep.CanBeCasted() && !E.IsStunned() && !E.IsHexed()
						&& Me.Distance2D(E) <= 900 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Sheep.Name) && Utils.SleepCheck("sheep"))
					{
						Sheep.UseAbility(E);
						Utils.Sleep(200, "sheep");
					}
					else countElse += 1;
					if (countElse == 14 && Me.Distance2D(E) <= 300 && Mail != null
						&& Mail.CanBeCasted() && (E.NetworkActivity == NetworkActivity.Attack || E.Spellbook.Spells.All(x => x.IsInAbilityPhase))
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Mail.Name)
							&& Utils.SleepCheck("mail"))
					{
						Mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					else countElse += 1;
					if (countElse == 15 && Lotus != null && Lotus.CanBeCasted() && Me.Distance2D(E) <= 600
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Lotus.Name)
						&& Utils.SleepCheck("lotus"))
					{
						Lotus.UseAbility(Me);
						Utils.Sleep(100, "lotus");
					}
					if ((R == null || !R.CanBeCasted() || !Q.CanBeCasted() && Me.Distance2D(E) >= R.GetCastRange() + Me.HullRadius && !E.HasModifier("pudge_meat_hook")) && !E.IsAttackImmune())
					{
						if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
						{
							Orbwalking.Orbwalk(E, 0, 1600, true, true);
						}
					}
				}
			}
		}
		
		public Vector3 Interception(Vector3 a, Vector2 v, Vector3 b, float s)
		{
			float ox = a.X - b.X;
			float oy = a.Y - b.Y;

			float h1 = v.X * v.X + v.Y * v.Y - s * s;
			float h2 = ox * v.X + oy * v.Y;
			float t;

		    double TOLERANCE = 0;
		    if (Math.Abs(h1) < TOLERANCE)
			{
				t = -(ox * ox + oy * oy) / 2 * h2;
			}
			else
			{
				float minusPHalf = -h2 / h1;
				float discriminant = minusPHalf * minusPHalf - (ox * ox + oy * oy) / h1;

				float root = (float)Math.Sqrt(discriminant);

				float t1 = minusPHalf + root;
				float t2 = minusPHalf - root;

				float tMin = Math.Min(t1, t2);
				float tMax = Math.Max(t1, t2);

				t = tMin > 0 ? tMin : tMax;
			}
			return new Vector3(a.X + t * v.X, a.Y + t * v.Y, a.Z);
		}

		public float GetTimeToTurn(Vector3 x)
		{
			Hero m = ObjectManager.LocalHero;
			float deltaY = m.Position.Y - x.Y;
			float deltaX = m.Position.X - x.X;
			float angle = (float)(Math.Atan2(deltaY, deltaX));

			return (float)((Math.PI - Math.Abs(Math.Atan2(Math.Sin(m.RotationRad - angle), Math.Cos(m.RotationRad - angle)))) * (0.03 / 0.7));
		}

		public void CancelHook(object s, ElapsedEventArgs args)
        {
            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;
			if (E.HasModifier("modifier_spirit_breaker_charge_of_darkness")) return;

			double travelTime = HookPosition.Distance2D(Me.Position) / 1600;
			Vector3 ePosition = new Vector3((float)((travelTime) * Math.Cos(E.RotationRad) * E.MovementSpeed + E.NetworkPosition.X),
										   (float)((travelTime) * Math.Sin(E.RotationRad) * E.MovementSpeed + E.NetworkPosition.Y), 0);
			if (E != null && E.NetworkActivity == NetworkActivity.Move && ePosition.Distance2D(HookPosition) > MinRangeHook + Menu.Item("x").GetValue<Slider>().Value)
			{
				Me.Stop();
				Time.Stop();
			}
			else
			{
				if (Q!=null)
					Time.Stop();
			}
		}
	}
}