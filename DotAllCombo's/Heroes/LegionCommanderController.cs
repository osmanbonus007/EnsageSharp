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

    internal class LegionCommanderController : Variables, IHeroController
    {
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _skills = new Menu("Skills", "Skills");
        private Ability _r, _w, _q;
		private double _damage;

		private Font _txt;
		private Font _noti;
		private Line _lines;
		private Item _blink, _armlet, _blademail, _bkb, _abyssal, _mjollnir, _halberd, _medallion, _madness, _urn, 
            _satanic, _solar, _dust, _sentry, _mango, _arcane, _buckler, _crimson, _lotusorb, _cheese, _stick, 
            _soul, _force, _cyclone, _sheep, _orchid, _atos;
		private Unit GetLowestToQ(List<Hero> units, Unit z)
		{

			_q = Me.Spellbook.SpellQ;
			int[] qDmg = { 40, 80, 120, 160 };
			int[] creepsDmg = { 14, 16, 18, 20 };
			int[] enemyDmg = { 20, 40, 60, 80 };
		    double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };
            foreach (var v in units.Where(x => !x.IsMagicImmune()))
			{
				var creepsECount = ObjectManager.GetEntities<Unit>().Where(creep =>
				    (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
				     || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
				     || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
				     || creep.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
				     || creep.ClassId == ClassId.CDOTA_Unit_SpiritBear
				     || creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit
				     || creep.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem
				     || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
				    creep.IsAlive && creep.Team != Me.Team && creep.IsVisible && v.Distance2D(creep) <= 330 &&
				    creep.IsSpawned).ToList().Count();
				var enemiesCount = ObjectManager.GetEntities<Hero>().Where(x =>
				    x.Team != Me.Team && x.IsAlive && x.IsVisible && v.Distance2D(x) <= 330).ToList().Count();
				if (enemiesCount == 0)
				{
					enemiesCount = 0;
				}
				if (creepsECount == 0)	
				{
					creepsECount = 0;
				}
				_damage = ((creepsECount * creepsDmg[_q.Level - 1] + enemiesCount * enemyDmg[_q.Level - 1]) +
							 qDmg[_q.Level - 1]) * (1 - v.MagicDamageResist);
				
				if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
				{
					_damage =
						Math.Floor((((creepsECount * creepsDmg[_q.Level - 1] + enemiesCount * enemyDmg[_q.Level - 1]) +
							 qDmg[_q.Level - 1]) *
								   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04))) * (1 - v.MagicDamageResist));
				}
				
				if (v.NetworkName == "CDOTA_Unit_Hero_Tusk" &&
					v.Spellbook.SpellW.CooldownLength - 3 > v.Spellbook.SpellQ.Cooldown)
					_damage = 0;

				var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
				if (rum) _damage = _damage * 0.5;
				var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
				if (mom) _damage = _damage * 1.3;

				var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);

                if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                {
                    var blood =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                    if (blood != null)
                        _damage = _damage * bloodrage[blood.Spellbook.Spell1.Level];
                    else
                        _damage = _damage * 1.4;
                }


                if (v.HasModifier("modifier_chen_penitence"))
                {
                    var chen =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                    if (chen != null)
                        _damage = _damage * penitence[chen.Spellbook.Spell1.Level];
                }


                if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                {
                    var demon =
                        ObjectManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                    if (demon != null)
                        _damage = _damage * soul[demon.Spellbook.Spell2.Level];
                }
                _damage = _damage * spellamplymult;
				//Console.WriteLine(damage);

				if (_damage >= v.Health && z.Distance2D(v) <= _q.GetCastRange())
					return v;
			}
			return null;
		}
		public void Combo()
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
				return;
			_q = Me.Spellbook.SpellQ;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			if (Menu.Item("steal").IsActive() && !Me.HasModifier("modifier_legion_commander_duel") && _q != null && _q.CanBeCasted())
			{
				if (!Me.IsAlive) return;
				var v =
				   ObjectManager.GetEntities<Hero>()
					   .Where(x => x.Team != Me.Team && x.IsAlive && x.Distance2D(Me) <= _q.GetCastRange() && !x.IsIllusion)
					   .ToList();
				var dmg = GetLowestToQ(v, Me);
				
				if (dmg != null && Utils.SleepCheck("Q") && (Menu.Item("-dmg").IsActive() && !_r.CanBeCasted() || !Menu.Item("-dmg").IsActive()))
				{
					_q.UseAbility(dmg.Position);
					Utils.Sleep(150, "Q");
				}
			}
			if (Active && !Game.IsChatOpen)
            {
                _r = Me.Spellbook.SpellR;
                _w = Me.Spellbook.SpellW;
                _blink = Me.FindItem("item_blink");
                _armlet = Me.FindItem("item_armlet");
                _blademail = Me.FindItem("item_blade_mail");
                _bkb = Me.FindItem("item_black_king_bar");
                _abyssal = Me.FindItem("item_abyssal_blade");
                _mjollnir = Me.FindItem("item_mjollnir");
                _halberd = Me.FindItem("item_heavens_halberd");
                _medallion = Me.FindItem("item_medallion_of_courage");
                _madness = Me.FindItem("item_mask_of_madness");
                _urn = Me.FindItem("item_urn_of_shadows");
                _satanic = Me.FindItem("item_satanic");
                _solar = Me.FindItem("item_solar_crest");
				_orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
				_dust = Me.FindItem("item_dust");
                _sentry = Me.FindItem("item_ward_sentry");
                _mango = Me.FindItem("item_enchanted_mango");
                _arcane = Me.FindItem("item_arcane_boots");
                _buckler = Me.FindItem("item_buckler");
                _crimson = Me.FindItem("item_crimson_guard");
                _lotusorb = Me.FindItem("item_lotus_orb");
                _cheese = Me.FindItem("item_cheese");
                _stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
                _soul = Me.FindItem("item_soul_ring");
                _force = Me.FindItem("item_force_staff");
                _cyclone = Me.FindItem("item_cyclone");
                _sheep = Me.FindItem("item_sheepstick");
                _atos = Me.FindItem("item_rod_of_atos");
                E = Toolset.ClosestToMouse(Me);

                float angle = Me.FindAngleBetween(E.Position, true);
                Vector3 pos = new Vector3((float)(E.Position.X - 55 * Math.Cos(angle)), (float)(E.Position.Y - 55 * Math.Sin(angle)), 0);
                if (E != null && E.IsAlive  && !E.IsInvul() &&
					(_blink != null ? Me.Distance2D(pos) <= 1180 : Me.Distance2D(E) <= 600))
				{
					if (Me.CanAttack() && Me.CanCast())
					{
						if (CanInvisCrit(Me))
							Me.Attack(E);
						else
						{
						    uint manacost = 0;
						    if (Me.IsAlive)
						    {
						        if (_blademail != null && _blademail.Cooldown <= 0 &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_blademail.Name))
						            manacost += _blademail.ManaCost;
						        if (_abyssal != null && _abyssal.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name))
						            manacost += _abyssal.ManaCost;
						        if (_mjollnir != null && _mjollnir.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name))
						            manacost += _mjollnir.ManaCost;
						        if (_halberd != null && _halberd.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_halberd.Name))
						            manacost += _halberd.ManaCost;
						        if (_madness != null && _madness.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_madness.Name))
						            manacost += _madness.ManaCost;
						        if (_lotusorb != null && _lotusorb.Cooldown <= 0 &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_lotusorb.Name))
						            manacost += _lotusorb.ManaCost;
						        if (_buckler != null && _buckler.Cooldown <= 0 &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_buckler.Name))
						            manacost += _buckler.ManaCost;
						        if (_crimson != null && _crimson.Cooldown <= 0 &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_crimson.Name))
						            manacost += _crimson.ManaCost;
						        if (_force != null && _force.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_force.Name))
						            manacost += _force.ManaCost;
						        if (_cyclone != null && _cyclone.CanBeCasted() &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_cyclone.Name))
						            manacost += _cyclone.ManaCost;
						        if (_sheep != null && _sheep.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
						            manacost += _sheep.ManaCost;
						        if (_w.Cooldown <= 0 && _w.Level > 0 &&
						            Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
						            manacost += _w.ManaCost;
						        if (_r.Cooldown <= 0 && _r.Level > 0)
						            manacost += _w.ManaCost;
						    }
						    if (manacost > Me.Mana)
						    {
						        if (_mango.CanBeCasted() && _mango != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_mango.Name) &&
						            Utils.SleepCheck("Mango"))
						        {
						            _mango.UseAbility();
						            Utils.Sleep(Game.Ping, "Mango");
						        }
						        if (_arcane.CanBeCasted() && _arcane != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_arcane.Name) &&
						            Utils.SleepCheck("Arcane"))
						        {
						            _arcane.UseAbility();
						            Utils.Sleep(Game.Ping, "Arcane");
						        }
						        if (_stick.CanBeCasted() && _stick != null &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_stick.Name) &&
						            Utils.SleepCheck("stick"))
						        {
						            _stick.UseAbility();
						            Utils.Sleep(Game.Ping, "stick");
						        }
						        if ((_cheese.CanBeCasted() && _cheese != null &&
						             Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_cheese.Name) &&
						             Me.Health <= Me.MaximumHealth * 0.5) ||
						            Me.Health <= Me.MaximumHealth * 0.30 && Utils.SleepCheck("Cheese"))
						        {
						            _cheese.UseAbility();
						            Utils.Sleep(Game.Ping, "Cheese");
						        }
						        if (_soul.CanBeCasted() && _soul != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_soul.Name) &&
						            Utils.SleepCheck("soul"))
						        {
						            _soul.UseAbility();
						            Utils.Sleep(Game.Ping, "soul");
						        }
						    }
						    if (E.IsLinkensProtected())
							{
								if ((_cyclone.CanBeCasted() || _force.CanBeCasted() || _halberd.CanBeCasted() ||
									 _sheep.CanBeCasted() || _abyssal.CanBeCasted() || _atos.CanBeCasted() ) && Utils.SleepCheck("Combo2"))
								{
									if (_blademail != null && _blademail.Cooldown <= 0 &&
										Menu.Item("Item")
											.GetValue<AbilityToggler>()
											.IsEnabled(_blademail.Name) && Me.Mana - _blademail.ManaCost >= 75)
										_blademail.UseAbility();
									if (_satanic != null && _satanic.Cooldown <= 0 && Me.Health <= Me.MaximumHealth * 0.5 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_satanic.Name))
										_satanic.UseAbility();
									if (_crimson != null && _crimson.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_crimson.Name))
										_crimson.UseAbility();
									if (_buckler != null && _buckler.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_buckler.Name) &&
										Me.Mana - _buckler.ManaCost >= 75)
										_buckler.UseAbility();
									if (_lotusorb != null && _lotusorb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_lotusorb.Name) &&
										Me.Mana - _lotusorb.ManaCost >= 75)
										_lotusorb.UseAbility(Me);
									if (_mjollnir != null && _mjollnir.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name) &&
										Me.Mana - _mjollnir.ManaCost >= 75)
										_mjollnir.UseAbility(Me);
									if (_armlet != null && !_armlet.IsToggled &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_armlet.Name) &&
										Utils.SleepCheck("armlet"))
									{
										_armlet.ToggleAbility();
										Utils.Sleep(300, "armlet");
									}
									if (_madness != null && _madness.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_madness.Name) &&
										Me.Mana - _madness.ManaCost >= 75)
										_madness.UseAbility();
									if (_w != null && _w.Level > 0 && _w.Cooldown <= 0 &&
										Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name) &&
										!Me.IsMagicImmune() && Me.Mana - _w.ManaCost >= 75)
										_w.UseAbility(Me);
									if (_bkb != null && _bkb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_bkb.Name) && Menu.Item("BKB").GetValue<KeyBind>().Active 
										&& !Game.IsChatOpen && (!_w.CanBeCasted() || _w == null))
										_bkb.UseAbility();
									if (_blink != null && _blink.Cooldown <= 0 && Me.Distance2D(pos) <= 1180 &&
										Me.Distance2D(E) >= 200 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
										_blink.UseAbility(pos);
									if (_urn != null && _urn.CurrentCharges > 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name))
										_urn.UseAbility(E);
									if (_solar != null && _solar.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_solar.Name))
										_solar.UseAbility(E);
									if (_medallion != null && _medallion.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medallion.Name))
										_medallion.UseAbility(E);
                                    if (_atos != null && _atos.CanBeCasted() &&
                                              Utils.SleepCheck("atos") &&
                                              Menu.Item("Items")
                                                  .GetValue<AbilityToggler>()
                                                  .IsEnabled(_atos.Name) && Me.Mana - _atos.ManaCost >= 75)
                                    {
                                        _atos.UseAbility(E);
                                        Utils.Sleep(100, "atos");
                                    }
                                    else if (_cyclone != null && _cyclone.CanBeCasted() &&
										Utils.SleepCheck("CycloneRemoveLinkens") &&
										Menu.Item("Link")
											.GetValue<AbilityToggler>()
											.IsEnabled(_cyclone.Name) && Me.Mana - _cyclone.ManaCost >= 75)
									{
										_cyclone.UseAbility(E);
										Utils.Sleep(100, "CycloneRemoveLinkens");
									}
									else if (_force != null && _force.CanBeCasted() &&
											 Utils.SleepCheck("ForceRemoveLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(_force.Name) && Me.Mana - _force.ManaCost >= 75)
									{
										_force.UseAbility(E);
										Utils.Sleep(100, "ForceRemoveLinkens");
									}
									else if (_halberd != null && _halberd.CanBeCasted() &&
											 Utils.SleepCheck("halberdLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(_halberd.Name) && Me.Mana - _halberd.ManaCost >= 75)
									{
										_halberd.UseAbility(E);
										Utils.Sleep(100, "halberdLinkens");
									}
									else if (_sheep != null && _sheep.CanBeCasted() &&
											 Utils.SleepCheck("sheepLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(_sheep.Name) && Me.Mana - _sheep.ManaCost >= 75)
									{
										_sheep.UseAbility(E);
										Utils.Sleep(100, "sheepLinkens");
									}
									else if (_abyssal != null && _abyssal.CanBeCasted() &&
											 Utils.SleepCheck("abyssal") &&
											 Menu.Item("Items")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(_abyssal.Name) && Me.Mana - _abyssal.ManaCost >= 75)
									{
										_abyssal.UseAbility(E);
										Utils.Sleep(100, "abyssal");
									}
									Utils.Sleep(200, "Combo2");
								}
							}
							else
							{
								if (UsedInvis(E))
								{
									if (Me.Distance2D(E) <= 500)
									{
										if (_dust != null && _dust.CanBeCasted() && Utils.SleepCheck("dust") &&
											_dust != null &&
											Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_dust.Name))
										{
											_dust.UseAbility();
											Utils.Sleep(100, "dust");
										}
										else if (_sentry != null && _sentry.CanBeCasted() && Utils.SleepCheck("sentry") &&
												 _sentry != null &&
												 Menu.Item("Items3")
													 .GetValue<AbilityToggler>()
													 .IsEnabled(_sentry.Name))
										{
											_sentry.UseAbility(Me.Position);
											Utils.Sleep(100, "sentry");
										}
									}
								}
								uint elsecount = 1;
								if (Utils.SleepCheck("combo"))
								{
									if (_blademail != null && _blademail.Cooldown <= 0 &&
										Menu.Item("Item")
											.GetValue<AbilityToggler>()
											.IsEnabled(_blademail.Name) && Me.Mana - _blademail.ManaCost >= 75)
										_blademail.UseAbility();
									else
										elsecount += 1;
									if (_satanic != null && _satanic.Cooldown <= 0 && Me.Health <= Me.MaximumHealth * 0.5 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_satanic.Name))
										_satanic.UseAbility();
									else
										elsecount += 1;
									if (_crimson != null && _crimson.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_crimson.Name))
										_crimson.UseAbility();
									else
										elsecount += 1;
									if (_buckler != null && _buckler.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(_buckler.Name) &&
										Me.Mana - _buckler.ManaCost >= 75)
										_buckler.UseAbility();
									else
										elsecount += 1;
									if (_lotusorb != null && _lotusorb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_lotusorb.Name) &&
										Me.Mana - _lotusorb.ManaCost >= 75)
										_lotusorb.UseAbility(Me);
									else
										elsecount += 1;
									if (_mjollnir != null && _mjollnir.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name) &&
										Me.Mana - _mjollnir.ManaCost >= 75)
										_mjollnir.UseAbility(Me);
									else
										elsecount += 1;
									if (_armlet != null && !_armlet.IsToggled &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_armlet.Name) &&
										Utils.SleepCheck("armlet"))
									{
										_armlet.ToggleAbility();
										Utils.Sleep(300, "armlet");
									}
									else
										elsecount += 1;
									if (_madness != null && _madness.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_madness.Name) &&
										Me.Mana - _madness.ManaCost >= 75)
										_madness.UseAbility();
									else
										elsecount += 1;
									if (_w != null && _w.Level > 0 && _w.Cooldown <= 0 &&
										Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name) &&
										!Me.IsMagicImmune() && Me.Mana - _w.ManaCost >= 75)
										_w.UseAbility(Me);
									else
										elsecount += 1;
									if (_bkb != null && _bkb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_bkb.Name)
										&& Menu.Item("BKB").GetValue<KeyBind>().Active 
										&& (!_w.CanBeCasted() || _w == null))
										_bkb.UseAbility();
									else
										elsecount += 1;
									if (_blink != null && _blink.Cooldown <= 0 && Me.Distance2D(pos) <= 1180 &&
										Me.Distance2D(E) >= 200 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name))
										_blink.UseAbility(pos);
									else
										elsecount += 1;
									if (_abyssal != null && _abyssal.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name) &&
										Me.Mana - _abyssal.ManaCost >= 75)
										_abyssal.UseAbility(E);
									else
										elsecount += 1;
									if (_urn != null && _urn.CurrentCharges > 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name))
										_urn.UseAbility(E);
									else
										elsecount += 1;
									if (_solar != null && _solar.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_solar.Name))
										_solar.UseAbility(E);
									else
										elsecount += 1;
									if (_medallion != null && _medallion.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medallion.Name))
										_medallion.UseAbility(E);
									else
										elsecount += 1;
									if (_orchid != null && _orchid.CanBeCasted() &&
										 Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_orchid.Name))
										_orchid.UseAbility(E);
									else
										elsecount += 1;
									if (_r != null && _r.Cooldown <= 0 && !E.IsLinkensProtected() &&
										!E.HasModifier("modifier_abaddon_borrowed_time") &&
										Utils.SleepCheck("R") && elsecount == 17)
									{
										_r.UseAbility(E);
										Utils.Sleep(100, "R");
									}
									else if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
									{
										Orbwalking.Orbwalk(E, 0, 1600, true, true);
									}
									Utils.Sleep(150, "combo");
								}
							}
						}
					}
				}
				if (Me.Distance2D(E) <= 1470 && Me.Distance2D(E) >= 350 && Utils.SleepCheck("Move"))
				{
					Me.Move(E.Position);
					Utils.Sleep(150, "Move");
				}
			}
        }
		private void DrawUltiDamage(EventArgs args)
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
			{
				return;
			}
			if (Menu.Item("dmg").IsActive())
			{
				_q = Me.Spellbook.SpellQ;
				int[] qDmg = {40, 80, 120, 160};
				int[] creepsDmg = {14, 16, 18, 20};
				int[] enemyDmg = {20, 40, 60, 80};
				int enemiesCount;
				int creepsECount;
                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };
                var units =
					ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team != Me.Team && x.IsAlive && !x.IsIllusion)
						.ToList();
				foreach (var v in units.Where(x => !x.IsMagicImmune()))
				{
					creepsECount = ObjectManager.GetEntities<Unit>().Where(creep =>
						(creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
						 || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
						 || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
						 || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
						creep.IsAlive && creep.Team != Me.Team && creep.IsVisible && v.Distance2D(creep) <= 330 &&
						creep.IsSpawned).ToList().Count();
					enemiesCount = ObjectManager.GetEntities<Hero>().Where(x =>
						x.Team != Me.Team && x.IsAlive && x.IsVisible && v.Distance2D(x) <= 330 && !x.IsIllusion).ToList().Count();
					if (enemiesCount == 0)
						enemiesCount = 0;
					if (creepsECount == 0)
						creepsECount = 0;
					
					var screenPos = HUDInfo.GetHPbarPosition(v);
					if (!OnScreen(v.Position)) continue;

                    _damage = ((creepsECount * creepsDmg[_q.Level - 1] + enemiesCount * enemyDmg[_q.Level - 1]) +
                              qDmg[_q.Level - 1]) * (1 - v.MagicDamageResist);

                    if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                    {
                        _damage =
                            Math.Floor((((creepsECount * creepsDmg[_q.Level - 1] + enemiesCount * enemyDmg[_q.Level - 1]) +
                                         qDmg[_q.Level - 1]) *
                                        (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04))) * (1 - v.MagicDamageResist));
                    }
                    if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                        v.Spellbook.SpellR.CanBeCasted())
                        _damage = 0;
                    var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
                    if (rum) _damage = _damage * 0.5;
                    var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
                    if (mom) _damage = _damage * 1.3;
                    var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            _damage = _damage * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            _damage = _damage * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            _damage = _damage * penitence[chen.Spellbook.Spell1.Level];
                    }


                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            _damage = _damage * soul[demon.Spellbook.Spell2.Level];
                    }
                    _damage = _damage * spellamplymult;
                    
                    var canKill = v.Health <= _damage;
                    var text = canKill ? "Yes: " + Math.Floor(_damage) : "No: " + Math.Floor(_damage);
					var size = new Vector2(18, 18);
					var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
					var position = new Vector2(screenPos.X - textSize.X + 91, screenPos.Y + 62);
					Drawing.DrawText(
						text,
						position,
						size,
						(canKill ? Color.LawnGreen : Color.Red),
						FontFlags.AntiAlias);
					Drawing.DrawText(
						text,
						new Vector2(screenPos.X - textSize.X + 92, screenPos.Y + 63),
						size,
						(Color.Black),
						FontFlags.AntiAlias);
				}
			}

		}
		private bool OnScreen(Vector3 v)
		{
			return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
		}
		
       
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");
			
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "ComboKey").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("BKB", "Black King Bar").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddSubMenu(_items);
            Menu.AddSubMenu(_skills);
            _items.AddItem(
				new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_armlet", true},
				    {"item_abyssal_blade", true},
				    {"item_mjollnir", true},
				    {"item_medallion_of_courage", true},
				    {"item_mask_of_madness", true},
				    {"item_urn_of_shadows", true},
				    {"item_solar_crest", true}
				})));
            _items.AddItem(
				new MenuItem("Item", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{

					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_black_king_bar", true},
				    {"item_blade_mail", true},
				    {"item_satanic", true},
				    {"item_lotus_orb", true},
				    {"item_magic_stick", true},
				    {"item_magic_wand", true}
				})));
			_items.AddItem(
				new MenuItem("Items3", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_dust", true},
					{"item_ward_sentry", true},
					{"item_enchanted_mango", true},
					{"item_arcane_boots", true},
					{"item_buckler", true},
					{"item_crimson_guard", true},
					{"item_cheese", true},
					{"item_soul_ring", true}
				})));
			_items.AddItem(
				new MenuItem("Link", "Auto triggre Linken").SetValue(
					new AbilityToggler(new Dictionary<string, bool>
					{
					    {"item_heavens_halberd", true},
					    {"item_force_staff", true},
                        {"item_rod_of_atos", true},
                        {"item_cyclone", true},
					    {"item_sheepstick", true}
					})));
            _skills.AddItem(new MenuItem("steal", "KillSteal Q").SetValue(true));
			_skills.AddItem(new MenuItem("-dmg", "Dont Use KillSteal if i have Duel").SetValue(true));
			_skills.AddItem(new MenuItem("dmg", "Show Damage Q Spell").SetValue(true));
			_skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"legion_commander_press_the_attack", true},
			})));

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
			Drawing.OnDraw += DrawUltiDamage;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
			Drawing.OnDraw-= DrawUltiDamage;
		}

		private bool UsedInvis(Hero v)
		{
			if (v.Modifiers.Any(
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
					 x.Name == "modifier_invoker_ghost_walk_enemy")))
				return true;
			return false;
		}

		private bool CanInvisCrit(Hero x)
		{
			if (
				x.Modifiers.Any(
					m =>
						m.Name == "modifier_item_invisibility_edge_windwalk" ||
						m.Name == "modifier_item_silver_edge_windwalk"))
				return true;
			return false;
		}


        private void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;
			_bkb = Me.FindItem("item_black_king_bar");
			if (_bkb != null)
			{
				if (!Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_bkb.Name)
					|| !Menu.Item("BKB").GetValue<KeyBind>().Active)
				{
					DrawBox(2, 490, 90, 20, 1, new ColorBGRA(0, 0, 90, 70));
					DrawFilledBox(2, 490, 90, 20, new ColorBGRA(0, 0, 0, 90));
					DrawShadowText(" BKB Disable", 4, 490, Color.Gold, _txt);
				}
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