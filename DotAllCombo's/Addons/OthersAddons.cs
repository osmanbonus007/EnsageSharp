using DotaAllCombo.Extensions;

namespace DotaAllCombo.Addons
{
	using System.Security.Permissions;
	using Ensage;
	using Ensage.Common.Extensions;
	using SharpDX;
	using System;
	using Heroes;

	using System.Linq;
	using SharpDX.Direct3D9;
	using Service;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	internal class OthersAddons : Variables, IAddon
	{
#pragma warning disable CS0414 // The field 'OthersAddons._load' is assigned but its value is never used
		private static bool _load;
		private static Font _text;
		public static Line Line = new Line(Drawing.Direct3DDevice9);

		public void Load()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			_text = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Monospace",
				   Height = 21,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });
			_load = false;
			Drawing.OnDraw += Drawing_OnDraw;
			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			
			OnLoadMessage();
			Me = ObjectManager.LocalHero;
		}

		public void Unload()
		{
			
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnDraw -= Drawing_OnDraw;
			_load = false;
		}
		private float _lastRange, _attackRange;
		private ParticleEffect _rangeDisplay, _particleEffect;
		/*
		public static readonly List<TrackingProjectile> Projectiles = ObjectManager.TrackingProjectiles.Where(x=>
						x.Source.ClassId == ClassId.CDOTA_Unit_Hero_ArcWarden
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Terrorblade
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_DrowRanger
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Weaver
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Enchantress
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Nevermore
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Obsidian_Destroyer
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Silencer
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Huskar
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Viper
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Sniper
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Razor
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_StormSpirit
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_Morphling
						|| x.Source.ClassId == ClassId.CDOTA_Unit_Hero_DragonKnight).ToList(); */
		public void RunScript()
		{
			if (!MainMenu.OthersMenu.Item("others").IsActive() || !Game.IsInGame || Me == null || Game.IsPaused || Game.IsWatchingGame) return;


            E = Toolset.ClosestToMouse(Me, 10000);
            //TODO:UNAGRRO
            if (MainMenu.OthersMenu.Item("Auto Un Aggro").GetValue<bool>())
			{
				Toolset.UnAggro(Me);
			}
			//TODO:ESCAPE
			/*
	        if (MainMenu.OthersMenu.Item("EscapeAttack").GetValue<bool>() && Me.Level>= Menu.Item("minLVL").GetValue<Slider>().Value)
			{
				
				var meed = Toolset.IfITarget(Me, Projectiles);
				var v =
					 ObjectManager.GetEntities<Hero>()
						 .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.ClassId == meed.Source.ClassId)
						 .ToList();
				foreach (var victim in v)
				{
					if (victim.Distance2D(Me) <= 1000 && Me.IsVisibleToEnemies && (victim.Handle!=e.Handle || Me.Health <= (Me.MaximumHealth * 0.4)))
					{
						AutoDodge.qqNyx();
						AutoDodge.qqTemplarRefraction();
						AutoDodge.qqallHex(victim);
						AutoDodge.qquseShiva();
						AutoDodge.qquseManta();
						AutoDodge.qquseHelbard(victim);
						AutoDodge.qquseGhost();
						AutoDodge.qquseEulEnem(victim);
						AutoDodge.qquseSDisription(victim);
						AutoDodge.qquseSheep(victim);
						AutoDodge.qquseColba(victim);
						AutoDodge.qqsilencerLastWord(victim);
						AutoDodge.qquseSDisription(victim);
						AutoDodge.qquseSheep(victim);
						AutoDodge.qquseColba(victim);
						AutoDodge.qqsilencerLastWord(victim);
						AutoDodge.qqabadonWme();
						AutoDodge.qqodImprisomentMe(victim);
						AutoDodge.qqallStun(victim);
					}
				}
	        }*/
		}

	    private void Drawing_OnDraw(EventArgs args)
		{

			if (Me == null)
				return;
			if (!MainMenu.OthersMenu.Item("others").IsActive() || !Game.IsInGame || Me == null || Game.IsPaused || Game.IsWatchingGame) return;

			//TODO:ATTACKRANGE
			if (MainMenu.OthersMenu.Item("ShowAttakRange").GetValue<bool>())
			{
                Item item = Me.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && (x.Name.Contains("item_dragon_lance") || x.Name.Contains("item_hurricane_pike")));
                
                if (Me.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && Me.HasModifier("modifier_troll_warlord_berserkers_rage"))
			        _attackRange = 150 + Me.HullRadius+24;
                   else if (Me.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && !Me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                    _attackRange = Me.GetAttackRange() + Me.HullRadius + 24;
                else
                if (Me.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin)
                    _attackRange = Me.GetAttackRange() + Me.HullRadius;
                else 
                if (Me.ClassId == ClassId.CDOTA_Unit_Hero_DragonKnight && Me.HasModifier("modifier_dragon_knight_dragon_form"))
                    _attackRange = Me.GetAttackRange() + Me.HullRadius+24;
                else
                if(item == null && Me.IsRanged)
                    _attackRange = Me.GetAttackRange() + Me.HullRadius + 24;
                else
               if (item !=null && Me.IsRanged)
                    _attackRange = Me.GetAttackRange() + Me.HullRadius + 24;
                else
                    _attackRange = Me.GetAttackRange() + Me.HullRadius;
                if (_rangeDisplay == null)
					{
						if (Me.IsAlive)
						{
							_rangeDisplay = Me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							_rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							_rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							_rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
					else
					{
						if (!Me.IsAlive)
						{
							_rangeDisplay.Dispose();
							_rangeDisplay = null;
						}
						else if (_lastRange.Equals(_attackRange))
						{
							_rangeDisplay.Dispose();
							_lastRange = _attackRange;
							_rangeDisplay = Me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							_rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							_rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							_rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
				}
				else
				{
					if (_rangeDisplay != null) _rangeDisplay.Dispose();
					_rangeDisplay = null;
				}


            //TODO:TARGETMARKER
            E = Toolset.ClosestToMouse(Me, 10000);
            if (E != null && E.IsValid && !E.IsIllusion && E.IsAlive && E.IsVisible &&
			    MainMenu.OthersMenu.Item("ShowTargetMarker").GetValue<bool>())
			{
				DrawTarget();
			}
			else if (_particleEffect != null)
			{
				_particleEffect.Dispose();
				_particleEffect = null;
			}
			// TY  splinterjke.:)
		}

		private void DrawTarget()
	    {
			if (_particleEffect == null)
			{
				_particleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", E);    
				_particleEffect.SetControlPoint(2, new Vector3(Me.Position.X, Me.Position.Y, Me.Position.Z));
				_particleEffect.SetControlPoint(6, new Vector3(1, 0, 0)); 
				_particleEffect.SetControlPoint(7, new Vector3(E.Position.X, E.Position.Y, E.Position.Z));
			}
			else 
			{
				_particleEffect.SetControlPoint(2, new Vector3(Me.Position.X, Me.Position.Y, Me.Position.Z));
				_particleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
				_particleEffect.SetControlPoint(7, new Vector3(E.Position.X, E.Position.Y, E.Position.Z));
			}
		}
		private static void Drawing_OnPostReset(EventArgs args)
		{
			_text.OnResetDevice();
		}

		private static void Drawing_OnPreReset(EventArgs args)
		{
			_text.OnLostDevice();
		}
		
		private void OnLoadMessage()
		{
			Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon OtherAddons is Loaded!</font>");
			Service.Debug.Print.ConsoleMessage.Encolored("@addon OtherAddons is Loaded!", ConsoleColor.Yellow);
		}
	}
}
