// credits: VickTheRock
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace AllUnitsPush
{    
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal class Program
    {
        private static readonly Menu Menu = new Menu("All Unit's Push", "All Unit's Push", true).SetFontColor(Color.Aqua);
        private static int range => Menu.Item("range").GetValue<Slider>().Value;
        private static int textX => Menu.Item("textX").GetValue<Slider>().Value;
        private static int textV => Menu.Item("textV").GetValue<Slider>().Value;
        private static bool activated;
        private static Font txt;

        private static readonly Vector3[] mid =
        {
            new Vector3(-5565, -5039, 384),
            new Vector3(-4436, -3942, 384),
            new Vector3(-3342, -2809, 256),
            new Vector3(-2265, -1869, 255),
            new Vector3(-1083, -887, 256),
            new Vector3(94, 136, 256),
            new Vector3(1398, 1045, 256),
            new Vector3(2522, 2054, 256),
            new Vector3(3509, 3139, 256),
            new Vector3(4663, 4079, 384),
            new Vector3(5233, 4711, 384),

        };

        private static readonly Vector3[] bot =
        {
            new Vector3(-4732, -6082, 384),
            new Vector3(-3248, -6018, 256),
            new Vector3(-1656, -6057, 256),
            new Vector3(-157, -6114, 384),
            new Vector3(1360, -6270, 384),
            new Vector3(2901, -6142, 384),
            new Vector3(4398, -5912, 384),
            new Vector3(5626, -4999, 384),
            new Vector3(5968, -3497, 384),
            new Vector3(6183, -2020, 384),
            new Vector3(6329, -486, 384),
            new Vector3(6232, 1032, 379),
            new Vector3(6295, 2602, 384),
            new Vector3(6368, 3812, 384)
        };

        private static readonly Vector3[] top =
        {
            new Vector3(-6641, -4192, 384),
            new Vector3(-6612, -2662, 256),
            new Vector3(-6443, -1187, 384),
            new Vector3(-6353, 288, 384),
            new Vector3(-6254, 1789, 384),
            new Vector3(-6247, 3348, 384),
            new Vector3(-6203, 4851, 384),
            new Vector3(-4948, 5695, 384),
            new Vector3(-3455, 5748, 384),
            new Vector3(-1975, 6056, 384),
            new Vector3(-466, 6005, 383),
            new Vector3(1039, 5867, 384),
            new Vector3(2555, 5812, 255),
            new Vector3(4084, 5802, 384),
        };
        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Push key", "Togle key Push").SetValue(new KeyBind('K', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("range", "Range").SetValue(new Slider(600, 0, 2000)));
            Menu.AddItem(new MenuItem("drawing", "Enable Drawing").SetValue(true));
            Menu.AddItem(new MenuItem("textX", "Drawing X").SetValue(new Slider(1200, 0, 2000)));
            Menu.AddItem(new MenuItem("textV", "Drawing V").SetValue(new Slider(37, 0, 1000)));
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;

            if (Drawing.RenderMode == RenderMode.Dx9)
            {
                txt = new Font(                       
                    Drawing.Direct3DDevice9,  
                    new FontDescription
                    {
                        FaceName = "Arial",
                        Height = 16,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Draft
                    });
            }                

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            var me = ObjectManager.LocalHero;
            if (!Game.IsInGame || me == null || Game.IsWatchingGame)
                return;
            activated = Menu.Item("Push key").GetValue<KeyBind>().Active;
            if (activated && !Game.IsPaused)
            {
                var unit = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                     || creep.ClassId == ClassId.CDOTA_BaseNPC_Additive                    
                     || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep
                     || creep.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling
                     || creep.IsIllusion)
                    && creep.IsAlive
                    && creep.NetworkActivity != NetworkActivity.Move
                    && creep.Team == me.Team
                    && creep.IsControllable
                    && creep.IsValid).ToList();
                if (unit.Count == 0) return;

                Unit fount = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);
                List<Unit> tower = ObjectManager.GetEntities<Unit>().Where(x => x.Team != me.Team && x.ClassId == ClassId.CDOTA_BaseNPC_Tower).ToList();                

                for (int i = 0; i < unit.Count(); ++i)
                {
                    Vector3 Mid = GetClosestToVector(mid, unit[i]);
                    Vector3 Top = GetClosestToVector(top, unit[i]);
                    Vector3 Bot = GetClosestToVector(bot, unit[i]);
                    if (me.Distance2D(unit[i]) <= range) return;
                    var v =
                            ObjectManager.GetEntities<Hero>()
                                .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                                .ToList();
                   
                    if (Mid.Distance2D(unit[i]) <= 4000)
                    {
                         for (int x = 0; x < mid.Count(); ++x)
                            {
                                var b = mid[x];
                                if (
                                    unit[i].Distance2D(fount) +170 < b.Distance2D(fount) && unit[i].Distance2D(b) <= 4000 && Utils.SleepCheck(unit[i].Handle.ToString()))
                                {
                                    unit[i].Attack(b); Utils.Sleep(1500, unit[i].Handle.ToString());
                                }
                            }
                    }
                    if (Bot.Distance2D(unit[i]) <= 4000)
                    {
                         for (int x = 0; x < bot.Count(); ++x)
                            {
                                var b = bot[x];
                                if (
                                    unit[i].Distance2D(fount) + 170 < b.Distance2D(fount) && unit[i].Distance2D(b) <= 4000 && Utils.SleepCheck(unit[i].Handle.ToString()))
                                {
                                    unit[i].Attack(b); Utils.Sleep(1500, unit[i].Handle.ToString());
                                }
                            }
                        
                    }
                    if (Top.Distance2D(unit[i]) <= 4000)
                    {
                        for (int x = 0; x < top.Count(); ++x)
                            {
                                var b = top[x];
                                if (
                                    unit[i].Distance2D(fount) + 170 < b.Distance2D(fount) && unit[i].Distance2D(b) <= 4000 && Utils.SleepCheck(unit[i].Handle.ToString()))
                                {
                                    unit[i].Attack(b); Utils.Sleep(1500, unit[i].Handle.ToString());
                                }
                            }                      
                    }
                }
            }
        }
        private static Vector3 GetClosestToVector(Vector3[] coords, Unit z)
        {
            var closestVector = coords.First();
            foreach (var v in coords.Where(v => closestVector.Distance2D(z) > v.Distance2D(z)))
                closestVector = v;
            return closestVector;
        }
        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            txt.Dispose();
        }
        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            var me = ObjectManager.LocalHero;
            if (me == null)
                return;

            if (activated && Menu.Item("drawing").GetValue<bool>())
            {
                txt.DrawText(null, "Unit Push Active", textX, textV, Color.Lime);
                txt.DrawText(null, "Inactive Range", textX, textV + 20, Color.Lime);
                txt.DrawText(null, range.ToString(), textX + 90, textV + 20, Color.Aqua);
            }

            if (!activated && Menu.Item("drawing").GetValue<bool>())
            {
                txt.DrawText(null, "Unit Push UnActive", textX, textV, Color.Red);
            }
        }
        static void Drawing_OnPostReset(EventArgs args)
        {
            txt.OnResetDevice();
        }
        static void Drawing_OnPreReset(EventArgs args)
        {
            txt.OnLostDevice();
        }
    }
}

