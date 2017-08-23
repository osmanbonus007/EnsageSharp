using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using SharpDX;
using Ensage.Common.Menu;


namespace InvokerNinja
{
    class SunStrike
    {
        private static readonly Menu Menu = new Menu("Invoker", "Invoker", true, "npc_dota_hero_Invoker", true);
        private static int OrbMinDist => Menu.Item("orbwalk.minDistance").GetValue<Slider>().Value;
        private static int SunstrikeTimeConfig => Menu.Item("sunstrike.timeconfig").GetValue<Slider>().Value;
        private static int SunstrikeTimeConfig2 => Menu.Item("sunstrike.timeconfig2").GetValue<Slider>().Value;
        private static int SunstrikeMinHP => Menu.Item("Minimum amount of health for sunstrike").GetValue<Slider>().Value;
        private static int quasthreshold => Menu.Item("quas threshold health").GetValue<Slider>().Value;
        private static int boxsize => Menu.Item("Box Size").GetValue<Slider>().Value;
        private static int boxX => Menu.Item("Box Position X").GetValue<Slider>().Value;
        private static int boxY => Menu.Item("Box Position Y").GetValue<Slider>().Value;
        private static int starttickcount, currenttickcount;
        private static uint aditionalkey;
        private static string SaveSelectedTargetName;
        private static Dictionary<int, int> TurntimeOntick = new Dictionary<int, int> { };
        private static Hero me, target, EnemykillablebySS, DisabledEnemy;
        private static uint nextskillvalue, combonumber, nextskillflee = 0;
        private static Ability quas, wex, exort, invoke, coldsnap, meteor, alacrity, tornado, forgespirit, blast, sunstrike, emp, icewall, ghostwalk;
        private static Item eul, medallion, solar_crest, malevolence, bloodthorn, urn, vyse, refresher, ethereal, dagon;
        private static bool comboing = false, target_magic_imune, target_isinvul, target_meteor_ontiming, target_emp_ontiming, target_sunstrike_ontiming, target_blast_ontiming, quas_level, exort_level, wex_level, forge_in_my_side, ice_wall_distance, refresher_use = false;
        private static float distance_me_target, targetisturning_delay = -777;
        private static ParticleEffect targetParticle;
        private static List<Unit> myunits;
        // Update Version 1.0.0.14
        // > Fixed error static targets prediction(missing sunstrike on not moving targets)
        // > Fixed Combo sometimes wasn't working
        // > Fixed Combo canceling
        // Next updates:
        // - Fix tornado usage(I recommend to use tornado by yourself script is missing so much, if you want to use is better be closer so script propably will not miss.)
        // - Fix tornado on icewall
        // - ADD more combos
        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Combo Mode", "Combo Mode").SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Flee Mode", "Flee Mode").SetValue(new KeyBind('U', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Target Type: ", "Target Type: ").SetValue(new StringList(new[] { "Target Selector", "Closest to mouse" }))).SetTooltip("On target selector you can get a better position while comboing. but closest to mouse is more easier");
            if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0)
                Menu.AddItem(new MenuItem("Target Select", "Target Select").SetValue(new KeyBind('G', KeyBindType.Press)));


            var SunstrikeMenu = new Menu("Sunstrike", "AutoSunstrike");
            var UiSkillBar = new Menu("Ui SkillBar", "Ui SkillBar");
            Menu.AddSubMenu(UiSkillBar);
            UiSkillBar.AddItem(new MenuItem("Enable Skill UI Bar", "Enable Skill UI Bar").SetValue(true).SetTooltip("Enable/Disable UI information bar."));
            UiSkillBar.AddItem(new MenuItem("Box Size", "Box Size").SetValue(new Slider(25, 5, 300)).SetTooltip("Set The box size of Menu"));
            UiSkillBar.AddItem(new MenuItem("Box Position Y", "Box Position Y").SetValue(new Slider(Drawing.Height / 2, 0, Drawing.Height)).SetTooltip("Set the Y position of Menu"));
            UiSkillBar.AddItem(new MenuItem("Box Position X", "Box Position X").SetValue(new Slider(Drawing.Width / 2, 0, Drawing.Width)).SetTooltip("Set the X position of Menu"));


            var QuickcastSpells = new Menu("Quick Cast Spells", "Quick Cast Spells");

            Menu.AddSubMenu(QuickcastSpells);

            QuickcastSpells.AddItem(new MenuItem("Key +", "Key +").SetValue(new StringList(new[] { "None", "Shift", "Alt", "Ctrl" }))).SetTooltip("(Quick cast spell key) + (None,shift,Alt or Ctrl)");
            QuickcastSpells.AddItem(new MenuItem("Enable Quick Spells", "Enable Quick Spells").SetValue(true).SetTooltip("Enable/Disable instant cast spell using key. Reload script after set this option. Note: Quick spells doesn't mean quickcast. "));
            QuickcastSpells.AddItem(new MenuItem("ColdSnap", "Cold Snap").SetValue(new KeyBind('1', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Forge Spirit", "Forge Spirit").SetValue(new KeyBind('2', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Alacrity", "Alacrity").SetValue(new KeyBind('3', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Tornado", "Tornado").SetValue(new KeyBind('4', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Emp", "Emp").SetValue(new KeyBind('5', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Meteor", "Meteor").SetValue(new KeyBind('6', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("sunstrike", "sunstrike").SetValue(new KeyBind('7', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Icewall", "Icewall").SetValue(new KeyBind('8', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Ghostwalk", "Ghostwalk").SetValue(new KeyBind('9', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));
            QuickcastSpells.AddItem(new MenuItem("Defeaning blast", "Defeaning blast").SetValue(new KeyBind('0', KeyBindType.Press)).Show(Menu.Item("Enable Quick Spells").GetValue<bool>()));

            var orbmenu = new Menu("OrbChanging", "Orb Menu");
            Menu.AddSubMenu(SunstrikeMenu);
            Menu.AddSubMenu(orbmenu);

            orbmenu.AddItem(new MenuItem("Enable OrbChanging", "Enable OrbChanging").SetValue(true).SetTooltip("Enable/Disable automatic orb Changing."));
            orbmenu.AddItem(new MenuItem("quas threshold health", "Quas Threshold Health").SetValue(new Slider(90, 1, 100)).SetTooltip("Percentage of HP threshold to change orbs for quas while not attacking."));

            SunstrikeMenu.AddItem(new MenuItem("Enable AutoSunstrike", "Enable AutoSunstrike").SetValue(true));
            SunstrikeMenu.AddItem(new MenuItem("Sunstrike Onlywhensafe", "Sunstrike Onlywhensafe").SetValue(true).SetTooltip("Just send sunstrike when is safe (target stunned,euls timing, skills timing..)."));
            SunstrikeMenu.AddItem(new MenuItem("Sunstrike Always When Disabled", "Sunstrike Always When Disabled").SetValue(true).SetTooltip("It will always send sunstrike when target has minus health than minimum amount of health configured."));
            SunstrikeMenu.AddItem(new MenuItem("sunstrike.timeconfig", "Sunstrike Delay").SetValue(new Slider(600, 100, 3000)).SetTooltip("Wait enemy walk in straight line delay. It will make sunstrike more accurate."));
            SunstrikeMenu.AddItem(new MenuItem("sunstrike.timeconfig2", "First Vision Delay").SetValue(new Slider(2500, 500, 3000)).SetTooltip("Time to wait before start to calculate sunstrike.(it's good because when target lose his HP he may change the place where he is going.)"));
            SunstrikeMenu.AddItem(new MenuItem("Minimum amount of health for sunstrike", "Minimum amount of health for sunstrike").SetValue(new Slider(300, 100, 1000)).SetTooltip("This value will be summed with sunstrike damage value."));

            Menu.AddItem(new MenuItem("orbwalk.minDistance", "Orbwalk min distance").SetValue(new Slider(250, 0, 700)).SetTooltip("the min distance to stop orbwalking and just auto attack."));
            Menu.AddToMainMenu();

            Game.OnWndProc += Exploding;
            Drawing.OnDraw += Target_esp;
            Game.OnUpdate += orb_checker;
            Orbwalking.Load();
            starttickcount = Environment.TickCount;
        }
        public static void orb_checker(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame || Game.IsPaused)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Invoker)
                return;
            if (Game.IsKeyDown(Menu.Item("Flee Mode").GetValue<KeyBind>().Key) || Game.IsKeyDown(Menu.Item("Combo Mode").GetValue<KeyBind>().Key))
                return;
            Find_skillsAndItens();
            if (Menu.Item("Key +").GetValue<StringList>().SelectedIndex == 0)
                aditionalkey = 0;
            else if (Menu.Item("Key +").GetValue<StringList>().SelectedIndex == 1)
                aditionalkey = 16;
            else if (Menu.Item("Key +").GetValue<StringList>().SelectedIndex == 2)
                aditionalkey = 18;
            else if (Menu.Item("Key +").GetValue<StringList>().SelectedIndex == 3)
                aditionalkey = 17;
            if (Menu.Item("Enable Quick Spells").GetValue<bool>() && Utils.SleepCheck("skillcasted") && (aditionalkey > 0 ? Game.IsKeyDown(aditionalkey) : true))
            {
                if (Game.IsKeyDown(Menu.Item("ColdSnap").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(coldsnap, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Forge Spirit").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(forgespirit, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Alacrity").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(alacrity, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Tornado").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(tornado, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Emp").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(emp, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Meteor").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(meteor, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("sunstrike").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(sunstrike, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Icewall").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(icewall, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Ghostwalk").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(ghostwalk, false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Defeaning blast").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    InvokeSkill(blast, false);
                    Utils.Sleep(500, "skillcasted");
                }
            }
            level_checker();
            if (me.CanCast() && !me.IsChanneling() && !me.UnitState.HasFlag(UnitState.Invisible) && Utils.SleepCheck("KEYPRESSED") && Menu.Item("Enable OrbChanging").GetValue<bool>())
            {
                if ((me.NetworkActivity.HasFlag(NetworkActivity.Attack) || me.NetworkActivity.HasFlag(NetworkActivity.Attack2) || me.NetworkActivity.HasFlag(NetworkActivity.AttackEvent)) && ((me.Modifiers.Count(x => x.Name.Contains("exort")) < 4 && exort_level) || (me.Modifiers.Count(x => x.Name.Contains("wex")) < 3 && wex_level)) && Utils.SleepCheck("orbchange"))
                {
                    if (exort_level)
                    {
                        orb_type(exort);
                        Utils.Sleep(400, "orbchange");
                    }
                    else if (wex_level)
                    {
                        orb_type(wex);
                        Utils.Sleep(400, "orbchange");
                    }
                    Utils.Sleep(900, "orbchange2");
                }
                else if (me.Health < me.MaximumHealth * ((float)quasthreshold / 100) && me.Modifiers.Count(x => x.Name.Contains("quas")) < 4 && quas_level && Utils.SleepCheck("orbchange") && Utils.SleepCheck("orbchange2"))
                {
                    orb_type(quas);
                    Utils.Sleep(400, "orbchange");
                }
                else if (me.Health >= me.MaximumHealth * ((float)quasthreshold / 100) && wex_level && me.Modifiers.Count(x => x.Name.Contains("wex")) < 4 && Utils.SleepCheck("orbchange") && Utils.SleepCheck("orbchange2"))
                {
                    orb_type(wex);
                    Utils.Sleep(400, "orbchange");
                }
                if (me.NetworkActivity.HasFlag(NetworkActivity.Attack) || me.NetworkActivity.HasFlag(NetworkActivity.Attack2) || me.NetworkActivity.HasFlag(NetworkActivity.AttackEvent))
                    Utils.Sleep(900, "orbchange2");
            }
            if (Menu.Item("Enable AutoSunstrike").GetValue<bool>() && !me.IsChanneling() && !me.IsInvisible())
            {
                currenttickcount = Environment.TickCount - starttickcount;
                var Sunstrikedamage = 100 + ((exort.Level - 1) * 62.5);
                if (me.AghanimState())
                    Sunstrikedamage += 62.5;
                EnemykillablebySS = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.IsValid && x.Team != me.Team && !x.IsIllusion && x.IsAlive && x.Health <= Sunstrikedamage);
                DisabledEnemy = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.IsValid && x.Team != me.Team && !x.IsIllusion && x.IsAlive && IsOnTiming(sunstrike, x) && !x.HasModifier("modifier_invoker_cold_snap") && x.Health <= Sunstrikedamage + SunstrikeMinHP);
                if (((EnemykillablebySS != null || (DisabledEnemy != null && Menu.Item("Sunstrike Always When Disabled").GetValue<bool>())) && sunstrike != null && sunstrike.Cooldown == 0 && exort.Level > 0))
                {
                    if (EnemykillablebySS == null)
                        EnemykillablebySS = DisabledEnemy;
                    if (IsOnTiming(sunstrike, EnemykillablebySS))
                    {
                        if (sunstrike.Cooldown == 0 && (Iscasted(sunstrike) ? me.Mana > sunstrike.ManaCost : me.Mana > (sunstrike.ManaCost + invoke.ManaCost)) && invoke.CanBeCasted() && Utils.SleepCheck("cd_sunstrike"))
                        {
                            InvokeSkill(sunstrike, true);
                            if (EnemykillablebySS.HasModifier("modifier_invoker_cold_snap"))
                                sunstrike.UseAbility(Prediction.PredictedXYZ(EnemykillablebySS, (float)(1700 / 3.4) + EnemykillablebySS.MovementSpeed), false);
                            else
                                sunstrike.UseAbility(EnemykillablebySS.NetworkPosition, false);
                            Utils.Sleep(250, "cd_sunstrike");
                            Utils.Sleep(700, "cd_sunstrike_a");
                        }
                    }
                    else if (!Menu.Item("Sunstrike Onlywhensafe").GetValue<bool>())
                    {
                        if (sunstrike.Cooldown == 0 && (Iscasted(sunstrike) ? me.Mana > sunstrike.ManaCost : me.Mana > (sunstrike.ManaCost + invoke.ManaCost)) && invoke.CanBeCasted() && Utils.SleepCheck("cd_sunstrike") && TargetIsTurning(EnemykillablebySS) && !EnemykillablebySS.IsInvul())
                        {
                            InvokeSkill(sunstrike, true);
                            sunstrike.UseAbility(EnemykillablebySS.NetworkPosition, false);
                            Utils.Sleep(250, "cd_sunstrike");
                            Utils.Sleep(700, "cd_sunstrike_a");
                        }
                    }
                }
            }
        }
        public static void Target_esp(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Invoker)
                return;
            if (targetParticle == null && target != null)
            {
                targetParticle = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
            }
            if ((target == null || !target.IsVisible || !target.IsAlive) && targetParticle != null)
            {
                targetParticle.Dispose();
                targetParticle = null;
            }
            if (target != null && targetParticle != null)
            {
                targetParticle.SetControlPoint(2, me.Position);
                targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
                targetParticle.SetControlPoint(7, target.Position);
            }
            if (Menu.Item("Enable Skill UI Bar").GetValue<bool>())
            {
                int i = 0;
                foreach (Ability spells in me.Spellbook.Spells)
                {
                    if (spells == null) continue;
                    if (spells.Name.Contains("empty")) continue;
                    if (spells.Name.Contains("quas")) continue;
                    if (spells.Name.Contains("wex")) continue;
                    if (spells.Name.Contains("exort")) continue;
                    if (spells.Name == "invoker_invoke") continue;
                    Drawing.DrawRect(new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/" + spells.Name + ".vmat"));
                    Drawing.DrawRect(new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.Black, true);
                    if (spells.Cooldown > 0)
                        Drawing.DrawRect(new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(0x8B, 0x00, 0x00, 0x9E), false);
                    if (me.Mana < spells.ManaCost)
                        Drawing.DrawRect(new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), new Color(0x5F, 0x00, 0xCE, 0x9E), false);
                    if (Menu.Item("Enable Quick Spells").GetValue<bool>())
                    {
                        if (spells.Name.Contains("cold_snap"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("ColdSnap").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("forge"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Forge Spirit").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("alacrity"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Alacrity").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("tornado"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Tornado").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("emp"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Emp").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("meteor"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Meteor").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("sun_strike"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("sunstrike").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("ice_wall"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Icewall").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("ghost_walk"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Ghostwalk").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                        if (spells.Name.Contains("deafening_blast"))
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Defeaning blast").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + i, boxY), new Vector2(boxsize, boxsize), Color.White, FontFlags.None);
                    }
                    i += boxsize;
                }
            }
        }
        public static void Exploding(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame || Game.IsPaused)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Invoker)
                return;
            for (uint i = 48; i <= 90; i++)
            {
                if (Game.IsKeyDown(keyCode: i))
                {
                    Utils.Sleep(1000, "KEYPRESSED");
                }
            }
            if (Game.IsKeyDown(Menu.Item("Flee Mode").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                Find_skillsAndItens();
                if (me.CanMove() || me.CanCast())
                {
                    nextskillflee = NextSkillFlee();
                    if (nextskillflee == 1)
                    {
                        if (!Iscasted(ghostwalk) && invoke.CanBeCasted() && Utils.SleepCheck("ghostcast"))
                        {
                            InvokeSkill(ghostwalk, true);
                            Utils.Sleep(250, "ghostcast");
                        }
                        if (!me.HasModifier("modifier_invoker_ghost_walk_self") && !me.IsInvisible() && Utils.SleepCheck("Ghostwalk_usage"))
                        {
                            if ((me.Health / (float)me.MaximumHealth) <= 0.5 && Utils.SleepCheck("Ghostwalk_usage"))
                            {
                                if (me.Modifiers.Count(x => x.Name.Contains("quas")) < 4 && quas.Level > 0)
                                {
                                    orb_type(quas);
                                    Utils.Sleep(250, "ORBGHOST");
                                }
                            }
                            else
                            {
                                if (me.Modifiers.Count(x => x.Name.Contains("wex")) < 4 && wex.Level > 0 && Utils.SleepCheck("Ghostwalk_usage"))
                                {
                                    orb_type(wex);
                                    Utils.Sleep(250, "ORBGHOST");
                                }
                            }
                            if (Iscasted(ghostwalk) && Utils.SleepCheck("ORBGHOST"))
                            {
                                ghostwalk.UseAbility(false);
                                Utils.Sleep(750, "Ghostwalk_usage");
                            }
                        }
                    }
                    if (Utils.SleepCheck("movingnow"))
                    {
                        me.Move(Game.MousePosition, false);
                        Utils.Sleep(300, "movingnow");
                    }
                }
            }
            if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0 && Game.IsKeyDown(Menu.Item("Target Select").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                target = me.ClosestToMouseTarget(1000);
                SaveSelectedTargetName = target.Name;
            }
            if (Game.IsKeyDown(Menu.Item("Combo Mode").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                Find_skillsAndItens();
                if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0)
                {
                    target = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.IsAlive && !x.IsIllusion && x.Distance2D(me) < 3000 && x.IsVisible && x.Name == SaveSelectedTargetName);
                    if (target == null)
                        target = me.BestAATarget(1000);
                }
                else if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 1)
                    target = me.ClosestToMouseTarget(1000);
                //Console.WriteLine(target.Modifiers.LastOrDefault().Name);
                if (target != null && target.IsValid && !target.IsIllusion)
                {
                    if (Utils.SleepCheck("Variable Checker"))
                    {
                        distance_me_target = target.NetworkPosition.Distance2D(me.NetworkPosition);
                        myunits = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.IsControllable && x != null && x.IsAlive && x.Distance2D(target) <= 2000 && x.IsControllable && x.IsValid).ToList();
                        forge_in_my_side = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.IsControllable && x != null && x.IsAlive && x.Distance2D(target) <= 700 && x.Name.Contains("npc_dota_invoker_forged_spirit") && x.IsValid).Any();
                        ice_wall_distance = me.InFront(120).Distance2D(target.Position) <= 300;
                        target_magic_imune = target.IsMagicImmune();
                        target_isinvul = target.IsInvul();
                        target_blast_ontiming = IsOnTiming(blast, null);
                        target_meteor_ontiming = IsOnTiming(meteor, null);
                        target_emp_ontiming = IsOnTiming(emp, null);
                        target_sunstrike_ontiming = IsOnTiming(sunstrike, null);
                        Utils.Sleep(200, "Variable Checker");
                    }
                    level_checker();
                    if ((quas.Level > 0 || wex.Level > 0 || exort.Level > 0) && invoke.Level >= 1)
                    {
                        if (IsComboPrepared() != 0 || comboing)
                        {
                            // combo 1 - > euls -> meteor -> sunstrike -> defineblast ou coldsnap
                            // //combo 2 -> emp -> tornado -> cold snap
                            // //combo 2 -> tornado -> emp -> meteor -> cold snap or defeaning blast (requires a ultimate with 4 seconds of cd or less)
                            // //Combo 3 -> tornado -> meteor -> sunstrike -> colds snap or defeaning blast (requires a ultimate with 4 seconds of cd or less)
                            // //Combo 3 -> tornado -> meteor -> cold snap or deafeaning blast(don't require low cd invoke)
                            if (!comboing)
                            {
                                comboing = true;
                                combonumber = IsComboPrepared();
                                Utils.Sleep(5000, "combotime");
                            }
                            if (Utils.SleepCheck("combotime"))
                            {
                                comboing = false;
                                refresher_use = false;
                            }
                            if (combonumber == 1)
                            {
                                AttackTarget();
                                if (eul.CanBeCasted() && Utils.SleepCheck("eul") && sunstrike.Cooldown == 0 && meteor.Cooldown == 0 && !target_meteor_ontiming && !target_sunstrike_ontiming)
                                {
                                    eul.UseAbility(target, false);
                                    Utils.Sleep(500, "eul");
                                }
                                if (meteor.Cooldown == 0)
                                {
                                    InvokeSkill(meteor, true);
                                    if (meteor.CanBeCasted() && Utils.SleepCheck("cd_meteor") && target_meteor_ontiming && Utils.SleepCheck("eul"))
                                    {
                                        meteor.UseAbility(target.NetworkPosition, false);
                                        Utils.Sleep(250, "cd_meteor");
                                        Utils.Sleep(250, "cd_meteor_a");
                                    }
                                }
                                if (sunstrike.Cooldown == 0)
                                {
                                    InvokeSkill(sunstrike, true);
                                    if (sunstrike.CanBeCasted() && Utils.SleepCheck("cd_sunstrike") && target_sunstrike_ontiming && Utils.SleepCheck("eul"))
                                    {
                                        sunstrike.UseAbility(target.NetworkPosition, false);
                                        Utils.Sleep(250, "cd_sunstrike");
                                        Utils.Sleep(700, "cd_sunstrike_a");
                                    }
                                }
                                if (blast.Cooldown == 0 && sunstrike.Cooldown > 0 && meteor.Cooldown > 0)
                                {
                                    InvokeSkill(blast, true);
                                    if (blast.CanBeCasted() && distance_me_target <= 900 && Utils.SleepCheck("cd_blast") && target_blast_ontiming && Utils.SleepCheck("eul"))
                                    {
                                        blast.UseAbility(target.NetworkPosition, false);
                                        comboing = false;
                                        Utils.Sleep(250, "cd_blast");
                                        Utils.Sleep(800, "cd_blast_a");
                                    }
                                }
                            }
                            if (combonumber == 2)
                            {
                                bool invokecd = ((invoke.Level == 4 && me.AghanimState()) || ((invoke.Level >= 3 && me.FindItem("item_octarine_core") != null) && me.AghanimState()));
                                if (refresher_use == false)
                                {
                                    AttackTarget();
                                    if (tornado.Cooldown == 0)
                                    {
                                        InvokeSkill(tornado, true);
                                        if (tornado.CanBeCasted() && Utils.SleepCheck("cd_tornado"))
                                        {
                                            tornado.UseAbility(Prediction.PredictedXYZ(target, (distance_me_target / 1100 * 1000) + target.MovementSpeed), false);
                                            Utils.Sleep(250, "cd_tornado");
                                            Utils.Sleep(((distance_me_target / 1000) * 1000) + 300, "cd_tornado_a");
                                            Utils.Sleep(((distance_me_target / 1000) * 1000) + 300, "combotime");
                                        }
                                    }
                                    if (emp.Cooldown == 0)
                                    {
                                        InvokeSkill(emp, true);
                                        if (emp.CanBeCasted() && target_emp_ontiming && Utils.SleepCheck("cd_emp") && Utils.SleepCheck("cd_tornado_a"))
                                        {
                                            emp.UseAbility(target.NetworkPosition, false);
                                            Utils.Sleep(250, "cd_emp");
                                            Utils.Sleep(1000, "cd_emp_a");
                                        }
                                    }
                                    if (invokecd && exort_level)
                                    {
                                        Use_Item("ALL");
                                        if (meteor.Cooldown == 0 && emp.Cooldown > 0 && tornado.Cooldown > 0)
                                        {
                                            InvokeSkill(meteor, true);
                                            if (meteor.CanBeCasted() && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp") && Utils.SleepCheck("cd_meteor") && target_meteor_ontiming)
                                            {
                                                meteor.UseAbility(target.NetworkPosition, false);
                                                Utils.Sleep(250, "cd_meteor");
                                                Utils.Sleep(250, "cd_meteor_a");
                                            }
                                        }
                                        if (blast.Cooldown > 0 && coldsnap.Cooldown == 0 && emp.Cooldown > 0 && tornado.Cooldown > 0)
                                        {
                                            InvokeSkill(coldsnap, true);
                                            if (coldsnap.CanBeCasted() && distance_me_target <= 750 && !target_isinvul && !target_magic_imune && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp"))
                                            {
                                                coldsnap.UseAbility(target, false);
                                                comboing = false;
                                                Utils.Sleep(250, "cd_coldsnap");
                                            }
                                        }
                                        if (blast.Cooldown == 0 && emp.Cooldown > 0 && tornado.Cooldown > 0)
                                        {
                                            if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0)
                                            {
                                                InvokeSkill(blast, true);
                                                if (blast.CanBeCasted() && distance_me_target <= 900 && (target_isinvul ? target_blast_ontiming : true) && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_blast"))
                                                {
                                                    blast.UseAbility(target.NetworkPosition, false);
                                                    Utils.Sleep(250, "cd_blast");
                                                    Utils.Sleep(800, "cd_blast_a");
                                                }
                                            }
                                        }
                                    }
                                    if (blast.Cooldown > 0 && meteor.Cooldown > 0 && Utils.SleepCheck("refreshertimer") && (dagon == null || !dagon.CanBeCasted()) && (ethereal == null || !ethereal.CanBeCasted()))
                                    {
                                        if ((refresher == null || !refresher.CanBeCasted() || me.Mana < refresher.ManaCost + meteor.ManaCost + blast.ManaCost))
                                            comboing = false;
                                        else
                                        {
                                            refresher_use = true;
                                            Utils.Sleep(5000, "combotime");
                                        }
                                        Utils.Sleep(2500, "refreshertimer");
                                    }
                                    else
                                    {
                                        if (coldsnap.Cooldown == 0 && emp.Cooldown > 0 && tornado.Cooldown > 0)
                                        {
                                            InvokeSkill(coldsnap, true);
                                            if (coldsnap.CanBeCasted() && distance_me_target <= 750 && !target_isinvul && !target_magic_imune && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp"))
                                            {
                                                coldsnap.UseAbility(target, false);
                                                comboing = false;
                                                Utils.Sleep(250, "cd_coldsnap");
                                            }
                                        }
                                        if (coldsnap.Cooldown > 0 && emp.Cooldown > 0 && tornado.Cooldown > 0)
                                        {
                                            if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0)
                                            {
                                                InvokeSkill(blast, true);
                                                if (blast.CanBeCasted() && distance_me_target <= 900 && target_blast_ontiming && Utils.SleepCheck("cd_blast") && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp"))
                                                {
                                                    blast.UseAbility(target.NetworkPosition, false);
                                                    comboing = false;
                                                    Utils.Sleep(250, "cd_blast");
                                                    Utils.Sleep(800, "cd_blast_a");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (refresher != null && refresher.CanBeCasted() && Utils.SleepCheck("Refresher usage") && meteor.Cooldown > 0 && blast.Cooldown > 0)
                                    {
                                        Utils.Sleep(5000, "combotime");
                                        refresher.UseAbility(false);
                                        Utils.Sleep(250, "Refresher usage");
                                    }
                                    Use_Item("ALL");
                                    if (meteor.Cooldown == 0)
                                    {
                                        InvokeSkill(meteor, true);
                                        if (meteor.CanBeCasted() && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_meteor"))
                                        {
                                            meteor.UseAbility(target.NetworkPosition, false);
                                            Utils.Sleep(250, "cd_meteor");
                                            Utils.Sleep(250, "cd_meteor_a");
                                        }
                                    }
                                    if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0 && meteor.Cooldown > 0 && (dagon == null || !dagon.CanBeCasted()) && (ethereal == null || !ethereal.CanBeCasted()))
                                    {
                                        InvokeSkill(blast, true);
                                        if (blast.CanBeCasted() && distance_me_target <= 900 && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_blast"))
                                        {
                                            blast.UseAbility(target.NetworkPosition, false);
                                            comboing = false;
                                            refresher_use = false;
                                            Utils.Sleep(250, "cd_blast");
                                            Utils.Sleep(800, "cd_blast_a");
                                        }
                                    }
                                }
                            }
                            if (combonumber == 3)
                            {
                                bool invokecd = ((invoke.Level == 4 && me.AghanimState()) || ((invoke.Level >= 3 && me.FindItem("item_octarine_core") != null) && me.AghanimState()));
                                if (refresher_use == false)
                                {
                                    AttackTarget();
                                    if (tornado.Cooldown == 0)
                                    {
                                        InvokeSkill(tornado, true);
                                        if (tornado.CanBeCasted() && Utils.SleepCheck("cd_tornado"))
                                        {
                                            tornado.UseAbility(Prediction.PredictedXYZ(target, (distance_me_target / 1100 * 1000) + target.MovementSpeed), false);
                                            Utils.Sleep(250, "cd_tornado");
                                            Utils.Sleep(((distance_me_target / 1000) * 1000) + 300, "cd_tornado_a");
                                            Utils.Sleep(((distance_me_target / 1000) * 1000) + 300, "combotime");
                                        }
                                    }
                                    if (meteor.Cooldown == 0)
                                    {
                                        InvokeSkill(meteor, true);
                                        if (meteor.CanBeCasted() && Utils.SleepCheck("cd_meteor") && target_meteor_ontiming && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_emp"))
                                        {
                                            meteor.UseAbility(target.NetworkPosition, false);
                                            Utils.Sleep(250, "cd_meteor");
                                            Utils.Sleep(250, "cd_meteor_a");
                                        }
                                    }
                                    if (invokecd && exort_level)
                                    {
                                        if (sunstrike.Cooldown == 0 && tornado.Cooldown > 0)
                                        {
                                            InvokeSkill(sunstrike, true);
                                            if (sunstrike.CanBeCasted() && Utils.SleepCheck("cd_sunstrike") && target_sunstrike_ontiming && Utils.SleepCheck("cd_tornado_a"))
                                            {
                                                sunstrike.UseAbility(target.NetworkPosition, false);
                                                Utils.Sleep(250, "cd_sunstrike");
                                                Utils.Sleep(5000, "combotime");
                                                Utils.Sleep(700, "cd_sunstrike_a");
                                            }
                                        }
                                        if (blast.Cooldown > 0 && coldsnap.Cooldown == 0 && tornado.Cooldown > 0 && sunstrike.Cooldown > 0 && meteor.Cooldown > 0 && Utils.SleepCheck("cd_blast_a"))
                                        {
                                            InvokeSkill(coldsnap, true);
                                            if (coldsnap.CanBeCasted() && distance_me_target <= 900 && !target_isinvul && !target_magic_imune && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap"))
                                            {
                                                coldsnap.UseAbility(target, false);
                                                comboing = false;
                                                Utils.Sleep(250, "cd_coldsnap");
                                            }
                                        }
                                        if (blast.Cooldown == 0 && tornado.Cooldown > 0 && tornado.Cooldown > 0 && sunstrike.Cooldown > 0 && meteor.Cooldown > 0)
                                        {
                                            if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0 && Utils.SleepCheck("cd_blast") && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp"))
                                            {
                                                InvokeSkill(blast, true);
                                                if (blast.CanBeCasted() && distance_me_target <= 900 && target_blast_ontiming)
                                                {
                                                    blast.UseAbility(target.NetworkPosition, false);
                                                    Utils.Sleep(250, "cd_blast");
                                                    Utils.Sleep(800, "cd_blast_a");
                                                }
                                            }
                                        }
                                        Use_Item("ALL");
                                        if (blast.Cooldown > 0 && sunstrike.Cooldown > 0 && Utils.SleepCheck("refreshertimer") && (dagon == null || !dagon.CanBeCasted()) && (ethereal == null || !ethereal.CanBeCasted()))
                                        {
                                            if ((refresher == null || !refresher.CanBeCasted() || me.Mana < refresher.ManaCost + sunstrike.ManaCost + blast.ManaCost))
                                                comboing = false;
                                            else
                                            {
                                                refresher_use = true;
                                                Utils.Sleep(5000, "combotime");
                                            }
                                            Utils.Sleep(2500, "refreshertimer");
                                        }
                                    }
                                    else
                                    {
                                        if (blast.Cooldown > 0 && coldsnap.Cooldown == 0 && tornado.Cooldown > 0 && meteor.Cooldown > 0)
                                        {
                                            InvokeSkill(coldsnap, true);
                                            if (coldsnap.CanBeCasted() && distance_me_target <= 900 && !target_isinvul && !target_magic_imune && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap"))
                                            {
                                                coldsnap.UseAbility(target, false);
                                                comboing = false;
                                                Utils.Sleep(250, "cd_coldsnap");
                                            }
                                        }
                                        if (blast.Cooldown == 0 && tornado.Cooldown > 0 && meteor.Cooldown > 0)
                                        {
                                            if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0 && Utils.SleepCheck("cd_blast") && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_coldsnap") && Utils.SleepCheck("cd_emp"))
                                            {
                                                InvokeSkill(blast, true);
                                                if (blast.CanBeCasted() && distance_me_target <= 900 && target_blast_ontiming)
                                                {
                                                    blast.UseAbility(target.NetworkPosition, false);
                                                    comboing = false;
                                                    Utils.Sleep(250, "cd_blast");
                                                    Utils.Sleep(800, "cd_blast_a");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (refresher != null && refresher.CanBeCasted() && Utils.SleepCheck("Refresher usage") && sunstrike.Cooldown > 0 && blast.Cooldown > 0)
                                    {
                                        Utils.Sleep(5000, "combotime");
                                        refresher.UseAbility(false);
                                        Utils.Sleep(250, "Refresher usage");
                                    }
                                    Use_Item("ALL");
                                    if (sunstrike.Cooldown == 0)
                                    {
                                        InvokeSkill(sunstrike, true);
                                        if (sunstrike.CanBeCasted() && Utils.SleepCheck("cd_sunstrike") && Utils.SleepCheck("cd_tornado_a"))
                                        {
                                            sunstrike.UseAbility(new Vector3(me.NetworkPosition.X + (distance_me_target + 200) * (float)Math.Cos(me.NetworkPosition.ToVector2().FindAngleBetween(target.NetworkPosition.ToVector2(), true)), me.NetworkPosition.Y + (distance_me_target + 200) * (float)Math.Sin(me.NetworkPosition.ToVector2().FindAngleBetween(target.NetworkPosition.ToVector2(), true)), 100), false);
                                            Utils.Sleep(250, "cd_sunstrike");
                                            Utils.Sleep(700, "cd_sunstrike_a");
                                        }
                                    }
                                    if (blast.Cooldown == 0 && quas.Level > 0 && wex.Level > 0 && exort.Level > 0 && sunstrike.Cooldown > 0 && (dagon == null || !dagon.CanBeCasted()) && (ethereal == null || !ethereal.CanBeCasted()))
                                    {
                                        InvokeSkill(blast, true);
                                        if (blast.CanBeCasted() && distance_me_target <= 900 && Utils.SleepCheck("cd_tornado_a") && Utils.SleepCheck("cd_blast"))
                                        {
                                            blast.UseAbility(target.NetworkPosition, false);
                                            comboing = false;
                                            refresher_use = false;
                                            Utils.Sleep(250, "cd_blast");
                                            Utils.Sleep(800, "cd_blast_a");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (NextSkill() != nextskillvalue)
                            {
                                nextskillvalue = NextSkill();
                            }
                            if (nextskillvalue != 0)
                            {
                                // 1 - coldsnap, 2- meteor, 3 - alacrity, 4 - tornado, 5 -forgespirit, 6 -blast, 7 - sunstrike, 8 - emp, 9 - icewall, 10 - sunstrike
                                if (nextskillvalue == 1 && Utils.SleepCheck("cd_coldsnap"))
                                {
                                    InvokeSkill(coldsnap, true);
                                    if (coldsnap.CanBeCasted())
                                    {
                                        coldsnap.UseAbility(target, false);
                                        Utils.Sleep(250, "cd_coldsnap");
                                    }
                                }
                                if (nextskillvalue == 2 && Utils.SleepCheck("cd_meteor"))
                                {
                                    InvokeSkill(meteor, true);
                                    if (meteor.CanBeCasted())
                                    {
                                        meteor.UseAbility(Prediction.PredictedXYZ(target, 1300 / 5 + target.MovementSpeed), false);
                                        Utils.Sleep(250, "cd_meteor");
                                        Utils.Sleep(250, "cd_meteor_a");
                                    }
                                }
                                if (nextskillvalue == 3 && Utils.SleepCheck("cd_alacrity"))
                                {
                                    InvokeSkill(alacrity, true);
                                    if (alacrity.CanBeCasted())
                                    {
                                        alacrity.UseAbility(me, false);
                                        Utils.Sleep(250, "cd_alacrity");
                                    }
                                }
                                if (nextskillvalue == 4 && Utils.SleepCheck("cd_tornado"))
                                {
                                    InvokeSkill(tornado, true);
                                    if (tornado.CanBeCasted())
                                    {
                                        tornado.UseAbility(Prediction.PredictedXYZ(target, (distance_me_target / 1100 * 1000) + target.MovementSpeed), false);
                                        Utils.Sleep(250, "cd_tornado");
                                        Utils.Sleep(((distance_me_target / 1000) * 1000) + 300, "cd_tornado_a");
                                    }
                                }
                                if (nextskillvalue == 5 && Utils.SleepCheck("cd_forgespirit"))
                                {
                                    InvokeSkill(forgespirit, true);
                                    if (forgespirit.CanBeCasted())
                                    {
                                        forgespirit.UseAbility(false);
                                        Utils.Sleep(250, "cd_forgespirit");
                                    }
                                }
                                if (nextskillvalue == 6 && Utils.SleepCheck("cd_blast"))
                                {
                                    InvokeSkill(blast, true);
                                    if (blast.CanBeCasted())
                                    {
                                        blast.UseAbility(Prediction.PredictedXYZ(target, (distance_me_target / 1100 * 1000) + target.MovementSpeed), false);
                                        Utils.Sleep(250, "cd_blast");
                                        Utils.Sleep(800, "cd_blast_a");
                                    }
                                }
                                if (nextskillvalue == 8 && Utils.SleepCheck("cd_emp"))
                                {
                                    InvokeSkill(emp, true);
                                    if (emp.CanBeCasted())
                                    {
                                        emp.UseAbility(Prediction.PredictedXYZ(target, 1700 / 3 + target.MovementSpeed), false);
                                        Utils.Sleep(250, "cd_emp");
                                        Utils.Sleep(1000, "cd_emp_a");
                                    }
                                }
                                if (nextskillvalue == 9 && Utils.SleepCheck("cd_icewall"))
                                {
                                    InvokeSkill(icewall, true);
                                    if (icewall.CanBeCasted())
                                    {
                                        icewall.UseAbility(false);
                                        Utils.Sleep(250, "cd_icewall");
                                    }
                                }
                                if (nextskillvalue == 10 && Utils.SleepCheck("cd_sunstrike"))
                                {
                                    InvokeSkill(sunstrike, true);
                                    if (sunstrike.CanBeCasted())
                                    {
                                        sunstrike.UseAbility(Prediction.PredictedXYZ(target, 1700 / 4 + target.MovementSpeed), false);
                                        Utils.Sleep(250, "cd_sunstrike");
                                        Utils.Sleep(700, "cd_sunstrike_a");
                                    }
                                }
                                if (nextskillvalue == 0 && Utils.SleepCheck("moving_idle"))
                                {
                                    me.Move(Game.MousePosition, false);
                                    Utils.Sleep(300, "moving_idle");
                                }
                            }
                        }
                    }
                    //itens
                    Use_Item(medallion);
                    Use_Item(solar_crest);
                    Use_Item(malevolence);
                    Use_Item(vyse);
                    Use_Item(bloodthorn);
                    Use_Item(urn);
                    Use_Item(medallion);
                    AttackTarget();
                    if (myunits != null)
                    {
                        foreach (Unit unit in myunits)
                        {
                            if (unit == null && unit.IsAlive) continue;
                            if (unit.Name.Contains("necronomicon") || unit.Name.Contains("npc_dota_invoker_forged_spirit"))
                            {
                                Ability spell;
                                if (Utils.SleepCheck("attack" + unit.Handle))
                                {
                                    unit.Attack(target);
                                    if (unit.Name.Contains("npc_dota_necronomicon_archer"))
                                    {
                                        spell = unit.Spellbook.Spell1;
                                        if (spell != null && spell.CanBeCasted(target))
                                            spell.UseAbility(target);
                                    }
                                    Utils.Sleep(1000, "attack" + unit.Handle);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Utils.SleepCheck("moving_idle"))
                    {
                        if (me.Modifiers.Count(x => x.Name.Contains("wex")) < 4 && Utils.SleepCheck("orbchange"))
                        {
                            orb_type(wex);
                            Utils.Sleep(900, "orbchange");
                        }
                        me.Move(Game.MousePosition, false);
                        Utils.Sleep(300, "moving_idle");
                    }
                }
            }
        }
        private static uint IsComboPrepared()
        {
            if (Iscasted(meteor) && (Iscasted(sunstrike) || !Utils.SleepCheck("cd_sunstrike_a")) && (sunstrike.Cooldown == 0 || !Utils.SleepCheck("cd_sunstrike_a")) && meteor.Cooldown == 0 && (me.FindItem("item_cyclone") != null || (target_meteor_ontiming && target_sunstrike_ontiming)))
                return 1;
            if (Iscasted(emp) && ((Iscasted(tornado) && tornado.Cooldown <= 5) || target.IsInvul()) && emp.Cooldown <= 5)
                return 2;
            if (Iscasted(meteor) && ((Iscasted(tornado) && tornado.Cooldown <= 5 ) || target.IsInvul()) && meteor.Cooldown <= 5)
                return 3;
            return 0;
        }
        private static bool IsOnTiming(Ability skill, Hero Enemy)
        {
            if (Enemy == null)
                Enemy = target;
            Single distance_me_target_2 = Enemy.NetworkPosition.Distance2D(me.NetworkPosition);
            double timing = 10, timing_a = 0.4;
            if (skill.Name == sunstrike.Name)
            {
                timing = 1.7 + (Game.Ping / 1000) - me.GetTurnTime(Enemy);
                timing_a = 1.1;
            }
            else if (skill.Name == meteor.Name)
            {
                timing = 1.5 + (Game.Ping / 1000) - me.GetTurnTime(Enemy);
                timing_a = 0.8;
            }
            else if (skill.Name == emp.Name)
            {
                timing = 2.9 + (Game.Ping / 1000) - me.GetTurnTime(Enemy);
                timing_a = 1;
            }
            else if (skill.Name == blast.Name)
            {
                timing = (distance_me_target_2 / 1100) - me.GetTurnTime(Enemy);
                timing_a = 0;
            }
            else
                return false;
            var modifierInvokerDeafiningBlastKnockback = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_invoker_deafining_blast_knockback");
            var modifierInvokerTornado = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_invoker_tornado");
            var modifierEulCyclone = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_eul_cyclone");
            var modifierObsidianDestroyerAstralImprisonmentPrison = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison");
            var modifierInvokerColdSnap = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_invoker_cold_snap");
            var modifierStunned = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_stunned");
            var modifierWindrunnerShackleShot = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_windrunner_shackle_shot");
            var modifierShadowDemonDisruption = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_shadow_demon_disruption");
            var modifierPudgeDismember = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_pudge_dismember");
            var modifierlegionduel = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_legion_commander_duel");
            var modifierbaneult = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_bane_fiends_grip");
            var modifieraxeberserkerscall = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_axe_berserkers_call");
            var modifiershackles = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_shadow_shaman_shackles");
            var modifiercrono = Enemy.Modifiers.FirstOrDefault(x => x.Name == "modifier_faceless_void_chronosphere_freeze");
            if (Enemy.HasModifier("modifier_invoker_deafining_blast_knockback")
                && modifierInvokerDeafiningBlastKnockback != null
                && modifierInvokerDeafiningBlastKnockback.RemainingTime <= timing
                && modifierInvokerDeafiningBlastKnockback.RemainingTime >= timing_a)
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_invoker_tornado") && modifierInvokerTornado != null
                && modifierInvokerTornado.RemainingTime <= timing
                && modifierInvokerTornado.RemainingTime >= timing_a)
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_eul_cyclone") && modifierEulCyclone != null
                && modifierEulCyclone.RemainingTime <= timing
                && modifierEulCyclone.RemainingTime >= timing_a)
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                && modifierObsidianDestroyerAstralImprisonmentPrison != null
                && modifierObsidianDestroyerAstralImprisonmentPrison.RemainingTime <= timing
                && modifierObsidianDestroyerAstralImprisonmentPrison.RemainingTime > timing_a)
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_pudge_dismember")
                && modifierPudgeDismember != null
                && modifierPudgeDismember.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_legion_commander_duel")
                && modifierlegionduel != null
                && modifierlegionduel.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_bane_fiends_grip")
                && modifierbaneult != null
                && modifierbaneult.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_shadow_shaman_shackles")
                && modifiershackles != null
                && modifiershackles.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_axe_berserkers_call")
                && modifieraxeberserkerscall != null
                && modifieraxeberserkerscall.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_faceless_void_chronosphere_freeze")
                && modifiercrono != null
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_invoker_cold_snap")
                && modifierInvokerColdSnap != null
                && modifierInvokerColdSnap.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_stunned") && modifierStunned != null
                && modifierStunned.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            if (Enemy.HasModifier("modifier_windrunner_shackle_shot")
                && modifierWindrunnerShackleShot != null
                && modifierWindrunnerShackleShot.RemainingTime >= timing_a
                && !Enemy.IsInvul())
            {
                return true;
            }
            return Enemy.HasModifier("modifier_shadow_demon_disruption")
                   && modifierShadowDemonDisruption != null
                   && modifierShadowDemonDisruption.RemainingTime <= timing
                   && modifierShadowDemonDisruption.RemainingTime >= timing_a;
        }
        private static bool InvokerCanCast(Ability skill)
        {
            if (skill == null)
                return false;
            if (skill.Cooldown == 0 && me.Mana >= skill.ManaCost && skill.Level > 0 && me.CanCast())
            {
                if (skill.Name == coldsnap.Name && quas.Level > 0)
                    return true;
                else if (skill.Name == meteor.Name && wex.Level > 0 && exort.Level > 0)
                    return true;
                else if (skill.Name == alacrity.Name && wex.Level > 0 && exort.Level > 0)
                    return true;
                else if (skill.Name == tornado.Name && quas.Level > 0 && wex.Level > 0)
                    return true;
                else if (skill.Name == forgespirit.Name && quas.Level > 0 && exort.Level > 0)
                    return true;
                else if (skill.Name == blast.Name && quas.Level > 0 && wex.Level > 0 && exort.Level > 0)
                    return true;
                else if (skill.Name == sunstrike.Name && exort.Level > 0)
                    return true;
                else if (skill.Name == ghostwalk.Name && quas.Level > 0 && wex.Level > 0)
                    return true;
                else if (skill.Name == icewall.Name && exort.Level > 0 && exort.Level > 0)
                    return true;
                else if (skill.Name == emp.Name && wex.Level > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        private static uint NextSkillFlee()
        {
            try
            {
                // 1 - ghostwalk, 2 icewall, 3 - coldsnap, 4 - blast
                if (Iscasted(ghostwalk) && ghostwalk.CanBeCasted() && !me.IsInvisible())
                    return 1;
                if (InvokerCanCast(ghostwalk) && !me.IsInvisible())
                    return 1;
                return 0;
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }
        private static uint NextSkill()
        {
            // 1 - coldsnap, 2- meteor, 3 - alacrity, 4 - tornado, 5 -forgespirit, 6 -blast, 7 - sunstrike(off), 8 - emp, 9 - icewall, 10 - sunstrike
            try
            {
                if (!Utils.SleepCheck("cd_tornado_a"))
                    return 0;
                if (Iscasted(coldsnap) && coldsnap.CanBeCasted() && (distance_me_target <= 900 || (me.MovementSpeed >= target.MovementSpeed + 30 && distance_me_target <= 900)) && !target_isinvul && !target_magic_imune)
                    return 1;
                if (Iscasted(forgespirit) && forgespirit.CanBeCasted() && distance_me_target <= 900 && !target_isinvul)
                    return 5;
                if (Iscasted(alacrity) && alacrity.CanBeCasted() && distance_me_target <= 900 && !target_isinvul)
                    return 3;
                if (Iscasted(icewall) && icewall.CanBeCasted() && distance_me_target <= 200 && !target_magic_imune)
                    return 9;
                if (Iscasted(blast) && blast.CanBeCasted() && distance_me_target <= 900 && (!target_isinvul || target_blast_ontiming))
                    return 6;
                if (Iscasted(tornado) && tornado.CanBeCasted() && !target_isinvul && !target_magic_imune && distance_me_target <= 2800 && Utils.SleepCheck("bloodpop") && Utils.SleepCheck("malepop") && Utils.SleepCheck("vysepop"))
                    return 4;
                if (Iscasted(meteor) && meteor.CanBeCasted() && !target_magic_imune && (target.MovementSpeed <= 250 || target_meteor_ontiming) && distance_me_target <= 700)
                    return 2;
                if (Iscasted(sunstrike) && sunstrike.CanBeCasted() && (target.MovementSpeed < 200 || target_sunstrike_ontiming))
                    return 10;
                if (Iscasted(emp) && emp.CanBeCasted() && (target.MovementSpeed <= 190 || target_emp_ontiming) && distance_me_target <= 700)
                    return 8;
                level_checker();
                //skills sequence
                if (quas_level && me.Mana >= coldsnap.ManaCost + invoke.ManaCost && coldsnap.Cooldown == 0 && (distance_me_target <= 750 || (me.MovementSpeed >= target.MovementSpeed + 30 && distance_me_target <= 900)) && !target_isinvul && !target_magic_imune && quas.Level > 0)
                    return 1;
                if (exort_level && me.Mana >= forgespirit.ManaCost + invoke.ManaCost && forgespirit.Cooldown == 0 && distance_me_target <= 600 && !target_isinvul && !forge_in_my_side && quas.Level > 0 && exort.Level > 0)
                    return 5;
                if ((exort_level || wex_level) && me.Mana >= alacrity.ManaCost + invoke.ManaCost && alacrity.Cooldown == 0 && (distance_me_target <= 750 || (me.MovementSpeed >= target.MovementSpeed + 30 && distance_me_target <= 900)) && !target_isinvul && exort.Level > 0 && wex.Level > 0)
                    return 3;
                if (quas_level && me.Mana >= icewall.ManaCost + invoke.ManaCost && icewall.Cooldown == 0 && !target_magic_imune && ice_wall_distance && quas.Level > 0 && exort.Level > 0)
                    return 9;
                if (wex_level && me.Mana >= tornado.ManaCost + invoke.ManaCost && tornado.Cooldown == 0 && !target_isinvul && !target_magic_imune && distance_me_target <= 2800 && distance_me_target >= 900 && Utils.SleepCheck("cd_meteor_a") && Utils.SleepCheck("cd_blast_a") && Utils.SleepCheck("cd_emp_a") && Utils.SleepCheck("bloodpop") && Utils.SleepCheck("malepop") && Utils.SleepCheck("vysepop") && quas.Level > 0 && wex.Level > 0)
                    return 4;
                if (exort_level && me.Mana >= meteor.ManaCost + invoke.ManaCost && meteor.Cooldown == 0 && !target_magic_imune && (target.MovementSpeed <= 250 || target_meteor_ontiming) && distance_me_target <= 700 && wex.Level > 0 && exort.Level > 0)
                    return 2;
                if (exort_level && me.Mana >= sunstrike.ManaCost + invoke.ManaCost && sunstrike.Cooldown == 0 && (target.MovementSpeed < 200 || target_sunstrike_ontiming) && me.AttackSpeedValue >= 150 && exort.Level > 0 && !target.HasModifier("modifier_invoker_cold_snap"))
                    return 10;
                if ((exort_level || quas_level || wex_level) && me.Mana >= blast.ManaCost + invoke.ManaCost && blast.Cooldown == 0 && !target_magic_imune && distance_me_target <= 950 && !target_isinvul && exort.Level > 0 && quas.Level > 0 && wex.Level > 0)
                    return 6;
                if (wex_level && emp.Cooldown == 0 && (target.MovementSpeed <= 190 || target_emp_ontiming) && me.Mana >= emp.ManaCost + invoke.ManaCost && distance_me_target <= 700 && (target.Mana > target.MaximumMana * 0.35) && wex.Level > 0)
                    return 8;

                return 0;
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }
        private static void orb_type(Ability skill)
        {
            if (skill == null) return;
            if (!Utils.SleepCheck("PINGCANCEL")) return;
            if (skill.Name == quas.Name)
            {
                quas.UseAbility(false);
                quas.UseAbility(false);
                quas.UseAbility(false);
                Utils.Sleep(Game.Ping, "PINGCANCEL");
            }
            else if (skill.Name == wex.Name)
            {
                wex.UseAbility(false);
                wex.UseAbility(false);
                wex.UseAbility(false);
                Utils.Sleep(Game.Ping, "PINGCANCEL");
            }
            else if (skill.Name == exort.Name)
            {
                exort.UseAbility(false);
                exort.UseAbility(false);
                exort.UseAbility(false);
                Utils.Sleep(Game.Ping, "PINGCANCEL");
            }
            else
                return;
        }
        private static void InvokeSkill(Ability skill, bool IsCasted)
        {
            if (skill == null) return;
            if (!Utils.SleepCheck("PINGCANCEL")) return;
            if (skill.Name == coldsnap.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == meteor.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == alacrity.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == tornado.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == forgespirit.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == blast.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    quas.UseAbility(false);
                    wex.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == sunstrike.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == emp.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == icewall.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else if (skill.Name == ghostwalk.Name)
            {
                if ((IsCasted ? !Iscasted(skill) : true) && invoke.CanBeCasted())
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(Game.Ping, "PINGCANCEL");
                    return;
                }
            }
            else
                return;

        }
        private static bool TargetIsTurning(Hero Enemy)
        {
            if (Enemy != null && Enemy.IsValid && Utils.SleepCheck("TargetIsTurning_delay"))
            {
                if (!TurntimeOntick.ContainsKey(currenttickcount))
                    TurntimeOntick.Add(currenttickcount, (int)Enemy.NetworkRotation);
                if (targetisturning_delay == -777)
                {
                    Utils.Sleep(SunstrikeTimeConfig2, "TargetIsTurning2");
                    targetisturning_delay = -666;
                }
                if (((TurntimeOntick.FirstOrDefault(x => x.Key >= currenttickcount - (SunstrikeTimeConfig + 50) && x.Key <= currenttickcount - (SunstrikeTimeConfig - 50)).Value - (int)Enemy.NetworkRotation) != 0))
                {
                    Utils.Sleep(200, "TargetIsTurning");
                }
                if (Utils.SleepCheck("TargetIsTurning") && Utils.SleepCheck("TargetIsTurning2"))
                {
                    targetisturning_delay = Enemy.NetworkRotation;
                }
            }
            if (Enemy.NetworkRotation - targetisturning_delay == 0)
            {
                targetisturning_delay = -777;
                TurntimeOntick.Clear();
                Utils.Sleep(1500, "TargetIsTurning_delay");
                return true;
            }
            else
                return false;
        }
        private static bool Iscasted(Ability skill)
        {
            if (skill.AbilitySlot == AbilitySlot.Slot_4 || skill.AbilitySlot == AbilitySlot.Slot_5)
                return true;
            else
                return false;
        }
        private static void AttackTarget()
        {
            if (Utils.SleepCheck("orbwalker"))
            {
                if (me.Distance2D(target) >= OrbMinDist)
                {
                    Orbwalking.Orbwalk(target);
                    if (exort_level && me.Modifiers.Count(x => x.Name.Contains("exort")) < 4 && Utils.SleepCheck("orbchange"))
                    {
                        orb_type(exort);
                        Utils.Sleep(800, "orbchange");
                    }
                    else if (wex_level && me.Modifiers.Count(x => x.Name.Contains("wex")) < 4 && Utils.SleepCheck("orbchange"))
                    {
                        orb_type(wex);
                        Utils.Sleep(800, "orbchange");
                    }
                }
                else
                {
                    me.Attack(target, false);
                    if (exort_level && me.Modifiers.Count(x => x.Name.Contains("exort")) < 4 && Utils.SleepCheck("orbchange"))
                    {
                        orb_type(exort);
                        Utils.Sleep(800, "orbchange");
                    }
                    else if (wex_level && me.Modifiers.Count(x => x.Name.Contains("wex")) < 4 && Utils.SleepCheck("orbchange"))
                    {
                        orb_type(wex);
                        Utils.Sleep(800, "orbchange");
                    }
                }
                Utils.Sleep(250, "orbwalker");
            }
        }
        private static void Find_skillsAndItens()
        {
            if (Utils.SleepCheck("ORBSFIND"))
            {
                quas = me.Spellbook.SpellQ;
                wex = me.Spellbook.SpellW;
                exort = me.Spellbook.SpellE;
                invoke = me.Spellbook.SpellR;
                coldsnap = me.FindSpell("invoker_cold_snap");
                forgespirit = me.FindSpell("invoker_forge_spirit");
                meteor = me.FindSpell("invoker_chaos_meteor");
                alacrity = me.FindSpell("invoker_alacrity");
                tornado = me.FindSpell("invoker_tornado");
                blast = me.FindSpell("invoker_deafening_blast");
                sunstrike = me.FindSpell("invoker_sun_strike");
                emp = me.FindSpell("invoker_emp");
                icewall = me.FindSpell("invoker_ice_wall");
                ghostwalk = me.FindSpell("invoker_ghost_walk");
                eul = me.FindItem("item_cyclone");
                medallion = me.FindItem("item_medallion_of_courage");
                solar_crest = me.FindItem("item_solar_crest");
                malevolence = me.FindItem("item_orchid");
                vyse = me.FindItem("item_sheepstick");
                bloodthorn = me.FindItem("item_bloodthorn");
                urn = me.FindItem("item_urn_of_shadows");
                refresher = me.FindItem("item_refresher");
                ethereal = me.FindItem("item_ethereal_blade");
                dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
                Utils.Sleep(500, "ORBSFIND");
            }
        }
        private static void Use_Item(dynamic Item_Name)
        {
            if (Item_Name == null) return;
            if (!(Item_Name is Item || Item_Name is string)) throw new ArgumentException("INVALID PARAMETERS! => Item_Name isn't a valid parameter.", "Item_Name");
            if (Item_Name is Item) Item_Name = Item_Name.Name;
            if ((ethereal != null && Item_Name == ethereal.Name) || Item_Name == "ALL")
            {
                if (ethereal != null && ethereal.CanBeCasted() && Utils.SleepCheck("Ethereal") && Utils.SleepCheck("cd_tornado_a"))
                {
                    ethereal.UseAbility(target, false);
                    Utils.Sleep(250, "Ethereal");
                }
            }
            if ((dagon != null && Item_Name == dagon.Name) || Item_Name == "ALL")
            {
                if (dagon != null && dagon.CanBeCasted() && Utils.SleepCheck("Dagon") && Utils.SleepCheck("cd_tornado_a"))
                {
                    dagon.UseAbility(target, false);
                    Utils.Sleep(250, "Dagon");
                }
            }
            if ((medallion != null && Item_Name == medallion.Name) || Item_Name == "ALL")
            {
                if (medallion.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("medallion"))
                {
                    medallion.UseAbility(target, false);
                    Utils.Sleep(500, "medallion");
                }
            }
            if ((solar_crest != null && Item_Name == solar_crest.Name) || Item_Name == "ALL")
            {
                if (solar_crest.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("crest"))
                {
                    solar_crest.UseAbility(target, false);
                    Utils.Sleep(500, "crest");
                }
            }
            if ((malevolence != null && Item_Name == malevolence.Name) || Item_Name == "ALL")
            {
                if (malevolence.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("male") && !(IsComboPrepared() != 0 || comboing))
                {
                    malevolence.UseAbility(target, false);
                    Utils.Sleep(500, "male");
                    Utils.Sleep(5000, "malepop");
                }
            }
            if ((vyse != null && Item_Name == vyse.Name) || Item_Name == "ALL")
            {
                if (vyse.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("vyse") && !(IsComboPrepared() != 0 || comboing))
                {
                    vyse.UseAbility(target, false);
                    Utils.Sleep(500, "vyse");
                    Utils.Sleep(3500, "vysepop");
                }
            }
            if ((bloodthorn != null && Item_Name == bloodthorn.Name) || Item_Name == "ALL")
            {
                if (bloodthorn.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("blood") && !(IsComboPrepared() != 0 || comboing))
                {
                    bloodthorn.UseAbility(target, false);
                    Utils.Sleep(500, "blood");
                    Utils.Sleep(5000, "bloodpop");
                }
            }
            if ((urn != null && Item_Name == urn.Name) || Item_Name == "ALL")
            {
                if (urn.CanBeCasted() && !target_magic_imune && !target_isinvul && Utils.SleepCheck("urn") && !(IsComboPrepared() != 0 || comboing))
                {
                    urn.UseAbility(target, false);
                    Utils.Sleep(800, "urn");
                }
            }
            return;
        }
        private static void level_checker()
        {
            //quas, wex, exort checker
            if (me.Level <= 6)
            {
                if (quas.Level >= 2)
                    quas_level = true;
                else
                    quas_level = false;
                if (wex.Level >= 2)
                    wex_level = true;
                else
                    wex_level = false;
                if (exort.Level >= 2)
                    exort_level = true;
                else
                    exort_level = false;
            }
            else if (me.Level <= 10)
            {
                if (quas.Level >= 3)
                    quas_level = true;
                else
                    quas_level = false;
                if (wex.Level >= 3)
                    wex_level = true;
                else
                    wex_level = false;
                if (exort.Level >= 3)
                    exort_level = true;
                else
                    exort_level = false;
            }
            else if (me.Level <= 15)
            {
                if (quas.Level >= 4)
                    quas_level = true;
                else
                    quas_level = false;
                if (wex.Level >= 4)
                    wex_level = true;
                else
                    wex_level = false;
                if (exort.Level >= 4)
                    exort_level = true;
                else
                    exort_level = false;
            }
            else if (me.Level <= 25)
            {
                if (quas.Level >= 5)
                    quas_level = true;
                else
                    quas_level = false;
                if (wex.Level >= 5)
                    wex_level = true;
                else
                    wex_level = false;
                if (exort.Level >= 5)
                    exort_level = true;
                else
                    exort_level = false;
            }
        }
    }
}
