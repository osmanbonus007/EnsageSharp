using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using System;

namespace InvokerSkillBar
{
    class Invoker
    {
        private static readonly Menu Menu = new Menu("Invoker", "Invoker", true, "npc_dota_hero_Invoker", true);

        private static int boxsize => Menu.Item("Box Size").GetValue<Slider>().Value;
        private static int boxX => Menu.Item("Box Position X").GetValue<Slider>().Value;
        private static int boxY => Menu.Item("Box Position Y").GetValue<Slider>().Value;
        private static int Coldsnap => Menu.Item("UIColdSnap").GetValue<Slider>().Value;
        private static int Forge => Menu.Item("UIForgeSpirit").GetValue<Slider>().Value;
        private static int Alacrity => Menu.Item("UIAlacrity").GetValue<Slider>().Value;
        private static int Tornado => Menu.Item("UITornado").GetValue<Slider>().Value;
        private static int Emp => Menu.Item("UIEmp").GetValue<Slider>().Value;
        private static int Meteor => Menu.Item("UIMeteor").GetValue<Slider>().Value;
        private static int Sunstrike => Menu.Item("UISunstrike").GetValue<Slider>().Value;
        private static int Icewall => Menu.Item("UIIcewall").GetValue<Slider>().Value;
        private static int Ghostwalk => Menu.Item("UIGhostwalk").GetValue<Slider>().Value;
        private static int Defeaningblast => Menu.Item("UIDefeaningblast").GetValue<Slider>().Value;

        private static Hero me;
        private static Ability quas, wex, exort, invoke, coldsnap, meteor, alacrity, tornado, forgespirit, blast, sunstrike, emp, icewall, ghostwalk;
        
