using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Service.Debug;

    internal class TuskController : Variables, IHeroController
    {
        
        private Ability _q, _w, _e, _r;

        private Item _blink, _mjollnir, _medall, _urn, _mail, _bkb, _abyssal, _satanic, _halberd, _dagon, _shiva, _soul, _arcane, _mom;

        //private int[] qDmg = new int[4] {40, 80, 120, 160};

        public void Combo()
        {
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            E = Toolset.ClosestToMouse(Me);

            if (!Menu.Item("enabled").IsActive())
                return;
            if (E == null) return;
            if (Active && E.IsAlive && !E.IsInvul() && !E.IsIllusion)
            {
                _q = Me.Spellbook.SpellQ;

				_w = Me.Spellbook.SpellW ?? Me.FindSpell("tusk_launch_snowball");

				_e = Me.Spellbook.SpellE;

                _r = Me.Spellbook.SpellR;

                _urn = Me.FindItem("item_urn_of_shadows");

                _blink = Me.FindItem("item_blink");

                _satanic = Me.FindItem("item_satanic");

                _dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

                _halberd = Me.FindItem("item_heavens_halberd");

                _medall = Me.FindItem("item_medallion_of_courage") ?? Me.FindItem("item_solar_crest");

                _abyssal = Me.FindItem("item_abyssal_blade");

                _mjollnir = Me.FindItem("item_mjollnir");

                _soul = Me.FindItem("item_soul_ring");

                _arcane = Me.FindItem("item_arcane_boots");

                _mom = Me.FindItem("item_mask_of_madness");

                _shiva = Me.FindItem("item_shivas_guard");

                _mail = Me.FindItem("item_blade_mail");

                _bkb = Me.FindItem("item_black_king_bar");

                var v =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                        .ToList();

                var linkens = E.IsLinkensProtected();
                var modifW = Me.HasModifier("modifier_tusk_snowball_movement");
                var medallModiff =
                    E.HasModifier("modifier_item_medallion_of_courage_armor_reduction") ||
                    E.HasModifier("modifier_item_solar_crest_armor_reduction");

				

                if (!Me.IsInvisible())
                {
                    if ( // Q Skill
                    _q != null
                    && _q.CanBeCasted()
                    && Me.CanCast()
                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                    && modifW
                    && !E.IsMagicImmune()
                    && Me.Distance2D(E) <= 300
                    && Utils.SleepCheck("Q")
                    )
                    {
                        _q.UseAbility(E.Predict(400));
                        Utils.Sleep(200, "Q");
                    } // Q Skill end

                    if ( //R Skill
                        _r != null
                        && (medallModiff
                        || E.IsMagicImmune()
                        || _medall == null)
                        && _r.CanBeCasted()
                        && Me.CanCast()
                        && !linkens
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                        && Me.Distance2D(E) <= 700
                        && Utils.SleepCheck("R")
                        )
                    {
                        _r.UseAbility(E);
                        Utils.Sleep(150, "R");
                    } // R Skill end


                    if (_urn != null && _urn.CanBeCasted() && _urn.CurrentCharges > 0 && Me.Distance2D(E) <= 800 &&
                        Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_urn.Name) && Utils.SleepCheck("urn"))
                    {
                        _urn.UseAbility(E);
                        Utils.Sleep(240, "urn");
                    }
                    float angle = Me.FindAngleBetween(E.Position, true);
                    Vector3 pos = new Vector3((float)(E.Position.X + 30 * Math.Cos(angle)), (float)(E.Position.Y + 30 * Math.Sin(angle)), 0);
                    if (
                        _blink != null
                        && _q.CanBeCasted()
                        && Me.CanCast()
                        && _blink.CanBeCasted()
                        && Me.Distance2D(pos) >= Me.GetAttackRange()+Me.HullRadius+24
                        && Me.Distance2D(pos) <= 1190
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        _blink.UseAbility(pos);
                        Utils.Sleep(250, "blink");
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
                    if ( // E Skill
                        _e != null
                        && _e.CanBeCasted()
                        && Me.CanCast()
                        && modifW
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
                        && !E.IsMagicImmune()
                        && Me.Distance2D(E) <= 500
                        && Utils.SleepCheck("E")
                        )
                    {
                        _e.UseAbility();
                        Utils.Sleep(350, "E");
                    } // E Skill end
                    if ( // SoulRing Item 
                        _soul != null
                        && Me.Mana <= _q.ManaCost
                        && _soul.CanBeCasted()
                        && Utils.SleepCheck("soul")
                        )
                    {
                        _soul.UseAbility();
                        Utils.Sleep(250, "soul");
                    } // SoulRing Item end

                    if ( // Arcane Boots Item
                        _arcane != null
                        && Me.Mana <= _q.ManaCost
                        && _arcane.CanBeCasted()
                        && Utils.SleepCheck("arcane")
                        )
                    {
                        _arcane.UseAbility();
                        Utils.Sleep(250, "arcane");
                    } // Arcane Boots Item end

                    if ( // Shiva Item
                        _shiva != null &&
                        _shiva.CanBeCasted() &&
                        Me.CanCast() &&
                        !E.IsMagicImmune() &&
                        Utils.SleepCheck("shiva") &&
                        Me.Distance2D(E) <= 600
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                        )
                    {
                        _shiva.UseAbility();
                        Utils.Sleep(250, "shiva");
                    } // Shiva Item end

                    if ( // MOM
                        _mom != null &&
                        _mom.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mom.Name)
                        && Me.CanCast()
                        && Utils.SleepCheck("mom")
                        && Me.Distance2D(E) <= 700
                        )
                    {
                        _mom.UseAbility();
                        Utils.Sleep(250, "mom");
                    } // MOM Item end

                    if ( // Medall
                        _medall != null &&
                        _medall.CanBeCasted() &&
                        !Me.IsInvisible()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_medall.Name)
                        && Utils.SleepCheck("Medall")
                        && Me.Distance2D(E) <= 500
                        )
                    {
                        _medall.UseAbility(E);
                        Utils.Sleep(250, "Medall");
                    } // Medall Item end


                    if ( // Hellbard
                        _halberd != null
                        && _halberd.CanBeCasted()
                        && Me.CanCast()
                        && !E.IsMagicImmune()
                        && Utils.SleepCheck("halberd")
                        && Me.Distance2D(E) <= 700
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_halberd.Name)
                        )
                    {
                        _halberd.UseAbility(E);
                        Utils.Sleep(250, "halberd");
                    } // Hellbard Item end

                    if ( // Mjollnir
                        _mjollnir != null
                        && _mjollnir.CanBeCasted()
                        && Me.CanCast()
                        && !E.IsMagicImmune()
                        && Utils.SleepCheck("mjollnir")
                        && Me.Distance2D(E) <= 900
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_mjollnir.Name)
                        )
                    {
                        _mjollnir.UseAbility(Me);
                        Utils.Sleep(250, "mjollnir");
                    } // Mjollnir Item end

                    if ( // Dagon
                        _dagon != null
                        && _dagon.CanBeCasted()
                        && Me.CanCast()
                        && !E.IsMagicImmune()
                        && Utils.SleepCheck("dagon")
                        )
                    {
                        _dagon.UseAbility(E);
                        Utils.Sleep(150, "dagon");
                    } // Dagon Item end


                    if ( // Satanic 
                        _satanic != null
                        && Me.Health <= (Me.MaximumHealth * 0.3)
                        && _satanic.CanBeCasted()
                        && Me.Distance2D(E) <= 300
                        && Utils.SleepCheck("satanic")
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_satanic.Name)
                        )
                    {
                        _satanic.UseAbility();
                        Utils.Sleep(150, "satanic");
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


                    if ( // W Skill
                        _w != null
                        && _w.CanBeCasted()
                        && !E.IsMagicImmune()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                        && Utils.SleepCheck("W")
                        )
                    {
                        _w.UseAbility(E);
                        _w.UseAbility();
                        Utils.Sleep(120, "W");
                    }

                    var sigl = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassId == ClassId.CDOTA_BaseNPC_Tusk_Sigil)
                                                                        && x.IsAlive && x.IsControllable);

                    if (Menu.Item("SiglControl").IsActive() && sigl != null)
                    {
                        if (E.Position.Distance2D(sigl.Position) < 1550 &&
                                    Utils.SleepCheck(sigl.Handle.ToString()))
                        {
                            sigl.Move(Prediction.InFront(E, 350));
                            Utils.Sleep(350, sigl.Handle.ToString());
                        }
                    }
                }
                if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
                {
                    Orbwalking.Orbwalk(E, 0, 1600, true, true);
                }
            }
            if (Active)
            {
                var modifW = Me.HasModifier("modifier_tusk_snowball_movement");
                if (modifW && Menu.Item("SnowBall").IsActive())
                {

                    var teamarm = ObjectManager.GetEntities<Hero>().Where(ally =>
                        ally.Team == Me.Team && ally.IsAlive && Me.Distance2D(ally) <= 395
                        && ally.Health >= (ally.MaximumHealth * 0.4)
                        && !ally.HasModifier("modifier_tusk_snowball_movement_friendly")).ToList();

                    var unitToSnow =
                        ObjectManager.GetEntities<Unit>().Where(x =>
                        ((x.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit
                        || x.ClassId == ClassId.CDOTA_Unit_SpiritBear
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem
                        || x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
                        || x.ClassId == ClassId.CDOTA_BaseNPC_Creep)
                        && !x.IsAttackImmune() && !x.IsInvul() && x.IsVisible
                        && x.IsAlive && Me.Distance2D(x) <= 395)
                        && x.IsAlive && x.IsControllable
                        && !x.HasModifier("modifier_tusk_snowball_movement_friendly")
                        && !x.HasModifier("modifier_tusk_snowball_movement")).ToList();
                    if (teamarm != null)
                    {
                        foreach (Hero v in teamarm)
                        {
                            if (modifW && v.Distance2D(Me) < 395 &&
                                !v.HasModifier("modifier_tusk_snowball_movement_friendly") && !v.IsInvul() &&
                                !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                Me.Attack(v);
                                Utils.Sleep(200, v.Handle.ToString());
                            }
                        }
                    }
                    if (unitToSnow != null)
                    {
                        foreach (Unit v in unitToSnow)
                        {
                            if (modifW && v.Distance2D(Me) < 395 &&
                                !v.HasModifier("modifier_tusk_snowball_movement_friendly") && !v.IsInvul() &&
                                !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                Me.Attack(v);
                                Utils.Sleep(200, v.Handle.ToString());
                            }
                        }
                    }
                }
            }
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.01");

            Print.LogMessage.Success("Who's ready for a fight? The first hit is free! Anyone? One-Punch Man! xD");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("SiglControl", "SiglControl").SetValue(true));
            Menu.AddItem(new MenuItem("SnowBall", "Pick up allies in SnowBall").SetValue(true));
            Menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"tusk_launch_snowball", true},
                    {"tusk_ice_shards", true},
                    {"tusk_frozen_sigil", true},
                    {"tusk_snowball", true},
                    {"tusk_walrus_punch", true}
                })));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_blink", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true},
                    { "item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
                    {"item_veil_of_discord", true},
                    {"item_abyssal_blade", true},
                    {"item_shivas_guard", true},
                    {"item_blade_mail", true},
                    {"item_black_king_bar", true},
                    {"item_satanic", true},
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true}
                })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
        }

        public void OnCloseEvent()
        {

        }
    }
}