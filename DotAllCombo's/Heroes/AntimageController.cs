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
    using Service.Debug;

    internal class AntimageController : Variables, IHeroController
    {
        private Ability _w, _r;
        private Item _urn, _dagon, _soul, _phase, _cheese, _halberd, _ethereal,
            _mjollnir, _orchid, _abyssal, _stick, _mom, _shiva, _mail, _bkb, _satanic, _medall, _blink, _sheep, _manta;
        
        private readonly double[] _ult = { 0, 0.6, 0.85, 1.1 };
        public void Combo()
        {
            if (!Menu.Item("enabled").IsActive())
                return;

            E = Toolset.ClosestToMouse(Me);
            if (E == null)
                return;
            _w = Me.Spellbook.SpellW;
            _r = Me.Spellbook.SpellR;
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

            _shiva = Me.FindItem("item_shivas_guard");
            _ethereal = Me.FindItem("item_ethereal_blade");
            _mom = Me.FindItem("item_mask_of_madness");
            _urn = Me.FindItem("item_urn_of_shadows");
			_manta = Me.FindItem("item_manta");
			_dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            _halberd = Me.FindItem("item_heavens_halberd");
            _mjollnir = Me.FindItem("item_mjollnir");
            _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
            _abyssal = Me.FindItem("item_abyssal_blade");
            _mail = Me.FindItem("item_blade_mail");
            _bkb = Me.FindItem("item_black_king_bar");
            _satanic = Me.FindItem("item_satanic");
            _blink = Me.FindItem("item_blink");
            _medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");
            _sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
            _cheese = Me.FindItem("item_cheese");
            _soul = Me.FindItem("item_soul_ring");
            _stick = Me.FindItem("item_magic_stick") ?? Me.FindItem("item_magic_wand");
            _phase = Me.FindItem("item_phase_boots");
            var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();

	        var stoneModif = E.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
				{
					Orbwalking.Orbwalk(E, 0, 1600, true, true);
				}
			}
            if (Active && Me.Distance2D(E) <= 1400 && E != null && E.IsAlive && !Me.IsInvisible())
            {
                if (
                       _w != null 
                       && _w.CanBeCasted() 
                       && Me.Distance2D(E) <= _w.GetCastRange()-100
                       && Me.Distance2D(E) >= Me.AttackRange+200
                       && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                       && Utils.SleepCheck("_w")
                       )
                {
                    _w.UseAbility(Prediction.InFront(E, 230));
                    Utils.Sleep(200, "_w");
                }

				if ((_manta != null 
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name)) 
					&& _manta.CanBeCasted() && Me.IsSilenced() && Utils.SleepCheck("_manta"))
				{
					_manta.UseAbility();
					Utils.Sleep(400, "_manta");
				}
				if ((_manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_manta.Name))
					&& _manta.CanBeCasted() && (E.Position.Distance2D(Me.Position) <= Me.GetAttackRange()+Me.HullRadius)
					&& Utils.SleepCheck("_manta"))
				{
					_manta.UseAbility();
					Utils.Sleep(150, "_manta");
				}
				if ( // MOM
                    _mom != null
                    && _mom.CanBeCasted()
                    && Me.CanCast()
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
                    && Utils.SleepCheck("_mom")
                    && Me.Distance2D(E) <= 700
                    )
                {
                    _mom.UseAbility();
                    Utils.Sleep(250, "_mom");
                }
                if ( // Hellbard
                    _halberd != null
                    && _halberd.CanBeCasted()
                    && Me.CanCast()
                    && !E.IsMagicImmune()
                    && (E.NetworkActivity == NetworkActivity.Attack
                        || E.NetworkActivity == NetworkActivity.Crit
                        || E.NetworkActivity == NetworkActivity.Attack2)
                    && Utils.SleepCheck("_halberd")
                    && Me.Distance2D(E) <= 700
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
                    )
                {
                    _halberd.UseAbility(E);
                    Utils.Sleep(250, "_halberd");
                }
                if ( // Mjollnir
                    _mjollnir != null
                    && _mjollnir.CanBeCasted()
                    && Me.CanCast()
                    && !E.IsMagicImmune()
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
                    && Utils.SleepCheck("_mjollnir")
                    && Me.Distance2D(E) <= 900
                    )
                {
                    _mjollnir.UseAbility(Me);
                    Utils.Sleep(250, "_mjollnir");
                } // Mjollnir Item end
                if (
                    // _cheese
                    _cheese != null
                    && _cheese.CanBeCasted()
                    && Me.Health <= (Me.MaximumHealth * 0.3)
                    && Me.Distance2D(E) <= 700
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_cheese.Name)
                    && Utils.SleepCheck("_cheese")
                    )
                {
                    _cheese.UseAbility();
                    Utils.Sleep(200, "_cheese");
                } // _cheese Item end
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

                if ( // _sheep
                    _sheep != null
                    && _sheep.CanBeCasted()
                    && Me.CanCast()
                    && !E.IsLinkensProtected()
                    && !E.IsMagicImmune()
                    && Me.Distance2D(E) <= 1400
                    && !stoneModif
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
                    && Utils.SleepCheck("_sheep")
                    )
                {
                    _sheep.UseAbility(E);
                    Utils.Sleep(250, "_sheep");
                } // _sheep Item end
                if ( // Abyssal Blade
                    _abyssal != null
                    && _abyssal.CanBeCasted()
                    && Me.CanCast()
                    && !E.IsStunned()
                    && !E.IsHexed()
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_abyssal.Name)
                    && Utils.SleepCheck("_abyssal")
                    && Me.Distance2D(E) <= 400
                    )
                {
                    _abyssal.UseAbility(E);
                    Utils.Sleep(250, "_abyssal");
                } // Abyssal Item end
                if (_orchid != null && _orchid.CanBeCasted() && Me.Distance2D(E) <= 900
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
					&& Utils.SleepCheck("_orchid"))
                {
                    _orchid.UseAbility(E);
                    Utils.Sleep(100, "_orchid");
                }

                if (_shiva != null && _shiva.CanBeCasted() && Me.Distance2D(E) <= 600
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                    && !E.IsMagicImmune() && Utils.SleepCheck("_shiva"))
                {
                    _shiva.UseAbility();
                    Utils.Sleep(100, "_shiva");
                }
                if ( // _ethereal
                    _ethereal != null
                    && _ethereal.CanBeCasted()
                    && Me.CanCast()
                    && !E.IsLinkensProtected()
                    && !E.IsMagicImmune()
                    && !stoneModif
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                    && Utils.SleepCheck("_ethereal")
                    )
                {
                    _ethereal.UseAbility(E);
                    Utils.Sleep(200, "_ethereal");
                } // _ethereal Item end
                if (
                    _blink != null
                    && Me.CanCast()
                    && _blink.CanBeCasted()
                    && Me.Distance2D(E) >= 450
                    && Me.Distance2D(E) <= 1150
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                    && Utils.SleepCheck("_blink")
                    )
                {
                    _blink.UseAbility(E.Position);
                    Utils.Sleep(250, "_blink");
                }

                if ( // SoulRing Item 
                    _soul != null
                    && _soul.CanBeCasted()
                    && Me.CanCast()
                    && Me.Health >= (Me.MaximumHealth * 0.5)
                    && Me.Mana <= _r.ManaCost
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
                    )
                {
                    _soul.UseAbility();
                } // SoulRing Item end
                if ( // Dagon
                    Me.CanCast()
                    && _dagon != null
                    && (_ethereal == null
                        || (E.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
                            || _ethereal.Cooldown < 17))
                    && !E.IsLinkensProtected()
                    && _dagon.CanBeCasted()
                    && !E.IsMagicImmune()
                    && !stoneModif
                    && Utils.SleepCheck("_dagon")
                    )
                {
                    _dagon.UseAbility(E);
                    Utils.Sleep(200, "_dagon");
                } // Dagon Item end
                if (_phase != null
                    && _phase.CanBeCasted()
                    && Utils.SleepCheck("_phase")
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_phase.Name)
                    && !_blink.CanBeCasted()
                    && Me.Distance2D(E) >= Me.AttackRange + 20)
                {
                    _phase.UseAbility();
                    Utils.Sleep(200, "_phase");
                }
                if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 400
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("_urn"))
                {
                    _urn.UseAbility(E);
                    Utils.Sleep(240, "_urn");
                }
                if (
                    _stick != null
                    && _stick.CanBeCasted()
                    && _stick.CurrentCharges != 0
                    && Me.Distance2D(E) <= 700
                    && (Me.Health <= (Me.MaximumHealth * 0.5)
                        || Me.Mana <= (Me.MaximumMana * 0.5))
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_stick.Name))
                {
                    _stick.UseAbility();
                    Utils.Sleep(200, "mana_items");
                }
                if ( // Satanic 
                    _satanic != null 
					&& Me.Health <= (Me.MaximumHealth * 0.3) 
					&& _satanic.CanBeCasted() 
					&& Me.Distance2D(E) <= Me.AttackRange + 50
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
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
            }
            if (Menu.Item("autoUlt").GetValue<bool>() && Me.IsAlive)
            {
                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };

                _r = Me.Spellbook.SpellR;
                var ultLvl = _r.Level;
                var enemy =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                        .ToList();
                if (enemy.Count == 0) return;
                foreach (var z in enemy)
                {
                    if (!z.IsVisible || !z.IsAlive) continue;
                    var manna = (z.MaximumMana - z.Mana);
                    var damage = Math.Floor((manna*_ult[ultLvl])*(1 - z.MagicDamageResist));

                    var lens = Me.HasModifier("modifier_item_aether_lens");

                    if (z.NetworkName == "CDOTA_Unit_Hero_Spectre" && z.Spellbook.Spell3.Level > 0)
                    {
                        damage =
                            Math.Floor((manna*_ult[ultLvl])*
                                       (1 - (0.10 + z.Spellbook.Spell3.Level*0.04))*(1 - z.MagicDamageResist));
                    }
                    if (z.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                        z.Spellbook.SpellR.CanBeCasted())
                        damage = 0;
                    if (lens) damage = damage*1.08;
                    if (z.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage*0.5;
                    if (z.HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage*1.3;

                    if (z.HasModifier("modifier_chen_penitence"))
                        damage = damage*
                                 penitence[
                                     ObjectManager.GetEntities<Hero>()
                                         .FirstOrDefault(
                                             x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen)
                                         .Spellbook.Spell1.Level];

                    if (z.HasModifier("modifier_shadow_demon_soul_catcher"))
                        damage = damage*
                                 souls[
                                     ObjectManager.GetEntities<Hero>()
                                         .FirstOrDefault(
                                             x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon)
                                         .Spellbook.Spell2.Level];
                    
                    if (_r != null && _r.CanBeCasted()
                        && !z.HasModifier("modifier_tusk_snowball_movement")
                        && !z.HasModifier("modifier_snowball_movement_friendly")
                        && !z.HasModifier("modifier_templar_assassin_refraction_absorb")
                        && !z.HasModifier("modifier_ember_spirit_flame_guard")
                        && !z.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
                        && !z.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                        && !z.HasModifier("modifier_puck_phase_shift")
                        && !z.HasModifier("modifier_eul_cyclone")
                        && !z.HasModifier("modifier_dazzle_shallow_grave")
                        && !z.HasModifier("modifier_shadow_demon_disruption")
                        && !z.HasModifier("modifier_necrolyte_reapers_scythe")
                        && !z.HasModifier("modifier_medusa_stone_gaze_stone")
                        && !z.HasModifier("modifier_storm_spirit_ball_lightning")
                        && !z.HasModifier("modifier_ember_spirit_fire_remnant")
                        && !z.HasModifier("modifier_nyx_assassin_spiked_carapace")
                        && !z.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
                        && !z.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
                        !z.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
                        && Me.Distance2D(z) <= _r.GetCastRange() + 50
                        && !z.IsMagicImmune()
                        && enemy.Count(x => (x.Health - damage) <= 0 && x.Distance2D(z) <= 500)
                        >= Menu.Item("ulti").GetValue<Slider>().Value
                        && enemy.Count(x => x.Distance2D(z) <= 500)
                        >= Menu.Item("ulti").GetValue<Slider>().Value
                        && damage >= Menu.Item("minDMG").GetValue<Slider>().Value
                        && Utils.SleepCheck(z.Handle.ToString()))
                    {
                        _r.UseAbility(z);
                        Utils.Sleep(150, z.Handle.ToString());
                        return;
                    }
                }
            }
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("Blood!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            var ul = new Menu("AutoUlt", "_ult");
            //Menu.AddItem(new MenuItem("hpdraw", "Draw HP Bar").SetValue(true).SetTooltip("Will show ulti damage on HP Bar"));
            Menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"antimage_blink", true},
                    {"antimage_mana_void", true}
                })));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_ethereal_blade", true},
                    {"item_blink", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true}, {"item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
                    {"item_abyssal_blade", true},
                    {"item_shivas_guard", true},
                    {"item_blade_mail", true},
                    {"item_black_king_bar", true},
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true}
                })));
            Menu.AddItem(
               new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
               {
                   {"item_mask_of_madness", true},
                   {"item_sheepstick", true},
                   {"item_cheese", true},
                   {"item_soul_ring", true},
                   {"item_arcane_boots", true},
                   {"item_magic_stick", true},
                   {"item_magic_wand", true},
                   {"item_mjollnir", true},
                   {"item_satanic", true},
                   {"item_phase_boots", true},
				   {"item_manta", true}
			   })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(ul);
			ul.AddItem(new MenuItem("minDMG", "Min Damage to Ult").SetValue(new Slider(200, 100, 1000)));
			ul.AddItem(new MenuItem("autoUlt", "EnableAutoUlt").SetValue(true));
            ul.AddItem(new MenuItem("ulti", "Min targets to _ult").SetValue(new Slider(2, 1, 5)));

            Drawing.OnDraw += DrawUltiDamage;
        }
        private void DrawUltiDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
            {
                return;
            }

            double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };

            _r = Me.Spellbook.SpellR;
            var ultLvl = _r.Level;
            var enemy =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                    .ToList();
            if (enemy.Count == 0) return;
            foreach (var v in enemy)
            {
                if (!v.IsVisible || !v.IsAlive) continue;
                var manna = (v.MaximumMana - v.Mana);
                var damage = Math.Floor((manna * _ult[ultLvl]) * (1 - v.MagicDamageResist));

                var lens = Me.HasModifier("modifier_item_aether_lens");

                if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                {
                    damage =
                        Math.Floor((manna * _ult[ultLvl]) *
                                   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
                }
                if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                    v.Spellbook.SpellR.CanBeCasted())
                    damage = 0;
                if (lens) damage = damage * 1.08;
                if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage * 0.5;
                if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage * 1.3;

                if (v.HasModifier("modifier_chen_penitence"))
                {
                    var first = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                    if (first != null) damage = damage * penitence[first.Spellbook.Spell1.Level];
                }

                if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                {
                    var first = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                    if (first != null) damage = damage * soul[first.Spellbook.Spell2.Level];
                }

                var dmg = v.Health - damage;
                var canKill = dmg <= 0;
                var screenPos = HUDInfo.GetHPbarPosition(v);
                if (!OnScreen(v.Position)) continue;

                var text = canKill ? "Yes:" + Math.Floor(damage) : "No:" + Math.Floor(damage);
                var size = new Vector2(15, 15);
                var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                var position = new Vector2(screenPos.X - textSize.X - 2, screenPos.Y - 3);
                Drawing.DrawText(
                    text,
                    position,
                    size,
                    (canKill ? Color.LawnGreen : Color.Red),
                    FontFlags.AntiAlias);
            }
        }
        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }
        
        public void OnCloseEvent()
        {
            Drawing.OnDraw -= DrawUltiDamage;
        }
    }
}