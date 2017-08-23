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

    internal class ZuusController : Variables, IHeroController
    {
        private Ability _q, _w, _e, _r;
        private Item _orchid, _sheep, _vail, _soul, _arcane, _blink, _shiva, _dagon, _atos, _ethereal, _cheese, _ghost;
        private readonly Menu _skills = new Menu("Skills", "Skills");
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _ultR = new Menu("AutoUsage Ult(R) to kill Enemies ", "idR");
        private readonly Menu _ult = new Menu("AutoUsage Abilities and Items to Solo enemy kill", "id");
        private float _eDmg;
        private float _rDmg;
        private readonly double[] _wDmg = { 0, 100, 175, 275, 350 };
        private readonly double[] _qDmg = { 0, 85, 100, 115, 145 };
        private readonly double[] _penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        private readonly double[] _bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
        private readonly double[] _souls = { 0, 1.2, 1.3, 1.4, 1.5 };
        private readonly int[] _dagonDmg = { 0, 400, 500, 600, 700, 800 };

        private readonly Dictionary<uint, double> _damage = new Dictionary<uint, double>();

        private List<Hero> _enemies = new List<Hero>();

        public void Combo()
        {
            // Target initialization

            // Spells initialization
            _q = Me.Spellbook.SpellQ;
            _w = Me.Spellbook.SpellW;
            _e = Me.Spellbook.SpellE;
            _r = Me.Spellbook.SpellR;
            // Items initialization
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

            // State of keys initialization
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            Push = Game.IsKeyDown(Menu.Item("keyQ").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            _enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsFullMagicResist() && !x.IsIllusion).ToList();
            // OnUpdateEvent::END

            // [VickTheRock]

            if (Push)
            {
                if (_q == null) return;

                var unitsList = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                    || creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit
                    || creep.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem
                    || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep
                    || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                    || creep.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster_Boar
                    || creep.ClassId == ClassId.CDOTA_Unit_SpiritBear
                    || creep.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
                    )
                    && creep.IsAlive
                    && creep.Distance2D(Me) <= _q.GetCastRange() + Me.HullRadius
                    && creep.IsSpawned
                    && creep.Team != Me.Team
                    ).ToList();


                foreach (var v in unitsList)
                {
                    var damageQ = _qDmg[_q.Level];
                    if (Me.Distance2D(v) < 1200)
                        damageQ += _e.GetAbilityData("damage_health_pct") * 0.01 * v.Health;

                    var lens = Me.HasModifier("modifier_item_aether_lens");
                    var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
                    if (lens) damageQ *= 1.08;
                    damageQ *= spellamplymult;
                    damageQ *= (1 - v.MagicDamageResist);
                    if (_q.CanBeCasted() && v.Distance2D(Me) <= _q.GetCastRange() + Me.HullRadius && v.Health <= damageQ && Utils.SleepCheck("qq"))
                    {
                        _q.UseAbility(v);
                        Utils.Sleep(250, "qq");
                    }
                }
            } // if(Push)::END

            E = Toolset.ClosestToMouse(Me);
            if (E == null) return;

            var modifEther = E.HasModifier("modifier_item_ethereal_blade_slow");
            var stoneModif = E.HasModifier("modifier_medusa_stone_gaze_stone");
            _sheep = E.Name == "npc_dota_hero_tidehunter" ? null : Me.FindItem("item_sheepstick");
            
            if (Active && Me.IsAlive && E.IsAlive && Utils.SleepCheck("activated"))
            {
                var noBlade = E.HasModifier("modifier_item_blade_mail_reflect");
                if (E.IsVisible && Me.Distance2D(E) <= 2300 && !noBlade)
                {
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
                                    && Me.Distance2D(E) < _w.GetCastRange()+Me.HullRadius
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility(E.Position);
                                    Utils.Sleep(200, "W");
                                }
                                float angle = Me.FindAngleBetween(_e.Position, true);
                                Vector3 pos = new Vector3((float)(E.Position.X - 290 * Math.Cos(angle)), (float)(E.Position.Y - 290 * Math.Sin(angle)), 0);
                                var units = ObjectManager.GetEntities<Hero>().Where(x =>
                                             !x.Equals(E)
                                             && x.IsAlive
                                             && x.Distance2D(pos) < E.Distance2D(pos)
                                             && x.Distance2D(E) <= 320
                                             && x.Team != Me.Team
                                             ).ToList();
                                if (
                                   _w != null
                                   && _w.CanBeCasted()
                                   && Me.CanCast() 
                                   && units.Count(x => x.Distance2D(pos) <= 290) == 0
                                   && Me.Distance2D(E) > _w.GetCastRange() + Me.HullRadius
                                   && Me.Distance2D(E) < _w.GetCastRange() + 300
                                   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                   && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility(E.Position);
                                    Utils.Sleep(200, "W");
                                }
                                if (
                                    _q != null
                                    && _q.CanBeCasted()
                                    && (!_w.CanBeCasted() || E.Health <= (E.MaximumHealth * 0.5))
                                    && (E.IsLinkensProtected()
                                        || !E.IsLinkensProtected())
                                    && Me.CanCast()
                                    && Me.Distance2D(E) < 1400
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    _q.UseAbility(E);
                                    Utils.Sleep(330, "Q");
                                }
                                if (
                                    _r != null
                                    && _r.CanBeCasted()
                                    && !_q.CanBeCasted()
                                    && !_w.CanBeCasted()
                                    && Me.CanCast()
                                    && Me.Position.Distance2D(E) < 1200
                                    && E.Health <= (E.MaximumHealth * 0.5)
                                    && !stoneModif
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
                                    && Me.Distance2D(E) <= 1400
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
                    Utils.Sleep(200, "activated");
                } // if(e.IsVisible)::END

                if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
                {
                    Orbwalking.Orbwalk(E, 0, 1600, true, true);
                }
            } // if(Active)::END

            // Run real-time modules
            AutoSpells();
        } // Combo::END

        // [MaZaiPC]
        private bool IsDisembodied(Unit target)
        {
            string[] modifs =
            {
                "modifier_item_ethereal_blade_ethereal",
                "modifier_pugna_decrepify"
            };

            return target.HasModifiers(modifs);
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

        private void AutoSpells()
        {
            _enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsMagicImmune() && !x.IsMagicImmune() && !x.IsIllusion && !x.IsFullMagicSpellResist()).ToList();

            if (Menu.Item("AutoUsage").IsActive())
            {
                E = Toolset.ClosestToMouse(Me,8000);

                foreach (var v in _enemies)
                {
                    
                    if (Me.IsInvisible()) return;
                    if(v.IsFullMagiclResistZuus()) return;
                    _damage[v.Handle] = CalculateDamage(v);

                    var range = Me.HullRadius + (_dagon?.GetCastRange() ?? _w?.GetCastRange());

                    float angle = Me.FindAngleBetween(v.Position, true);
                    Vector3 pos = new Vector3((float)(v.Position.X - 290 * Math.Cos(angle)), (float)(v.Position.Y - 290 * Math.Sin(angle)), 0);
                    var d = v.Position.X - range * Math.Cos(angle);
                    if (d != null)
                    {
                        Vector3 posBlink = new Vector3((float)d, (float)(v.Position.Y - range * Math.Sin(angle)), 0);
                        var units = ObjectManager.GetEntities<Hero>().Where(x =>
                            !x.Equals(v)
                            && x.IsAlive
                            && x.Distance2D(pos) < v.Distance2D(pos)
                            && x.Distance2D(v) <= 320
                            && x.Team != Me.Team
                        ).ToList();
                        if (_enemies.Count(
                                x => x.Distance2D(v) <= 500) <= Menu.Item("Heelm").GetValue<Slider>().Value
                            && _blink != null
                            && _blink.CanBeCasted()
                            && Me.CanCast()
                            && Me.Health >= (Me.MaximumHealth / 100 * Menu.Item("minHealth").GetValue<Slider>().Value)
                            && v.Health <= _damage[v.Handle]
                            && Me.Distance2D(posBlink) <= 1180
                            && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                            && Me.Distance2D(v) > 500
                            && Utils.SleepCheck("blink")
                        )
                        {
                            _blink.UseAbility(posBlink);
                            Utils.Sleep(250, "blink");
                        }
                        if (v.Health <= _damage[v.Handle] && Me.Distance2D(v) <= _w.GetCastRange() + Me.HullRadius + 300)
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
                            if (!CanIncreaseMagicDmg(Me, v))
                            {
                                if (_dagon != null
                                    && _dagon.CanBeCasted()
                                    && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                    && Utils.SleepCheck("dagon"))
                                {
                                    _dagon.UseAbility(v);
                                    Utils.Sleep(250, "dagon");
                                }
                                else if(_q != null && _q.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                        && Utils.SleepCheck("Q"))
                                {
                                    _q.UseAbility(v);
                                    Utils.Sleep(250, "Q");
                                }
                                else if (_w != null && _w.CanBeCasted() && Me.Distance2D(v) <= _w.GetCastRange() + Me.HullRadius && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                         && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility(v.Position);
                                    Utils.Sleep(250, "W");
                                }
                                else if (_w != null && _w.CanBeCasted() && units.Count(x => x.Distance2D(pos) <= 300) == 0 && Me.Distance2D(v) <= _w.GetCastRange() + Me.HullRadius + 300 && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                         && Utils.SleepCheck("W"))
                                {
                                    _w.UseAbility(pos);
                                    Utils.Sleep(250, "W");
                                }
                                else if (_r != null
                                         && _r.CanBeCasted()
                                         && (_w == null || !_w.CanBeCasted() || !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name))
                                         && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                                         && Utils.SleepCheck("R"))
                                {
                                    _r.UseAbility();
                                    Utils.Sleep(250, "R");
                                }
                                else if (_shiva != null
                                         && _shiva.CanBeCasted()
                                         && Me.Distance2D(v) <= 600 + Me.HullRadius
                                         && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                                         && Utils.SleepCheck("shiva"))
                                {
                                    _shiva.UseAbility();
                                    Utils.Sleep(250, "shiva");
                                }
                                if(_w!=null && _w.CanBeCasted() && Me.Distance2D(v)>= _w.GetCastRange() + Me.HullRadius && Me.Distance2D(v) <= _w.GetCastRange() + Me.HullRadius + 325 && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name) && Utils.SleepCheck("Move"))
                                {
                                    Me.Move(v.Position);
                                    Utils.Sleep(250, "Move");
                                }
                            }

                        }
                    }
                    _damage[v.Handle] = CalculateDamageR(v);
                    if (_r != null && _r.CanBeCasted() &&
                        Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name))
                    {

                        if (
                            _enemies.Count(
                                x =>
                                    x.Health <= _damage[v.Handle]) >=
                            Menu.Item("Heel").GetValue<Slider>().Value)
                        {
                            if ( // SoulRing Item 
                                _soul != null
                                && _soul.CanBeCasted()
                                && Me.CanCast()
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
                            if (_ethereal != null
                                  && _ethereal.CanBeCasted()
                                  && Me.CanCast()
                                  && Me.Distance2D(v) <= _ethereal.GetCastRange()
                                  && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                                  && Utils.SleepCheck("ethereal")
                                  )
                            {
                                _ethereal.UseAbility(v);
                                Utils.Sleep(250, "ethereal");
                            } // orchid Item end
                            else if (_r != null
                            && _r.CanBeCasted()
                            && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                            && Utils.SleepCheck("R"))
                            {
                                _r.UseAbility();
                                Utils.Sleep(250, "R");
                            }
                            else if (_dagon != null
                            && _dagon.CanBeCasted() && Me.Distance2D(v) <= _dagon.GetCastRange()
                            && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                            && Utils.SleepCheck("dagon"))
                            {
                                _dagon.UseAbility(v);
                                Utils.Sleep(250, "dagon");
                            }
                        }
                    }
                } // foreach::END
            }
        } // AutoSpells::END

        private double CalculateDamageR(Hero victim)
        {
            double dmgResult = 0;
            _eDmg = _e.GetAbilityData("damage_health_pct") * 0.01f * victim.Health;
            _rDmg = Me.AghanimState() ? _r.GetAbilityData("damage_scepter") : _r.GetAbilityData("damage");

            if (_r != null &&
                _r.CanBeCasted() && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(_r.Name))
            {
                if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
                {
                    dmgResult += _rDmg * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
                }
                else
                    dmgResult += _rDmg;

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

                if (victim.Distance2D(Me) <= 1200 + Me.HullRadius)
                {
                    dmgResult += _eDmg;

                    _vail = Me.FindItem("item_veil_of_discord");
                    if (_vail != null && _vail.CanBeCasted() &&
                        !victim.HasModifier("modifier_item_veil_of_discord_debuff")
                        && Me.Distance2D(victim) <= _vail.GetCastRange()
                        && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name))
                    {
                        dmgResult *= 1.25;
                    }
                    int etherealdamage = (int)((Me.TotalStrength * 2) + 75);
                    if (_ethereal != null && _ethereal.CanBeCasted() && Me.Distance2D(victim) <= _ethereal.GetCastRange() &&
                        victim.Handle == E?.Handle)
                    {
                        dmgResult += etherealdamage * 1.4;
                    }
                    if (_dagon != null && _dagon.CanBeCasted() && Me.Distance2D(victim) <= _dagon.GetCastRange() &&
                        victim.Handle == E?.Handle &&
                        Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                        dmgResult += _dagonDmg[_dagon.Level];
                }

                dmgResult *= 1 - victim.MagicDamageResist;
            }
            return dmgResult;
        }

        private double CalculateDamage(Hero victim)
        {
            double dmgResult = 0;
            _eDmg = _e.GetAbilityData("damage_health_pct") * 0.01f * victim.Health;
            _rDmg = Me.AghanimState() ? _r.GetAbilityData("damage_scepter") : _r.GetAbilityData("damage");

            if (_r != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_r.Name) &&
            _r.CanBeCasted())
            {
                if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
                {
                    dmgResult += _rDmg * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
                }
                else
                    dmgResult += _rDmg;
            }
            if (_q != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_q.Name) && _q.CanBeCasted())
                dmgResult += _qDmg[_q.Level];

            if (_w != null && _w.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(_w.Name))
                dmgResult += _wDmg[_w.Level];

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

            if (victim.Distance2D(Me) <= 1200 + Me.HullRadius)
                dmgResult += _eDmg;


            var spellamplymult = 1 + (Me.TotalIntelligence / 16 / 100);
            dmgResult = dmgResult * spellamplymult;
            dmgResult *= 1 - victim.MagicDamageResist;
            int etherealdamage = (int)((Me.TotalIntelligence * 2) + 75);
            if (_ethereal != null && _ethereal.CanBeCasted() && victim.Handle == E?.Handle && (_vail == null || victim.HasModifier("modifier_item_veil_of_discord_debuff") || _vail.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name) || !Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_vail.Name)))
                dmgResult = dmgResult * 1.4 + etherealdamage;

            if (_dagon != null && _dagon.CanBeCasted() && victim.Handle == E?.Handle && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                dmgResult += _dagonDmg[_dagon.Level];
            _shiva = Me.FindItem("item_shivas_guard");
            if (_shiva != null && _shiva.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
                dmgResult += 200;



            return dmgResult;
        } // GetDamageTaken::END

        private void DrawUltiDamage(EventArgs args)
        {
            _enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != Me.Team && !x.IsFullMagicResist() && !x.IsIllusion).ToList();
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || _enemies.Count == 0) return;

            if (Menu.Item("dmg").IsActive())
            {
                foreach (var v in _enemies)
                {

                    _damage[v.Handle] = CalculateDamage(v);
                    var screenPos = HUDInfo.GetHPbarPosition(v);
                    if (!OnScreen(v.Position)) continue;
                    var text = v.Health <= _damage[v.Handle] ? "Yes: " + Math.Floor(_damage[v.Handle]) : "No: " + Math.Floor(_damage[v.Handle]);
                    var size = new Vector2(18, 18);
                    var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                    var position = new Vector2(screenPos.X - textSize.X + 85, screenPos.Y + 62);

                    Drawing.DrawText(
                        text,
                        new Vector2(screenPos.X - textSize.X + 84, screenPos.Y + 63),
                        size,
                        (Color.White),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        text,
                        position,
                        size,
                        (v.Health <= _damage[v.Handle] ? Color.LawnGreen : Color.Red),
                        FontFlags.AntiAlias);




                    _damage[v.Handle] = CalculateDamageR(v);
                    var textR = v.Health <= _damage[v.Handle] ? "ThundergodS" : "";
                    var positionR = new Vector2(screenPos.X - textSize.X + 60, screenPos.Y - 20);

                    Drawing.DrawText(
                        textR,
                        new Vector2(screenPos.X - textSize.X + 59, screenPos.Y - 19),
                        size,
                        (Color.White),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        textR,
                        positionR,
                        size,
                        (Color.LawnGreen),
                        FontFlags.AntiAlias);

                }
            }
        } // DrawUltiDamage::END
        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width
                  || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }
        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success("Ah these mortals and their futile games. Oh wait! I'm one of them!");

            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
            Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("keyQ", "Farm Creep Key").SetValue(new KeyBind('F', KeyBindType.Press)));

            _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true},
                {"zuus_lightning_bolt", true},
                {"zuus_thundergods_wrath", true}
            })));
            _items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
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
            _ultR.AddItem(new MenuItem("AutoUsage", "AutoUsage").SetValue(true));

            _ult.AddItem(new MenuItem("dmg", "Show Draw Damage").SetValue(true));

            _items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true}
            })));
            _ult.AddItem(new MenuItem("AutoSpells", "AutoSpells").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true},
                {"zuus_lightning_bolt", true},
                {"zuus_thundergods_wrath", true}
            })));
            _ult.AddItem(new MenuItem("AutoItems", "AutoItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_blink", true},
                {"item_dagon", true},
                {"item_shivas_guard", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            _ultR.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(2, 1, 5)));
            _ultR.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_thundergods_wrath", true}
            })));
            _ultR.AddItem(new MenuItem("AutoUltItems", "AutoUltItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            _ult.AddItem(new MenuItem("minHealth", "Min Me healh % to blink in killsteal").SetValue(new Slider(25, 05))); // x/ 10%
            _ult.AddItem(new MenuItem("Heelm", "Max Enemies in Range to solo kill").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(_skills);
            Menu.AddSubMenu(_items);
            Menu.AddSubMenu(_ultR);
            Menu.AddSubMenu(_ult);
            Drawing.OnDraw += DrawUltiDamage;
        }

        public void OnCloseEvent()
        {

            Drawing.OnDraw -= DrawUltiDamage;
        }
    }

    internal static class ToolsetZuus
    {
        public static bool IsFullMagiclResistZuus(this Unit source)
        {
            return source.HasModifier("modifier_medusa_stone_gaze_stone")
                   || source.HasModifier("modifier_huskar_life_break_charge")
                   || source.HasModifier("modifier_oracle_fates_edict")
                   || source.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                   || source.HasModifier("modifier_puck_phase_shift")
                   || source.HasModifier("modifier_eul_cyclone")
                   || source.HasModifier("modifier_invoker_tornado")
                   || source.HasModifier("modifier_dazzle_shallow_grave")
                   || source.HasModifier("modifier_winter_wyvern_winters_curse")
                   || (source.HasModifier("modifier_legion_commander_duel") && source.ClassId== ClassId.CDOTA_Unit_Hero_Legion_Commander && source.AghanimState())
                   || source.HasModifier("modifier_brewmaster_storm_cyclone")
                   || source.HasModifier("modifier_shadow_demon_disruption")
                   || source.HasModifier("modifier_tusk_snowball_movement")
                   || source.HasModifier("modifier_abaddon_borrowed_time")
                   || source.HasModifier("modifier_faceless_void_time_walk")
                   || source.HasModifier("modifier_huskar_life_break_charge");
        }
    }
}