        static void Main(string[] args)
        {

            var UiSkillBar = new Menu("Ui SkillBar", "Ui SkillBar");
            Menu.AddSubMenu(UiSkillBar);
            UiSkillBar.AddItem(new MenuItem("Enable Skill UI Bar", "Enable Skill UI Bar").SetValue(true).SetTooltip("Enable/Disable UI information bar."));
            UiSkillBar.AddItem(new MenuItem("Box Size", "Box Size").SetValue(new Slider(25, 5, 300)).SetTooltip("Set The box size of Menu"));
            UiSkillBar.AddItem(new MenuItem("Box Position Y", "Box Position Y").SetValue(new Slider(Drawing.Height, 0, Drawing.Height)).SetTooltip("Set the Y position of Menu"));
            UiSkillBar.AddItem(new MenuItem("Box Position X", "Box Position X").SetValue(new Slider(Drawing.Width, 0, Drawing.Width)).SetTooltip("Set the X position of Menu"));

            var QuickcastSpells2 = new Menu("Quick Cast Spells2", "Quick Cast Spells2");
            Menu.AddSubMenu(QuickcastSpells2);
           
            QuickcastSpells2.AddItem(new MenuItem("ColdSnap", "Cold Snap").SetValue(new KeyBind('1', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Forge Spirit", "Forge Spirit").SetValue(new KeyBind('2', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Alacrity", "Alacrity").SetValue(new KeyBind('3', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Tornado", "Tornado").SetValue(new KeyBind('4', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Emp", "Emp").SetValue(new KeyBind('5', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Meteor", "Meteor").SetValue(new KeyBind('6', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("sunstrike", "sunstrike").SetValue(new KeyBind('7', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Icewall", "Icewall").SetValue(new KeyBind('8', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Ghostwalk", "Ghostwalk").SetValue(new KeyBind('9', KeyBindType.Press)));
            QuickcastSpells2.AddItem(new MenuItem("Defeaning blast", "Defeaning blast").SetValue(new KeyBind('0', KeyBindType.Press)));

            var QuickcastSpells = new Menu("Quick Cast Spells", "Quick Cast Spells");

            Menu.AddSubMenu(QuickcastSpells);
            
            QuickcastSpells.AddItem(new MenuItem("UIColdSnap", "Cold Snap").SetValue(new Slider(1, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIForgeSpirit", "Forge Spirit").SetValue(new Slider(2, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIAlacrity", "Alacrity").SetValue(new Slider(3, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UITornado", "Tornado").SetValue(new Slider(4, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIEmp", "Emp").SetValue(new Slider(5, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIMeteor", "Meteor").SetValue(new Slider(6, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UISunstrike", "sunstrike").SetValue(new Slider(7, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIIcewall", "Icewall").SetValue(new Slider(8, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIGhostwalk", "Ghostwalk").SetValue(new Slider(9, 1, 10)));
            QuickcastSpells.AddItem(new MenuItem("UIDefeaningblast", "Defeaning blast").SetValue(new Slider(10, 1, 10)));
            Menu.AddToMainMenu();

            

            Drawing.OnDraw += Target_esp;
            Game.OnUpdate += orb_checker;
        }
        public static void orb_checker(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame || Game.IsPaused)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Invoker)
                return;
            Find_skillsAndItens();

            if (Utils.SleepCheck("skillcasted"))
            {
                if ((Game.IsKeyDown(Menu.Item("ColdSnap").GetValue<KeyBind>().Key) && !Game.IsChatOpen))
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Forge Spirit").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Alacrity").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Tornado").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    quas.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Emp").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Meteor").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("sunstrike").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Icewall").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Ghostwalk").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    quas.UseAbility(false);
                    quas.UseAbility(false);
                    wex.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
                if (Game.IsKeyDown(Menu.Item("Defeaning blast").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    quas.UseAbility(false);
                    wex.UseAbility(false);
                    exort.UseAbility(false);
                    invoke.UseAbility(false);
                    Utils.Sleep(500, "skillcasted");
                }
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
                Utils.Sleep(500, "ORBSFIND");
            }
        }
        public static void Target_esp(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Invoker)
                return;
            if (Menu.Item("Enable Skill UI Bar").GetValue<bool>())
            {             
                foreach (Ability spells in me.Spellbook.Spells)
                {
                    if (spells == null) continue;
                    var abilityState = spells.AbilityState;

                        if (spells.Name.Contains("invoker_cold_snap"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Coldsnap, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_cold_snap.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Coldsnap, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Coldsnap, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Coldsnap, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("ColdSnap").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Coldsnap, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }                            
                        if (spells.Name.Contains("invoker_forge"))
                        {                            
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Forge, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_forge_spirit.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Forge, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Forge, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Forge Spirit").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Forge, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }                                              
                        if (spells.Name.Contains("invoker_alacrity"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Alacrity, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_alacrity.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Alacrity, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Alacrity, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Alacrity, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Alacrity").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Alacrity, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                            
                        if (spells.Name.Contains("invoker_tornado"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Tornado, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_tornado.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Tornado, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Tornado, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Tornado, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Tornado").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Tornado, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                           
                        if (spells.Name.Contains("invoker_emp"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Emp, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_emp.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Emp, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Emp, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Emp, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Emp").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Emp, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }                            
                        if (spells.Name.Contains("invoker_chaos_meteor"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Meteor, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_chaos_meteor.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Meteor, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Meteor, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Meteor, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Meteor").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Meteor, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                        if (spells.Name.Contains("invoker_sun_strike"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Sunstrike, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_sun_strike.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Sunstrike, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Sunstrike, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Sunstrike, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("sunstrike").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Sunstrike, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                        if (spells.Name.Contains("invoker_ice_wall"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Icewall, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_ice_wall.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Icewall, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Icewall, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Icewall, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Icewall").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Icewall, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                        if (spells.Name.Contains("invoker_ghost_walk"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Ghostwalk, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_ghost_walk.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Ghostwalk, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Ghostwalk, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Ghostwalk, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Ghostwalk").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Ghostwalk, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                        if (spells.Name.Contains("invoker_deafening_blast"))
                        {
                            Drawing.DrawRect(new Vector2(boxX + boxsize * Defeaningblast, boxY), new Vector2(boxsize, boxsize), Drawing.GetTexture("materials/ensage_ui/spellicons/invoker_deafening_blast.vmat"));
                            if (spells.Cooldown > 0) Drawing.DrawRect(new Vector2(boxX + boxsize * Defeaningblast, boxY), new Vector2(boxsize, boxsize * (spells.Cooldown / spells.CooldownLength)), new Color(255, 0, 0, 130), false);
                            if (me.Mana < spells.ManaCost) Drawing.DrawRect(new Vector2(boxX + boxsize * Defeaningblast, boxY), new Vector2(boxsize, boxsize), new Color(0, 0, 255, 130), false);
                            if (abilityState == AbilityState.OnCooldown) Drawing.DrawText(Math.Min(spells.Cooldown + 0.1, 99).ToString("0"), new Vector2(boxX + boxsize * Defeaningblast, boxY), new Vector2(boxsize / 2.50f, boxsize / 2.50f), Color.Yellow, FontFlags.None);
                            Drawing.DrawText(Convert.ToChar(Menu.Item("Defeaning blast").GetValue<KeyBind>().Key).ToString(), new Vector2(boxX + boxsize * Defeaningblast, boxY + boxsize / 2.5f), new Vector2(boxsize / 1.50f, boxsize / 1.50f), Color.White, FontFlags.None);
                        }
                    }
                
            }
        } 
    }
}
