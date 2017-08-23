namespace DotaAllCombo.Extensions
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
    using Ensage;
	using SharpDX;
	using Ensage.Common.Extensions;
	using Ensage.Common;

    internal static class Toolset
	{
		private static Hero _me;

        public static bool IsFullMagicResist(this Unit source)
        {
            return source.IsMagicImmune()
                || source.HasModifier("modifier_medusa_stone_gaze_stone")
                || source.HasModifier("modifier_huskar_life_break_charge")
                || source.HasModifier("modifier_oracle_fates_edict");

        }
        public static bool IsFullMagicSpellResist(this Unit source)
        {
            return source.HasModifier("modifier_medusa_stone_gaze_stone")
                || source.HasModifier("modifier_huskar_life_break_charge")
                || source.HasModifier("modifier_oracle_fates_edict")
                || source.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                    || source.HasModifier("modifier_puck_phase_shift")
                    || source.HasModifier("modifier_eul_cyclone")
                    || source.HasModifier("modifier_invoker_tornado")
                    || source.HasModifier("modifier_brewmaster_storm_cyclone")
                    || source.HasModifier("modifier_shadow_demon_disruption")
                    || source.HasModifier("modifier_tusk_snowball_movement")
                    || source.HasModifier("modifier_abaddon_borrowed_time")
                    || source.HasModifier("modifier_faceless_void_time_walk")
                    || source.HasModifier("modifier_huskar_life_break_charge");
        }
        public static float RadiansToFace(Unit startUnit, dynamic v)
		{
			if (!(v is Unit || v is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "v");
			if (v is Unit) v = v.Position;

			float deltaY = startUnit.Position.Y - v.Y;
			float deltaX = startUnit.Position.X - v.X;
			float angle = (float) (Math.Atan2(deltaY, deltaX));

			return
				(float)
					(Math.PI - Math.Abs(Math.Atan2(Math.Sin(startUnit.RotationRad - angle), Math.Cos(startUnit.RotationRad - angle))));
		}

		public static float AttackRange;
	    public static Hero FirstOrDefault { get; private set; }

	    public static void Range()
		{
			Item item =
				_me.Inventory.Items.FirstOrDefault(
					x => x != null && x.IsValid && (x.Name.Contains("item_dragon_lance") || x.Name.Contains("item_hurricane_pike")));



			if (_me.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && _me.HasModifier("modifier_troll_warlord_berserkers_rage"))
				AttackRange = 150 + _me.HullRadius + 24;
			else if (_me.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && !_me.HasModifier("modifier_troll_warlord_berserkers_rage"))
				AttackRange = _me.GetAttackRange() + _me.HullRadius + 24;
			else if (_me.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin)
				AttackRange = _me.GetAttackRange() + _me.HullRadius;
			else if (_me.ClassId == ClassId.CDOTA_Unit_Hero_DragonKnight && _me.HasModifier("modifier_dragon_knight_dragon_form"))
				AttackRange = _me.GetAttackRange() + _me.HullRadius + 24;
			else if (item == null && _me.IsRanged)
				AttackRange = _me.GetAttackRange() + _me.HullRadius + 24;
			else if (item != null && _me.IsRanged)
				AttackRange = _me.GetAttackRange() + _me.HullRadius + 24;
			else
				AttackRange = _me.GetAttackRange() + _me.HullRadius;
		}

		public static bool CheckFace(Ability z, Hero v)
		{
		    FirstOrDefault = ObjectManager.GetEntities<Hero>()
		        .Where(x => x.Distance2D(v) < z.GetCastRange())
		        .OrderBy(x => RadiansToFace(x, v))
		        .FirstOrDefault();
		    return true;

		}

		public static void InitToolset(Hero myHero)
		{
			_me = myHero;
		}

		public static Unit GetClosestToUnit(List<Unit> units, Unit e)
		{
			Unit closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(e) > v.Distance2D(e)))
			{
				closestHero = v;
			}
			return closestHero;
		}
        public static Hero ClosestToMouse(Hero source, float range = 2500)
        {
            var mousePosition = Game.MousePosition;
            var enemyHeroes =
                ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.Team != source.Team && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(mousePosition) <= range);
            Hero[] closestHero = { null };
            foreach (
                var enemyHero in
                    enemyHeroes.Where(
                        enemyHero =>
                            closestHero[0] == null ||
                            closestHero[0].Distance2D(mousePosition) > enemyHero.Distance2D(mousePosition)))
            {
                closestHero[0] = enemyHero;
            }
            return closestHero[0];
        }
        public static Hero GetClosestToTarget(this List<Hero> units, Vector3 position)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(position) > v.Distance2D(position)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		public static void UnAggro(List<Unit> z, Unit v)
		{
			if (v == null) return;
			List<TrackingProjectile> projectiles =
				ObjectManager.TrackingProjectiles.Where(x => x.Target.Handle == v.Handle).ToList();
			if (projectiles.Count <= 0) return;
			for (int i = 0; i < projectiles.Count; ++i)
			{
				var closestCreepUnAgr = GetClosestToUnit(z, v);

				if (projectiles[i].Source.ClassId == ClassId.CDOTA_BaseNPC_Tower
				    || projectiles[i].Source.ClassId == ClassId.CDOTA_Unit_Fountain)
				{
					if (closestCreepUnAgr == null) return;
					if (closestCreepUnAgr.Distance2D(v) <= 500 & Utils.SleepCheck("UnAgr"))
					{
						v.Attack(closestCreepUnAgr);
						Utils.Sleep(500, "UnAgr");
					}
				}
			}
		}
		/*
		public static TrackingProjectile IfITarget(Unit v, List<TrackingProjectile> proj)
		{

			foreach (var projs in proj)
			{

				if (projs.Target.Handle == v.Handle) return projs;
			}
			return null;
		}
		*/

		public static void UnAggro(Unit v)
		{
			if (v == null) return;
			var creepsA = ObjectManager.GetEntities<Unit>().Where(creep =>
				(creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
				 || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
				 || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
				creep.IsAlive && creep.Team == _me.Team && creep.IsVisible && creep.IsSpawned).ToList();

			if (creepsA.Count(x => x.Distance2D(v) <= 500) == 0)
			{
				if (v.ClassId == ClassId.CDOTA_Unit_SpiritBear)
				{
					creepsA = ObjectManager.GetEntities<Unit>().Where(creep => (
						creep.HasInventory && creep.Handle != v.Handle)
					                    && creep.IsAlive && creep.Team == _me.Team
					                    && creep.ClassId != ClassId.CDOTA_Unit_Hero_LoneDruid
					                    && creep.IsVisible &&
					                    creep.Health >= (creep.MaximumHealth*0.5)).ToList();
				}
				else
				{
					creepsA = ObjectManager.GetEntities<Unit>().Where(creep => (
						creep.HasInventory && creep.Handle != v.Handle)
										&& creep.IsAlive && creep.Team == _me.Team &&
										creep.IsVisible &&
										creep.Health >= (creep.MaximumHealth*0.5)).ToList();
				}
			}
			if (creepsA.Count == 0) return;
			UnAggro(creepsA, v);
		}

		public static bool HasStun(Hero x)
		{
			if (x.FindSpell("dragon_knight_dragon_tail") != null &&
			    x.FindSpell("dragon_knight_dragon_tail").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("dragon_knight_dragon_tail").GetCastRange()
			    ||
			    x.FindSpell("earthshaker_echo_slam") != null && x.FindSpell("earthshaker_echo_slam").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("earthshaker_echo_slam").GetCastRange()
			    ||
			    x.FindSpell("legion_commander_duel") != null && x.FindSpell("legion_commander_duel").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("legion_commander_duel").GetCastRange()
			    ||
			    x.FindSpell("leshrac_split_earth") != null && x.FindSpell("leshrac_split_earth").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("leshrac_split_earth").GetCastRange()
			    ||
			    x.FindSpell("leoric_hellfire_blast") != null && x.FindSpell("leoric_hellfire_blast").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("leoric_hellfire_blast").GetCastRange()
			    ||
			    x.FindSpell("lina_light_strike_array") != null && x.FindSpell("lina_light_strike_array").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("lina_light_strike_array").GetCastRange()
			    ||
			    x.FindSpell("lion_impale") != null && x.FindSpell("lion_impale").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("lion_impale").GetCastRange()
			    ||
			    x.FindSpell("magnataur_reverse_polarity") != null &&
			    x.FindSpell("magnataur_reverse_polarity").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("magnataur_reverse_polarity").GetCastRange()
			    ||
			    x.FindSpell("nyx_assassin_impale") != null && x.FindSpell("nyx_assassin_impale").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("nyx_assassin_impale").GetCastRange()
			    ||
			    x.FindSpell("ogre_magi_fireblast") != null && x.FindSpell("ogre_magi_fireblast").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("ogre_magi_fireblast").GetCastRange()
			    ||
			    x.FindSpell("skeleton_king_hellfire_blast") != null &&
			    x.FindSpell("skeleton_king_hellfire_blast").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("skeleton_king_hellfire_blast").GetCastRange()
			    ||
			    x.FindSpell("sven_storm_bolt") != null && x.FindSpell("sven_storm_bolt").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("sven_storm_bolt").GetCastRange()
			    ||
			    x.FindSpell("tiny_avalanche") != null && x.FindSpell("tiny_avalanche").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("tiny_avalanche").GetCastRange()
			    ||
			    x.FindSpell("tusk_walrus_punch") != null && x.FindSpell("tusk_walrus_punch").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("tusk_walrus_punch").GetCastRange()
			    ||
			    x.FindSpell("vengefulspirit_magic_missile") != null &&
			    x.FindSpell("vengefulspirit_magic_missile").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("vengefulspirit_magic_missile").GetCastRange()
			    ||
			    x.FindSpell("windrunner_shackleshot") != null && x.FindSpell("windrunner_shackleshot").Cooldown <= 0 &&
			    _me.Distance2D(x) <= x.FindSpell("windrunner_shackleshot").GetCastRange()
				)
				return true;
			return false;
		}

		public static bool InvUnit(Hero z)
		{
		    return z.Modifiers.Any(
		        x =>
		            (x.Name == "modifier_bounty_hunter_wind_walk" ||
		             x.Name == "modifier_riki_permanent_invisibility" ||
		             x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
		             x.Name == "modifier_weaver_shukuchi" ||
		             x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
		             x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
		             x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
		             x.Name == "modifier_item_silver_edge_windwalk" ||
		             x.Name == "modifier_item_edge_windwalk" ||
		             x.Name == "modifier_nyx_assassin_vendetta" ||
		             x.Name == "modifier_invisible" ||
		             x.Name == "modifier_invoker_ghost_walk_enemy"));
		}
	    // ReSharper disable once InconsistentNaming
	    public static bool invUnit(Hero me)
	    {
	        return me.Modifiers.Any(
	            x =>
	                (x.Name == "modifier_bounty_hunter_wind_walk" ||
	                 x.Name == "modifier_riki_permanent_invisibility" ||
	                 x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
	                 x.Name == "modifier_weaver_shukuchi" ||
	                 x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
	                 x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
	                 x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
	                 x.Name == "modifier_item_silver_edge_windwalk" ||
	                 x.Name == "modifier_item_edge_windwalk" ||
	                 x.Name == "modifier_nyx_assassin_vendetta" ||
	                 x.Name == "modifier_invisible" ||
	                 x.Name == "modifier_invoker_ghost_walk_enemy"));
        }


        /*public static class Item
		{
			public static Ensage.Item
				mom,
				abyssal,
				soul,
				arcane,
				blink,
				shiva,
				halberd,
				mjollnir,
				satanic,
				dagon,
				medall,
				refresh,
				necros,
				diffusal,
				vail,
				sheepstick,
				etheral,
				orchid,
				bkb,
				manta;
		}

		private static void InitItem(Hero source, ref Ensage.Item destination, string itemName1, string itemName2 = null)
		{
			destination = source.Inventory.Items.FirstOrDefault(item => item.Name.Contains(itemName1))
			              ??
			              (itemName2 == null
				              ? null
				              : source.Inventory.Items.FirstOrDefault(item => item.Name.Contains(itemName1)));
		} // Program.InstanceInitItem

		private static void InitItemOnce(Hero source, ref Ensage.Item destination, string itemName1)
		{
			if (destination == null) destination = source.FindItem(itemName1);
		} // Program.InitSpell */
        //private static class Spell
        //{
        //	public static Ability Q, W, E, R;
        //}
        /* private static void InitSpells(Hero source)
         {
             //Spell.Q = source.Spellbook.SpellQ;
             //Spell.W = source.Spellbook.SpellW;
             //Spell.E = source.Spellbook.SpellE;
             //Spell.R = source.Spellbook.SpellR;

             //---------------- Protection ----------------
             /*InitItem(source, ref Item.bkb, "item_black_king_bar");
             //----------------- Initiate -----------------
             InitItem(source, ref Item.blink, "item_blink");
             InitItem(source, ref Item.refresh, "item_refresher");
             //----------------- Disable ------------------
             InitItem(source, ref Item.sheepstick, "item_sheepstick");
             InitItem(source, ref Item.abyssal, "item_abyssal_blade");
             InitItem(source, ref Item.halberd, "item_heavens_halberd");
             InitItem(source, ref Item.satanic, "item_satanic");
             //-------------- Damage Control --------------
             InitItem(source, ref Item.dagon, "item_dagon");
             InitItem(source, ref Item.necros, "item_necronomicon");
             InitItem(source, ref Item.mom, "item_mask_of_madness");
             InitItem(source, ref Item.mjollnir, "item_mjollnir");
             InitItem(source, ref Item.shiva, "item_shivas_guard");
             InitItem(source, ref Item.manta, "item_manta");
             //--------------- Mana Control ---------------
             InitItem(source, ref Item.arcane, "item_arcane_boots");
             InitItem(source, ref Item.soul, "item_soul_ring");
             //--------------- Increase dmg ---------------
             //InitItem(source, ref Item.etheral, "item_etheral_blade");
             Item.etheral = source.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_etheral_blade"));
             InitItem(source, ref Item.orchid, "item_orchid_malevolence");
             InitItem(source, ref Item.diffusal, "item_diffusal_blade");
             InitItem(source, ref Item.vail, "item_veil_of_discord");
             Item.medall = source.FindItem("item_medallion_of_courage") ?? source.FindItem("item_solar_crest");
         }
     }
 }






         private static class Cast
         {
              //TODO: Апнуть полную логику использования способностей.
              * 
              * 
              * 
              * 
              * 
              * 
             public static bool SpellQ()
             {
                 if (e.Modifiers.Any(m => m.Name == "modifier_arc_warden_flux")) return false;
                 // NOTE:
                 // • If the e has allies or neutral creeps within a 225 radius,
                 //	 the spell temporarily stops slowing and damaging the e, until
                 //	 it is alone again.
                 // • Cannot e spell immune units. Slows and attempts to damage spell immune units.
                 if (
                    Spell.Q != null
                 && Spell.Q.CanBeCasted() && source.CanCast() && !e.IsMagicImmune()
                 //&& ObjectManager.GetEntities<Creep>().Where(x => e.Distance2D(x) >= 225 && x.Team != _me.Team).Count() == 0
                 // UNDONE: Если рядом с целью в радиусе 225 есть юниты, не кастовать 1-ю способность, ибо она не сработает когда цель близко к существам.
                 //&& ObjectManager.GetEntities<Hero>().Where(x => e.Distance2D(x) >= 225 && x.Name != e.Name && x.Team != _me.Team).Count() == 0
                 && Utils.SleepCheck(Spell.Q.Name)
                 && source.Distance2D(e) <= 900
                 )
                 {
                     Spell.Q.UseAbility(e);
                     Utils.Sleep(250, Spell.Q.Name);
                     return true;
                 }
                 return false;
             } // Cast.SpellQ

             public static bool SpellW()
             {
                 if (
                    Spell.W != null
                 && Spell.W.CanBeCasted() && source.CanCast() && !e.IsMagicImmune()
                 && Utils.SleepCheck(Spell.W.Name)
                 && source.Distance2D(e) <= 900
                 )
                 {
                     Spell.W.UseAbility(source.Position);
                     Utils.Sleep(250, Spell.W.Name);
                     return true;
                 }
                 return false;
             } // Cast.SpellW

             public static bool SpellE()
             {
                 if (
                    Spell.E != null
                 && Spell.E.CanBeCasted() && source.CanCast() && !e.IsMagicImmune()
                 && Utils.SleepCheck(Spell.E.Name + source.Handle)
                 && source.Distance2D(e) <= 900
                 )
                 {
                     Spell.E.UseAbility(e.Position);
                     Utils.Sleep(250, Spell.E.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.SpellE

             public static bool SpellR()
             {
                 if (
                    Spell.R != null
                 && Spell.R.CanBeCasted() && source.CanCast()
                 && Utils.SleepCheck(Spell.R.Name + source.Handle)
                 && source.Distance2D(e) < (Item.blink != null && Item.blink.CanBeCasted() ? 1200 : Spell.Q.GetCastRange())
                 )
                 {
                     Spell.R.UseAbility();
                     Utils.Sleep(250, Spell.R.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.SpellR

             //------------------------ Items ------------------------
             public static bool MaskOfMadness(Hero source, Hero e)
             {
                 if (
                 Item.mom != null &&
                 Item.mom.CanBeCasted() &&
                 source.CanCast() &&
                 Utils.SleepCheck(Item.mom.Name + source.Handle) &&
                 source.Distance2D(e) <= 700
                 )
                 {
                     Item.mom.UseAbility();
                     Utils.Sleep(250, Item.mom.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.MaskOfMadness

             public static bool AbyssalBlade(Hero source, Hero e)
             {
                 if (e.IsStunned() || e.IsHexed() || e.IsRooted()) return false;

                 if (
                 Item.abyssal != null &&
                 Item.abyssal.CanBeCasted() &&
                 source.CanCast() &&
                 !e.IsMagicImmune() &&
                 Utils.SleepCheck(Item.abyssal.Name + source.Handle) &&
                 source.Distance2D(e) <= 400
                 )
                 {
                     Item.abyssal.UseAbility(e);
                     Utils.Sleep(250, Item.abyssal.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.AbyssalBlade

             public static bool SoulRing(Hero source, Hero e)
             {
                 if (
                 Item.soul != null &&
                 Item.soul.CanBeCasted() &&
                 source.Health >= source.MaximumHealth * 0.5 &&
                 //source.Mana <= Spell.R.ManaCost &&
                 Utils.SleepCheck(Item.soul.Name + source.Handle))
                 {
                     Item.soul.UseAbility();
                     Utils.Sleep(100, Item.soul.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.SoulRing

             public static bool ArcaneBoots(Hero source, Hero e)
             {
                 if (
                 Item.arcane != null &&
                 // source.Mana <= Spell.R.ManaCost &&
                 Item.arcane.CanBeCasted() &&
                 source.Distance2D(_me) >= Item.arcane.GetCastRange())
                 {
                     Item.arcane.UseAbility();
                     return true;
                 }
                 return false;
             } // Cast.ArcaneBoots

             public static bool BlinkDagger(Hero source, Hero e)
             {
                 float angle = source.FindAngleBetween(e.Position, true);
                 Vector3 predPos = new Vector3((float)(e.Position.X - 280 * Math.Cos(angle)), (float)(e.Position.Y - 280 * Math.Sin(angle)), 0);
                 if (Item.blink != null &&
                     Item.blink.CanBeCasted() &&
                     source.Distance2D(predPos) <= 1200 &&
                     source.Distance2D(e) >= _me.AttackRange - 50 &&
                     Utils.SleepCheck(Item.blink.Name + source.Handle))
                 {
                     Item.blink.UseAbility(predPos);
                     Utils.Sleep(300, Item.blink.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.BlinkDagger

             public static bool ShivasGuard(Hero source, Hero e)
             {
                 if (e.Modifiers.Any(m =>
                 m.Name == "modifier_item_shivas_guard_blast")) return false;

                 if (
                 Item.shiva != null &&
                 Item.shiva.CanBeCasted() &&
                 source.CanCast() &&
                 !e.IsMagicImmune() &&
                 Utils.SleepCheck(Item.shiva.Name + source.Handle) &&
                 source.Distance2D(e) <= 600
                 )
                 {
                     Item.shiva.UseAbility();
                     Utils.Sleep(250, Item.shiva.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.ShivasGuard

             public static bool HeavensHalberd(Hero source, Hero e)
             {
                 if (e.IsDisarmed() || e.IsStunned() || e.IsHexed() || e.IsRooted()) return false;

                 if (
                 Item.halberd != null &&
                 Item.halberd.CanBeCasted() &&
                 source.CanCast() &&
                 !e.IsMagicImmune() &&
                 Utils.SleepCheck(Item.halberd.Name + source.Handle) &&
                 source.Distance2D(e) <= e.AttackRange + 200
                 )
                 {
                     Item.halberd.UseAbility(e);
                     Utils.Sleep(150, Item.halberd.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.HeavensHalberd

             public static bool Mjollnir(Hero source, Hero e)
             {
                 if (
                 Item.mjollnir != null &&
                 Item.mjollnir.CanBeCasted() &&
                 source.CanCast() &&
                 Utils.SleepCheck(Item.mjollnir.Name + source.Handle) &&
                 source.Distance2D(e) <= 900
                )
                 {
                     Item.mjollnir.UseAbility(_me.Modifiers.Any(x => x.Name == "modifier_item_mjollnir") ? source : _me);
                     Utils.Sleep(250, Item.mjollnir.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.Mjollnir

             public static bool Satanic(Hero source, Hero e)
             {
                 if (
                 Item.satanic != null &&
                 source.Health <= (source.MaximumHealth * 0.3) &&
                 Item.satanic.CanBeCasted() &&
                 source.Distance2D(e) <= source.GetAttackRange()
                 )
                 {
                     Item.satanic.UseAbility();
                     return true;
                 }
                 return false;
             } // Cast.Satanic

             public static bool Dagon(Hero source, Hero e)
             {
                 if (
                 Item.dagon != null &&
                 Item.dagon.CanBeCasted() &&
                 source.CanCast() &&
                 !e.IsMagicImmune() &&
                 source.Distance2D(e) <= Item.dagon.GetCastRange()
                 && Utils.SleepCheck(Item.dagon.Name + source.Handle)
                )
                 {
                     Item.dagon.UseAbility(e);
                     Utils.Sleep(150, Item.dagon.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.Dagon

             public static bool MedallionOfCourage(Hero source, Hero e)
             {

                 if (
                 Item.medall != null &&
                 Item.medall.CanBeCasted() &&
                 Utils.SleepCheck(Item.medall.Name) &&
                 source.Distance2D(e) <= 600
                 )
                 {
                     Item.medall.UseAbility(
                         (_me.Distance2D(source) < Item.medall.GetCastRange() ? _me : e));
                     Utils.Sleep(250, Item.medall.Name);
                     return true;
                 }
                 return false;
             } // Cast.MedallionOfCourage

             public static bool RefresherOrb(Hero source, Hero e)
             {
                 if (
                 Item.refresh != null &&
                 Item.refresh.CanBeCasted() &&
                 Utils.SleepCheck(Item.refresh.Name + source.Handle) &&
                 source.Distance2D(e) <= 900
                 )
                 {
                     Item.refresh.UseAbility();
                     Utils.Sleep(250, Item.refresh.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.MedallionOfCourage

             public static bool Necronomicon(Hero source, Hero e)
             {
                 if (
                 Item.necros != null &&
                 Item.necros.CanBeCasted() &&
                 Utils.SleepCheck(Item.necros.Name + source.Handle) &&
                 source.Distance2D(e) <= _me.AttackRange - 150
                 )
                 {
                     Item.necros.UseAbility();
                     Utils.Sleep(250, Item.necros.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.Necronomicon

             public static bool DiffusalBlade(Hero source, Hero e, bool attackMode = true)
             {
                 if (e.Modifiers.Any(m =>
                 m.Name == "modifier_item_diffusal_blade_slow") ||
                 e.IsStunned() || e.IsHexed() || e.IsRooted()) return false;

                 if (
                 Item.diffusal != null &&
                 Item.diffusal.CanBeCasted() &&
                 Utils.SleepCheck(Item.diffusal.Name + source.Handle) &&
                 source.Distance2D(e) <= Item.diffusal.GetCastRange()
                 )
                 {
                     Item.diffusal.UseAbility(attackMode ? e : _me);
                     Utils.Sleep(250, Item.diffusal.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.DiffusalBlade

             public static bool VeilOfDiscord(Hero source, Hero e)
             {
                 if (e.Modifiers.Any(m =>
                 m.Name == "modifier_item_veil_of_discord_debuff")) return false;
                 if (
                 Item.vail != null &&
                 Item.vail.CanBeCasted() &&
                 Utils.SleepCheck(Item.vail.Name) &&
                 source.Distance2D(e) <= Item.vail.GetCastRange())
                 {
                     Item.vail.UseAbility(e.Position);
                     Utils.Sleep(250, Item.vail.Name);
                     return true;
                 }
                 return false;
             } // Cast.VeilOfDiscord

             public static bool ScytheOfVyse(Hero source, Hero e)
             {
                 if (e.IsHexed() || e.IsStunned() || e.IsRooted()) return false;

                 if (
                 Item.sheepstick != null &&
                 Item.sheepstick.CanBeCasted() &&
                 Utils.SleepCheck(Item.sheepstick.Name + source.Handle) &&
                 source.Distance2D(e) <= Item.sheepstick.GetCastRange()
                 )
                 {
                     Item.sheepstick.UseAbility(e);
                     Utils.Sleep(250, Item.sheepstick.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.ScytheOfVyse

             public static bool EtherealBlade(Hero source, Hero e) // UNDONE: Не работает Etherial Blade - профиксить.
             {
                 if (Utils.SleepCheck("qwe"))
                 {
                     Console.WriteLine("Ethereal function!");
                     Utils.Sleep(1000, "qwe");
                 }

                 if (e.Modifiers.Any(m =>
                 m.Name == "modifier_item_ethereal_blade_ethereal")) return false;

                 if (
                 Item.etheral != null &&
                 Item.etheral.CanBeCasted() &&
                 Utils.SleepCheck(Item.etheral.Name + source.Handle) &&
                 source.Distance2D(e) <= Item.etheral.GetCastRange()
                 )
                 {
                     Console.WriteLine("Ethereal casted!");
                     Item.etheral.UseAbility(e);
                     Utils.Sleep(250, Item.etheral.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.EtherealBlade

             public static bool BlackKingBar(Hero source, Hero e)
             {
                 if (
                 Item.bkb != null &&
                 Item.bkb.CanBeCasted() &&
                 Utils.SleepCheck(Item.bkb.Name + source.Handle) &&
                 source.Distance2D(e) <= (Item.blink.CanBeCasted() ? 1150 : 900)
                 )
                 {
                     Item.bkb.UseAbility();
                     Utils.Sleep(250, Item.bkb.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.BlackKingBar

             public static bool OrchidMalevolence(Hero source, Hero e)
             {
                 if (e.Modifiers.Any(m =>
                 m.Name == "modifier_item_orchid_malevolence")) return false;
                 if (
                 Item.orchid != null &&
                 Item.orchid.CanBeCasted() &&
                 Utils.SleepCheck(Item.orchid.Name + source.Handle) &&
                 source.Distance2D(e) <= Item.orchid.GetCastRange()
                 )
                 {
                     Item.etheral.UseAbility(e);
                     Utils.Sleep(150, Item.orchid.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.OrchidMalevolence

             public static bool MantaStyle(Hero source, Hero e)
             {
                 if (
                 Item.manta != null &&
                 Item.manta.CanBeCasted() &&
                 Utils.SleepCheck(Item.manta.Name + source.Handle) &&
                 source.Distance2D(e) <= source.AttackRange - 100
                 )
                 {
                     Item.manta.UseAbility();
                     Utils.Sleep(250, Item.manta.Name + source.Handle);
                     return true;
                 }
                 return false;
             } // Cast.MantaStyle
         } // Program.Cast

         class Necronomicon
         {
             private static List<Unit> necros;
             private static List<Hero> lowHPenemies;
             private static Hero victim;

             public static void Controller(Hero e)
             {
                 necros = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && x.Team == _me.Team &&
                     x.Name.Contains("npc_dota_necronomicon_warrior_") && x.IsControllable).ToList();
                 lowHPenemies = ObjectManager.GetEntities<Hero>().Where(x =>
                 x.Team != _me.Team && x.IsAlive && !x.IsIllusion && x.Health <= x.MaximumHealth * 0.15).ToList();
                 var count = necros.Count();
                 if(count<= 0) return;
                 for (int i = 0, n = count; i < n; ++i)
                 {
                     var manaBurn = necros[i].FindSpell("necronomicon_archer_mana_burn");
                     victim = e;
                     if (lowHPenemies.Count >= 1) victim = GetClosestToTarget(lowHPenemies, necros[i].Position);
                     if (
                         manaBurn != null
                         && Utils.SleepCheck(manaBurn.Name + necros[i].Handle)
                         && manaBurn.CanBeCasted() && necros[i].CanCast() && !e.IsMagicImmune()
                         && necros[i].Distance2D(e) < 900
                     )
                     {
                         Console.WriteLine("ManaBurn has been casted!");
                         manaBurn.UseAbility(e);
                         Utils.Sleep(200, manaBurn.Name + necros[i].Handle);
                     }
                     else Orbwalk(necros[i], e);
                 }
             }
         } // Program.Necronomicon

         //------------------------ Utility Methods -----------------------


             public static void Orbwalk(Unit source, Unit e)
             {
                 if (
                     source.Distance2D(e) <= source.GetAttackRange() + 100 && (!source.IsAttackImmune() || !e.IsAttackImmune())
                     && source.NetworkActivity != NetworkActivity.Attack && source.CanAttack() && Utils.SleepCheck("attack")
                     )
                 {
                     source.Attack(e);
                     Utils.Sleep(170, "attack");
                 }
                 else if ((!source.CanAttack() || source.Distance2D(e) >= 0) && source.NetworkActivity != NetworkActivity.Attack &&
                          source.Distance2D(e) <= 1200 && Utils.SleepCheck("Move")
                     )
                 {
                     source.Move(e.Predict(300));
                     Utils.Sleep(390, "Move");
                 } // endif
             }*/


    }
}

