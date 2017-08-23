using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma
{
    class BlackHole
    {
        private static readonly Menu Menu = new Menu("Enigma", "Enigma", true,"npc_dota_hero_enigma", true);
        private static readonly Menu Menu_Items = new Menu("Items: ", "Items: ");
        private static Ability midnightpulse, blackhole;
        private static Item blink, bkb, veil, shivas, refresher, glimmer;
        private static Hero me, target;
        private static Vector3 mousepos;
        private static bool SafeBlackHole;
        private static readonly Dictionary<string, bool> items = new Dictionary<string, bool>
            {
                {"item_blink",true},
                {"item_veil_of_discord",true},
                {"item_black_king_bar",true},
                {"item_shivas_guard",true},
                {"item_refresher",true},
                {"item_glimmer_cape",true},
            };
        private static readonly Dictionary<string, bool> Skills = new Dictionary<string, bool>
            {
                {"enigma_midnight_pulse",true},
                {"enigma_black_hole",true},
            };
        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Black Hole Key!", "Black Hole Key!").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Safe Black Hole", "Safe Black Hole").SetValue(true).SetTooltip("Black Hole only will be used if there's enemies in area."));
            Menu.AddSubMenu(Menu_Items);
            Menu_Items.AddItem(new MenuItem("Items: ", "Items: ").SetValue(new AbilityToggler(items)));
            Menu_Items.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(Skills)));
            Menu.AddToMainMenu();
            PrintSuccess("> Enigma Script!");
            Game.OnUpdate += Universe;
        }
        public static void Universe(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Enigma)
                return;
            if (Game.IsKeyDown(Menu.Item("Black Hole Key!").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                FindItems();
                if (me.CanCast() && !me.IsChanneling())
                {
                    mousepos = Game.MousePosition;
                    if (me.Distance2D(mousepos) <= ((blink != null /*&& blink.CanBeCasted()*/) ? (1200 + blackhole.CastRange) : blackhole.CastRange))
                    {
                        if (Utils.SleepCheck("blackhole"))
                        {
                            if((glimmer != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)) && glimmer.CanBeCasted() && Utils.SleepCheck("glimmer") && blackhole.CanBeCasted() && me.Mana > blackhole.ManaCost + glimmer.ManaCost)
                            {
                                glimmer.UseAbility(me,false);
                                Utils.Sleep(200, "glimmer");
                            }
                            if (!blackhole.CanBeCasted() && blackhole.Level > 0 && (refresher != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(refresher.Name)) && refresher.CanBeCasted() && Utils.SleepCheck("refresher") && me.Mana > refresher.ManaCost + blackhole.ManaCost)
                            {
                                refresher.UseAbility(false);
                                Utils.Sleep(2000, "refresher");
                            }
                            if ((bkb != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(bkb.Name)) && bkb.CanBeCasted() && Utils.SleepCheck("bkb") && blackhole.CanBeCasted())
                            {
                                bkb.UseAbility(false);
                                Utils.Sleep(200, "bkb");
                            }
                            if ((blink != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)) && blink.CanBeCasted() && Utils.SleepCheck("blink") && me.Distance2D(mousepos) >= blackhole.CastRange && blackhole.CanBeCasted())
                            {
                                blink.UseAbility(me.Distance2D(mousepos) < 1200 ? mousepos : new Vector3(me.NetworkPosition.X + 1150 * (float)Math.Cos(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), me.NetworkPosition.Y + 1150 * (float)Math.Sin(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), 100), false);
                                Utils.Sleep(200, "blink");
                            }
                            if ((veil != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)) && veil.CanBeCasted() && Utils.SleepCheck("veil") && me.Mana > veil.ManaCost + blackhole.ManaCost && blackhole.CanBeCasted())
                            {
                                veil.UseAbility(mousepos, false);
                                Utils.Sleep(200, "veil");
                            }
                            if ((midnightpulse != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(midnightpulse.Name)) && midnightpulse.CanBeCasted() && Utils.SleepCheck("pulse") && me.Mana > midnightpulse.ManaCost + blackhole.ManaCost && blackhole.CanBeCasted())
                            {
                                midnightpulse.UseAbility(mousepos, false);
                                Utils.Sleep(200, "pulse");
                            }
                            if ((shivas != null && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(shivas.Name)) && shivas.CanBeCasted() && Utils.SleepCheck("shivas") && me.Mana > shivas.ManaCost + blackhole.ManaCost && blackhole.CanBeCasted())
                            {
                                shivas.UseAbility(false);
                                Utils.Sleep(200, "shivas");
                            }
                        }
                        if ((!blink.CanBeCasted() || me.Distance2D(mousepos) <= blackhole.CastRange || (blink != null && !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(blink.Name))) && (!veil.CanBeCasted() || me.Mana < veil.ManaCost + blackhole.ManaCost || (veil != null && !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))) && (!midnightpulse.CanBeCasted() || me.Mana < midnightpulse.ManaCost + blackhole.ManaCost || (midnightpulse != null && !Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(midnightpulse.Name))) && (!bkb.CanBeCasted() || (bkb != null && !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(bkb.Name))) && (!shivas.CanBeCasted() || me.Mana < shivas.ManaCost + blackhole.ManaCost ||(shivas != null && !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(shivas.Name))))
                        {
                            target = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.IsAlive && !x.IsIllusion && x.IsValid && x.NetworkPosition.Distance2D(mousepos) < 420 && x.Team != me.Team);
                            if (Menu.Item("Safe Black Hole").GetValue<bool>())
                            {
                                if (target != null)
                                    SafeBlackHole = true;
                                else
                                    SafeBlackHole = false;
                            }
                            else
                                SafeBlackHole = true;
                            if ((blackhole != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(blackhole.Name)) && blackhole.CanBeCasted() && Utils.SleepCheck("blackhole") && SafeBlackHole)
                            {
                                blackhole.UseAbility(mousepos, false);
                                Utils.Sleep(2000, "blackhole");
                            }
                        }
                    }
                    else
                    {
                        if (me.CanMove())
                        {
                            me.Move(mousepos, false);
                        }
                    }
                }
            }
        }
        static void FindItems()
        {
            if (Utils.SleepCheck("FINDITEMS"))
            {
                blink = me.FindItem("item_blink");
                bkb = me.FindItem("item_black_king_bar");
                veil = me.FindItem("item_veil_of_discord");
                shivas = me.FindItem("item_shivas_guard");
                refresher = me.FindItem("item_refresher");
                glimmer = me.FindItem("item_glimmer_cape");
                midnightpulse = me.Spellbook.SpellE;
                blackhole = me.Spellbook.SpellR;
                Utils.Sleep(500, "FINDITEMS");
            }
        }
        private static void PrintSuccess(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Green, arguments);
        }
        private static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
    }
}
