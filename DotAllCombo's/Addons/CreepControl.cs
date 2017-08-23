namespace DotaAllCombo.Addons
{
    using System.Security.Permissions;
    using Ensage.Common.Menu;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Linq;
    using SharpDX.Direct3D9;
    using Service;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class CreepControl : IAddon
    {
        private Item _midas, _abyssal, _mjollnir, _boots, _medall, _mom;
        private Hero _e;
        private Hero _targetLock;
        private Font _txt;
        private Font _not;
        private Hero _me;

        public void Load()
        {
            _txt = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Tahoma",
                   Height = 11,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.Default
               });

            _not = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Tahoma",
                   Height = 12,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.Default
               });

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            OnLoadMessage();
        }

        public void Unload()
        {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnPreReset -= Drawing_OnPreReset;
        }

        public void RunScript()
        {
            _me = ObjectManager.LocalHero;
            if (!MainMenu.CcMenu.Item("controll").IsActive() || !Game.IsInGame || _me == null || Game.IsPaused ||
                Game.IsChatOpen) return;


            var holdKey = MainMenu.CcMenu.Item("Press Key").GetValue<KeyBind>().Active;
            var toggleKey = MainMenu.CcMenu.Item("Toogle Key").GetValue<KeyBind>().Active;
            var lockTargetKey = MainMenu.CcMenu.Item("Lock target Key").GetValue<KeyBind>().Active;

            var targetMode = MainMenu.CcMenu.Item("Target mode").GetValue<StringList>().SelectedIndex;
            
            var targetFindSource = MainMenu.CcMenu.Item("Target find source").GetValue<StringList>().SelectedIndex;
            var targetFindRange = MainMenu.CcMenu.Item("Target find range").GetValue<Slider>().Value;
            var units = ObjectManager.GetEntities<Unit>().Where(creep =>
                (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Additive
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Tusk_Sigil
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep
                || creep.ClassId == ClassId.CDOTA_Unit_VisageFamiliar
                || creep.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalEarth
                || creep.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalStorm
                || creep.ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalFire
                || creep.ClassId == ClassId.CDOTA_NPC_WitchDoctor_Ward
                || creep.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster_Boar
                || creep.ClassId == ClassId.CDOTA_Unit_SpiritBear
                || creep.ClassId == ClassId.CDOTA_BaseNPC_Venomancer_PlagueWard
                || creep.ClassId == ClassId.CDOTA_BaseNPC_ShadowShaman_SerpentWard
                || creep.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
                || creep.ClassId == ClassId.CDOTA_Unit_Elder_Titan_AncestralSpirit
                || creep.IsIllusion
                )
                && creep.IsAlive
                && creep.Team == _me.Team
                && creep.IsControllable).ToList();
            if (lockTargetKey)
            {
                _targetLock = TargetSelector.ClosestToMouse(_me, targetFindRange);
            }

            if (_targetLock != null)
            {
                if (_targetLock.IsAlive)
                {
                    _e = _targetLock;
                }
                else
                {
                    switch (targetMode)
                    {
                        case 0:
                            switch (targetFindSource)
                            {
                                case 0:
                                    var enemyHero0 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == _me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= targetFindRange).ToList();
                                    _e = enemyHero0.MinOrDefault(x => x.Distance2D(_me.Position));
                                    break;
                                case 1:
                                    _e = TargetSelector.ClosestToMouse(_me, 5000);
                                    break;
                            }
                            break;
                        case 1:
                            switch (targetFindSource)
                            {
                                case 0:
                                    var enemyHero0 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == _me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= targetFindRange).ToList();
                                    _e = enemyHero0.MinOrDefault(x => x.Health);
                                    break;
                                case 1:
                                    var enemyHero1 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == _me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(_me.Position) <= targetFindRange).ToList();
                                    _e = enemyHero1.MinOrDefault(x => x.Health);
                                    break;
                            }
                            break;
                    }
                }
            }
            else
            {
                switch (targetMode)
                {
                    case 0:
                        _e = TargetSelector.ClosestToMouse(_me, 5000);
                        break;
                    case 1:
                        var enemyHero = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == _me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= targetFindRange).ToList();
                        _e = enemyHero.MinOrDefault(x => x.Health);
                        break;
                }
            }
            if (Utils.SleepCheck("delay"))
            {
                if (_me.IsAlive)
                {
                    var count = units.Count();
                    if (count <= 0) return;
                    for (int i = 0; i < count; ++i)
                    {
                        var v = ObjectManager.GetEntities<Hero>()
                                          .Where(x => x.Team == _me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
                       if (units[i].Name == "npc_dota_juggernaut_healing_ward")

                            {
                                if (_me.Position.Distance2D(units[i].Position) > 5 && Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Move(_me.Position);
                                    Utils.Sleep(50, units[i].Handle.ToString());
                                }
                        }
                        else if (units[i].Name == "npc_dota_neutral_ogre_magi")
                        {
                            for (var z = 0; z < v.Count(); ++z)
                            {
                                var armor = units[i].Spellbook.SpellQ;

                                if ((!v[z].HasModifier("modifier_ogre_magi_frost_armor") || !_me.HasModifier("modifier_ogre_magi_frost_armor")) && armor.CanBeCasted() && units[i].Position.Distance2D(v[z]) <= 900
                                    && Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    armor.UseAbility(v[z]);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                        }
                        else if (units[i].Name == "npc_dota_neutral_forest_troll_high_priest")
                        {
                            if (units[i].Spellbook.SpellQ.CanBeCasted())
                            {
                                for (var z = 0; z < v.Count(); ++z)
                                {
                                    if (units[i].Position.Distance2D(v[z]) <= 900
                                    && Utils.SleepCheck(units[i].Handle + "high_priest"))
                                    {
                                        units[i].Spellbook.SpellQ.UseAbility(v[z]);
                                        Utils.Sleep(350, units[i].Handle + "high_priest");
                                    }
                                }
                            }
                        }


                        if (_e == null) return;
                        
                        if (_e.IsAlive && !_e.IsInvul() && (holdKey || toggleKey))
                        {

                            //spell
                            var checkStun = _e.HasModifier("modifier_centaur_hoof_stomp");
                            var checkSetka = _e.HasModifier("modifier_dark_troll_warlord_ensnare");
                            if (units[i].Name == "npc_dota_neutral_dark_troll_warlord")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 550 && (!checkSetka || !checkStun || !_e.IsHexed() || !_e.IsStunned()) && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                         Utils.SleepCheck(units[i].Handle + "warlord"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(450, units[i].Handle + "warlord");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_big_thunder_lizard")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 250 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "lizard");
                                }
                                if (_e.Position.Distance2D(units[i].Position) < 550 && units[i].Spellbook.SpellW.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellW.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "lizard");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_centaur_khan")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 200 && (!checkSetka || !checkStun || !_e.IsHexed() || !_e.IsStunned()) && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "centaur"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "centaur");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_hellcaller")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyr"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(350, units[i].Handle + "satyr");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_trickster")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyr_trickster"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(350, units[i].Handle + "satyr_trickster");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_soulstealer")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyrsoulstealer"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(350, units[i].Handle + "satyrsoulstealer");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_black_dragon")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 700 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "dragonspawn"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e.Predict(600));
                                    Utils.Sleep(350, units[i].Handle + "dragonspawn");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_big_thunder_lizard")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 200 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(350, units[i].Handle + "lizard");
                                }

                                for (var z = 0; z < v.Count(); z++)
                                {
                                    if (units[i].Spellbook.SpellW.CanBeCasted() && units[i].Position.Distance2D(v[z]) <= 900)
                                    {
                                        if (_e.Position.Distance2D(v[z]) < v[z].AttackRange + 150 &&
                                        Utils.SleepCheck(units[i].Handle + "lizard"))
                                        {
                                            units[i].Spellbook.SpellW.UseAbility(v[z]);
                                            Utils.Sleep(350, units[i].Handle + "lizard");
                                        }
                                    }
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_mud_golem")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 850 && (!checkSetka || !checkStun || !_e.IsHexed() || !_e.IsStunned())
                                    && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "golem"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(350, units[i].Handle + "golem");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_polar_furbolg_ursa_warrior")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 240 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "ursa"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(350, units[i].Handle + "ursa");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_harpy_storm")
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 900 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle + "harpy"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(350, units[i].Handle + "harpy");
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_BaseNPC_Tusk_Sigil)
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 1550 &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Move(_e.Predict(1500));
                                    Utils.Sleep(700, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_BaseNPC_Creep)
                            {
                                if (units[i].Name == "npc_dota_necronomicon_archer")
                                {
                                    if (_e.Position.Distance2D(units[i].Position) <= 700 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))

                                    {
                                        units[i].Spellbook.SpellQ.UseAbility(_e);
                                        Utils.Sleep(300, units[i].Handle.ToString());
                                    }
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_Unit_VisageFamiliar)
                            {
                                var damageModif = units[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");

                                if (_e.Position.Distance2D(units[i].Position) < 1550 && units[i].Health < 6 && units[i].Spellbook.Spell1.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.Spell1.UseAbility();
                                    Utils.Sleep(200, units[i].Handle.ToString());
                                }

                                if (_e.Position.Distance2D(units[i].Position) < 340 && ((damageModif.StackCount < 1) && !_e.IsStunned()) && units[i].Spellbook.Spell1.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.Spell1.UseAbility();
                                    Utils.Sleep(350, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalEarth)
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 1300 &&
                                    units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (_e.Position.Distance2D(units[i].Position) < 340 &&
                                    units[i].Spellbook.SpellR.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellR.UseAbility();
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_Unit_Brewmaster_PrimalStorm)
                            {
                                if (_e.Position.Distance2D(units[i].Position) < 700 &&
                                    units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(_e.Position);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (_e.Position.Distance2D(units[i].Position) < 900 &&
                                    units[i].Spellbook.SpellE.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellE.UseAbility();
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (_e.Position.Distance2D(units[i].Position) < 850 &&
                                    units[i].Spellbook.SpellR.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellR.UseAbility(_e);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_Unit_SpiritBear)
                            {
                                if ((!_me.AghanimState() && _me.Position.Distance2D(units[i]) <= 1200) || _me.AghanimState())
                                {
                                    _abyssal = units[i].FindItem("item_abyssal_blade");

                                    _mjollnir = units[i].FindItem("item_mjollnir");

                                    _boots = units[i].FindItem("item_phase_boots");

                                    _midas = units[i].FindItem("item_hand_of_midas");

                                    _mom = units[i].FindItem("item_mask_of_madness");

                                    _medall = units[i].FindItem("item_medallion_of_courage") ?? units[i].FindItem("item_solar_crest");


                                    if (_boots != null && _e.Position.Distance2D(units[i].Position) < 1550 && _boots.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        _boots.UseAbility();
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (_mjollnir != null && _e.Position.Distance2D(units[i].Position) < 525 && _mjollnir.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        _mjollnir.UseAbility(units[i]);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (_medall != null && _e.Position.Distance2D(units[i].Position) < 525 && _medall.CanBeCasted() &&
                                       Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        _medall.UseAbility(_e);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }

                                    if (_mom != null && _e.Position.Distance2D(units[i].Position) < 525 && _mom.CanBeCasted() &&
                                       Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        _mom.UseAbility();
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (_abyssal != null && _e.Position.Distance2D(units[i].Position) < 500 && _abyssal.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        _abyssal.UseAbility(_e);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (_midas != null)
                                    {
                                        var neutrals = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral || creep.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
                                            creep.IsAlive && creep.IsVisible && creep.IsSpawned && creep.Distance2D(units[i])<= 700 && creep.Team != _me.Team).OrderBy(x => x.Health).LastOrDefault();

                                       if (_midas.CanBeCasted() && Utils.SleepCheck(neutrals.Handle.ToString()))
                                        {
                                            _midas.UseAbility(neutrals);
                                            Utils.Sleep(350, neutrals.Handle.ToString());
                                        }
                                    }
                                }
                            }
                            else if (units[i].ClassId == ClassId.CDOTA_BaseNPC_Additive)
                            {
                                if (units[i].Name == "npc_dota_templar_assassin_psionic_trap")
                                {

                                    if (_e.Position.Distance2D(units[i].Position) < 250
                                        && units[i].Spellbook.Spell1.CanBeCasted()
                                        && _e.Distance2D(Game.MousePosition) <= 1000
                                        && Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        units[i].Spellbook.Spell1.UseAbility();
                                        Utils.Sleep(250, units[i].Handle.ToString());
                                    }
                                }
                            }
                            if (units[i].Distance2D(_e) <= units[i].AttackRange + 100 && !_e.IsAttackImmune()
                            && !units[i].IsAttacking() && units[i].CanAttack() && Utils.SleepCheck(units[i].Handle + "Attack")
                            )
                            {
                                units[i].Attack(_e);
                                Utils.Sleep(250, units[i].Handle + "Attack");
                            }
                            else if (!units[i].CanAttack() && !units[i].IsAttacking()
                                && units[i].CanMove() && units[i].Distance2D(_e) <= 4000 && Utils.SleepCheck(units[i].Handle + "Move")
                                )
                            {
                                units[i].Move(_e.Predict(300));
                                Utils.Sleep(350, units[i].Handle + "Move");
                            }
                            else if (units[i].Distance2D(_e) >= units[i].GetAttackRange() && !units[i].IsAttacking()
                               && units[i].CanMove() && units[i].Distance2D(_e) <= 4000 && Utils.SleepCheck(units[i].Handle + "MoveAttack")
                               )
                            {
                                units[i].Move(_e.Predict(300));
                                Utils.Sleep(350, units[i].Handle + "MoveAttack");
                            }
                        }
                        Utils.Sleep(500, "delay");
                    }
                }
            }
        }


        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            _txt.Dispose();
            _not.Dispose();
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            if (MainMenu.CcMenu.Item("controll").IsActive())
            {
                _txt.DrawText(null, "Creep Control On", 1200, 27, Color.Coral);
            }

            if (!MainMenu.CcMenu.Item("controll").IsActive())
            {
                _txt.DrawText(null, "Creep Control Off", 1200, 27, Color.Coral);
            }
        }


        private void Drawing_OnPostReset(EventArgs args)
        {
            _txt.OnResetDevice();
            _not.OnResetDevice();
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            _txt.OnLostDevice();
            _not.OnLostDevice();
        }

        private void OnLoadMessage()
        {
            Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon CreepControl is Loaded!</font>");
            Service.Debug.Print.ConsoleMessage.Encolored("@addon CreepControl is Loaded!", ConsoleColor.Yellow);
        }
    }
}
