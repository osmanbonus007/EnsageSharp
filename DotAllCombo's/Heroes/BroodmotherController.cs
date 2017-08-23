using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using SharpDX.Direct3D9;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;
	using Service.Debug;

	internal class BroodmotherController : Variables, IHeroController
	{
		private bool _combokey;
		private bool _chasekey;
		private bool _lastHitkey;
		private Item _mom, _abyssal, _soul, _orchid, _shiva, _halberd, _mjollnir, _satanic, _mail, _bkb, _dagon, _medall, _sheep;
		private Ability _q, _w, _r;
		private int _spiderDenies = 65;
		private int _spiderDmgStatick = 175;
		private readonly uint[] _spiderQ = { 74, 149, 224, 299 };
		private int _spiderDmg;
		private bool _useQ;
		private Font _txt;
		private Font _noti;
		private Line _lines;

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Me = ObjectManager.LocalHero;
            E = Toolset.ClosestToMouse(Me);
            _lastHitkey = Menu.Item("LastHit").GetValue<KeyBind>().Active;
			_combokey = Game.IsKeyDown(Menu.Item("ComboKey").GetValue<KeyBind>().Key);
			_chasekey = Game.IsKeyDown(Menu.Item("ChaseKey").GetValue<KeyBind>().Key);
			_useQ = Menu.Item("useQ").IsActive();

			if (_lastHitkey && !_combokey && !_chasekey && Utils.SleepCheck("combo") && !Game.IsPaused)
			{

				_q = Me.Spellbook.SpellQ;

				_soul = Me.FindItem("item_soul_ring");


				var spiderlingsLevel = Me.Spellbook.SpellQ.Level - 1;
				

				var enemies = ObjectManager.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != Me.Team).ToList();

				var creeps = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
				(creep.ClassId == ClassId.CDOTA_Unit_VisageFamiliar && creep.Team != Me.Team) || (creep.ClassId == ClassId.CDOTA_Unit_SpiritBear && creep.Team != Me.Team) || (creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit &&
				creep.Team != Me.Team) || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Health <= 259).ToList();

				var creepQ = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
				creep.ClassId == ClassId.CDOTA_Unit_SpiritBear || creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned)).ToList();

				var list = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling).ToList();


				// Creep Q lasthit
				if (_useQ && _q.CanBeCasted() && Me.IsAlive)
					foreach (var creep in creepQ)
					{
						if (creep.Health <= Math.Floor((_spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 45 && creep.Team != Me.Team)
						{
							if (creep.Position.Distance2D(Me.Position) <= 600 && Utils.SleepCheck("QQQ"))
							{
								if (_soul != null && _soul.CanBeCasted() && Me.Health >= 400)
								{
									_soul.UseAbility();
									Utils.Sleep(300, "QQQ");
								}
								else
									_q.UseAbility(creep);
								Utils.Sleep(300, "QQQ");

							}
						}
					}
				// Enemy Q lasthit
				if (_q.CanBeCasted() && Me.IsAlive)
				{
					foreach (var enemy in enemies)
					{
						if (enemy.Health <= (_spiderQ[spiderlingsLevel] - enemy.MagicDamageResist) && enemy.Health > 0)
						{
							if (enemy.Position.Distance2D(Me.Position) <= 600 && Utils.SleepCheck("QQQ"))
							{
								if (_soul != null && _soul.CanBeCasted() && Me.Health >= 400)
								{
									_soul.UseAbility();
									Utils.Sleep(300, "QQQ");
								}
								else
									_q.UseAbility(E);
								Utils.Sleep(300, "QQQ");
							}
						}
					}
				}

				//var Spiderling = ObjectManager.GetEntities<Unit>().Where(x => x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling && x.IsAlive && x.IsControllable && x.Team == Me.Team).ToList();
				var count = list.Count();
				if (count <= 0)
					return;
				// Autodenies
				for (var s = 0; s < count; ++s)
				{
				    if (list[s].Health <= 0 || list[s].Health > _spiderDenies) continue;
				    for (var z = 0; z < count; ++z)
				    {
				        if (!(list[s].Position.Distance2D(list[z].Position) <= 500) ||
				            !Utils.SleepCheck(list[z].Handle + "Spiderlings")) continue;
				        list[z].Attack(list[s]);
				        Utils.Sleep(350, list[z].Handle + "Spiderlings");
				    }
				}

				// Auto spider deny and lasthit
				var countcreep = creeps.Count();
				if (countcreep>=1)
				{
					for (var c = 0; c < countcreep; c++)
				{
					for (var s = 0; s < count; s++)
					{
						if (creeps[c].Position.Distance2D(list[s].Position) <= 500 &&
								creeps[c].Team != Me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(_spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(list[s].Handle + "Spiderling"))
							{
								{
									list[s].Attack(creeps[c]);
									Utils.Sleep(350, list[s].Handle + "Spiderling");
								}
							}
							else if (creeps[c].Position.Distance2D(list[s].Position) <= 500 &&
								creeps[c].Team == Me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(_spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(list[s].Handle + "Spiderlings"))
							{
								list[s].Attack(creeps[c]);
								Utils.Sleep(350, list[s].Handle + "Spiderlings");
							}
						}
					}
				}

				// Auto spider enemy lasthit
				var countenemy = enemies.Count();
				if (countenemy>=1)
				{
				for (var t = 0; t < countenemy; ++t)
				{
					for (var s = 0; s < count; ++s)
					{
						
							_spiderDmg = list.Count(y => y.Distance2D(enemies[t]) < 800) * list[s].MinimumDamage;

					    if (!((enemies[t].Position.Distance2D(list[s].Position)) <= 800) || enemies[t].Team == Me.Team ||
					        enemies[t].Health <= 0 || !(enemies[t].Health < Math.Floor(_spiderDmg * (1 - enemies[t].DamageResist))) ||
					        !Utils.SleepCheck(list[t].Handle + "AttackEnemies")) continue;
					    list[s].Attack(enemies[t]);
					    Utils.Sleep(350, list[t].Handle + "AttackEnemies");
					}
					}
				}
				Utils.Sleep(290, "combo");
			}
		}


		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "ty Vick");

			Print.LogMessage.Success("Web is World!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));

			var skills = new Dictionary<string, bool>
			{
				{"broodmother_spin_web",true},
				{"broodmother_spawn_spiderlings",true},
				{"broodmother_insatiable_hunger",true}
			};

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(skills)));
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("ChaseKey", "Chase Key").SetValue(new KeyBind('E', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("LastHit", "LastHitCreeps").SetValue(new KeyBind('F', KeyBindType.Toggle)));
			Menu.AddItem(new MenuItem("useQ", "Kill creep Q Spell").SetValue(true));
			Menu.AddItem(new MenuItem("Skills", "Skills: ").SetValue(new AbilityToggler(skills)));

			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Game.OnUpdate += Chasing;
			Game.OnUpdate += ChasingAll;

			Console.WriteLine("> This is your Mom# loaded!");

			_txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 19,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			_noti = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 30,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			_lines = new Line(Drawing.Direct3DDevice9);

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
			Game.OnUpdate -= ChasingAll;
			Game.OnUpdate -= Chasing;
		}


		public void Chasing(EventArgs args)
		{
			Me = ObjectManager.LocalHero;
		    if (!_chasekey) return;
		    var list = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling).ToList();
		    {
		        var count = list.Count();
		        if (E != null && E.IsAlive && !E.IsIllusion)
		        {
		            for (var t = 0; t < count; ++t)
		            {
		                if (!(list[t].Distance2D(E) <= 1000) ||
		                    !Utils.SleepCheck(list[t].Handle + "MoveAttack")) continue;
		                list[t].Attack(E);
		                Utils.Sleep(500, list[t].Handle + "MoveAttack");
		            }
		        }
		        else
		        {
		            for (var t = 0; t < count; ++t)
		            {
		                if (!Utils.SleepCheck(list[t].Handle + "Move")) continue;
		                list[t].Move(Game.MousePosition);
		                Utils.Sleep(500, list[t].Handle + "Move");
		            }
		        }

		    }
		}


		public void ChasingAll(EventArgs args)
		{

            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;
			if (_combokey  && E.IsAlive && !Me.IsVisibleToEnemies)
			{

				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
		    if (!_combokey || !E.IsAlive || !Me.IsVisibleToEnemies) return;
		    var list = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling).ToList();
		    var count = list.Count();
		    for (int s = 0; s < count; ++s)
		    {
		        if (list[s].Distance2D(E) <= 1500 && Utils.SleepCheck(list[s].Handle + "Spiderlings"))
		        {
		            list[s].Attack(E);
		            Utils.Sleep(500, list[s].Handle + "Spiderlings");
		        }
		    }
		    for (int s = 0; s < count; ++s)
		    {
		        if (list[s].Distance2D(E) >= 1500 && Utils.SleepCheck(list[s].Handle + "Spiderlings"))
		        {
		            list[s].Move(Game.MousePosition);
		            Utils.Sleep(500, list[s].Handle + "Spiderlings");
		        }
		    }


		    var linkens = E.IsLinkensProtected();
		    if (E.IsAlive && !E.IsIllusion && Me.Distance2D(E) <= 1000)
		    {
					
		        _q = Me.Spellbook.SpellQ;
					
		        _w = Me.Spellbook.SpellW;
					
		        _r = Me.Spellbook.SpellR;

		        // Item

		        _sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");

		        Me.FindItem("item_cheese");

		        _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");

		        _soul = Me.FindItem("item_soul_ring");

		        _shiva = Me.FindItem("item_shivas_guard");

		        _dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

		        _mom = Me.FindItem("item_mask_of_madness");

		        _abyssal = Me.FindItem("item_abyssal_blade");

		        _mjollnir = Me.FindItem("item_mjollnir");

		        _halberd = Me.FindItem("item_heavens_halberd");

		        _mail = Me.FindItem("item_blade_mail");

		        _bkb = Me.FindItem("item_black_king_bar");

		        _medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");

		        _satanic = Me.FindItem("item_satanic");

		        if ( // Q Skill
		            _q != null &&
		            _q.CanBeCasted() &&
		            Me.CanCast()
		            && Me.IsVisibleToEnemies &&
		            !E.IsMagicImmune()
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
		            && Me.Distance2D(E) <= 600
		            && Utils.SleepCheck("Q")
		        )

		        {
		            _q.UseAbility(E);
		            Utils.Sleep(250, "Q");
		        } // Q Skill end




		        if (//R Skill
		            _r != null &&
		            _r.CanBeCasted() &&
		            Me.CanCast() &&
		            Me.Distance2D(E) <= 350
		            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
		            && Utils.SleepCheck("R")
		        )
		        {
		            _r.UseAbility();
		            Utils.Sleep(250, "R");
		        } // R Skill end


		        if ( // orchid
		            _orchid != null &&
		            _orchid.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            !linkens &&
		            Utils.SleepCheck("orchid")
		            && Me.IsVisibleToEnemies
		            && Me.Distance2D(E) <= 1000
		        )
		        {
		            _orchid.UseAbility(E);
		            Utils.Sleep(250, "orchid");
		        } // orchid Item end

		        if ( // sheep
		            _sheep != null
		            && Me.IsVisibleToEnemies &&
		            _sheep.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            !linkens &&
		            Utils.SleepCheck("sheep") &&
		            Me.Distance2D(E) <= 600
		        )
		        {
		            _sheep.UseAbility(E);
		            Utils.Sleep(250, "sheep");
		        } // sheep Item end

		        if (// Soul Item 
		            _soul != null
		            && _q!=null 
		            && _q.CanBeCasted()
		            && Me.Health >= (Me.MaximumHealth * 0.6)
		            && Me.Mana <= _q.ManaCost
		            && _soul.CanBeCasted())
		        {
		            _soul.UseAbility();
		        } // Soul Item end

		        if (// Shiva Item
		            _shiva != null
		            && Me.IsVisibleToEnemies &&
		            _shiva.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            Utils.SleepCheck("shiva") &&
		            Me.Distance2D(E) <= 600
		        )
		        {
		            _shiva.UseAbility();
		            Utils.Sleep(250, "shiva");
		        } // Shiva Item end

		        if (// MOM
		            _mom != null &&
		            _mom.CanBeCasted() &&
		            Me.CanCast() &&
		            Utils.SleepCheck("mom") &&
		            Me.Distance2D(E) <= 700
		        )
		        {
		            _mom.UseAbility();
		            Utils.Sleep(250, "mom");
		        } // MOM Item end

		        if ( // Medall
		            _medall != null
		            && Me.IsVisibleToEnemies &&
		            _medall.CanBeCasted() &&

		            Utils.SleepCheck("Medall") &&
		            Me.Distance2D(E) <= 500
		        )
		        {
		            _medall.UseAbility(E);
		            Utils.Sleep(250, "Medall");
		        } // Medall Item end

		        if ( // Abyssal Blade
		            _abyssal != null
		            && Me.IsVisibleToEnemies &&
		            _abyssal.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            Utils.SleepCheck("abyssal") &&
		            Me.Distance2D(E) <= 400
		        )
		        {
		            _abyssal.UseAbility(E);
		            Utils.Sleep(250, "abyssal");
		        } // Abyssal Item end

		        if ( // Hellbard
		            _halberd != null
		            && Me.IsVisibleToEnemies &&
		            _halberd.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            Utils.SleepCheck("halberd") &&
		            Me.Distance2D(E) <= 700
		        )
		        {
		            _halberd.UseAbility(E);
		            Utils.Sleep(250, "halberd");
		        } // Hellbard Item end

		        if ( // Mjollnir
		            _mjollnir != null
		            && Me.IsVisibleToEnemies &&
		            _mjollnir.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            Utils.SleepCheck("mjollnir") &&
		            Me.Distance2D(E) <= 600
		        )
		        {
		            _mjollnir.UseAbility(Me);
		            Utils.Sleep(250, "mjollnir");
		        } // Mjollnir Item end
		        var v =
		            ObjectManager.GetEntities<Hero>()
		                .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
		                .ToList();
		        if (_mail != null && _mail.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
		                                                   (Menu.Item("Heelm").GetValue<Slider>().Value))
		            && Utils.SleepCheck("mail"))
		        {
		            _mail.UseAbility();
		            Utils.Sleep(100, "mail");
		        }
		        if (_bkb != null && _bkb.CanBeCasted() && (v.Count(x => x.Distance2D(Me) <= 650) >=
		                                                 (Menu.Item("Heel").GetValue<Slider>().Value))
		            && Utils.SleepCheck("bkb"))
		        {
		            _bkb.UseAbility();
		            Utils.Sleep(100, "bkb");
		        }
		        if (// Dagon
		            _dagon != null &&
		            _dagon.CanBeCasted() &&
		            Me.CanCast() &&
		            !E.IsMagicImmune() &&
		            Utils.SleepCheck("dagon")
		        )
		        {
		            _dagon.UseAbility(E);
		            Utils.Sleep(250, "dagon");
		        } // Dagon Item end


		        if (// Satanic 
		            _satanic != null 
		            && Me.Health <= (Me.MaximumHealth * 0.3)
		            && _satanic.CanBeCasted() 
		            && Me.Distance2D(E) <= 300
		            && Utils.SleepCheck("Satanic")
		        )
		        {
		            _satanic.UseAbility();
		            Utils.Sleep(250, "Satanic");
		        } // Satanic Item end

		        if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
		        {
		            Orbwalking.Orbwalk(E, 0, 1600, true, true);
		        }


		        /***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/

		        var web =
		            ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_broodmother_web").ToList();
		        var spinWeb = GetClosestToWeb(web, Me);
		        if (_w != null && _w.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
		        {
		            if ((Me.Distance2D(spinWeb) >= 900 && E.Distance2D(spinWeb) >= 900) && Me.Distance2D(E) <= 800 && Utils.SleepCheck(spinWeb.Handle + "SpideWeb"))
		            {
		                _w.UseAbility(E.Predict(1100));
		                Utils.Sleep(300, spinWeb.Handle + "SpideWeb");
		            }
		        }
		    }
		    /***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/
		}

		private Unit GetClosestToWeb(List<Unit> units, Hero x)
		{
			Unit closestHero = null;
			foreach (var b in units.Where(v => closestHero == null || closestHero.Distance2D(x) > v.Distance2D(x)))
			{
				closestHero = b;
			}
			return closestHero;
		}

	    private void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			if (!_useQ)
			{
				DrawBox(2, 490, 90, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 490, 90, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("  Q DISABLE", 4, 490, Color.Gold, _txt);
			}

			if (_combokey)
			{
				DrawBox(2, 530, 132, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 530, 132, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("BroodMother: Active!", 4, 530, Color.Gold, _txt);
			}
			if (_chasekey)
			{
				DrawBox(2, 530, 120, 20, 1, new ColorBGRA(0, 0, 30, 70));
				DrawFilledBox(2, 530, 120, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("Spiderling: Active!", 4, 530, Color.Gold, _txt);
			}
			if (_lastHitkey)
			{
				DrawBox(2, 510, 120, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 510, 120, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("LastHit Active", 4, 510, Color.Gold, _txt);
			}
		}


	    private void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			_txt.Dispose();
			_noti.Dispose();
			_lines.Dispose();
		}


	    private void Drawing_OnPostReset(EventArgs args)
		{
			_txt.OnResetDevice();
			_noti.OnResetDevice();
			_lines.OnResetDevice();
		}

	    private void Drawing_OnPreReset(EventArgs args)
		{
			_txt.OnLostDevice();
			_noti.OnLostDevice();
			_lines.OnLostDevice();
		}

		public void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			_lines.GLLines = true;
			_lines.Antialias = false;
			_lines.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			_lines.Begin();
			_lines.Draw(vLine, color);
			_lines.End();
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
	}
}


