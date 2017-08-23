using DotaAllCombo.Extensions;

namespace DotaAllCombo.Heroes
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using SharpDX;
    using Service.Debug;

    internal class TinkerController : Variables, IHeroController
    {
        private const int HideAwayRange = 130;
        private Ability _q, _w, _r, _e;
        private Item _dagon, _sheep, _soul, _ethereal, _shiva, _ghost, _eul, _blink, _force, _glimmer, _vail, _orchid, _guardian, _travel, _lotus;
        public readonly List<ParticleEffect> Effects = new List<ParticleEffect>();
        private const string EffectPath = @"particles\range_display_blue.vpcf";
        private const string EffectPanicPath = @"particles\range_display_red.vpcf";
        private readonly Menu _skills = new Menu("Skills", "Skills");
        private readonly Menu _items = new Menu("Items", "Items");
        private readonly Menu _panics = new Menu("Panic", "Panic Menu");
        private readonly Menu _push = new Menu("Push", "Push Menu");
        //private readonly Menu farm = new Menu("Farm", "Farm Menu");

        public Vector3 GetClosestToVector(Vector3[] coords, Unit z)
        {
            var closestVector = coords.First();
            foreach (var v in coords.Where(v => closestVector.Distance2D(z) > v.Distance2D(z)))
                closestVector = v;
            return closestVector;
        }
        public void Combo()
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;

            if (Me == null)
                return;
            Push = Game.IsKeyDown(Menu.Item("keyPush").GetValue<KeyBind>().Key);
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
            CastW = Game.IsKeyDown(Menu.Item("keySpamW").GetValue<KeyBind>().Key);
            CastE = Game.IsKeyDown(Menu.Item("keySpamE").GetValue<KeyBind>().Key);
            List<Unit> fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain).ToList();
            var creeps = ObjectManager.GetEntities<Creep>().Where(creep =>
                   (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
                  creep.IsAlive && creep.Team != Me.Team && creep.IsVisible && creep.IsSpawned).ToList();
            Vector3 panic = GetClosestToVector(TinkerCords.PanicPos, Me);
            Vector3 safe = GetClosestToVector(TinkerCords.SafePos, Me);

            if (Menu.Item("panicMod").IsActive() && Me.IsAlive)
            {
                _blink = Me.FindItem("item_blink");
                _e = Me.Spellbook.SpellE;
                _r = Me.Spellbook.SpellR;
                _travel = Me.FindItem("item_travel_boots") ?? Me.FindItem("item_travel_boots_2");
                _soul = Me.FindItem("item_soul_ring");
                var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                    .ToList();
                if (v.Count(x => x.Distance2D(Me) <= 1190) >= 1
                        && !Active
                        && Me.Distance2D(panic) <= 1190
                        && !Push
                        && Me.Distance2D(safe) <= HideAwayRange
                        && Me.Distance2D(panic) >= HideAwayRange
                        && _blink != null
                        && Me.CanCast()
                        && Utils.SleepCheck("blink")
                        && _blink.CanBeCasted())
                {
                    {
                        _blink.UseAbility(panic);
                        Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                        Utils.Sleep(250, "blink");
                    }
                }

                if (v.Count(x => x.Distance2D(Me) <= 2000) >= 1
                    && Me.Health <= (Me.MaximumHealth / 100 * Menu.Item("Healh").GetValue<Slider>().Value))
                {
                    if (
                        _blink != null
                        && Me.CanCast()
                        && Me.Distance2D(panic) <= 1190
                        && Me.Distance2D(panic) >= HideAwayRange
                        && !_r.IsChanneling
                        && Utils.SleepCheck("blink")
                        && _blink.CanBeCasted()
                        )
                    {
                        _blink.UseAbility(panic);
                        Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                        Utils.Sleep(250, "blink");
                    }
                    else if (
                        _travel != null
                        && _travel.CanBeCasted()
                        && Menu.Item("Panic Items").GetValue<AbilityToggler>().IsEnabled("item_travel_boots")
                        && Me.Distance2D(panic) <= HideAwayRange
                        && Utils.SleepCheck("travel")
                        )
                    {
                        _travel.UseAbility(fount.First().Position);
                        Utils.Sleep(300, "travel");
                    }
                    else
                        if (_r.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling()) return;
                    else if (
                           _r != null
                           && _r.CanBeCasted()
                           && Me.Distance2D(panic) <= HideAwayRange
                           && _travel != null
                           && !_travel.CanBeCasted()
                           && !_r.IsChanneling
                           && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                           && Utils.SleepCheck("R")
                           )
                    {
                        _r.UseAbility();
                        Utils.Sleep(800, "R");
                    }
                }
            }
            if (Active && !Game.IsChatOpen && Me.IsAlive)
            {
                E = Me.ClosestToMouseTarget(2500);
                //Skils
                _q = Me.Spellbook.SpellQ;
                _w = Me.Spellbook.SpellW;
                _r = Me.Spellbook.SpellR;
                //Items
                _lotus = Me.FindItem("item_lotus_orb");
                _blink = Me.FindItem("item_blink");
                _dagon = Me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
                _sheep = Me.FindItem("item_sheepstick");
                _soul = Me.FindItem("item_soul_ring");
                _ethereal = Me.FindItem("item_ethereal_blade");
                _shiva = Me.FindItem("item_shivas_guard");
                _ghost = Me.FindItem("item_ghost");
                _eul = Me.FindItem("item_cyclone");
                _force = Me.FindItem("item_force_staff");
                _glimmer = Me.FindItem("item_glimmer_cape");
                _vail = Me.FindItem("item_veil_of_discord");
                _orchid = Me.FindItem("item_orchid") ?? Me.FindItem("item_bloodthorn");
                _guardian = Me.FindItem("item_guardian_greaves");
                if (E == null) return;
                if (E.IsAlive && !Me.IsChanneling())
                {

                    if (E.IsLinkensProtected() && Utils.SleepCheck("Linkens"))
                    {
                        if (_eul != null
                            && _eul.CanBeCasted()
                            && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_eul.Name))
                        {
                            _eul.UseAbility(E);
                        }
                        else if (_force != null
                            && _force.CanBeCasted()
                            && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_force.Name))
                        {
                            _force.UseAbility(E);
                        }
                        else if (_dagon != null
                            && _dagon.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                        {
                            _dagon.UseAbility(E);
                        }
                        else if (_q != null
                            && _q.CanBeCasted()
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name))
                        {
                            _q.UseAbility(E);
                        }
                        else if (_ethereal != null
                            && _ethereal.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
                        {
                            _ethereal.UseAbility(E);
                            Utils.Sleep(200, "Linkens");
                            Utils.Sleep((Me.NetworkPosition.Distance2D(E.NetworkPosition) / 650) * 1000, "Linkens");
                        }
                        else if (_sheep != null
                            && _sheep.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
                        {
                            _sheep.UseAbility(E);
                        }
                        Utils.Sleep(450, "Linkens");
                    }
                    else
                    {
                        float angle = Me.FindAngleBetween(E.Position, true);
                        Vector3 pos = new Vector3((float)(E.Position.X - 550 * Math.Cos(angle)), (float)(E.Position.Y - 550 * Math.Sin(angle)), 0);
                        uint elsecount = 0;
                        var v =
                         ObjectManager.GetEntities<Hero>()
                            .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                            .ToList();

                        bool magicimune = (!E.IsMagicImmune() && !E.HasModifier("modifier_eul_cyclone"));
                        if (Utils.SleepCheck("combo"))
                        {

                            if (_blink != null
                                && _blink.CanBeCasted()
                                && !Me.IsChanneling()
                                && Me.Distance2D(pos) <= 1200
                                && Me.Mana > _q.ManaCost
                                && Me.Distance2D(E) >= 500
                                && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_blink.Name)
                                && Utils.SleepCheck("Rearm"))
                                _blink.UseAbility(pos);
                            elsecount += 1;
                            if (_sheep != null
                                 && _sheep.CanBeCasted()
                                 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name)
                                 && magicimune && Utils.SleepCheck("Rearm"))
                                _sheep.UseAbility(E);
                            else elsecount += 1;
                            if (_orchid != null
                                && _orchid.CanBeCasted()
                                && !Me.IsChanneling()
                                && !E.IsSilenced()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_orchid.Name)
                                && Utils.SleepCheck("Rearm"))
                                _orchid.UseAbility(E);
                            else elsecount += 1;
                            if (_vail != null
                                && _vail.CanBeCasted()
                                && !Me.IsChanneling()
                                && Me.Distance2D(pos) <= 1000
                                && Me.Mana > _r.ManaCost
                                && !E.HasModifier("modifier_item_veil_of_discord_debuff")
                                && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_vail.Name)
                                && Utils.SleepCheck("Rearm"))
                                _vail.UseAbility(E.Position);
                            else elsecount += 1;
                            if (_glimmer != null
                                && _glimmer.CanBeCasted()
                                && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_glimmer.Name)
                                && Utils.SleepCheck("Rearm"))
                                _glimmer.UseAbility(Me);
                            else
                                elsecount += 1;
                            if (_lotus != null
                                && _lotus.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_lotus.Name)
                                && Utils.SleepCheck("Rearm"))
                                _lotus.UseAbility(Me);
                            else
                                elsecount += 1;
                            if (_soul != null
                                && _soul.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
                                && Utils.SleepCheck("Rearm"))
                                _soul.UseAbility();
                            else
                                elsecount += 1;
                            if (_ethereal != null
                                && _ethereal.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                            {
                                _ethereal.UseAbility(E);
                                if (Utils.SleepCheck("TimeEther") && Me.Distance2D(E) <= _ethereal.GetCastRange())
                                    Utils.Sleep((Me.NetworkPosition.Distance2D(E.NetworkPosition) / 620) * 1000, "TimeEther");
                            }
                            else
                                elsecount += 1;
                            if (_ghost != null
                                && _ghost.CanBeCasted()
                                && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
                                && Utils.SleepCheck("Rearm"))
                                _ghost.UseAbility();
                            else
                                elsecount += 1;
                            if (_q != null
                                && _q.CanBeCasted()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                _q.UseAbility(E);
                            else
                                elsecount += 1;
                            if (_e != null
                                && _e.CanBeCasted()
                                && v.Count(x => x.Distance2D(Me) <= 800) >= 2
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                _e.UseAbility(Prediction.InFront(Me, 150));
                            else
                                elsecount += 1;
                            if (_dagon != null
                                && _dagon.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && magicimune && Utils.SleepCheck("Rearm")
                                && Utils.SleepCheck("TimeEther"))
                                _dagon.UseAbility(E);
                            else
                                elsecount += 1;
                            if (_w != null
                                && _w.CanBeCasted()
                                && Me.Distance2D(E) <= 2500
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                            {
                                _w.UseAbility();
                                if (Utils.SleepCheck("TimeW")
                                    && Me.Distance2D(E) <= _w.GetCastRange())
                                    Utils.Sleep((Me.NetworkPosition.Distance2D(E.NetworkPosition) / 600) * 1000, "TimeW");
                            }
                            else
                                elsecount += 1;
                            if (_shiva != null
                                && _shiva.CanBeCasted()
                                && Me.Distance2D(E) <= 600
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                _shiva.UseAbility();
                            else elsecount += 1;
                            if (_guardian != null
                                && _guardian.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_guardian.Name)
                                && Utils.SleepCheck("Rearm"))
                                _guardian.UseAbility();
                            else
                                elsecount += 1;
                            if (elsecount == 15
                                && _eul != null
                                && _eul.CanBeCasted()
                                && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_eul.Name)
                                && magicimune && Utils.SleepCheck("Rearm") && Utils.SleepCheck("TimeEther")
                                && Utils.SleepCheck("TimeW"))
                                _eul.UseAbility(E);
                            else
                                elsecount += 1;
                            if (elsecount == 16
                                && _r != null && _r.CanBeCasted()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                                && !_r.IsChanneling
                                && Utils.SleepCheck("Rearm")
                                && CheckRefresh())
                            {
                                _r.UseAbility();
                                Utils.Sleep(800, "Rearm");
                            }
                            else
                            {
                                if (!Me.IsChanneling() && Utils.SleepCheck("Rearm"))
                                {
                                    if (Menu.Item("orbwalk").GetValue<bool>() && Me.Distance2D(E) <= 1900)
                                    {
                                        Orbwalking.Orbwalk(E, 0, 1600, true, true);
                                    }
                                }

                            }
                            Utils.Sleep(150, "combo");
                        }
                    }
                }
            }
            if (CastW && !Game.IsChatOpen && Me.IsAlive)
            {
                E = Me.ClosestToMouseTarget(2500);
                //Skils
                _w = Me.Spellbook.SpellW;
                _r = Me.Spellbook.SpellR;
                //Items
                _soul = Me.FindItem("item_soul_ring");
                _guardian = Me.FindItem("item_guardian_greaves");
                if (E == null || _r == null || _w == null) return;
                if (E.IsAlive && !E.IsIllusion && !Me.IsChanneling() && Me.Distance2D(E) <= 2500)
                {
                    if (_r.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling()) return;
                    uint elsecount = 0;
                    bool magicimune = (!E.IsMagicImmune() && !E.HasModifier("modifier_eul_cyclone"));
                    if (Utils.SleepCheck("combo"))
                    {

                        if (_soul != null
                            && _soul.CanBeCasted()
                            && Me.Health >= (Me.MaximumHealth * 0.5)
                            && Utils.SleepCheck("Rearm"))
                            _soul.UseAbility();
                        else
                            elsecount += 1;
                        if (_w != null
                            && _w.CanBeCasted()
                            && magicimune && Utils.SleepCheck("Rearm"))
                        {
                            _w.UseAbility();
                            if (Utils.SleepCheck("TimeW")
                                && Me.Distance2D(E) <= _w.GetCastRange())
                                Utils.Sleep((Me.NetworkPosition.Distance2D(E.NetworkPosition) / 600) * 1000, "TimeW");
                        }
                        else
                            elsecount += 1;
                        if (_guardian != null
                            && _guardian.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            _guardian.UseAbility();
                        else
                            elsecount += 1;
                        if (elsecount == 3
                            && _r != null
                            && _r.CanBeCasted()
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                            && !_r.IsChanneling
                            && ((((_soul != null && !_soul.CanBeCasted())) || _soul == null)
                            || (((_w != null && !_w.CanBeCasted())) || _w == null)
                            || (((_guardian != null && !_guardian.CanBeCasted())) || _guardian == null)
                               )
                            && Utils.SleepCheck("Rearm")
                            )
                        {
                            _r.UseAbility();
                            Utils.Sleep(900, "Rearm");
                        }
                        Utils.Sleep(150, "combo");
                    }
                }
            }
            if (CastE && !Game.IsChatOpen && Me.IsAlive)
            {
                //Skils
                _e = Me.Spellbook.SpellE;
                _r = Me.Spellbook.SpellR;
                //Items
                _soul = Me.FindItem("item_soul_ring");
                _guardian = Me.FindItem("item_guardian_greaves");
                if (_r == null || _e == null) return;
                if (_r.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling()) return;
                if (!Me.IsChanneling())
                {

                    uint elsecount = 0;
                    if (Utils.SleepCheck("combo"))
                    {

                        if (_soul != null
                            && _soul.CanBeCasted()
                            && Me.Health >= (Me.MaximumHealth * 0.5)
                            && Utils.SleepCheck("Rearm"))
                            _soul.UseAbility();
                        else
                            elsecount += 1;
                        if (_e != null
                            && _e.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            _e.UseAbility(Prediction.InFront(Me, 290));
                        else
                            elsecount += 1;
                        if (_guardian != null
                            && _guardian.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            _guardian.UseAbility();
                        else
                            elsecount += 1;
                        if (elsecount == 3
                            && _r != null && _r.CanBeCasted()
                            && !_r.IsChanneling
                            && (
                            (((_soul != null
                            && !_soul.CanBeCasted()))
                            || _soul == null)
                            || (((_e != null
                            && !_e.CanBeCasted()))
                            || _e == null)
                            | (((_guardian != null
                            && !_guardian.CanBeCasted()))
                            || _guardian == null)
                            )
                            && Utils.SleepCheck("Rearm")
                            )
                        {
                            _r.UseAbility();
                            Utils.Sleep(900, "Rearm");
                        }
                        Utils.Sleep(150, "combo");
                    }
                }

            }

            if (Menu.Item("pushMod").IsActive() && !Active && !CastW && !CastE && Me.IsAlive)
            {
                _blink = Me.FindItem("item_blink");
                _e = Me.Spellbook.SpellE;
                _r = Me.Spellbook.SpellR;
                _travel = Me.FindItem("item_travel_boots") ?? Me.FindItem("item_travel_boots_2");
                _soul = Me.FindItem("item_soul_ring");
                _lotus = Me.FindItem("item_lotus_orb");
                _glimmer = Me.FindItem("item_glimmer_cape");
                _ghost = Me.FindItem("item_ghost");
                if ((Me.HasModifier("modifier_fountain_aura_buff") && Menu.Item("pushModif").IsActive()) || Push)
                {

                    if (_r.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling() || _r == null) return;

                    if (creeps.Count(x => x.Distance2D(Me) <= 1100) >= 1)
                    {
                        if (_ghost != null
                            && _ghost.CanBeCasted()
                            && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(_ghost.Name)
                            && Utils.SleepCheck("ghost"))
                        {
                            _ghost.UseAbility();
                            Utils.Sleep(250, "ghost");
                        }
                        if (_lotus != null
                            && _lotus.CanBeCasted()
                            && creeps.Count(x => x.Distance2D(Me) <= 1100) >= 2
                            && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(_lotus.Name)
                            && Utils.SleepCheck("lotus"))
                        {
                            _lotus.UseAbility(Me);
                            Utils.Sleep(250, "lotus");
                        }
                        if (
                            _glimmer != null
                            && _glimmer.CanBeCasted()
                            && creeps.Count(x => x.Distance2D(Me) <= 1100) >= 2
                            && Me.Distance2D(safe) >= HideAwayRange
                            && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(_glimmer.Name)
                            && Utils.SleepCheck("glimmer"))
                        {
                            _glimmer.UseAbility(Me);
                            Utils.Sleep(250, "glimmer");
                        }
                        if (
                          _e != null && _e.CanBeCasted()
                          && !_r.IsChanneling
                          && (Me.Distance2D(safe) <= HideAwayRange
                          || !Menu.Item("pushSafe").IsActive())
                          && creeps.Count(x => x.Distance2D(Me) <= 900) >= 2
                          && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
                          && Utils.SleepCheck("E")
                          )
                        {
                            _e.UseAbility(Prediction.InFront(Me, 200));
                            Utils.Sleep(250, "E");
                        }
                        if (
                          _e != null && _e.CanBeCasted()
                          && !_r.IsChanneling
                          && (creeps.Count(x => x.Distance2D(safe) <= 900) <= 1 || Me.Distance2D(safe) >= 1190)
                          && creeps.Count(x => x.Distance2D(Me) <= 900) >= 2
                          && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
                          && Utils.SleepCheck("E")
                          )
                        {
                            _e.UseAbility(Prediction.InFront(Me, 200));
                            Utils.Sleep(250, "E");
                        }
                        if (
                         _e != null && !_e.CanBeCasted()
                         && !_r.IsChanneling
                         && Me.Distance2D(safe) >= 1190
                         && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_e.Name)
                         && Utils.SleepCheck("E")
                         )
                        {
                            Me.Move(safe);
                            Utils.Sleep(250, "E");
                        }
                        if (
                        _blink != null
                        && Me.CanCast()
                        && (Menu.Item("pushSafe").IsActive()
                        || !_e.CanBeCasted())
                        && !_r.IsChanneling
                        && _blink.CanBeCasted()
                        )
                        {
                            if (Me.Distance2D(safe) <= 1190
                                && Me.Distance2D(safe) >= 100
                                && Utils.SleepCheck("blink"))
                            {
                                _blink.UseAbility(safe);
                                Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                                Utils.Sleep(250, "blink");
                            }
                        }
                        if (
                           _blink != null
                           && Me.CanCast()
                           && Menu.Item("panicMod").IsActive()
                           && !_r.IsChanneling
                           && _blink.CanBeCasted()
                           )
                        {
                            if (Me.Distance2D(safe) >= 1190
                                && Me.Distance2D(panic) <= 1190
                                && Utils.SleepCheck("blink"))
                            {
                                _blink.UseAbility(panic);
                                Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                                Utils.Sleep(250, "blink");
                            }
                        }
                    }
                    if (_soul != null
                        && _soul.CanBeCasted()
                        && !_r.IsChanneling
                        && Me.Health >= (Me.MaximumHealth * 0.5)
                        && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name)
                        && Utils.SleepCheck("soul"))
                    {
                        _soul.UseAbility();
                        Utils.Sleep(250, "soul");
                    }
                    if (
                      _r != null
                      && _r.CanBeCasted()
                      && _travel != null
                      && !_travel.CanBeCasted()
                      && Me.Distance2D(fount.First().Position) <= 900
                      && !_r.IsChanneling
                      && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                      && Utils.SleepCheck("R")
                      )
                    {
                        _r.UseAbility();
                        Utils.Sleep(900, "R");
                    }
                }
                if (_r.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling()) return;


                if (Me.Distance2D(safe) >= 150) return;
                if (_soul != null
                        && _soul.CanBeCasted()
                        && !_r.IsChanneling
                        && Me.Health >= (Me.MaximumHealth * 0.5)
                        && Me.Distance2D(safe) <= HideAwayRange
                        && Utils.SleepCheck("soul"))
                {
                    _soul.UseAbility();
                    Utils.Sleep(500, "soul");
                }

                if (
                    _travel != null
                    && _travel.CanBeCasted()
                    && !_r.IsChanneling
                    && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled("item_travel_boots")
                    && Me.Mana <= _r.ManaCost + 75
                    && Me.Distance2D(safe) <= HideAwayRange
                    && Utils.SleepCheck("travel")
                   )
                {
                    _travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }

                if (
                    _travel != null
                    && _travel.CanBeCasted()
                    && creeps.Count(x => x.Distance2D(Me) <= 1100) <= 2
                    && !_r.IsChanneling
                    && Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled("item_travel_boots")
                    && Me.Distance2D(safe) <= HideAwayRange
                    && Utils.SleepCheck("travel")
                   )
                {
                    _travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }
                else
                if (
                    _r != null
                    && _r.CanBeCasted()
                    && !_e.CanBeCasted()
                    && creeps.Count(x => x.Distance2D(Me) >= 1100) >= 2
                    && !_r.IsChanneling
                    && Me.Mana >= _r.ManaCost + 75
                    && Me.Distance2D(safe) <= HideAwayRange
                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_r.Name)
                    && Utils.SleepCheck("R")
                   )
                {
                    _r.UseAbility();
                    Utils.Sleep(900, "R");
                }
            }
            /*
            if (Menu.Item("farmMod").IsActive() && Me.IsAlive)
            {
                var bestPos = GetClosestToTarget(TinkerCords.BestPos, Me);
                var farmPos = GetClosestToTarget(TinkerCords.FarmPos, Me);
                E = Me.Spellbook.SpellE;
                R = Me.Spellbook.SpellR;
                soul = Me.FindItem("item_soul_ring");
                var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                    .ToList();

                if (Me.NetworkActivity == NetworkActivity.Move || Me.Distance2D(farmPos)>100) return;
                if (v.Count(x => x.Distance2D(Me) <= 1000) <= 0)
                {
                        if (R.IsChanneling || Me.HasModifier("modifier_tinker_rearm") || Me.IsChanneling()) return;
                    if (soul != null
                        && soul.CanBeCasted()
                        && E != null
                        && E.CanBeCasted()
                        && !R.IsChanneling
                        && Me.Health >= (Me.MaximumHealth * 0.45)
                        && Me.Distance2D(farmPos) <= HIDE_AWAY_RANGE
                        && Utils.SleepCheck("soul"))
                    {
                        soul.UseAbility();
                        Utils.Sleep(500, "soul");
                    }
                    else if (
                           E != null
                           && E.CanBeCasted()
                           && Me.Distance2D(farmPos) <= HIDE_AWAY_RANGE
                           && !R.IsChanneling
                           && Utils.SleepCheck("E")
                           )
                    {
                        E.UseAbility(bestPos);
                        Utils.Sleep(250, "E");
                    }

                }
            }*/


        }

        private bool CheckRefresh()
        {
            if ((_ghost != null && _ghost.CanBeCasted() && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_ghost.Name))
                || (_soul != null && _soul.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_soul.Name))
                || (_sheep != null && _sheep.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_sheep.Name))
                || (_q != null && _q.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_q.Name))
                || (_ethereal != null && _ethereal.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_ethereal.Name))
                || (_dagon != null && _dagon.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                || (_w != null && _w.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(_w.Name))
                || (_guardian != null && _guardian.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_guardian.Name))
                || (_shiva != null && _shiva.CanBeCasted() && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(_shiva.Name))
                || (_eul != null && _eul.CanBeCasted() && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_eul.Name))
                || (_glimmer != null && _glimmer.CanBeCasted() && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(_glimmer.Name)))
                return false;
            return true;
        }
        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.2");

            Print.LogMessage.Success(" I have several theories I'd like to put into practice.");
            Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
            Menu.AddItem(new MenuItem("keyBind", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            _push.AddItem(new MenuItem("keyPush", "Press Push Key").SetValue(new KeyBind('E', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("keySpamW", "Use Missile and Mana items(DefMode)").SetValue(new KeyBind('F', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("keySpamE", "Use March and Mana items(DefMode)").SetValue(new KeyBind('G', KeyBindType.Press)));
            _push.AddItem(new MenuItem("pushMod", "AutoPushHalper").SetValue(true));
            _push.AddItem(new MenuItem("pushModif", "Use auto Push if i have modif Fountain").SetValue(true));
            _push.AddItem(new MenuItem("pushSafe", "Use March Only Safe Pos Range!").SetValue(true));
            _panics.AddItem(new MenuItem("panicMod", "Auto Blink and Travel base position if Healt <=| and have enemy").SetValue(true));
            _panics.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35))); // x/ 10%
            _push.AddItem(new MenuItem("drawPart", "Draw the position for use Blink").SetValue(true));
            _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"tinker_march_of_the_machines",true},
                {"tinker_laser",true},
                {"tinker_heat_seeking_missile",true},
                {"tinker_rearm",true}
            })));
            _panics.AddItem(new MenuItem("Panic Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_travel_boots",true},
            })));
            _items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon",true},
                {"item_sheepstick",true},
                {"item_soul_ring",true},
                {"item_orchid",true},
                {"item_guardian_greaves",true},
                {"item_ethereal_blade",true},
                {"item_shivas_guard",true}
            })));
            _push.AddItem(new MenuItem("Push Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_travel_boots",true},
                {"item_soul_ring",true},
                {"item_lotus_orb",true},
                {"item_ghost",true},
                {"item_glimmer_cape",true}
            })));
            _items.AddItem(new MenuItem("Item", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_lotus_orb",true},
                {"item_ghost",true},
                {"item_cyclone",true},
                {"item_force_staff",true},
                {"item_glimmer_cape",true},
                { "item_veil_of_discord",true},
                { "item_blink",true}
            })));
            Menu.AddSubMenu(_panics);
            Menu.AddSubMenu(_push);
            Menu.AddSubMenu(_skills);
            Menu.AddSubMenu(_items);
            Drawing.OnDraw += ParticleDraw;
        }

        public void OnCloseEvent()
        {
            Drawing.OnDraw -= ParticleDraw;
        }

        private bool _iscreated;

        private void ParticleDraw(EventArgs args)
        {
            //
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;

            if (Me == null) return;
            if (Menu.Item("drawPart").IsActive())
            {
                for (int i = 0; i < TinkerCords.SafePos.Count(); ++i)
                {
                    if (!_iscreated)
                    {
                        ParticleEffect effect = new ParticleEffect(EffectPath, TinkerCords.SafePos[i]);
                        effect.SetControlPoint(1, new Vector3(HideAwayRange, 0, 0));
                        Effects.Add(effect);
                    }
                }
                if (Menu.Item("panicMod").IsActive())
                {
                    for (int i = 0; i < TinkerCords.PanicPos.Count(); ++i)
                    {
                        if (!_iscreated)
                        {
                            ParticleEffect effect = new ParticleEffect(EffectPanicPath, TinkerCords.PanicPos[i]);
                            effect.SetControlPoint(1, new Vector3(HideAwayRange, 0, 0));
                            Effects.Add(effect);
                        }
                    }
                }
                _iscreated = true;
            }
        }
        internal class TinkerCords
        {
            /* public static readonly Vector3[]
                BestPos =
                {
                    new Vector3(-6752, 4325, 384),
                };
           public static readonly Vector3[]
                FarmPos =
                {
                    new Vector3(-6752, 4325, 384),
                };*/
            public static readonly Vector3[]
            SafePos =
            {
            new Vector3(-6752, 4325, 384),
            new Vector3(-5017, 5099, 384),
            new Vector3(-4046, 5282, 384),
            new Vector3(-2531, 5419, 384),
            new Vector3(-1561, 5498, 384),
            new Vector3(-1000, 5508, 384),
            new Vector3(-749, 6791, 384),
            new Vector3(359, 6668, 384),
            new Vector3(1624, 6780, 256),
            new Vector3(-6877, 3757, 384),
            new Vector3(-5662, 2268, 384),
            new Vector3(-6941, 1579, 384),
            new Vector3(-6819, 608, 384),
            new Vector3(-6848, 68, 384),
            new Vector3(-7005, -681, 384),
            new Vector3(-7082, -1160, 384),
            new Vector3(-2605, -2657, 256),
            new Vector3(-2207, -2394, 256),
            new Vector3(-1446, -1871, 256),
            new Vector3(-2068, -1151, 256),
            new Vector3(659, 929, 256),
            new Vector3(1065, 1241, 256),
            new Vector3(2259, 667, 256),
            new Vector3(2426, 812, 256),
            new Vector3(2647, 1009, 256),
            new Vector3(2959, 1283, 256),
            new Vector3(2110, 2431, 256),
            new Vector3(6869, 613, 384),
            new Vector3(6832, -206, 384),
            new Vector3(6773, -431, 384),
            new Vector3(6742, -1549, 384),
            new Vector3(6813, -3591, 384),
            new Vector3(6745, -4689, 384),
            new Vector3(6360, -5215, 384),
            new Vector3(4637, -5579, 384),
            new Vector3(4756, -6491, 384),
            new Vector3(4249, -6553, 384),
            new Vector3(2876, -5666, 384),
            new Vector3(3180, -6627, 384),
            new Vector3(2013, -6684, 384),
            new Vector3(-560, -6810, 384),
            new Vector3(-922, -6797, 384),
            new Vector3(-1130, -6860, 384),
            new Vector3(1000, -6928, 384),
            new Vector3(814, 981, 256),
            new Vector3(-6690, 5024, 384),
            new Vector3(-5553, 1961, 384),
        };
            public static readonly Vector3[]
            PanicPos =
            {
            new Vector3(-752, -6961, 383),
            new Vector3(93, -6959, 383),
            new Vector3(758, -7021, 384),
            new Vector3(1816, -6797, 384),
            new Vector3(2705, -6839, 384),
            new Vector3(2506, -5658, 384),
            new Vector3(4658, -5404, 384),
            new Vector3(5558, -6553, 384),
            new Vector3(6261, -6097, 384),
            new Vector3(6744, -5342, 384),
            new Vector3(5271, -4309, 384),
            new Vector3(6994, -3484, 384),
            new Vector3(2654, -4520, 384),
            new Vector3(-492, -5489, 384),
            new Vector3(-1103, -4761, 384),
            new Vector3(-2219, -4762, 255),
            new Vector3(-2458, -4182, 256),
            new Vector3(-2280, -3277, 256),
            new Vector3(-2080, -2578, 256),
            new Vector3(-3698, -1303, 384),
            new Vector3(-2296, -1253, 256),
            new Vector3(-3238, -601, 384),
            new Vector3(-3595, 242, 640),
            new Vector3(-4003, 1318, 384),
            new Vector3(-4530, 1976, 384),
            new Vector3(-5466, 2152, 384),
            new Vector3(-5121, 2041, 640),
            new Vector3(-5417, 1188, 384),
            new Vector3(-5052, 791, 383),
            new Vector3(-4627, -530, 640),
            new Vector3(-7242, -288, 384),
            new Vector3(-6951, 358, 384),
            new Vector3(-7139, 693, 384),
            new Vector3(-7385, 1317, 384),
            new Vector3(-7352, -1285, 384),
            new Vector3(-7464, -3214, 384),
            new Vector3(-7461, -3619, 383),
            new Vector3(-7572, -3872, 384),
            new Vector3(-7407, -4303, 384),
            new Vector3(-7330, -5013, 384),
            new Vector3(-5562, -6994, 384),
            new Vector3(-4830, -7086, 384),
            new Vector3(-4015, -7129, 384),
            new Vector3(-3066, -6947, 256),
            new Vector3(3975, -5508, 384),
            new Vector3(7217, -2250, 384),
            new Vector3(6938, -973, 384),
            new Vector3(6943, -334, 384),
            new Vector3(7000, 316, 384),
            new Vector3(7198, 771, 384),
            new Vector3(7590, 1568, 256),
            new Vector3(7591, 2070, 256),
            new Vector3(7477, 2790, 384),
            new Vector3(7522, 2949, 384),
            new Vector3(7464, 5083, 384),
            new Vector3(5350, 1011, 640),
            new Vector3(4648, -652, 384),
            new Vector3(2395, -251, 384),
            new Vector3(2602, 805, 256),
            new Vector3(2822, 1096, 255),
            new Vector3(844, 1474, 255),
            new Vector3(602, 1064, 256),
            new Vector3(-932, 1953, 384),
            new Vector3(-3115, 931, 384),
            new Vector3(-4439, 3226, 384),
            new Vector3(-2662, 4506, 384),
            new Vector3(-1389, 5406, 384),
            new Vector3(-1258, 5480, 384),
            new Vector3(1012, 4587, 640),
            new Vector3(1652, 7235, 384),
            new Vector3(2020, 6933, 256),
            new Vector3(2493, 7178, 384),
            new Vector3(3864, 6726, 384),
            new Vector3(4612, 6880, 384),
            new Vector3(5654, 6752, 384),
            new Vector3(5469, 6752, 384),
            new Vector3(4505, 2020, 256),
            new Vector3(5210, -643, 384),
            new Vector3(5467, -1202, 384),
            new Vector3(6894, -4705, 384),
            new Vector3(-735, -3173, 256),
            new Vector3(-24, -3628, 384),
            new Vector3(1438, -3047, 384),
            new Vector3(3001, -3094, 384),
            new Vector3(4345, -2316, 383),
            new Vector3(3625, -1479, 384),
            new Vector3(2350, 2732, 256),
            new Vector3(-367, 3230, 384),
            new Vector3(-1816, 4064, 640),
            new Vector3(-4493, 4960, 384),
            new Vector3(-6864, 5850, 384),
            new Vector3(-6308, 6498, 384),
            new Vector3(-5156, 6642, 383),
            new Vector3(-4521, 6699, 384),
            new Vector3(-3690, 6627, 384),
            new Vector3(-2719, 6764, 384),
            new Vector3(-2286, 6778, 384),
            new Vector3(-1637, 6881, 384),
            new Vector3(-676, 6944, 384),
            new Vector3(338, 6875, 384),
            new Vector3(8, 3375, 384),
            new Vector3(1727, 3367, 384),
            new Vector3(-3565, -4903, 256),
            new Vector3(610, -4438, 384),
            new Vector3(4271, -4513, 384),
            new Vector3(3930, -6864, 384),
            new Vector3(-6972, 3864, 384),
            new Vector3(-5545, 4132, 383),
            new Vector3(-7008, 5027, 384),
        };
        }
    }
}
