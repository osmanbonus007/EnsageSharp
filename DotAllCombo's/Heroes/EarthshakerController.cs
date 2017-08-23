using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    //TODO Only Love MazaiPC
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using SharpDX;


    using Service.Debug;

    internal class EarthshakerController : Variables, IHeroController
    {
        private Ability _q, _w, _e, _r;
        private readonly Menu _skills = new Menu("Skills", "Skills");
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _ult = new Menu("AutoUsage", "AutoUsage");

        private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost;

        private readonly int[] _eDmg = new[] { 0, 50, 75, 100, 125 };
        private readonly int[] _rDmg = new[] { 0, 160, 210, 270 };
        private readonly int[] _wDmg = new[] { 0, 100, 200, 300, 400 };
        private readonly int[] _qDmg = new[] { 0, 110, 160, 210, 260 };
        private readonly int[] _dagonDmg = new[] { 0, 400, 500, 600, 700, 800 };
        private readonly int[] _creepsDmg = { 0, 40, 55, 70 };

        private readonly double[] _penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        private readonly double[] _bloodrage = { 0, 1.25, 1.3, 1.35, 1.4 };
        private readonly double[] _souls = { 0, 1.2, 1.3, 1.4, 1.5 };


        private Dictionary<uint, double> _damage = new Dictionary<uint, double>();

        private List<Hero> _enemies = new List<Hero>();

        public void Combo()
        {
            //spell
            _q = Me.Spellbook.SpellQ;
            _w = Me.FindSpell("earthshaker_enchant_totem");
            _e = Me.Spellbook.SpellE;
            _r = Me.Spellbook.SpellR;

            // Item
            _ethereal = Me.FindItem("item_ethereal_blade");
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

            if (Active && Me.IsAlive && Utils.SleepCheck("activated"))
            {
                E = Toolset.ClosestToMouse(Me);
                if (E == null) return; var modifEther = E.HasModifier("modifier_item_ethereal_blade_slow");
                var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
                var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
                _sheep = E.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter ? null : Me.FindItem("item_sheepstick");
                if (E.IsAlive && E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade)
                {
                    if (Me.HasModifier("modifier_earthshaker_enchant_totem") && !Me.IsAttacking() && Me.Distance2D(E) <= 300 && Utils.SleepCheck("WMod"))
                    {
                        Me.Attack(E);
                        Utils.Sleep(250, "WMod");
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
                        Utils.Sleep(250, "atos");
                    } // atos Item end

                    if (
                        _blink != null
                        && Me.CanCast()
                        && _blink.CanBeCasted()
                        && Me.Distance2D(E) > 400
                        && Me.Distance2D(E) <= 1180
                        && !stoneModif
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        _blink.UseAbility(E.Position);
                        Utils.Sleep(250, "blink");
                    }
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
                                    _w != null
                                    && _w.CanBeCasted()
                                    && Me.CanCast()
                                    && !Me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Me.Distance2D(E) < 2300
                                    && Me.Distance2D(E) >= 1200
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility();
                                    Utils.Sleep(200, "W");
                                }
                                if (
                                    _w != null
                                    && _w.CanBeCasted()
                                    && Me.CanCast()
                                    && !Me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Me.Distance2D(E) < _w.GetCastRange()
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility();
                                    Utils.Sleep(200, "W");
                                }
                                if (Me.AghanimState())
                                {
                                    if (
                                    _w != null
                                    && _w.CanBeCasted()
                                    && Me.CanCast()
                                    && !Me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Me.Distance2D(E) >= 300
                                    && Me.Distance2D(E) < 900 + Me.HullRadius
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && Utils.SleepCheck("W"))
                                    {
                                        _w.UseAbility(E.Position);
                                        Utils.Sleep(200, "W");
                                    }
                                    if (
                                    _w != null
                                    && _w.CanBeCasted()
                                    && Me.CanCast()
                                    && !Me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Me.Distance2D(E) <= 300
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && Utils.SleepCheck("W"))
                                    {
                                        _w.UseAbility(Me);
                                        Utils.Sleep(200, "W");
                                    }
                                }
                                if (
                                    _q != null
                                    && _q.CanBeCasted()
                                    && (E.IsLinkensProtected()
                                        || !E.IsLinkensProtected())
                                    && Me.CanCast()
                                    && Me.Distance2D(E) < _q.GetCastRange() + Me.HullRadius + 24
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    _q.UseAbility(E.Position);
                                    Utils.Sleep(330, "Q");
                                }
                                if ( // SoulRing Item 
                                    _soul != null
                                    && _soul.CanBeCasted()
                                    && Me.CanCast()
                                    && Me.Health >= (Me.MaximumHealth * 0.6)
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
                                    && !E.IsRooted()
                                    && !E.IsHexed()
                                    && !E.IsStunned()
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
                                            || _ethereal.Cooldown < 18))
                                    && !E.IsLinkensProtected()
                                    && _dagon.CanBeCasted()
                                    && Me.Distance2D(E) <= 1400
                                    && !E.IsMagicImmune()
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
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
                    if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1600 && !Me.HasModifier("modifier_earthshaker_enchant_totem"))
                    {
                        Orbwalking.Orbwalk(E, 0, 1600, true, true);
                    }
                }
                Utils.Sleep(150, "activated");
            }
            AutoSpells();

        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");


            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
            Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"earthshaker_enchant_totem", true},
                {"earthshaker_fissure", true}
            })));
            _items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon", true},
                {"item_orchid", true},
                { "item_bloodthorn", true},
                {"item_ethereal_blade", true},
                {"item_veil_of_discord", true},
                {"item_rod_of_atos", true},
                {"item_sheepstick", true},
                {"item_arcane_boots", true},
                {"item_shivas_guard",true},
                {"item_blink", true},
                {"item_soul_ring", true},
                {"item_ghost", true},
                {"item_cheese", true}
            })));
            _ult.AddItem(new MenuItem("autoUlt", "AutoUsage").SetValue(true));
            _ult.AddItem(new MenuItem("dmg", "Show Ult Damage Spell(R)").SetValue(true));

            _ult.AddItem(new MenuItem("AutoSpells", "AutoSpells").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"earthshaker_fissure", true},
                {"earthshaker_enchant_totem", true},
                {"earthshaker_echo_slam", true}
            })));
            _ult.AddItem(new MenuItem("AutoItems", "AutoItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon", true},
                {"item_blink", false},
                {"item_shivas_guard", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            _ult.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(3, 1, 5)));
            _ult.AddItem(new MenuItem("Heelm", "Max Enemies in Range to solo kill").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(_skills);
            Menu.AddSubMenu(_items);
            Menu.AddSubMenu(_ult);
            Drawing.OnDraw += DrawUltiDamage;
            Print.LogMessage.Success("Time to shake things up and see where they settle!");
        }

        public void OnCloseEvent()
        {
            Drawing.OnDraw -= DrawUltiDamage;
        }
        private bool CanIncreaseMagicDmg(Hero source, Unit target)
        {
            //var orchid = source.FindItem("item_orchid") ?? source.FindItem("item_bloodthorn");
            var veil = source.FindItem("item_veil_of_discord");
            _ethereal = source.FindItem("item_ethereal_blade");

            return (//(orchid != null && orchid.CanBeCasted() && !target.HasModifier("modifier_orchid_malevolence_debuff"))||
                  (veil != null && veil.CanBeCasted() && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                 || (_ethereal != null && _ethereal.CanBeCasted() && !IsDisembodied(target))
                 )
                 && source.CanUseItems();
        }

        private bool IsDisembodied(Unit target)
        {
            string[] modifs =
            {
                "modifier_item_ethereal_blade_ethereal",
                "modifier_pugna_decrepify"
            };

            return target.HasModifiers(modifs);
        }

        // При передаче по значению метод получает не саму переменную, а ее копию.
        // А при передаче параметра по ссылке (ref) метод получает адрес переменной в памяти, что в свою очередь экономит память.
        private double GetDamageTaken(Hero victim, ref List<Hero> enemies)
        {
            double dmgResult;

            List<Unit> creeps = ObjectManager.GetEntities<Unit>().Where(x =>
                     (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                      || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                      || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                      || x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
                      || x.ClassId == ClassId.CDOTA_Unit_SpiritBear
                      || x.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit
                      || x.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem
                      || x.ClassId == ClassId.CDOTA_BaseNPC_Creep
                      || x.HasInventory) && !x.IsMagicImmune() &&
                     x.IsAlive && x.Team != Me.Team && x.IsVisible && victim.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24 &&
                     x.IsSpawned && x.IsValidTarget()).ToList();
            int creepsECount = creeps.Count;

            dmgResult = _r != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name) &&
                _r.CanBeCasted() && enemies.Count(
                            x => victim.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value ? ((creepsECount * _creepsDmg[_r.Level]) + _rDmg[_r.Level]) + _eDmg[_e.Level] : 0;

            if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
            {
                dmgResult = _r != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name) &&
                    _r.CanBeCasted() && enemies.Count(
                            x => victim.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value ? ((creepsECount * _creepsDmg[_r.Level]) + _rDmg[_r.Level]) + _eDmg[_e.Level] *
                      (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04)) : 0;

                if (Me.Distance2D(victim) < 300 + Me.HullRadius)
                    dmgResult += _eDmg[_e.Level] * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
            }

            if (_q != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_q.Name) && _q.CanBeCasted())
            {
                dmgResult += _qDmg[_q.Level];

                if (enemies.Count(x => x.Distance2D(victim) <= 300 + Me.HullRadius && !Equals(x)) > 1)
                    dmgResult += _eDmg[_e.Level];
            }

            if (_w != null && _w.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name))
            {
                if (enemies.Count(x => x.Distance2D(victim) <= 300 + Me.HullRadius && !Equals(x)) > 1)
                    dmgResult += _eDmg[_e.Level];
            }

            if (victim.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" && victim.Spellbook.SpellR.CanBeCasted())
                dmgResult = 0;

            if (victim.HasModifier("modifier_kunkka_ghost_ship_damage_absorb"))
                dmgResult *= 0.5;

            if (victim.HasModifier("modifier_bloodseeker_bloodrage"))
            {
                var blood = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker);
                if (blood != null)
                    dmgResult *= _bloodrage[blood.Spellbook.Spell1.Level];
                else
                    dmgResult *= 1.4;
            }

            if (victim.HasModifier("modifier_chen_penitence"))
            {
                var chen =
                    ObjectManager.GetEntities<Hero>()
                        .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Chen);
                if (chen != null)
                    dmgResult *= _penitence[chen.Spellbook.Spell1.Level];
            }

            if (victim.HasModifier("modifier_shadow_demon_soul_catcher"))
            {
                var demon = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Hero_Shadow_Demon);
                if (demon != null)
                    dmgResult *= _souls[demon.Spellbook.Spell2.Level];
            }

            if (victim.HasModifier("modifier_item_mask_of_madness_berserk"))
                dmgResult *= 1.3;

            _vail = Me.FindItem("item_veil_of_discord");
            if (_vail != null && _vail.CanBeCasted() && !victim.HasModifier("modifier_item_veil_of_discord_debuff")
                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name))
            {
                dmgResult *= 1.25;
            }

           

            var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
            dmgResult = dmgResult * spellamplymult;
            _shiva = Me.FindItem("item_shivas_guard");
            if (_shiva != null && _shiva.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
                dmgResult += 200;

            int etherealdamage = (int)((Me.TotalStrength * 2) + 75);
            if (_ethereal != null && _ethereal.CanBeCasted() && victim.Handle == E?.Handle)
                dmgResult += etherealdamage * 1.4;
            if (_dagon != null && _dagon.CanBeCasted() && victim.Handle == E?.Handle)
                dmgResult += _dagonDmg[_dagon.Level];


                if (_vail != null && _vail.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name) && !victim.HasModifier("modifier_item_veil_of_discord_debuff") &&
                _ethereal != null && _ethereal.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name) && victim.Handle == E?.Handle)
                dmgResult += etherealdamage * 1.4;

            dmgResult *= 1 - victim.MagicDamageResist;
            if (!Me.HasModifier("modifier_earthshaker_enchant_totem") && _w != null && _w.CanBeCasted()
                && victim.Handle == E?.Handle
                && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name))
            {
                dmgResult += (((((Me.MaximumDamage + Me.MinimumDamage) / 2) * (_wDmg[_w.Level] / 100)) + Me.BonusDamage) - victim.DamageResist) + (_eDmg[_e.Level] * (1 - victim.MagicDamageResist));
            }

            if (Me.HasModifier("modifier_earthshaker_enchant_totem")
               && victim.Handle == E?.Handle
               && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name))
            {
                dmgResult += ((((Me.MaximumDamage + Me.MinimumDamage) / 2) + Me.BonusDamage) - victim.DamageResist) + (_eDmg[_e.Level] * (1 - victim.MagicDamageResist));
            }

            return dmgResult;
        } // GetDamageTaken::END

        private void AutoSpells()
        {
            _enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsMagicImmune() && !x.IsIllusion).ToList();

            if (Menu.Item("autoUlt").IsActive())
            {
                E = Toolset.ClosestToMouse(Me, 9000);

                foreach (var v in _enemies.Where(x => !x.IsMagicImmune()))
                {
                    _damage[v.Handle] = GetDamageTaken(v, ref _enemies);

                    if (Me.IsInvisible()) return;

                    if (
                        _enemies.Count(
                            x => x.Health <= _damage[v.Handle] && v.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value)
                    {
                        uint elsecount = 0; elsecount += 1;
                        if (_blink != null
                            && Me.CanCast()
                            && _blink.CanBeCasted()
                            && Me.Distance2D(v) > 100
                            && Me.Distance2D(v) <= 1180
                            && Utils.SleepCheck("blink")
                            )
                        {
                            _blink.UseAbility(v.Position);
                            Utils.Sleep(250, "blink");
                        }
                        else if (_w != null && _w.CanBeCasted() && Me.Distance2D(v) <= 900 + Me.HullRadius + 24 && Me.AghanimState()
                            && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                            && (_blink == null || !_blink.CanBeCasted())
                        && Utils.SleepCheck("W"))
                        {
                            _w.UseAbility(v.Position);
                            Utils.Sleep(250, "W");
                        }
                        else elsecount += 1;
                        if (_enemies.Count(
                        x => Me.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value)
                        {
                            if (_vail != null
                                && _vail.CanBeCasted()
                                && Me.CanCast()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
                                && Me.Distance2D(v) <= 1190
                                && Utils.SleepCheck("vail")
                                )
                            {
                                _vail.UseAbility(v.Position);
                                Utils.Sleep(250, "vail");
                            } // orchid Item endelse 

                            else elsecount += 1;
                            if (elsecount == 3 &&
                                _ethereal != null
                                && _ethereal.CanBeCasted()
                                && Me.CanCast()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                                && (v.Handle == E.Handle || E.Distance2D(v) > 700)
                                && Me.Distance2D(v) <= 1190
                                && Utils.SleepCheck("ethereal")
                                )
                            {
                                _ethereal.UseAbility(v);
                                Utils.Sleep(250, "ethereal");
                            } // orchid Item end
                            else elsecount += 1;
                            if (!CanIncreaseMagicDmg(Me, v))
                            {
                                if (elsecount == 4 && _r != null && _r.CanBeCasted() && Utils.SleepCheck("R")
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name))
                                {
                                    _r.UseAbility();
                                    Utils.Sleep(250, "R");
                                }
                                if (_r == null || !_r.CanBeCasted() ||
                                    !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name))
                                {
                                    if (_q != null && _q.CanBeCasted() && v.Distance2D(Me) <= 525 + Me.HullRadius + 24
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                        && Utils.SleepCheck("Q"))
                                    {
                                        _q.UseAbility(v.Position);
                                        Utils.Sleep(250, "Q");
                                    }
                                    if (_shiva != null
                                      && _shiva.CanBeCasted()
                                      && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                                      && v.Distance2D(Me) <= _shiva.GetCastRange()
                                      && Utils.SleepCheck("shiva"))
                                    {
                                        _shiva.UseAbility();
                                        Utils.Sleep(250, "shiva");
                                    }
                                }

                                if (Me.AghanimState())
                                {
                                    if (_w != null && _w.CanBeCasted()
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                        && v.Distance2D(Me) <= 300 + Me.HullRadius + 24 &&
                                    !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                    {
                                        _w.UseAbility(Me);
                                        Utils.Sleep(250, "W");
                                    }
                                }
                                else if (_w != null && _w.CanBeCasted()
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                        && v.Distance2D(Me) <= _w.GetCastRange() + Me.HullRadius + 24 &&
                                        !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility();
                                    Utils.Sleep(250, "W");
                                }
                            }
                        }
                    }
                    if (_enemies.Count(
                        x => x.Distance2D(v) <= 500) <= Menu.Item("Heelm").GetValue<Slider>().Value
                           && _blink != null
                           && _blink.CanBeCasted()
                           && Me.CanCast()
                           && v.Health <= _damage[v.Handle]
                           && Me.Distance2D(v) <= 1180
                           && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                           && Me.Distance2D(v) > 300
                           && Utils.SleepCheck("blink")
                           )
                    {
                        _blink.UseAbility(v.Position);
                        Utils.Sleep(250, "blink");
                    }
                    else if (_enemies.Count(
                       x => Me.Distance2D(x) <= 300 + Me.HullRadius + 24 && v.Health <= _damage[v.Handle]) >= 1)
                    {

                        if (_r == null || !_r.CanBeCasted() ||
                            !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name) || (_r.CanBeCasted() && _enemies.Count(
                            x => x.Health <= _damage[v.Handle] && Me.Distance2D(x) <= _r.GetCastRange() + Me.HullRadius + 24) <=
                        Menu.Item("Heel").GetValue<Slider>().Value))
                        {
                            if (Me.Distance2D(v) <= 500 + Me.HullRadius + 24 && Me.Distance2D(v) >= 300 + Me.HullRadius + 24 && Utils.SleepCheck("Move"))
                            {
                                Me.Move(v.Position);
                                Utils.Sleep(500, "Move");
                            }
                            if (v.Distance2D(Me) <= 300 + Me.HullRadius + 24)
                            {
                                if (_vail != null
                                 && _vail.CanBeCasted()
                                 && Me.CanCast()
                                 && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
                                 && Utils.SleepCheck("vail")
                                 )
                                {
                                    _vail.UseAbility(v.Position);
                                    Utils.Sleep(250, "vail");
                                } // orchid Item endelse 
                                else if (_ethereal != null
                                    && _ethereal.CanBeCasted()
                                    && Me.CanCast()
                                    && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                                    && Utils.SleepCheck("ethereal")
                                    )
                                {
                                    _ethereal.UseAbility(v);
                                    Utils.Sleep(250, "ethereal");
                                } // orchid Item end
                                else if (_q != null && _q.CanBeCasted()
                                    && Utils.SleepCheck("Q"))
                                {
                                    _q.UseAbility(v.Position);
                                    Utils.Sleep(250, "Q");
                                }
                                else if (_shiva != null
                                  && _shiva.CanBeCasted()
                                  && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                                  && Utils.SleepCheck("shiva"))
                                {
                                    _shiva.UseAbility();
                                    Utils.Sleep(250, "shiva");
                                }
                                else if (_dagon != null
                                && _dagon.CanBeCasted()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && Utils.SleepCheck("dagon"))
                                {
                                    _dagon.UseAbility(v);
                                    Utils.Sleep(250, "dagon");
                                }
                                else if (_w != null && _w.CanBeCasted() && !Me.AghanimState()
                             && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                             && !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility();
                                    Utils.Sleep(250, "W");
                                }
                                else if (_w != null && _w.CanBeCasted() && Me.AghanimState()
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name) &&
                                !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility(Me);
                                    Utils.Sleep(250, "W");
                                }
                                else if (!Me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && v.Health <= (((((Me.MaximumDamage + Me.MinimumDamage) / 2) * (_wDmg[_w.Level] / 100)) + Me.BonusDamage) + _eDmg[_e.Level]))
                                {
                                    if (Me.AghanimState())
                                    {
                                        if (_w != null && _w.CanBeCasted()
                                         && !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                        {
                                            _w.UseAbility(Me);
                                            Utils.Sleep(250, "W");
                                        }
                                    }
                                    else if (_w != null && _w.CanBeCasted()
                                        && !Me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                    {
                                        _w.UseAbility();
                                        Utils.Sleep(250, "W");
                                    }
                                }
                            }

                        }

                    }
                    if (Me.HasModifier("modifier_earthshaker_enchant_totem"))
                    {
                        if (v.Health <=
                            (((Me.MinimumDamage + Me.MaximumDamage) / 2) + Me.BonusDamage) - v.DamageAverage
                            && !Me.IsAttacking()
                            && Me.Distance2D(v) <= 300 + Me.HullRadius + 24
                            && Utils.SleepCheck("Attack"))
                        {
                            Me.Attack(v);
                            Utils.Sleep(250, "Attack");
                        }
                        else if (Me.Distance2D(v) <= 300 + Me.HullRadius + 24
                            && !Me.IsAttacking() &&
                            Utils.SleepCheck("Attack"))
                        {
                            Me.Attack(v);
                            Utils.Sleep(250, "Attack");
                        }
                    }
                } // foreach::END
            }
        } // AutoSpells::END

        private void DrawUltiDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || _enemies.Count == 0) return;

            if (Menu.Item("dmg").IsActive())
            {
                foreach (var v in _enemies)
                {
                    var screenPos = HUDInfo.GetHPbarPosition(v);
                    if (!OnScreen(v.Position)) continue;
                    var text = (v.Health <= _damage[v.Handle] ? "Yes: " + Math.Floor(_damage[v.Handle]) : "No: " + Math.Floor(_damage[v.Handle]));
                    var size = new Vector2(18, 18);
                    var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                    var position = new Vector2(screenPos.X - textSize.X + 91, screenPos.Y + 62);
                    Drawing.DrawText(
                        text,
                        position,
                        size,
                        (v.Health <= _damage[v.Handle] ? Color.LawnGreen : Color.Red),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        text,
                        new Vector2(screenPos.X - textSize.X + 92, screenPos.Y + 63),
                        size,
                        (Color.Black),
                        FontFlags.AntiAlias);
                }
            }
        } // DrawUltiDamage::END

        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width
                  || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }


    }
}