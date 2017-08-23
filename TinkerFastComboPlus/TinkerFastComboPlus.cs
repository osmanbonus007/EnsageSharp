// credits: Air13, ObiXah, Jumpering, beminee
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TinkerFastComboPlus
{
    class TinkerFastComboPlus
    {
        private static CancellationTokenSource tks;        
        private static Task RearmBlink;
        private static readonly int[] RearmTime = { 3010, 1510, 760 };
        private static int time;
        private static int GetRearmTime(Ability s) => RearmTime[s.Level - 1];

        private const int HIDE_AWAY_RANGE = 130;
        private static bool iscreated;
        private static Ability Laser, Rocket, Refresh, March;
        private static Item blink, dagon, sheep, soulring, ethereal, shiva, ghost, cyclone, forcestaff, glimmer, bottle, travel, veil, atos;
        private static Hero me, target;
        private static List<Hero> Alies;
		private static readonly Dictionary<Unit, ParticleEffect> VisibleUnit = new Dictionary<Unit, ParticleEffect>();
		private static readonly Dictionary<Unit, ParticleEffect> VisibleUnit2 = new Dictionary<Unit, ParticleEffect>();
		private static readonly Dictionary<Unit, ParticleEffect> VisibleUnit3 = new Dictionary<Unit, ParticleEffect>();
		private static readonly Dictionary<Unit, ParticleEffect> VisibleUnit4 = new Dictionary<Unit, ParticleEffect>();

        private static readonly List<ParticleEffect> Effects = new List<ParticleEffect>();
        private const string EffectPath = @"materials\ensage_ui\particles\other_range_blue.vpcf";        

        private static readonly Menu Menu = new Menu("TinkerFastComboPlus", "TinkerFastComboPlus", true, "npc_dota_hero_tinker", true).SetFontColor(Color.Aqua);
        private static readonly Menu _Combo = new Menu("Combo", "Combo");
        private static readonly Menu _RocketSpam = new Menu("Rocket Spam", "Rocket Spam");
        private static readonly Menu _MarchSpam = new Menu("March Spam", "March Spam");

        private static int red => Menu.Item("red").GetValue<Slider>().Value;
        private static int green => Menu.Item("green").GetValue<Slider>().Value;
        private static int blue => Menu.Item("blue").GetValue<Slider>().Value;

        private static bool BlockRearm => Menu.Item("BlockRearm").GetValue<bool>();
        private static bool NoBlockRearmFountain => Menu.Item("NoBlockRearmFountain").GetValue<bool>();
        private static bool NoBlockRearmTeleporting => Menu.Item("NoBlockRearmTeleporting").GetValue<bool>();

        private static bool FastRearmBlink => Menu.Item("FastRearmBlink").GetValue<KeyBind>().Active;

        private static readonly Dictionary<string, bool> ComboSkills = new Dictionary<string, bool>
            {
				{"tinker_rearm",true},
                {"tinker_march_of_the_machines",true},
                {"tinker_heat_seeking_missile",true},
                {"tinker_laser",true}
            };
        private static readonly Dictionary<string, bool> ComboItems = new Dictionary<string, bool>
            {
                {"item_blink",true},
                {"item_glimmer_cape",true},
                {"item_shivas_guard",true},
                {"item_bottle",true},
                {"item_soul_ring",true},
                {"item_veil_of_discord",true},
                {"item_rod_of_atos",true},
                {"item_sheepstick",true},
                {"item_ghost",true},
                {"item_ethereal_blade",true},
                {"item_dagon",true}
            };

        private static readonly Dictionary<string, bool> LinkenBreaker = new Dictionary<string, bool>
        {
            {"item_force_staff",true},
            {"item_cyclone",true},
            {"tinker_laser",true}
        };
            
        private static readonly Dictionary<string, bool> RocketSpamSkills = new Dictionary<string, bool>
        {
            {"tinker_rearm",true},
            {"tinker_heat_seeking_missile",true},
        };

        private static readonly Dictionary<string, bool> RocketSpamItems = new Dictionary<string, bool>
        {
            {"item_blink",false},
            {"item_glimmer_cape",true},
            {"item_bottle",true},
            {"item_soul_ring",true},
            {"item_ghost",true},
            {"item_ethereal_blade",false},
        };

        private static readonly Dictionary<string, bool> MarchSpamItems = new Dictionary<string, bool>
        {
            {"item_blink",false},
            {"item_glimmer_cape",false},
            {"item_bottle",true},
            {"item_soul_ring",true},
            {"item_ghost",false}
        };

        private static readonly string[] SoulringSpells = 
			{
            "tinker_heat_seeking_missile",
            "tinker_rearm",
            "tinker_march_of_the_machines"
			};			
			
        private static int[] laser_damage = new int[4] { 80, 160, 240, 320 };
		private static int[] rocket_damage = new int[4] { 125, 200, 275, 350 };	
		
        private static int[] laser_mana = new int[4] { 95, 120, 145, 170 };
		private static int[] rocket_mana = new int[4] { 120, 140, 160, 180 };	
		private static int[] rearm_mana = new int[3] { 100, 200, 300 };	
		
        private static int[] dagondistance = new int[5] { 600, 650, 700, 750, 800 };	
        private static int[] dagondamage = new int[5] { 400, 500, 600, 700, 800 };	

		private static int ensage_error = 50;

		private static int castrange = 0;
        private static double angle;
		
        private static ParticleEffect rangedisplay_dagger, rangedisplay_rocket, rangedisplay_laser;
		private static ParticleEffect effect2, effect3, effect4;
        private static ParticleEffect blinkeffect;

        private static int range_dagger, range_rocket, range_laser;
			
        static void Main(string[] args)
        {
			
            me = ObjectManager.LocalHero;
            if (me == null)
                return;
            if (me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;

            // Menu Options	                                                          
            _Combo.AddItem(new MenuItem("ComboSkills: ", "Skills:").SetValue(new AbilityToggler(ComboSkills)));
            _Combo.AddItem(new MenuItem("ComboItems: ", "Items:").SetValue(new AbilityToggler(ComboItems)));
            _Combo.AddItem(new MenuItem("LinkenBreaker: ", "Linken Breaker:").SetValue(new AbilityToggler(LinkenBreaker)));
            Menu.AddSubMenu(_Combo);

            _RocketSpam.AddItem(new MenuItem("RocketSpamSkills: ", "Skills:").SetValue(new AbilityToggler(RocketSpamSkills)));
            _RocketSpam.AddItem(new MenuItem("RocketSpamItems: ", "Items:").SetValue(new AbilityToggler(RocketSpamItems)));
            Menu.AddSubMenu(_RocketSpam);

            _MarchSpam.AddItem(new MenuItem("MarchSpamItems: ", "Items:").SetValue(new AbilityToggler(MarchSpamItems)));
            Menu.AddSubMenu(_MarchSpam);


            var _autopush = new Menu("Auto Push", "Auto Push");            
            _autopush.AddItem(new MenuItem("autoPush", "Enable auto push helper").SetValue(false));
            _autopush.AddItem(new MenuItem("autoRearm", "Enable auto rearm in fountain when travel boots on cooldown").SetValue(false));
            _autopush.AddItem(new MenuItem("pushFount", "Use auto push if I have modif Fountain").SetValue(false));
            _autopush.AddItem(new MenuItem("pushSafe", "Use march only after blinking to a safe spot").SetValue(false));
            Menu.AddSubMenu(_autopush);

            var _ranges = new Menu("Drawing", "Drawing");            
            _ranges.AddItem(new MenuItem("Blink Range", "Show Blink Dagger Range").SetValue(true));
            _ranges.AddItem(new MenuItem("Blink Range Incoming TP", "Show incoming TP Blink Range").SetValue(true));
            _ranges.AddItem(new MenuItem("Rocket Range", "Show Rocket Range").SetValue(true));
            _ranges.AddItem(new MenuItem("Laser Range", "Show Laser Range").SetValue(true));
            _ranges.AddItem(new MenuItem("Show Direction", "Show Direction Vector on Rearming").SetValue(true));
            _ranges.AddItem(new MenuItem("Show Target Effect", "Show Target Effect").SetValue(true));
            _ranges.AddItem(new MenuItem("red", "Red").SetValue(new Slider(0, 0, 255)).SetFontColor(Color.Red));
            _ranges.AddItem(new MenuItem("green", "Green").SetValue(new Slider(255, 0, 255)).SetFontColor(Color.Green));
            _ranges.AddItem(new MenuItem("blue", "Blue").SetValue(new Slider(255, 0, 255)).SetFontColor(Color.Blue));
            Menu.AddSubMenu(_ranges);

            var _blockrearm = new Menu("Blocker Rearm", "Blocker Rearm");
            Menu.AddSubMenu(_blockrearm);
            _blockrearm.AddItem(new MenuItem("BlockRearm", "Block Rearm").SetValue(true)).SetTooltip("It does not allow double-cast rearm");
            _blockrearm.AddItem(new MenuItem("NoBlockRearmFountain", "No Block Rearm in Fountain").SetValue(true));
            _blockrearm.AddItem(new MenuItem("NoBlockRearmTeleporting", "No Block Rearm with Teleporting").SetValue(true));

            var _settings = new Menu("Settings", "Settings UI");
            _settings.AddItem(new MenuItem("HitCounter", "Enable target hit counter").SetValue(true));
			_settings.AddItem(new MenuItem("RocketCounter", "Enable target rocket counter").SetValue(true));
			_settings.AddItem(new MenuItem("TargetCalculator", "Enable target dmg calculator").SetValue(true));			
			_settings.AddItem(new MenuItem("Calculator", "Enable UI calculator").SetValue(true));
            _settings.AddItem(new MenuItem("BarPosX", "UI Calculator Position X").SetValue(new Slider(600, -1500, 1500)));
            _settings.AddItem(new MenuItem("BarPosY", "UI Calculator Position Y").SetValue(new Slider(0, -1500, 1500)));		
			_settings.AddItem(new MenuItem("CalculatorRkt", "Enable Rocket calculator").SetValue(true));
            _settings.AddItem(new MenuItem("BarPosXr", "Rocket Calc Position X").SetValue(new Slider(950, -1500, 1500)));
            _settings.AddItem(new MenuItem("BarPosYr", "Rocket Calc Position Y").SetValue(new Slider(-300, -1500, 1500)));
            _settings.AddItem(new MenuItem("ComboModeDrawing", "Enable Combo Mode drawing").SetValue(true));
            _settings.AddItem(new MenuItem("debug", "Enable debug").SetValue(false));
            Menu.AddSubMenu(_settings);
            
            Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("ComboMode", "Combo Mode")).SetValue(new StringList(new[] { "Fast", "MpSaving" }));
            Menu.AddItem(new MenuItem("TargetLock", "Target Lock")).SetValue(new StringList(new[] { "Free", "Lock" }));
            Menu.AddItem(new MenuItem("Chase", "Chase Toggle").SetValue(new KeyBind('F', KeyBindType.Toggle, false)).SetTooltip("Toggle for chasing"));

            Menu.AddItem(new MenuItem("Rocket Spam Key", "Rocket Spam Key").SetValue(new KeyBind('W', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("March Spam Key", "March Spam Key").SetValue(new KeyBind('E', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("FastRearmBlink", "Fast Rearm Blink").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu.AddItem(new MenuItem("autoDisable", "Auto disable/counter enemy").SetValue(true));
            Menu.AddItem(new MenuItem("autoKillsteal", "Auto killsteal enemy").SetValue(true));
            //Menu.AddItem(new MenuItem("autoSoulring", "Auto SoulRing by manual spell usage").SetValue(true).SetTooltip("Disable it if you have some bugs with rearming or use other auto soulring/items assemblies"));

            Menu.AddToMainMenu();

            Orbwalking.Load();


            //Game.OnWndProc += ComboEngine;
            Game.OnUpdate += ComboEngine;
			Game.OnUpdate += AD;

            GameDispatcher.OnUpdate += OnUpdate;

            //Player.OnExecuteOrder += Player_OnExecuteAction;
            Player.OnExecuteOrder += OnExecuteOrder;

            Drawing.OnDraw += Information;
			Drawing.OnDraw += DrawRanges;
            Drawing.OnDraw += ParticleDraw;
        }
        public static async void OnUpdate(EventArgs args)
        {           
            if (FastRearmBlink && Utils.SleepCheck("updateAdd"))
            {
                var safeRange = 1200 + castrange;
                var blinkparticlerange = Game.MousePosition;

                if (me.Distance2D(Game.MousePosition) > safeRange + ensage_error)
                {
                    var tpos = me.Position;
                    var a = tpos.ToVector2().FindAngleBetween(Game.MousePosition.ToVector2(), true);

                    safeRange -= (int)me.HullRadius;
                    blinkparticlerange = new Vector3(
                        tpos.X + safeRange * (float)Math.Cos(a),
                        tpos.Y + safeRange * (float)Math.Sin(a),100);
                }
                blinkeffect?.Dispose();
                blinkeffect = new ParticleEffect("materials/ensage_ui/particles/tinker_blink.vpcf", blinkparticlerange);
                blinkeffect.SetControlPoint(1, new Vector3(0, 255, 255));
                blinkeffect.SetControlPoint(2, new Vector3(255));
                Effects.Add(blinkeffect);
                Utils.Sleep(2000, "updateAdd");
            }
            else if (FastRearmBlink && Utils.SleepCheck("updateRemover"))
            {
                DelayAction.Add(time, () =>
                {
                    blinkeffect?.Dispose();                
                });
                Utils.Sleep(2000, "updateRemover");
            }
            if (RearmBlink != null && !RearmBlink.IsCompleted)
            {
                return;
            }
            if (FastRearmBlink)
            {
                tks = new CancellationTokenSource();
                RearmBlink = Action(tks.Token);
                try
                {
                    await RearmBlink;
                    RearmBlink = null;
                }
                catch (OperationCanceledException)
                {
                    RearmBlink = null;
                }
            }
        }
        
        private static async Task Action(CancellationToken cancellationToken)
        {
            {
                var rearm = me.Spellbook().SpellR;
                var blink = me.FindItem("item_blink");
                if (Utils.SleepCheck("FASTBLINK"))
                {                    
                    me.MoveToDirection(Game.MousePosition);
                    Utils.Sleep(100, "FASTBLINK");
                }
                var blinkrange = 1200 + castrange;
                var fastblink = Game.MousePosition;
                if (rearm.CanBeCasted())
                {
                    DelayAction.Add(50, () =>
                    {
                        if (me.Distance2D(Game.MousePosition) > blinkrange + ensage_error)
                        {
                            var tpos = me.Position;
                            var a = tpos.ToVector2().FindAngleBetween(Game.MousePosition.ToVector2(), true);

                            blinkrange -= (int)me.HullRadius;
                            fastblink = new Vector3(
                                tpos.X + blinkrange * (float)Math.Cos(a),
                                tpos.Y + blinkrange * (float)Math.Sin(a),
                                100);
                        }
                        rearm?.UseAbility();
                    });
                    time = (int)(GetRearmTime(rearm) + Game.Ping + 50 + rearm.FindCastPoint() * 1000);
                    await Task.Delay(time, cancellationToken);
                }
                blink?.UseAbility(fastblink);
                await Task.Delay(0, cancellationToken);

                blink?.UseAbility(fastblink);
                await Task.Delay(10, cancellationToken);

                blink?.UseAbility(fastblink);
                await Task.Delay(20, cancellationToken);

                blink?.UseAbility(fastblink);
                await Task.Delay(30, cancellationToken);

                blink?.UseAbility(fastblink);
                await Task.Delay(50, cancellationToken);
            }                                   
        }
        private static void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!BlockRearm) return;
            {
                if ((!me.HasModifier("modifier_fountain_aura_buff") || !NoBlockRearmFountain) && (!me.HasModifier("modifier_teleporting") || !NoBlockRearmTeleporting))
                {
                    if (args.Ability?.Name == "tinker_rearm" && args.OrderId == OrderId.Ability &&             
                        (me.IsChanneling() || args.Ability.IsInAbilityPhase))
                    {
                        args.Process = false;
                    }
                }                
            }            
        }
        
        /*private static void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args) 
		{
            me = ObjectManager.LocalHero;
            if (me == null)
                return;
            if (me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;
		
            switch (args.OrderId) {

                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                case OrderId.ToggleAbility:
                    if (!Game.IsKeyDown(16))
                        CastSpell(args);
                    break;
                case OrderId.MoveLocation:
                case OrderId.MoveTarget:
                default:
                    break;
            }
        }	

		private static void CastSpell(ExecuteOrderEventArgs args) 
		{
            var spell = args.Ability;
            if (!SoulringSpells.Any(spell.StoredName().Equals))
                return;			
			
            var soulring = me.FindItem("item_soul_ring");
            var bottle = me.FindItem("item_bottle");
			
            //if (!Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
            //    return;
				
            if (soulring == null)
                return;

			if (!Menu.Item("autoSoulring").GetValue<bool>())
				return;

            args.Process = false;
			


            switch (args.OrderId) 
			{
				
                case OrderId.AbilityTarget: 
				{
                    var target = args.Target as Unit;
                    if (target != null && target.IsAlive) {
                        spell.UseAbility(target);
                    }
                    break;
                }
                case Order.AbilityLocation: 
				{
			
					if (soulring != null && soulring.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)) 
						soulring.UseAbility();		
					if (bottle != null && bottle.CanBeCasted() && !me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(bottle.Name) )
						bottle.UseAbility();
                    spell.UseAbility(Game.MousePosition);
                    break;
                }
                case Order.Ability: 
				{
					if (soulring != null && soulring.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)) 
						 soulring.UseAbility();				
                    spell.UseAbility();
                    break;
                }
                case Order.ToggleAbility: 
				{
                    spell.ToggleAbility();
                    break;
                }
            }
        }*/

        public static void ComboEngine(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null)
                return;
            if (me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;

            List<Unit> fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain).ToList();
            var creeps = ObjectManager.GetEntities<Creep>().Where(creep =>
                   (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
                   creep.IsAlive && creep.Team != me.Team && creep.IsVisible && creep.IsSpawned).ToList();

            Vector3 safe = GetClosestToVector(TinkerCords.SafePos, me);

            //Castrange Calculation (Tinker Talent20 and Aether Lens)
            castrange = 0;

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);

            if (aetherLens != null)
            {
                castrange += (int)aetherLens.AbilitySpecialData.First(x => x.Name == "cast_range_bonus").Value;
            }

            var talent20 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_cast_range_75");
            if (talent20.Level > 0)
            {
                castrange += (int)talent20.AbilitySpecialData.First(x => x.Name == "value").Value;
            }

            //Print safespots into console
            /*if(Game.IsKeyDown(new KeyBind('O', KeyBindType.Press).Key) && !Game.IsChatOpen)
            {
                Console.WriteLine(me.Position.ToString());
                Utils.Sleep(1000, "Safeposition");
            }*/

            //Auto Push
            // ghost -> glimmer -> march -> move or blink to safe -> soulring -> rearm
            if (Menu.Item("autoPush").IsActive()
                && !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)
                && !Game.IsKeyDown(Menu.Item("Rocket Spam Key").GetValue<KeyBind>().Key)
                && !Game.IsKeyDown(Menu.Item("March Spam Key").GetValue<KeyBind>().Key)
                && !Game.IsChatOpen
                && me.IsAlive)
            {
                FindItems();

                if ((me.HasModifier("modifier_fountain_aura_buff") && Menu.Item("pushFount").IsActive()))
                {
                    if (me.IsChanneling() || me.HasModifier("modifier_tinker_rearm") || Refresh == null) return;

                    if (creeps.Count(x => x.Distance2D(me) <= 1100) >= 1)
                    {
                        if (ghost != null
                            && ghost.CanBeCasted()
                            //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                            && Utils.SleepCheck("ghost"))
                        {
                            ghost.UseAbility();
                            Utils.Sleep(250, "ghost");
                        }

                        /*
                        if (lotus != null
                            && lotus.CanBeCasted()
                            && creeps.Count(x => x.Distance2D(me) <= 1100) >= 2
                            //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name)
                            && Utils.SleepCheck("lotus"))
                        {
                            lotus.UseAbility(me);
                            Utils.Sleep(250, "lotus");
                        }*/

                        if (
                            glimmer != null
                            && glimmer.CanBeCasted()
                            && creeps.Count(x => x.Distance2D(me) <= 1100) >= 2
                            && me.Distance2D(safe) >= HIDE_AWAY_RANGE
                            //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
                            && Utils.SleepCheck("glimmer"))
                        {
                            glimmer.UseAbility(me);
                            Utils.Sleep(250, "glimmer");
                        }

                        if (March != null && March.CanBeCasted()
                            && !Refresh.IsChanneling
                            && (me.Distance2D(safe) <= HIDE_AWAY_RANGE
                            || !Menu.Item("pushSafe").IsActive())
                            && creeps.Count(x => x.Distance2D(me) <= 900) >= 2
                            //&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                            && Utils.SleepCheck("March")
                          )
                        {
                            var closestCreep = ObjectManager.GetEntities<Creep>().Where(creep =>
                                   (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
                                   creep.IsAlive && creep.Team != me.Team && creep.IsVisible && creep.IsSpawned).MinOrDefault(x => x.Distance2D(me)).Position;

                            March.UseAbility(Vector3.Add(me.Position, Vector3.Multiply(Vector3.Subtract(closestCreep, me.Position), 0.1f)));
                            //Console.WriteLine("March1");
                            Utils.Sleep(500, "March");
                        }

                        if (March != null && March.CanBeCasted()
                            && !Refresh.IsChanneling
                            && (creeps.Count(x => x.Distance2D(safe) <= 900) <= 1 || me.Distance2D(safe) >= 1190 + castrange)
                            && creeps.Count(x => x.Distance2D(me) <= 900) >= 2
                            //&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                            && Utils.SleepCheck("March")
                          )
                        {
                            var closestCreep = ObjectManager.GetEntities<Creep>().Where(creep =>
                                   (creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral
                                   || creep.ClassId == ClassId.CDOTA_BaseNPC_Creep) &&
                                   creep.IsAlive && creep.Team != me.Team && creep.IsVisible && creep.IsSpawned).MinOrDefault(x => x.Distance2D(me)).Position;
                            March.UseAbility(Vector3.Add(me.Position, Vector3.Multiply(Vector3.Subtract(closestCreep, me.Position), 0.1f)));
                            //Console.WriteLine("March2");
                            Utils.Sleep(500, "March");
                        }

                        if (
                         March != null && !March.CanBeCasted()
                         && !Refresh.IsChanneling
                         && me.Distance2D(safe) >= (1190 + castrange)
                         //&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                         && Utils.SleepCheck("March")
                         )
                        {
                            me.Move(safe);
                            Utils.Sleep(500, "March");
                        }

                        if (
                        blink != null
                        && me.CanCast()
                        && (Menu.Item("pushSafe").IsActive()
                        || !March.CanBeCasted())
                        && !Refresh.IsChanneling
                        && blink.CanBeCasted()
                        )
                        {
                            if (me.Distance2D(safe) <= (1190 + castrange)
                                && me.Distance2D(safe) >= 100
                                && Utils.SleepCheck("blink"))
                            {
                                blink.UseAbility(safe);
                                Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                                Utils.Sleep(500, "blink");
                            }
                        }

                    }

                    if (soulring != null
                        && soulring.CanBeCasted()
                        && !me.IsChanneling()
                        && me.Health >= (me.MaximumHealth * 0.5)
                        //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                        && Utils.SleepCheck("soulring"))
                    {
                        soulring.UseAbility();
                        Utils.Sleep(250, "soulring");
                    }

                    if (Refresh != null
                        && Refresh.CanBeCasted()
                        && travel != null
                        && !travel.CanBeCasted()
                        && me.Distance2D(fount.First().Position) <= 900
                        && !me.IsChanneling()
                        //&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Refresh.Name)
                        && Utils.SleepCheck("Rearms"))
                    {
                        Refresh.UseAbility();
                        if (Refresh.Level == 1)
                            Utils.Sleep(3010, "Rearms");
                        if (Refresh.Level == 2)
                            Utils.Sleep(1510, "Rearms");
                        if (Refresh.Level == 3)
                            Utils.Sleep(760, "Rearms");
                    }
                }

                if (Refresh.IsChanneling || me.HasModifier("modifier_tinker_rearm") || me.IsChanneling()) return;

                if (me.Distance2D(safe) >= 150) return;

                if (soulring != null
                            && soulring.CanBeCasted()
                            && !Refresh.IsChanneling
                            && me.Health >= (me.MaximumHealth * 0.5)
                            && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                            && Utils.SleepCheck("soul"))
                {
                    soulring.UseAbility();
                    Utils.Sleep(250, "soul");
                    travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }

                if (
                    travel != null
                    && travel.CanBeCasted()
                    && !Refresh.IsChanneling
                    //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled("item_travel_boots")
                    && me.Mana <= Refresh.ManaCost + 75
                    && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                    && Utils.SleepCheck("travel")
                   )
                {
                    travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }

                if (
                    travel != null
                    && travel.CanBeCasted()
                    && creeps.Count(x => x.Distance2D(me) <= 1100) <= 2
                    && !Refresh.IsChanneling
                    //&& Menu.Item("Push Items").GetValue<AbilityToggler>().IsEnabled("item_travel_boots")
                    && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                    && Utils.SleepCheck("travel")
                   )
                {
                    travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }
                else
                if (
                    Refresh != null
                    && Refresh.CanBeCasted()
                    && !March.CanBeCasted()
                    && creeps.Count(x => x.Distance2D(me) >= 1100) >= 2
                    && !Refresh.IsChanneling
                    && me.Mana >= Refresh.ManaCost + 75
                    && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                    //&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Refresh.Name)
                    && Utils.SleepCheck("Rearms")
                   )
                {
                    Refresh.UseAbility();
                    if (Refresh.Level == 1)
                        Utils.Sleep(3010, "Rearms");
                    if (Refresh.Level == 2)
                        Utils.Sleep(1510, "Rearms");
                    if (Refresh.Level == 3)
                        Utils.Sleep(760, "Rearms");
                }
            }

            //Rocket Spam Mode
            if (Game.IsKeyDown(Menu.Item("Rocket Spam Key").GetValue<KeyBind>().Key) 
                && Utils.SleepCheck("RocketSpam") 
                && !Game.IsChatOpen)
            {
				FindItems();

				if (blink != null && blink.CanBeCasted() && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                    && !me.IsChanneling()  
					&& Utils.SleepCheck("Rearms") 
					&& (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture") || (me.Distance2D(Game.MousePosition)>1325 && castrange!=0))
					&& (me.Distance2D(Game.MousePosition) > 650 + castrange + ensage_error)
                    && Utils.SleepCheck("Blinks"))
				{
                    var safeRange = 1200 + castrange;
					var p = Game.MousePosition;
					
					if (me.Distance2D(Game.MousePosition) > safeRange)
					{
						var tpos = me.Position;
						var a = tpos.ToVector2().FindAngleBetween(Game.MousePosition.ToVector2(), true);
						
						safeRange -= (int)me.HullRadius;
						p = new Vector3(
							tpos.X + safeRange * (float)Math.Cos(a),
							tpos.Y + safeRange * (float)Math.Sin(a),
							100);
					}
					else p = Game.MousePosition;				
				
					blink.UseAbility(p);
                    Utils.Sleep(50, "Blinks");
				}
               						
				/*
				if (soulring != null && soulring.CanBeCasted() && !me.IsChanneling() && (blink!=null && me.Distance2D(Game.MousePosition) > 650+ castrange  + ensage_error) && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name) && Utils.SleepCheck("Rearms"))
				{
					soulring.UseAbility();
				}
				*/
                if (bottle != null 
                    && bottle.CanBeCasted() 
                    && !me.IsChanneling() 
                    && (blink==null || (blink!=null && me.Distance2D(Game.MousePosition) <= 650+ castrange  + ensage_error)) 
                    && !me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") 
                    && (me.MaximumMana-me.Mana)>60 && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(bottle.Name) 
                    && Utils.SleepCheck("Rearms"))
				{
					bottle.UseAbility();
				}
				
				/*
				if (ghost != null && ghost.CanBeCasted() && !me.IsChanneling() && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) && Utils.SleepCheck("Rearms"))
				{
					ghost.UseAbility(false);
				}
				*/

				var enemies = ObjectManager.GetEntities<Hero>().Where(x => x.IsVisible && x.IsAlive && x.Team == me.GetEnemyTeam() && !x.IsIllusion);
				foreach (var e in enemies)
				{
					if ((Rocket != null && Rocket.CanBeCasted() || (soulring.CanBeCasted() && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
                        || !Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name))
                        &&  me.Distance2D(e) < 2500 
						//&& (blink == null || !blink.CanBeCasted() || me.Distance2D(Game.MousePosition) <= 650+ castrange + ensage_error || (me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture") && (me.Distance2D(Game.MousePosition)<=1325 || castrange==0)))                       
						&& !me.IsChanneling() 
						&& !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)  
						&& Utils.SleepCheck("Rearms")
						//&& me.Mana >= Rocket.ManaCost + 75 
						)
					{
                        if (soulring != null
                            && soulring.CanBeCasted() 
                            && !me.IsChanneling() 
                            && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
                            && (soulring.CanBeCasted() || Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
                            && Utils.SleepCheck("Rearms"))
						{
							soulring.UseAbility();
						}

                        if (ghost != null && ghost.CanBeCasted()              
                            && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                            && (ghost.CanBeCasted() || Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
                            && Utils.SleepCheck("Rearms"))
                        {
                            ghost.UseAbility();
                        }

                        if (ethereal != null && ghost == null && ethereal.CanBeCasted()               
                            && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                            && (ethereal.CanBeCasted() || Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
                            && !me.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal")
                            && Utils.SleepCheck("Rearms"))
                        {
                            ethereal.UseAbility(me);
                        }

                        if (glimmer != null && glimmer.CanBeCasted() 
                            && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
                            && (glimmer.CanBeCasted() || Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
                            && !me.Modifiers.Any(y => y.Name == "modifier_invisible")
                            && Utils.SleepCheck("Rearms"))
                        {
                            glimmer.UseAbility(me);
                        }  
                        else                       
                         if (Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name)
                            && (Rocket.CanBeCasted() || Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name)))
                        {
                            Rocket.UseAbility();
                        }                      
					}
                   
                    if ((soulring == null || !soulring.CanBeCasted() || !Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)) 
                         
                        && me.Distance2D(e) <= 2500 
                        && (!Rocket.CanBeCasted() || Rocket.Level <= 0 || !Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name))  
                        //&& (blink == null || !blink.CanBeCasted() || me.Distance2D(Game.MousePosition) <= 650+ castrange  + ensage_error) 
                        && (Refresh.Level >= 0 && Refresh.CanBeCasted()) 
                        && !me.IsChanneling()
                        && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) 
                        && Utils.SleepCheck("Rearms") 
                        && Utils.SleepCheck("Blinks"))
					{
                        if (Menu.Item("RocketSpamSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name))
                        {
                            Refresh.UseAbility();
                            if (Refresh.Level == 1)
                                Utils.Sleep(3010, "Rearms");
                            if (Refresh.Level == 2)
                                Utils.Sleep(1510, "Rearms");
                            if (Refresh.Level == 3)
                                Utils.Sleep(760, "Rearms");
                        }
                                                                       
					}                                      
                }

                //if ((soulring == null || !soulring.CanBeCasted() || !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)) && (blink != null && me.Distance2D(Game.MousePosition) > 650+ castrange + ensage_error) && (Refresh.Level >= 0 && Refresh.CanBeCasted()) && !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) && Utils.SleepCheck("Rearms") && Utils.SleepCheck("Blinks"))           
                if ((blink != null && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)                  
                    && me.Distance2D(Game.MousePosition) > 650 + castrange + ensage_error)
                    && (Refresh.Level >= 0 && Refresh.CanBeCasted())
                    && (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture") || (me.Distance2D(Game.MousePosition) > 1325 && castrange != 0))
                    && !me.IsChanneling()
                    && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
                    && Utils.SleepCheck("Rearms")
                    && Utils.SleepCheck("Blinks"))
                {
                    if (soulring != null                 
                        && soulring.CanBeCasted()          
                        && !me.IsChanneling()     
                        && (blink != null && me.Distance2D(Game.MousePosition) > 650 + castrange + ensage_error) 
                        && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)       
                        && Utils.SleepCheck("Rearms"))
                    {
                        soulring.UseAbility();
                    }

                    Refresh.UseAbility();
                    if (Refresh.Level == 1)
                        Utils.Sleep(3010, "Rearms");
                    if (Refresh.Level == 2)
                        Utils.Sleep(1510, "Rearms");
                    if (Refresh.Level == 3)
                        Utils.Sleep(760, "Rearms");
                }
                
                				
				if ((blink==null || (blink!=null && me.Distance2D(Game.MousePosition) <= 650+ castrange  + ensage_error) && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)) 
				&& !me.IsChanneling() 
				&& !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
				&& !me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture")
				&& Utils.SleepCheck("Rearms")) 
				{
					me.Move(Game.MousePosition);
				}

                if (blink!=null
                    && !me.IsChanneling()
                    && !Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)            
                    && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)            
                    && !me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture")              
                    && Utils.SleepCheck("Rearms"))
                {
                    me.Move(Game.MousePosition);
                }
                if (Utils.SleepCheck("Autoattack"))
                {
                    Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                    Utils.Sleep(10000, "Autoattack");
                }           
                Utils.Sleep(120, "RocketSpam");
			}
			
            //March Spam Mode
			if (Game.IsKeyDown(Menu.Item("March Spam Key").GetValue<KeyBind>().Key) && Utils.SleepCheck("MarchSpam") && !Game.IsChatOpen)
            {
				FindItems();                
                if (blink != null && blink.CanBeCasted() 
					&& Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name) 
					&& !me.IsChanneling()  
					&& Utils.SleepCheck("Rearms") 
					&& (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture") || (me.Distance2D(Game.MousePosition)>1325 && castrange!=0))
					&& (me.Distance2D(Game.MousePosition) > 650+ castrange  + ensage_error)
					)
				{
                    var safeRange = 1200 + castrange;
					var p = Game.MousePosition;
					
					if (me.Distance2D(Game.MousePosition) > safeRange)
					{
						var tpos = me.Position;
						var a = tpos.ToVector2().FindAngleBetween(Game.MousePosition.ToVector2(), true);
						
						safeRange -= (int)me.HullRadius;
						p = new Vector3(
							tpos.X + safeRange * (float)Math.Cos(a),
							tpos.Y + safeRange * (float)Math.Sin(a),
							100);
					}
					else p = Game.MousePosition;

                    blink.UseAbility(p);
                    Utils.Sleep(250, "Blinks");
				}
				
				
				/*
				if (ghost != null && ghost.CanBeCasted() && !me.IsChanneling() && Menu.Item("RocketSpamItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) && Utils.SleepCheck("Rearms"))
				{
					ghost.UseAbility(false);
				}
				*/
				if (soulring != null 
                    && soulring.CanBeCasted() 
                    && !me.IsChanneling() 
                    && Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name) 
                    && Utils.SleepCheck("Rearms"))
				{
					soulring.UseAbility();
				}

                if (ghost != null
                    && ghost.CanBeCasted()
                    && !me.IsChanneling()
                    && Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                    && Utils.SleepCheck("Rearms"))
                {
                    ghost.UseAbility();
                }

                if (bottle != null
                    && bottle.CanBeCasted() 
                    && !me.IsChanneling() 
                    && !me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") 
                    && Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled(bottle.Name) 
                    && Utils.SleepCheck("Rearms"))
				{
					bottle.UseAbility();
				}

				if (March != null 
                    && March.CanBeCasted() 
                    && (blink == null || !blink.CanBeCasted() || me.Distance2D(Game.MousePosition) <= 650+ castrange + ensage_error || !Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled("item_blink")) 
                    && !me.IsChanneling()                      
                    && Utils.SleepCheck("Rearms")) //&& me.Mana >= March.ManaCost + 75 
				{
					March.UseAbility(Game.MousePosition);
				}                
                if ((soulring == null || !soulring.CanBeCasted() || !Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)) 
                    && (blink == null || !blink.CanBeCasted() || me.Distance2D(Game.MousePosition) <= 650+ castrange + ensage_error || !Menu.Item("MarchSpamItems: ").GetValue<AbilityToggler>().IsEnabled("item_blink"))
                    && (!March.CanBeCasted()  || March.Level <= 0)
                    && (Refresh.Level >= 0 && Refresh.CanBeCasted()) 
                    && !me.IsChanneling() 
                    && Utils.SleepCheck("Rearms"))
				{
					Refresh.UseAbility();
					if (Refresh.Level == 1)
						Utils.Sleep(3010, "Rearms");
					if (Refresh.Level == 2)
						Utils.Sleep(1510, "Rearms");
					if (Refresh.Level == 3)
						Utils.Sleep(760, "Rearms");
				}
                if (Utils.SleepCheck("Autoattack"))
                {
                    Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                    Utils.Sleep(10000, "Autoattack");
                } 
                /*else if ()
                {
                    me.Move(Game.MousePosition);
                }  */             
                Utils.Sleep(150, "MarchSpam");
			}
				
            //Combo Mode
			if (!Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
                target = null;
           	
            if ((Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) 
                && (!Menu.Item("Chase").GetValue<KeyBind>().Active) 
                && !Game.IsChatOpen)
            {
                //target = me.ClosestToMouseTarget(2000);
				
				var targetLock =
					Menu.Item("TargetLock").GetValue<StringList>().SelectedIndex;

                if (Utils.SleepCheck("UpdateTarget")
                    && (target == null || !target.IsValid || !target.IsAlive || !target.IsVisible || (target.IsVisible && targetLock == 0)))
                {
                    target = TargetSelector.ClosestToMouse(me, 2000);
                    Utils.Sleep(250, "UpdateTarget");
                }
				
                if (target != null 
                    && target.IsAlive 
                    && !target.IsIllusion 
                    && !me.IsChanneling() 
                    && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase))
                {
                    FindItems();

					if (Utils.SleepCheck("FASTCOMBO"))
					{
						uint elsecount = 0;
						//bool EzkillCheck = EZKill(target);
						bool magicimune = (!target.IsMagicImmune() && !target.Modifiers.Any(x => x.Name == "modifier_eul_cyclone"));
						// soulring -> glimmer -> sheep -> veil-> ghost ->  ->   -> ethereal -> dagon ->  laser -> rocket -> shivas 

						if (soulring != null && soulring.CanBeCasted() 
							&& target.NetworkPosition.Distance2D(me) <= 2500
							&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)  )
						{
							soulring.UseAbility();
						}
						else
                        {
                            elsecount += 1;	
                        }
													
						if (glimmer != null && glimmer.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name) )
						{
							glimmer.UseAbility(me);
						}
						else
                        {
                            elsecount += 1;
                        }
							
						/*
                        if (blink != null && blink.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name) && !me.IsChanneling())
                        {
                            blink.UseAbility(Game.MousePosition);
                        }*/
						if (blink != null && blink.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name) 
							&& !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
							&& (me.Distance2D(Game.MousePosition) > 650+castrange + ensage_error)  
							&& (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture") || (me.Distance2D(Game.MousePosition)>1325 && castrange!=0))
							&& (target.NetworkPosition.Distance2D(me) <= 1200 + 650 + ensage_error*2 +castrange*2)
							&& Utils.SleepCheck("Blinks")
							// && Utils.SleepCheck("Rearms"))
                            )
						{
                            var safeRange = 1200 + castrange;
							var p13 = Game.MousePosition;
							
							if (me.Distance2D(Game.MousePosition) > safeRange + ensage_error)
							{
								var tpos = me.Position;
								var a = tpos.ToVector2().FindAngleBetween(Game.MousePosition.ToVector2(), true);
								
								safeRange -= (int)me.HullRadius;
								p13 = new Vector3(
									tpos.X + safeRange * (float)Math.Cos(a),
									tpos.Y + safeRange * (float)Math.Sin(a),
									100);
							}
							else p13 = Game.MousePosition;				
						
							blink.UseAbility(p13);
							Utils.Sleep(200, "Blinks");
						}

						/*
						if (blink != null && blink.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(blink.Name) && !me.IsChanneling()  &&  me.NetworkPosition.Distance2D(target.NetworkPosition) > 600+castrange)// && Utils.SleepCheck("Rearms"))
						{
							var safeRange = me.FindItem("item_aether_lens") == null ? 1200 : 1420;
							var closeRange = me.FindItem("item_aether_lens") == null ? 600 : 820;
							var p13 = Game.MousePosition;
							
							if (me.NetworkPosition.Distance2D(target.NetworkPosition) > safeRange + closeRange)
							{
								var tpos = me.NetworkPosition;
								var a = tpos.ToVector2().FindAngleBetween(target.NetworkPosition.ToVector2(), true);
								
								safeRange -= (int)me.HullRadius;
								p13 = new Vector3(
									tpos.X + (safeRange + closeRange)  * (float)Math.Cos(a),
									tpos.Y + (safeRange + closeRange) * (float)Math.Sin(a),
									100);
							}
							else 
							{
								var tpos = me.NetworkPosition;
								var a = tpos.ToVector2().FindAngleBetween(target.NetworkPosition.ToVector2(), true);
								var uncloseRange = me.NetworkPosition.Distance2D(target.NetworkPosition) - closeRange;
								
								safeRange -= (int)me.HullRadius;
								p13 = new Vector3(
									tpos.X + uncloseRange * (float)Math.Cos(a),
									tpos.Y + uncloseRange * (float)Math.Sin(a),
									100);							
							}
							blink.UseAbility(p13);
							//Utils.Sleep(250, "Blinks");

						}*/		
                        				
						else
                        {
                            elsecount += 1;
                        }
							
						if (!me.IsChanneling() 
							&& me.CanAttack() 
							&& !target.IsAttackImmune() 
							&& !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
							&& OneHitLeft(target)
							&& target.NetworkPosition.Distance2D(me) <= me.GetAttackRange()+50
							//&& Utils.SleepCheck("attack")	
							//&& Utils.SleepCheck("Blinks")
							)
						{
							me.Attack(target);
							//Orbwalking.Orbwalk(target);
							//Utils.Sleep(250, "attack");
						}
						else
                        {
                            elsecount += 1;
                        }

						if (target.IsLinkensProtected() 
                            && Utils.SleepCheck("combo2"))
						{
							if (forcestaff != null && forcestaff.CanBeCasted() && Menu.Item("LinkenBreaker: ").GetValue<AbilityToggler>().IsEnabled(forcestaff.Name)) 
                                forcestaff.UseAbility(target);	
							else if (cyclone != null && cyclone.CanBeCasted() && Menu.Item("LinkenBreaker: ").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
								cyclone.UseAbility(target);
							else if (Laser.Level >= 1 && Laser.CanBeCasted() && Menu.Item("LinkenBreaker: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name))
								Laser.UseAbility(target);
								
							Utils.Sleep(200, "combo2");

						}
						else
						{


                            /*
							if (sheep != null && sheep.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name) && magicimune   )
							{
								sheep.UseAbility(target);
							}
							else
								elsecount += 1;
							*/
                            if (atos != null && atos.CanBeCasted()

                                && magicimune
                                && target.NetworkPosition.Distance2D(me) <= 1150 + castrange + ensage_error
                                && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                                && Utils.SleepCheck("Blinks"))
                                atos.UseAbility(target);
                            else
                                elsecount += 1;
                            

                            if (sheep != null && sheep.CanBeCasted() 
								//&& !target.UnitState.HasFlag(UnitState.Hexed) 
								//&& !target.UnitState.HasFlag(UnitState.Stunned) 
								&& magicimune 
								//&& ((target.FindItem("item_manta") != null && target.FindItem("item_manta").CanBeCasted()) || (target.FindItem("item_black_king_bar") != null && target.FindItem("item_black_king_bar").CanBeCasted()))
								&& target.NetworkPosition.Distance2D(me) <= 800+castrange + ensage_error
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
								&& Utils.SleepCheck("Blinks"))
								sheep.UseAbility(target);
							else
								elsecount += 1;								/*
							if(veil != null && veil.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name) )
							{
								veil.UseAbility(target.Position);
							}
							else
								elsecount += 1;		*/
							if (veil != null && veil.CanBeCasted() 
								&& magicimune
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)
								&& target.NetworkPosition.Distance2D(me) <= 1600+castrange + ensage_error
								//&& !OneHitLeft(target)
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
								&& !target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff")
								&& Utils.SleepCheck("Blinks"))
								{
									if (me.Distance2D(target) > 1000 + castrange + ensage_error)
									{
										var a = me.Position.ToVector2().FindAngleBetween(target.Position.ToVector2(), true);
										var p1 = new Vector3(
											me.Position.X + (me.Distance2D(target) - 500) * (float)Math.Cos(a),
											me.Position.Y + (me.Distance2D(target) - 500) * (float)Math.Sin(a),
											100);
										veil.UseAbility(p1);
									}
									else if (me.Distance2D(target) <= 1000 + castrange + ensage_error)
										veil.UseAbility(target.NetworkPosition);
								}

							else
								elsecount += 1;		
								
							if (ghost != null && ethereal == null && ghost.CanBeCasted() 
								&& target.NetworkPosition.Distance2D(me) <= 800+castrange + ensage_error
								&& !OneHitLeft(target)
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name) )
							{
								ghost.UseAbility();
							}
							else
								elsecount += 1;

								
							var comboMode =
								Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex;
							if (Rocket.Level > 0 && Rocket.CanBeCasted()
                                && target.NetworkPosition.Distance2D(me) <= 2500
								//&& (!EzkillCheck)// || target.NetworkPosition.Distance2D(me) >= 800+castrange + ensage_error)
								&& !OneHitLeft(target)
								&& magicimune  
								&& (!target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect") || me.IsMagicImmune())
								&& (!target.Modifiers.Any(y => y.Name == "modifier_nyx_assassin_spiked_carapace") || me.IsMagicImmune())
								&& (((veil == null 
                                    || !veil.CanBeCasted() 
                                    || target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") 
                                    | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))))// && target.NetworkPosition.Distance2D(me) <= 1600 + castrange + ensage_error))// || target.NetworkPosition.Distance2D(me) > 1600 + castrange + ensage_error)
								&& (((ethereal == null 
                                    || (ethereal!=null && !ethereal.CanBeCasted()) 
                                    || IsCasted(ethereal) /*|| target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal")*/ 
                                    | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))))//&& target.NetworkPosition.Distance2D(me) <= 800+castrange + ensage_error)) //|| target.NetworkPosition.Distance2D(me) > 800+castrange + ensage_error)
								&& (Laser == null ||  !Laser.CanBeCasted() || comboMode==0)
								&& (dagon == null ||  !dagon.CanBeCasted() || comboMode==0)
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                && Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name)
                                && Utils.SleepCheck("Blinks"))
							{
                                Rocket.UseAbility();
							}
							else
								elsecount += 1;
						
						

							/*
							if (ethereal != null && ethereal.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) && magicimune && me.Distance2D(target) <= ethereal.CastRange && target.Health >= target.DamageTaken(dagondamage[dagon.Level - 1],DamageType.Magical,me,false,0,0,0))
							{
								ethereal.UseAbility(target);
							}
							else
								elsecount += 1;
							if (dagon != null && dagon.CanBeCasted() && (!ethereal.CanBeCasted() || target.Health <= target.DamageTaken(dagondamage[dagon.Level - 1], DamageType.Magical, me, false, 0, 0, 0)) && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon") && magicimune )
							{
								dagon.UseAbility(target);
							}
							else
								elsecount += 1;*/

							if (ethereal != null && ethereal.CanBeCasted() 
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
								&& (!veil.CanBeCasted() || target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") || veil == null | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)) 
								//&& (!silence.CanBeCasted() || target.Ishexed())
								&& magicimune
								&& !OneHitLeft(target)
								&& (!CanReflectDamage(target) || me.IsMagicImmune())
								&& target.NetworkPosition.Distance2D(me) <= 800+castrange + ensage_error
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
								//&& !target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect")
								&& Utils.SleepCheck("Blinks")

								)
							{
								ethereal.UseAbility(target);
								//if (Utils.SleepCheck("etherealDelay") && me.Distance2D(target) <= ethereal.CastRange)
								//	Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200) * 1000), "etherealDelay");
							}
							else
								elsecount += 1;

							if (dagon != null && dagon.CanBeCasted() 
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon")
								&& (!veil.CanBeCasted() || target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") || veil == null | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)) 
								&& (ethereal == null || (ethereal!=null && !IsCasted(ethereal) && !ethereal.CanBeCasted()) || target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal") | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)) 
								//&& (!silence.CanBeCasted() || target.Ishexed())
								&& magicimune
								&& (!CanReflectDamage(target) || me.IsMagicImmune())
								&& !OneHitLeft(target)
								&& target.NetworkPosition.Distance2D(me) <= dagondistance[dagon.Level - 1]+castrange + ensage_error
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
								//&& !target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect")
								&& Utils.SleepCheck("Blinks")
								)
								dagon.UseAbility(target);
							else
								elsecount += 1;								
							
							
							/*
							if (Laser != null && Laser.CanBeCasted() && Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name) && magicimune )
							{
								Laser.UseAbility(target);
							}
							else
								elsecount += 1;							
							if (Rocket != null && Rocket.CanBeCasted() && Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name) && magicimune  && me.Distance2D(target) <= Rocket.CastRange)
							{
								Rocket.UseAbility();

							}
							else
								elsecount += 1;
							*/

								
							if (Laser.Level > 0 && Laser.CanBeCasted() 
								&& Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name)
								//&& !EzkillCheck 
								&& !OneHitLeft(target)
								&& magicimune 
								&& (!CanReflectDamage(target) || me.IsMagicImmune())
								&& target.NetworkPosition.Distance2D(me) <= 650+castrange + ensage_error
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
								&& Utils.SleepCheck("Blinks"))
								Laser.UseAbility(target);
							else
								elsecount += 1;					
								
							/*
							if (shiva != null && shiva.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name) && magicimune )
							{
								shiva.UseAbility();
							}
							else
								elsecount += 1;*/

								
							if (shiva != null && shiva.CanBeCasted() 
								//&& !EzkillCheck 
								&& magicimune
								&& !OneHitLeft(target)
								&& (!target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect") || me.IsMagicImmune())
								&& (!target.Modifiers.Any(y => y.Name == "modifier_nyx_assassin_spiked_carapace") || me.IsMagicImmune())
								&& target.NetworkPosition.Distance2D(me) <= 900 + ensage_error
								&& !(target.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
								&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
								&& Utils.SleepCheck("Blinks"))
                            {
                                shiva.UseAbility();
                            }
								
							else
								elsecount += 1;

							/*
							if (elsecount == 11)
							{
								if (me.Distance2D(target) > me.GetAttackRange()-150)
									Orbwalking.Orbwalk(target);
								else if (!target.IsAttackImmune())
									me.Attack(target);
								else
									me.Move(Game.MousePosition, false);
							}*/ 
							
							if (elsecount == 13 
								&& Refresh != null && Refresh.CanBeCasted() 
								&& Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Refresh.Name) 
								&& !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) 
								//&& !OneHitLeft(target)
								&& Utils.SleepCheck("Rearm") 
								&& Ready_for_refresh()
								&& Utils.SleepCheck("Blinks"))
							{
								Refresh.UseAbility();
								if (Refresh.Level == 1)
									Utils.Sleep(3010, "Rearm");
								if (Refresh.Level == 2)
									Utils.Sleep(1510, "Rearm");
								if (Refresh.Level == 3)
									Utils.Sleep(760, "Rearm");
							}
							else if (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture"))
							{
								if (!me.IsChanneling() 
									&& !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
									&& me.CanAttack() 
									&& !target.IsAttackImmune() 
									&& (!target.Modifiers.Any(y => y.Name == "modifier_nyx_assassin_spiked_carapace") || me.IsMagicImmune())
									&& Utils.SleepCheck("Rearm"))
									{
										if (me.Distance2D(target) > me.GetAttackRange()-100 )
											Orbwalking.Orbwalk(target);
										else 
											me.Attack(target);
									}
								//else
								//	me.Move(Game.MousePosition, false);
							}

							Utils.Sleep(150, "FASTCOMBO");
						}
                    }
                }
                else
                {
                    if (!me.IsChanneling() 
						&& !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
						&& !me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture")
                        && Utils.SleepCheck("MousePosition"))
                    {
                        me.Move(Game.MousePosition);
                        Utils.Sleep(150, "MousePosition");
                    }
                        
                }
            }
			
			
            if ((Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) 
                && (Menu.Item("Chase").GetValue<KeyBind>().Active) 
                && !Game.IsChatOpen)
            {
				var targetLock =
					Menu.Item("TargetLock").GetValue<StringList>().SelectedIndex;

                if (Utils.SleepCheck("UpdateTarget")
                    && (target == null || !target.IsValid || !target.IsAlive || !target.IsVisible || (target.IsVisible && targetLock == 0)))
                {
                    target = TargetSelector.ClosestToMouse(me, 2000);
                    Utils.Sleep(250, "UpdateTarget");
                }

                if (target != null && target.IsAlive && !target.IsIllusion && !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase))
                {
                    if (!me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture"))
                    {
                        if (!me.IsChanneling() && me.CanAttack() 			
                            && !target.IsAttackImmune() 
							&& (!target.Modifiers.Any(y => y.Name == "modifier_nyx_assassin_spiked_carapace") || me.IsMagicImmune())
							&& Utils.SleepCheck("Rearm"))
                        {
                            if (me.Distance2D(target) > me.GetAttackRange()-100 )
                                Orbwalking.Orbwalk(target);
                            else
                                me.Attack(target);
                        }
                        else
                            me.Move(Game.MousePosition, false);
                    }				                   			
                }
                else
                {
                    if (!me.IsChanneling() 			
                        && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)					
                        && !me.Modifiers.Any(y => y.Name == "modifier_bloodseeker_rupture")
                        && Utils.SleepCheck("SpeedChase"))
                    {
                        me.Move(Game.MousePosition);
                        Utils.Sleep(150, "SpeedChase");
                    }
                        
                }
            }
			

        }

        public static void AD(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;

            //aether = me.FindItem("item_aether_lens");
            //cyclone = me.FindItem("item_cyclone");
            //ghost = me.FindItem("item_ghost");
            //sheep = me.FindItem("item_sheepstick");
            //atos = me.FindItem("item_rod_of_atos");
            FindItems();

            //Castrange Calculation (Tinker Talent20 and Aether Lens)
            castrange = 0;

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);

            if (aetherLens != null)
            {
                castrange += (int)aetherLens.AbilitySpecialData.First(x => x.Name == "cast_range_bonus").Value;
            }

            var talent20 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_cast_range_75");
            if (talent20.Level > 0)
            {
                castrange += (int)talent20.AbilitySpecialData.First(x => x.Name == "value").Value;
            }

            if (bottle != null && !me.IsInvisible() && !me.IsChanneling() && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase) && !March.IsInAbilityPhase && me.Modifiers.Any(x => x.Name == "modifier_fountain_aura_buff") && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(bottle.Name) && Utils.SleepCheck("bottle1"))
            {
                if (!me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") && (me.Health < me.MaximumHealth || me.Mana < me.MaximumMana))
                    bottle.UseAbility();
                Alies = ObjectManager.GetEntities<Hero>().Where(x => x.Team == me.Team && x != me && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana) && !x.Modifiers.Any(y => y.Name == "modifier_bottle_regeneration") && x.IsAlive && !x.IsIllusion && x.Distance2D(me) <= bottle.CastRange).ToList();
                foreach (Hero v in Alies)
                    if (v != null)
                        bottle.UseAbility(v);
                Utils.Sleep(255, "bottle1");
            }


            var enemies = ObjectManager.GetEntities<Hero>().Where(x => x.IsVisible && x.IsAlive && x.Team == me.GetEnemyTeam() && !x.IsIllusion);


            foreach (var e in enemies)
            {
                if (e == null)
                    return;
                //distance = me.Distance2D(e);
                angle = Math.Abs(e.FindAngleR() - Utils.DegreeToRadian(e.FindAngleForTurnTime(me.NetworkPosition)));

                if (Menu.Item("autoDisable").GetValue<bool>() && me.IsAlive && me.IsVisibleToEnemies)
                {
                    //break linken if tp
                    if (!me.IsChanneling()
                        && me.Distance2D(e) <= 800 + castrange + ensage_error
                        && me.Distance2D(e) >= 300 + ensage_error
                        && e.Modifiers.Any(y => y.Name == "modifier_teleporting")
                        && e.IsLinkensProtected()
                        && Utils.SleepCheck("tplink")
                        )
                    {
                        if ((cyclone != null && cyclone.CanBeCasted()) || (sheep != null && sheep.CanBeCasted()))
                        {
                            if (atos != null && atos.CanBeCasted())
                                atos.UseAbility(e);
                            else if (me.Spellbook.SpellQ != null && me.Spellbook.SpellQ.CanBeCasted())
                                me.Spellbook.SpellQ.UseAbility(e);
                            else if (ethereal != null && ethereal.CanBeCasted())
                                ethereal.UseAbility(e);
                            else if (dagon != null && dagon.CanBeCasted())
                                dagon.UseAbility(e);
                            else if ((sheep != null && sheep.CanBeCasted()) && (cyclone != null && cyclone.CanBeCasted()))
                                sheep.UseAbility(e);
                            //else if (cyclone != null && cyclone.CanBeCasted())
                            //    cyclone.UseAbility(e);
                        }

                        Utils.Sleep(150, "tplink");
                    }

                    //break TP 
                    if (!me.IsChanneling()
                        && me.Distance2D(e) <= 800 + castrange + ensage_error
                        && e.Modifiers.Any(y => y.Name == "modifier_teleporting")
                        //&& e.IsChanneling()
                        && !e.IsHexed()
                        && !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
                        && !e.IsLinkensProtected()
                        && Utils.SleepCheck("tplink1")
                        )
                    {
                        if (sheep != null && sheep.CanBeCasted())
                            sheep.UseAbility(e);
                        else if (cyclone != null && cyclone.CanBeCasted())
                            cyclone.UseAbility(e);

                        Utils.Sleep(150, "tplink1");
                    }

                    //break channel by Hex
                    if (!me.IsChanneling()
                        && sheep != null && sheep.CanBeCasted()
                        && me.Distance2D(e) <= 800 + castrange + ensage_error
                        && !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
                        && !e.IsSilenced()
                        && !e.IsMagicImmune()
                        && !e.IsLinkensProtected()
                        && !e.Modifiers.Any(y => y.Name == "modifier_teleporting")
                        && Utils.SleepCheck(e.Handle.ToString())
                        && (e.IsChanneling()
                            || (e.FindItem("item_blink") != null && IsCasted(e.FindItem("item_blink")))
                            //break escape spells (1 hex, 2 seal) no need cyclone
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_QueenOfPain && e.FindSpell("queenofpain_blink").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage && e.FindSpell("antimage_blink").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_StormSpirit && e.FindSpell("storm_spirit_ball_lightning").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Shredder && e.FindSpell("shredder_timber_chain").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Weaver && e.FindSpell("weaver_time_lapse").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid && e.FindSpell("faceless_void_time_walk").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && e.FindSpell("phoenix_icarus_dive").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && e.FindSpell("magnataur_skewer").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Morphling && e.FindSpell("morphling_waveform").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomAssassin && e.FindSpell("phantom_assassin_phantom_strike").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && e.FindSpell("riki_blink_strike").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Spectre && e.FindSpell("spectre_haunt").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Furion && e.FindSpell("furion_sprout").IsInAbilityPhase

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomLancer && e.FindSpell("phantom_lancer_doppelwalk").IsInAbilityPhase



                            //break special (1 hex, 2 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen")
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && e.Modifiers.Any(y => y.Name == "modifier_phoenix_icarus_dive")
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && e.Modifiers.Any(y => y.Name == "modifier_magnataur_skewer_movement")



                            //break rats shadow blades and invis (1 hex, 2 seal, 3 cyclone)
                            || e.IsMelee && me.Distance2D(e) <= 350 //test
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Legion_Commander && e.FindSpell("legion_commander_duel").Cooldown < 2 && me.Distance2D(e) < 480 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Tiny && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Pudge && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_BountyHunter && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nevermore && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Weaver && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Broodmother && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Slark && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Ursa && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Earthshaker && (e.Spellbook.SpellQ.Cooldown <= 1 || e.Spellbook.SpellR.Cooldown <= 1)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Alchemist && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && me.Distance2D(e) <= 350 && !me.IsAttackImmune()

                            //break rats blinkers (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Ursa && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomAssassin && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Spectre && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage && me.Distance2D(e) <= 350 && !me.IsAttackImmune()

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin && me.Distance2D(e) <= e.GetAttackRange() + 50 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Morphling && me.Distance2D(e) <= e.GetAttackRange() + 50 && !me.IsAttackImmune()

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_QueenOfPain && me.Distance2D(e) <= 800 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Puck && me.Distance2D(e) <= 800 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_StormSpirit && me.Distance2D(e) <= 800 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && me.Distance2D(e) <= 800 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && me.Distance2D(e) <= 800 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid && me.Distance2D(e) <= 800 + castrange + ensage_error


                            //break mass dangerous spells (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Necrolyte && e.FindSpell("necrolyte_reapers_scythe").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid && e.FindSpell("faceless_void_chronosphere").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && e.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DoomBringer && e.FindSpell("doom_bringer_doom").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter && e.FindSpell("tidehunter_ravage").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Enigma && e.FindSpell("enigma_black_hole").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Rattletrap && e.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Luna && e.FindSpell("luna_eclipse").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nevermore && e.FindSpell("nevermore_requiem").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.FindSpell("spirit_breaker_nether_strike").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Naga_Siren && e.FindSpell("naga_siren_song_of_the_siren").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Medusa && e.FindSpell("medusa_stone_gaze").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Treant && e.FindSpell("treant_overgrowth").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage && e.FindSpell("antimage_mana_void").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Warlock && e.FindSpell("warlock_rain_of_chaos").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Terrorblade && e.FindSpell("terrorblade_sunder").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DarkSeer && e.FindSpell("dark_seer_wall_of_replica").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DarkSeer && e.FindSpell("dark_seer_surge").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Dazzle && e.FindSpell("dazzle_shallow_grave").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Omniknight && e.FindSpell("omniknight_guardian_angel").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Omniknight && e.FindSpell("omniknight_repel").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster && e.FindSpell("beastmaster_primal_roar").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.FindSpell("chaos_knight_reality_rift").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.FindSpell("chaos_knight_phantasm").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Life_Stealer && e.FindSpell("life_stealer_infest").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && e.FindSpell("sven_gods_strength").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DrowRanger && e.FindSpell("drow_ranger_wave_of_silence").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && e.FindSpell("nyx_assassin_mana_burn").IsInAbilityPhase

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Mirana && e.Spellbook.SpellW.IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_BountyHunter && e.Spellbook.SpellR.IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && e.FindSpell("phoenix_icarus_dive").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_EarthSpirit && e.FindSpell("earth_spirit_magnetize").IsInAbilityPhase


                            //break stun spells (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Ogre_Magi && e.FindSpell("ogre_magi_fireblast").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Axe && e.FindSpell("axe_berserkers_call").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Lion && e.FindSpell("lion_impale").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && e.FindSpell("nyx_assassin_impale").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Rubick && e.FindSpell("rubick_telekinesis").IsInAbilityPhase
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Rubick && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + ensage_error)
                            //|| (e.ClassId == ClassId.CDOTA_Unit_Hero_Alchemist && e.FindSpell("alchemist_unstable_concoction_throw").IsInAbilityPhase)


                            //break flying stun spells if enemy close (1 hex, 2 seal, 3 cyclone)  have cyclone
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sniper && e.Spellbook.SpellR.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)//e.FindSpell("sniper_assassinate").Cooldown > 0 && me.Modifiers.Any(y => y.Name == "modifier_sniper_assassinate"))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.1 && me.Distance2D(e) <= 400)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && e.Spellbook.SpellQ.IsInAbilityPhase && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_SkeletonKing && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_VengefulSpirit && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)


                            //break flying stun spells if enemy close (1 hex, 2 seal, 3 cyclone)  no cyclone
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sniper && e.Spellbook.SpellR.IsInAbilityPhase && angle <= 0.03 && (cyclone == null || !cyclone.CanBeCasted()))//e.FindSpell("sniper_assassinate").Cooldown > 0 && me.Modifiers.Any(y => y.Name == "modifier_sniper_assassinate"))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.1 && (cyclone == null || !cyclone.CanBeCasted()))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && e.Spellbook.SpellQ.IsInAbilityPhase && (cyclone == null || !cyclone.CanBeCasted()))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_SkeletonKing && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && (cyclone == null || !cyclone.CanBeCasted()))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && (cyclone == null || !cyclone.CanBeCasted()))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_VengefulSpirit && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && (cyclone == null || !cyclone.CanBeCasted()))




                            //break common dangerous spell (1 hex, 2 seal) //no need cyclone
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Bloodseeker && e.FindSpell("bloodseeker_rupture").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Mirana && e.FindSpell("mirana_invis").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && e.FindSpell("riki_smoke_screen").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && e.FindSpell("riki_tricks_of_the_trade").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Viper && e.FindSpell("viper_viper_strike").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Chen && e.FindSpell("chen_hand_of_god").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DeathProphet && e.FindSpell("death_prophet_silence").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DeathProphet && e.FindSpell("death_prophet_exorcism").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Invoker // =)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_EmberSpirit // =)



                            //break hex spell
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Lion && e.Spellbook.SpellW.Level > 0 && e.Spellbook.SpellW.Cooldown < 1 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + ensage_error)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ShadowShaman && e.Spellbook.SpellW.Level > 0 && e.Spellbook.SpellW.Cooldown < 1 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + ensage_error)
                            || (e.FindItem("item_sheepstick") != null && e.FindItem("item_sheepstick").Cooldown < 1 && me.Distance2D(e) < e.FindItem("item_sheepstick").GetCastRange() + ensage_error)




                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Omniknight && e.FindSpell("omniknight_purification").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Ursa && e.FindSpell("ursa_overpower").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Silencer && e.FindSpell("silencer_last_word").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Silencer && e.FindSpell("silencer_global_silence").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_ShadowShaman && e.FindSpell("shadow_shaman_mass_serpent_ward").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_QueenOfPain && e.FindSpell("queenofpain_sonic_wave").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Obsidian_Destroyer && e.FindSpell("obsidian_destroyer_astral_imprisonment").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Obsidian_Destroyer && e.FindSpell("obsidian_destroyer_sanity_eclipse").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Pugna && e.FindSpell("pugna_nether_ward").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Lich && e.FindSpell("lich_chain_frost").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_StormSpirit && e.FindSpell("storm_spirit_electric_vortex").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Zuus && e.FindSpell("zuus_thundergods_wrath").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Brewmaster && e.FindSpell("brewmaster_primal_split").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Bane && e.FindSpell("bane_fiends_grip").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Bane && e.FindSpell("bane_nightmare").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Undying && e.FindSpell("undying_tombstone").IsInAbilityPhase

                            )
                        )
                    {
                        sheep.UseAbility(e);
                        Utils.Sleep(200, e.Handle.ToString());
                    }


                    //break channel by cyclone if not hex
                    if (!me.IsChanneling()
                        && cyclone != null
                        && cyclone.CanBeCasted()
                        && (sheep == null || !sheep.CanBeCasted() || e.IsLinkensProtected())
                        && me.Distance2D(e) <= 575 + castrange + ensage_error
                        && !e.IsHexed()
                        && !e.IsMagicImmune()
                        && !e.IsSilenced()
                        && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")

                        && !e.Modifiers.Any(y => y.Name == "modifier_teleporting")
                        && Utils.SleepCheck(e.Handle.ToString())
                        && (e.IsChanneling()
                            || (e.FindItem("item_blink") != null && IsCasted(e.FindItem("item_blink")))

                            //break rats shadow blades and invis if they appear close(1 hex, 2 seal, 3 cyclone)
                            || (e.IsMelee && me.Distance2D(e) <= 350 && (me.Spellbook.SpellR == null || !me.Spellbook.SpellR.CanBeCasted())) //test
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Legion_Commander && e.FindSpell("legion_commander_duel").Cooldown < 2 && me.Distance2D(e) < 480 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Tiny && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Pudge && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_BountyHunter && me.Distance2D(e) <= 350
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Weaver && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Broodmother && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Slark && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Earthshaker && (e.Spellbook.SpellQ.Cooldown <= 1 || e.Spellbook.SpellR.Cooldown <= 1)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Alchemist && me.Distance2D(e) <= 350 && !me.IsAttackImmune()
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && me.Distance2D(e) <= 350 && !me.IsAttackImmune()


                            //break rats blinkers (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_QueenOfPain && me.Distance2D(e) <= 575 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Puck && me.Distance2D(e) <= 575 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_StormSpirit && me.Distance2D(e) <= 575 + castrange + ensage_error
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid && me.Distance2D(e) <= 575 + castrange + ensage_error


                            //break mass dangerous spells (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Necrolyte && e.FindSpell("necrolyte_reapers_scythe").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_FacelessVoid && e.FindSpell("faceless_void_chronosphere").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && e.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DoomBringer && e.FindSpell("doom_bringer_doom").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter && e.FindSpell("tidehunter_ravage").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Enigma && e.FindSpell("enigma_black_hole").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Rattletrap && e.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Luna && e.FindSpell("luna_eclipse").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nevermore && e.FindSpell("nevermore_requiem").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.FindSpell("spirit_breaker_nether_strike").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Naga_Siren && e.FindSpell("naga_siren_song_of_the_siren").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Medusa && e.FindSpell("medusa_stone_gaze").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Treant && e.FindSpell("treant_overgrowth").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage && e.FindSpell("antimage_mana_void").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Warlock && e.FindSpell("warlock_rain_of_chaos").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Terrorblade && e.FindSpell("terrorblade_sunder").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DarkSeer && e.FindSpell("dark_seer_wall_of_replica").IsInAbilityPhase
                            //|| e.ClassId == ClassId.CDOTA_Unit_Hero_DarkSeer && e.FindSpell("dark_seer_surge").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Dazzle && e.FindSpell("dazzle_shallow_grave").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Omniknight && e.FindSpell("omniknight_guardian_angel").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Omniknight && e.FindSpell("omniknight_repel").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster && e.FindSpell("beastmaster_primal_roar").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.FindSpell("chaos_knight_reality_rift").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.FindSpell("chaos_knight_phantasm").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Life_Stealer && e.FindSpell("life_stealer_infest").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && e.FindSpell("sven_gods_strength").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_DrowRanger && e.FindSpell("drow_ranger_wave_of_silence").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && e.FindSpell("nyx_assassin_mana_burn").IsInAbilityPhase

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && e.FindSpell("phoenix_icarus_dive").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_EarthSpirit && e.FindSpell("earth_spirit_magnetize").IsInAbilityPhase

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Furion && e.FindSpell("furion_sprout").IsInAbilityPhase


                            //break stun spells (1 hex, 2 seal, 3 cyclone)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Ogre_Magi && e.FindSpell("ogre_magi_fireblast").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Axe && e.FindSpell("axe_berserkers_call").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Lion && e.FindSpell("lion_impale").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nyx_Assassin && e.FindSpell("nyx_assassin_impale").IsInAbilityPhase
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Rubick && e.FindSpell("rubick_telekinesis").IsInAbilityPhase
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Rubick && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + ensage_error)


                            //break hex spell
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Lion && e.Spellbook.SpellW.Level > 0 && e.Spellbook.SpellW.Cooldown < 1 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + ensage_error)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ShadowShaman && e.Spellbook.SpellW.Level > 0 && e.Spellbook.SpellW.Cooldown < 1 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + ensage_error)
                            || (e.FindItem("item_sheepstick") != null && e.FindItem("item_sheepstick").Cooldown < 1 && me.Distance2D(e) < e.FindItem("item_sheepstick").GetCastRange() + ensage_error)


                            //break flying stun spells if enemy close (1 hex, 2 seal, 3 cyclone)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sniper && e.Spellbook.SpellR.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)//e.FindSpell("sniper_assassinate").Cooldown > 0 && me.Modifiers.Any(y => y.Name == "modifier_sniper_assassinate"))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.1 && me.Distance2D(e) <= 400)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && e.Spellbook.SpellQ.IsInAbilityPhase && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_SkeletonKing && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_VengefulSpirit && e.Spellbook.SpellQ.IsInAbilityPhase && angle <= 0.03 && me.Distance2D(e) <= 300)

                            )
                        )
                    {
                        cyclone.UseAbility(e);
                        Utils.Sleep(50, e.Handle.ToString());
                    }




                    //cyclone dodge	
                    if (Utils.SleepCheck("item_cyclone") && cyclone != null && cyclone.CanBeCasted())
                    {
                        //use on me
                        var mod =
                            me.Modifiers.FirstOrDefault(
                                x =>
                                    x.Name == "modifier_lina_laguna_blade" ||

                                    //x.Name == "modifier_orchid_malevolence_debuff" || 
                                    //x.Name == "modifier_skywrath_mage_ancient_seal" ||

                                    x.Name == "modifier_lion_finger_of_death");

                        if (cyclone != null && cyclone.CanBeCasted() &&
                            (mod != null
                            || (me.IsRooted() && !me.Modifiers.Any(y => y.Name == "modifier_razor_unstablecurrent_slow"))
                            //|| e.ClassId == ClassId.CDOTA_Unit_Hero_Zuus && e.FindSpell("zuus_thundergods_wrath").IsInAbilityPhase  //zuus can cancel
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Huskar && IsCasted(e.Spellbook.SpellR) && angle <= 0.15 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 250) //( (e.FindSpell("huskar_life_break").Cooldown >= 3 && e.AghanimState()) || (e.FindSpell("huskar_life_break").Cooldown >= 11 && !e.AghanimState())) && me.Distance2D(e) <= 400)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Juggernaut && e.Modifiers.Any(y => y.Name == "modifier_juggernaut_omnislash") && me.Distance2D(e) <= 300 && !me.IsAttackImmune())// && (ghost == null || !ghost.CanBeCasted()) && (ghost == null || !ghost.CanBeCasted())&& !me.Modifiers.Any(y => y.Name == "modifier_item_ghost_scepter")


                            //dodge flying stuns
                            || (e.FindItem("item_ethereal_blade") != null && IsCasted(e.FindItem("item_ethereal_blade")) && angle <= 0.1 && me.Distance2D(e) < e.FindItem("item_ethereal_blade").GetCastRange() + 250)

                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sniper && IsCasted(e.Spellbook.SpellR) && me.Distance2D(e) > 300 && me.Modifiers.Any(y => y.Name == "modifier_sniper_assassinate"))//e.FindSpell("sniper_assassinate").Cooldown > 0 && me.Modifiers.Any(y => y.Name == "modifier_sniper_assassinate"))
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Tusk && angle <= 0.35 && e.Modifiers.Any(y => y.Name == "modifier_tusk_snowball_movement") && me.Distance2D(e) <= 575)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && IsCasted(e.Spellbook.SpellQ) && angle <= 0.12 && me.Distance2D(e) > 400 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 550)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Sven && IsCasted(e.Spellbook.SpellQ) && angle <= 0.3 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 500)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_SkeletonKing && IsCasted(e.Spellbook.SpellQ) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_ChaosKnight && IsCasted(e.Spellbook.SpellQ) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_VengefulSpirit && IsCasted(e.Spellbook.SpellQ) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Alchemist && e.FindSpell("alchemist_unstable_concoction_throw").IsInAbilityPhase && angle <= 0.3 && me.Distance2D(e) < e.FindSpell("alchemist_unstable_concoction_throw").GetCastRange() + 500)

                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Viper && IsCasted(e.Spellbook.SpellR) && angle <= 0.1 && me.Distance2D(e) < e.Spellbook.SpellR.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomLancer && IsCasted(e.Spellbook.SpellQ) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Morphling && IsCasted(e.Spellbook.SpellW) && angle <= 0.1 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + 350)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Tidehunter && IsCasted(e.Spellbook.SpellQ) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellQ.GetCastRange() + 150)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Visage && IsCasted(e.Spellbook.SpellW) && angle <= 0.1 && me.Distance2D(e) > 300 && me.Distance2D(e) < e.Spellbook.SpellW.GetCastRange() + 250)
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Lich && IsCasted(e.Spellbook.SpellR) && angle <= 0.5 && me.Distance2D(e) < e.Spellbook.SpellR.GetCastRange() + 350)


                            //free silence
                            || (me.IsSilenced() && !me.IsHexed() && !me.Modifiers.Any(y => y.Name == "modifier_doom_bringer_doom") && !me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen") && !me.Modifiers.Any(y => y.Name == "modifier_disruptor_static_storm")))

                            //free debuff
                            || me.Modifiers.Any(y => y.Name == "modifier_oracle_fortunes_end_purge")
                            || me.Modifiers.Any(y => y.Name == "modifier_life_stealer_open_wounds")
                            )
                        {
                            cyclone.UseAbility(me);
                            Utils.Sleep(150, "item_cyclone");
                            return;
                        }

                        /*
						//use on enemy cyclone
						else if (cyclone != null 
								&& cyclone.CanBeCasted() 
								&& me.Distance2D(e) <= 575 + 50 + castrange 
								&& (e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen")
									|| e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
									)
								)
						{
							cyclone.UseAbility(e);
							Utils.Sleep(150, "item_cyclone");
							return;

						}
						 //use on enemy sheep
						else if ((cyclone ==null || !cyclone.CanBeCasted() || me.Distance2D(e) > 575 + 50 + castrange) 
								&& sheep != null && sheep.CanBeCasted()
								&& me.Distance2D(e) <= 800 + 50 + castrange
								&& (e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen")
									|| e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
									)
								)
						{
							sheep.UseAbility(e);
							Utils.Sleep(150, "item_cyclone");
							return;

						}
						*/


                    }




                    //Laser dodge close enemy
                    if (Laser != null
                        && Laser.CanBeCasted()
                        && (sheep == null || !sheep.CanBeCasted())
                        && !me.IsAttackImmune()
                        && !e.IsHexed()
                        && !e.IsMagicImmune()
                        && !me.IsChanneling()
                        && angle <= 0.03
                        && ((e.IsMelee && me.Position.Distance2D(e) < 250)
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Weaver && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Huskar && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Nevermore && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && me.Distance2D(e) <= e.GetAttackRange() + 50 && IsCasted(e.Spellbook.SpellR)// && e.Modifiers.Any(y => y.Name == "modifier_windrunner_focusfire"))
                            )
                        && e.IsAttacking()
                        && Utils.SleepCheck("Ghost"))
                    {
                        Laser.UseAbility(e);
                        Utils.Sleep(150, "Ghost");
                    }
                    /*
                    else if (Laser != null
                            && Laser.CanBeCasted()
                            && !me.IsAttackImmune()
                            && e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && IsCasted(e.Spellbook.SpellR)//&& e.Modifiers.Any(y => y.Name == "modifier_windrunner_focusfire")
                            //&& e.IsAttacking() 
                           && angle <= 0.03
                            && Utils.SleepCheck("Ghost")
                            )
                    {
                        ghost.UseAbility();
                        Utils.Sleep(150, "Ghost");
                    }*/


                    //ghost dodge close enemy
                    if (ghost != null
                        && ghost.CanBeCasted()
                        && (sheep == null || !sheep.CanBeCasted())
                        && (Laser == null || !Laser.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_juggernaut_omnislash"))
                        && !me.IsAttackImmune()
                        && !e.IsHexed()
                        && (!e.Modifiers.Any(y => y.Name == "modifier_tinker_laser_blind") || e.Modifiers.Any(y => y.Name == "modifier_juggernaut_omnislash"))
                        && !me.IsChanneling()
                        && angle <= 0.03
                        && ((e.IsMelee && me.Position.Distance2D(e) < 250)
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Tiny
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Shredder
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Nyx_Assassin
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Meepo
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Earthshaker
                            && e.ClassId != ClassId.CDOTA_Unit_Hero_Centaur

                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TrollWarlord && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Clinkz && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Weaver && me.Distance2D(e) <= e.GetAttackRange() + 50
                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Huskar && me.Distance2D(e) <= e.GetAttackRange() + 50
                            //|| e.Modifiers.Any(y => y.Name == "modifier_juggernaut_omnislash")
                            || (e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && IsCasted(e.Spellbook.SpellR))// && e.Modifiers.Any(y => y.Name == "modifier_windrunner_focusfire"))

                            )
                        && e.IsAttacking()
                        && Utils.SleepCheck("Ghost"))
                    {
                        ghost.UseAbility();
                        Utils.Sleep(150, "Ghost");
                    }
                    /*
                    else if (ghost != null
                            && ghost.CanBeCasted()
                            && !me.IsAttackImmune()
                            && e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && IsCasted(e.Spellbook.SpellR)//&& e.Modifiers.Any(y => y.Name == "modifier_windrunner_focusfire")
                            //&& e.IsAttacking() 
                           && angle <= 0.03
                            && Utils.SleepCheck("Ghost")
                            )
                    {
                        ghost.UseAbility();
                        Utils.Sleep(150, "Ghost");
                    }*/


                    //cyclone dodge attacking close enemy		
                    if (
                                        (ghost == null || !ghost.CanBeCasted())
                                        && (sheep == null || !sheep.CanBeCasted())
                                        && (Laser == null || !Laser.CanBeCasted())
                                        //&& (me.Spellbook.SpellE == null || !me.Spellbook.SpellE.CanBeCasted())

                                        && cyclone != null
                                        && cyclone.CanBeCasted()
                                        && me.Distance2D(e) <= 575 + castrange + ensage_error
                                        && !me.IsAttackImmune()
                                        && !e.IsHexed()
                                        && !me.IsChanneling()
                                        && !e.Modifiers.Any(y => y.Name == "modifier_tinker_laser_blind")
                                        && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")

                                        && angle <= 0.03
                                        && (e.ClassId == ClassId.CDOTA_Unit_Hero_Ursa
                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomAssassin
                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki

                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Sven
                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Spectre
                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage

                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin
                                            || e.ClassId == ClassId.CDOTA_Unit_Hero_Morphling

                                            )

                                        && e.IsAttacking()
                                        && Utils.SleepCheck("Ghost"))
                    {
                        cyclone.UseAbility(e);
                        Utils.Sleep(150, "Ghost");
                    }
                    else if ( //если цель под ультой ская
                                    (ghost == null || !ghost.CanBeCasted())
                                    && (sheep == null || !sheep.CanBeCasted())
                                    && (Laser == null || !Laser.CanBeCasted())
                                    //&& (me.Spellbook.SpellE == null || !me.Spellbook.SpellE.CanBeCasted())
                                    && cyclone != null
                                    && cyclone.CanBeCasted()
                                    && me.Distance2D(e) <= 575 + castrange + ensage_error
                                    && !me.IsAttackImmune()
                                    && !e.IsHexed()
                                    && e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect") ////!!!!!!!!

                                    && angle <= 0.03
                                    && (e.ClassId == ClassId.CDOTA_Unit_Hero_Ursa
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_PhantomAssassin
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_Riki
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_Spectre
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_AntiMage

                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_TemplarAssassin
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_Morphling
                                        )

                                    && e.IsAttacking()
                                    && Utils.SleepCheck("Ghost"))
                    {
                        cyclone.UseAbility(me);
                        Utils.Sleep(150, "Ghost");
                    }


                    else if ( //break special (1 hex, 2 cyclone)
                                    !me.IsChanneling()
                                    //&& (me.Spellbook.SpellE == null || !me.Spellbook.SpellE.CanBeCasted())
                                    && cyclone != null
                                    && cyclone.CanBeCasted()
                                    && (sheep == null || !sheep.CanBeCasted())
                                    && me.Distance2D(e) <= 575 + castrange + ensage_error
                                    && !e.IsHexed()
                                    && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect") ////!!!!!!!!

                                    && (
                                        //break special (1 hex, 2 cyclone)
                                        e.ClassId == ClassId.CDOTA_Unit_Hero_Riki && me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen")
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_SpiritBreaker && e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_Phoenix && e.Modifiers.Any(y => y.Name == "modifier_phoenix_icarus_dive")
                                        || e.ClassId == ClassId.CDOTA_Unit_Hero_Magnataur && e.Modifiers.Any(y => y.Name == "modifier_magnataur_skewer_movement")

                                        )
                                    && Utils.SleepCheck("Ghost"))
                    {
                        cyclone.UseAbility(e);
                        Utils.Sleep(150, "Ghost");
                    }

                    else if ( // Если ВРка
                            (ghost == null || !ghost.CanBeCasted())
                            && (Laser == null || !Laser.CanBeCasted())
                            && cyclone != null
                            && cyclone.CanBeCasted()
                            && !me.IsAttackImmune()
                            && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
                            && e.ClassId == ClassId.CDOTA_Unit_Hero_Windrunner && IsCasted(e.Spellbook.SpellR)//&& e.Modifiers.Any(y => y.Name == "modifier_windrunner_focusfire")
                                                                                                              //&& e.IsAttacking() 
                           && angle <= 0.03
                            && Utils.SleepCheck("Ghost")
                            )
                    {
                        cyclone.UseAbility(e);
                        Utils.Sleep(150, "Ghost");
                    }
                }

                //Auto Killsteal
                if (Menu.Item("autoKillsteal").GetValue<bool>()
                    && me.IsAlive
                    && me.IsVisible
                    && (Menu.Item("Chase").GetValue<KeyBind>().Active || !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)))
                {
                    if (e.Health < GetComboDamageByDistance(e)
                        && me.Mana >= manafactdamage(e)
                        && (!CanReflectDamage(e) || me.IsMagicImmune())
                        //&& (!e.FindSpell("abaddon_borrowed_time").CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect"))
                        && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect")
                        && !e.Modifiers.Any(y => y.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison")
                        && !e.Modifiers.Any(y => y.Name == "modifier_puck_phase_shift")
                        && !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
                        && !e.Modifiers.Any(y => y.Name == "modifier_dazzle_shallow_grave")
                        && !e.Modifiers.Any(y => y.Name == "modifier_brewmaster_storm_cyclone")
                        && !e.Modifiers.Any(y => y.Name == "modifier_shadow_demon_disruption")
                        && !e.Modifiers.Any(y => y.Name == "modifier_tusk_snowball_movement")
                        && !me.Modifiers.Any(y => y.Name == "modifier_pugna_nether_ward_aura")
                        && !me.IsSilenced()
                        && !me.IsHexed()
                        && !me.Modifiers.Any(y => y.Name == "modifier_doom_bringer_doom")
                        && !me.Modifiers.Any(y => y.Name == "modifier_riki_smoke_screen")
                        && !me.Modifiers.Any(y => y.Name == "modifier_disruptor_static_storm"))
                    {
                        if (Utils.SleepCheck("AUTOCOMBO") && !me.IsChanneling())
                        {

                            bool EzkillCheck = EZKill(e);
                            bool magicimune = (!e.IsMagicImmune() && !e.Modifiers.Any(x => x.Name == "modifier_eul_cyclone"));

                            if (!me.IsChanneling()
                                && me.CanAttack()
                                && !e.IsAttackImmune()
                                && !me.Spellbook.Spells.Any(x => x.IsInAbilityPhase)
                                && OneHitLeft(e)
                                && e.NetworkPosition.Distance2D(me) <= me.GetAttackRange() + 50
                                //&& Utils.SleepCheck("attack")			
                                )
                            {
                                me.Attack(e);
                                //Orbwalking.Orbwalk(e);
                                //Utils.Sleep(250, "attack");
                            }

                            if (soulring != null && soulring.CanBeCasted()
                                //&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)  
                                && e.NetworkPosition.Distance2D(me) < 2500
                                && magicimune
                                && !OneHitLeft(e)
                                && (((veil == null || !veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff")  /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)*/) && e.NetworkPosition.Distance2D(me) <= 1600 + castrange) || ((e.NetworkPosition.Distance2D(me) > 1600 + castrange) && (e.Health < (int)GetRocketDamage() * (1 - e.MagicDamageResist))))
                                && (((ethereal == null || (ethereal != null && !ethereal.CanBeCasted()) || IsCasted(ethereal) /*|| e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal")*/ /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)*/) && e.NetworkPosition.Distance2D(me) <= 800 + castrange) || ((e.NetworkPosition.Distance2D(me) > 800 + castrange) && (e.Health < (int)GetRocketDamage() * (1 - e.MagicDamageResist))))

                                )
                                soulring.UseAbility();


                            if (veil != null && veil.CanBeCasted()
                                && magicimune
                                //&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)
                                && e.NetworkPosition.Distance2D(me) <= 1600 + castrange + ensage_error
                                && !OneHitLeft(e)
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                && !e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff"))
                            {
                                if (me.Distance2D(e) > 1000 + castrange + ensage_error)
                                {
                                    var a = me.Position.ToVector2().FindAngleBetween(e.Position.ToVector2(), true);
                                    var p1 = new Vector3(
                                        me.Position.X + (me.Distance2D(e) - 500) * (float)Math.Cos(a),
                                        me.Position.Y + (me.Distance2D(e) - 500) * (float)Math.Sin(a),
                                        100);
                                    veil.UseAbility(p1);
                                }
                                else if (me.Distance2D(e) <= 1000 + castrange + ensage_error)
                                    veil.UseAbility(e.NetworkPosition);

                            }

                            if (ethereal != null && ethereal.CanBeCasted()
                                //&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                && (!veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") || veil == null /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)*/)
                                && !OneHitLeft(e)
                                && magicimune
                                && e.NetworkPosition.Distance2D(me) <= 800 + castrange + ensage_error
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                )
                                ethereal.UseAbility(e);

                            if (dagon != null && dagon.CanBeCasted()
                                //&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && (!veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") || veil == null /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)*/)
                                && (ethereal == null || (ethereal != null && !IsCasted(ethereal) && !ethereal.CanBeCasted()) || e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal") /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)*/)
                                && !OneHitLeft(e)
                                && magicimune
                                && e.NetworkPosition.Distance2D(me) <= dagondistance[dagon.Level - 1] + castrange + ensage_error
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                )
                                dagon.UseAbility(e);
                            
                            if (Rocket.Level > 0 && Rocket.CanBeCasted()
                                && e.NetworkPosition.Distance2D(me) <= 2500
                                && (!EzkillCheck || e.NetworkPosition.Distance2D(me) >= 800 + castrange + ensage_error)
                                && !OneHitLeft(e)
                                && magicimune
                                //&& Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name) 
                                //&& (((veil == null || !veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff")  | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)) && e.NetworkPosition.Distance2D(me) <= 1600+castrange)|| ((e.NetworkPosition.Distance2D(me) > 1600+castrange) && (e.Health < (int)(e.DamageTaken(rocket_damage[Rocket.Level - 1], DamageType.Magical, me, false, 0, 0, 0)*spellamplymult*lensmult)))   )
                                //&& (((ethereal == null || (ethereal!=null && !ethereal.CanBeCasted()) || IsCasted(ethereal) /*|| e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal")*/ | !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))&& e.NetworkPosition.Distance2D(me) <= 800+castrange)|| ((e.NetworkPosition.Distance2D(me) > 800+castrange) && (e.Health < (int)(e.DamageTaken(rocket_damage[Rocket.Level - 1], DamageType.Magical, me, false, 0, 0, 0)*spellamplymult*lensmult)))   )
                                && (((veil == null || !veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff")  /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)*/) && e.NetworkPosition.Distance2D(me) <= 1600 + castrange) || (e.NetworkPosition.Distance2D(me) > 1600 + castrange))
                                && (((ethereal == null || (ethereal != null && !ethereal.CanBeCasted()) || IsCasted(ethereal) /*|| e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal")*/ /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)*/) && e.NetworkPosition.Distance2D(me) <= 800 + castrange) || (e.NetworkPosition.Distance2D(me) > 800 + castrange))
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                )
                                Rocket.UseAbility();

                            if (Laser.Level > 0 && Laser.CanBeCasted()
                                //&& Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name)
                                && !EzkillCheck
                                && !OneHitLeft(e)
                                && magicimune
                                && e.NetworkPosition.Distance2D(me) <= 650 + castrange + ensage_error
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                )
                                Laser.UseAbility(e);

                            if (shiva != null && shiva.CanBeCasted()
                                //&& Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                && (!veil.CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff") || veil == null /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name)*/)
                                && (ethereal == null || (ethereal != null && !IsCasted(ethereal) && !ethereal.CanBeCasted()) || e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal") /*| !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)*/)
                                && !EzkillCheck
                                && !OneHitLeft(e)
                                && magicimune
                                && e.NetworkPosition.Distance2D(me) <= 900 + ensage_error
                                && !(e.Modifiers.Any(y => y.Name == "modifier_teleporting") && IsEulhexFind())
                                )
                                shiva.UseAbility();

                            Utils.Sleep(150, "AUTOCOMBO");
                        }
                    }
                }
            }
        }

        public static void DrawRanges(EventArgs args)
        {
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || !Utils.SleepCheck("VisibilitySleep"))
				return;
			//Utils.Sleep(150, "VisibilitySleep");
				
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;

            castrange = 0;

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);

            if (aetherLens != null)
            {
                castrange += (int)aetherLens.AbilitySpecialData.First(x => x.Name == "cast_range_bonus").Value;
            }

            var talent20 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_cast_range_75");
            if (talent20.Level > 0)
            {
                castrange += (int)talent20.AbilitySpecialData.First(x => x.Name == "value").Value;
            }

            if (Menu.Item("Show Direction").GetValue<bool>())
			{
											
				if (me.IsChanneling())
				{
					if (effect3 == null)
					{						     
						effect3 = new ParticleEffect(@"materials\ensage_ui\particles\line.vpcf", me);     
						
						effect3.SetControlPoint(1, me.Position);
						effect3.SetControlPoint(2, FindVector(me.Position, me.Rotation, 1200 + castrange));
                        effect3.SetControlPoint(3, new Vector3(100, 70, 10));
                        effect3.SetControlPoint(4, new Vector3(150, 255, 255));
                    }
					else 
					{
						effect3.SetControlPoint(1, me.Position);
						effect3.SetControlPoint(2, FindVector(me.Position, me.Rotation, 1200 + castrange));
                        effect3.SetControlPoint(3, new Vector3(100, 70, 10));
                        effect3.SetControlPoint(4, new Vector3(150, 255, 255));
					} 
				}
				else if (effect3 != null)
				{
				   effect3.Dispose();
				   effect3 = null;
				}  
				
			}

            if (target != null && target.IsValid && !target.IsIllusion && target.IsAlive && target.IsVisible && me.Distance2D(target.Position) < 2000 && Menu.Item("Show Target Effect").GetValue<bool>())
            {
                if (effect4 == null)
                {
                    effect4 = new ParticleEffect(@"materials\ensage_ui\particles\target.vpcf", target);
                    effect4.SetControlPoint(2, me.Position);
                    effect4.SetControlPoint(5, new Vector3(red, green, blue));
                    effect4.SetControlPoint(6, new Vector3(1, 0, 0));
                    effect4.SetControlPoint(7, target.Position);
                }
                else
                {
                    effect4.SetControlPoint(2, me.Position);
                    effect4.SetControlPoint(5, new Vector3(red, green, blue));
                    effect4.SetControlPoint(6, new Vector3(1, 0, 0));
                    effect4.SetControlPoint(7, target.Position);
                }
            }
            else if (effect4 != null)
            {
                effect4.Dispose();
                effect4 = null;
            }  		
            
            if (Menu.Item("Blink Range").GetValue<bool>())
			{
				if (me.FindItem("item_blink")!=null)
				{	
					if(rangedisplay_dagger == null)
					{
						rangedisplay_dagger = me.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
						range_dagger = 1200  + castrange;						
						rangedisplay_dagger.SetControlPoint(1, new Vector3(range_dagger, 255, 5));
                        rangedisplay_dagger.SetControlPoint(2, new Vector3(150, 255, 255));
                    }
					if (range_dagger != 1200  + castrange)
					{
						range_dagger = 1200  + castrange;
						if(rangedisplay_dagger != null)
							rangedisplay_dagger.Dispose();
						rangedisplay_dagger = me.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
                        rangedisplay_dagger.SetControlPoint(1, new Vector3(range_dagger, 255, 5));
                        rangedisplay_dagger.SetControlPoint(2, new Vector3(150, 255, 255));
                    }
				}
				
                else
				{
					if(rangedisplay_dagger != null)
						rangedisplay_dagger.Dispose();
					rangedisplay_dagger = null;
				}

			}
			else if (rangedisplay_dagger!=null)
			{
			    rangedisplay_dagger.Dispose();
			    rangedisplay_dagger = null;
			}
			
			
			
			if (Menu.Item("Blink Range Incoming TP").GetValue<bool>())
			{
				if (me.FindItem("item_blink")!=null )
				{	

					var units = ObjectManager.GetEntities<Unit>().Where
					(x =>
					(x is Hero && x.Team == me.Team)
					||(x is Creep && x.Team == me.Team)
					|| (x is Building && x.Team == me.Team)
					|| (!(x is Hero) && !(x is Building) && !(x is Creep) 
						&& x.ClassId != ClassId.CDOTA_NPC_TechiesMines && x.ClassId != ClassId.CDOTA_NPC_Observer_Ward
						&& x.ClassId != ClassId.CDOTA_NPC_Observer_Ward_TrueSight && x.Team == me.Team)
					).ToList();
					
					foreach (var unit in units)
					{
					    HandleEffectR(unit);
					    HandleEffectD(unit);
					}

				}
			}
			

			
			if (Menu.Item("Rocket Range").GetValue<bool>())
			{
				if(rangedisplay_rocket == null)
				{
				    rangedisplay_rocket = me.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
				    range_rocket = 2500;
				    rangedisplay_rocket.SetControlPoint(1, new Vector3(range_rocket, 255, 5));
                    rangedisplay_rocket.SetControlPoint(2, new Vector3(255, 255, 0));
                }
			}
			else if (rangedisplay_rocket!=null)
			{
			    rangedisplay_rocket.Dispose();
			    rangedisplay_rocket = null;
			}
			
			
			
			if (Menu.Item("Laser Range").GetValue<bool>())
			{
				if(rangedisplay_laser == null)
				{
				    rangedisplay_laser = me.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
				    range_laser = 650 + castrange;
				    rangedisplay_laser.SetControlPoint(1, new Vector3(range_laser, 255, 5));
                    rangedisplay_laser.SetControlPoint(2, new Vector3(0, 150, 255));
                }
				if (range_laser != 650 + castrange)
				{
					range_laser = 650 + castrange;
					if(rangedisplay_laser != null)
						rangedisplay_laser.Dispose();
					rangedisplay_laser = me.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
                    rangedisplay_laser.SetControlPoint(1, new Vector3(range_laser, 255, 5));
                    rangedisplay_laser.SetControlPoint(2, new Vector3(0, 150, 255));
                }				
			}
			else if (rangedisplay_laser!=null)
			{
			    rangedisplay_laser.Dispose();
			    rangedisplay_laser = null;
			}
		}
		
        private static void HandleEffectR(Unit unit)
        {
            if (unit == null) return;
            ParticleEffect effect;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;
			
            if (unit.Modifiers.Any(y => y.Name == "modifier_boots_of_travel_incoming") && me.HasModifier("modifier_teleporting"))
            {
                if (VisibleUnit.TryGetValue(unit, out effect)) return;
                effect = unit.AddParticleEffect(@"materials\ensage_ui\particles\range_display_mod.vpcf");
				range_dagger = 1200 + castrange;
				effect.SetControlPoint(1, new Vector3(range_dagger, 255, 5));
                effect.SetControlPoint(2, new Vector3(150, 255, 255));
                VisibleUnit.Add(unit, effect);
            }
            else
            {
                if (!VisibleUnit.TryGetValue(unit, out effect)) return;
                effect.Dispose();
                VisibleUnit.Remove(unit);
            }
        }
		
        private static void HandleEffectD(Unit unit)
        {
						
            if (unit == null) return;
            me = ObjectManager.LocalHero;
            if (me == null || me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;
			
			
            if (unit != null && unit.IsValid && unit.IsAlive && unit.Modifiers.Any(y => y.Name == "modifier_boots_of_travel_incoming") && me.HasModifier("modifier_teleporting"))
			{
				if (effect2 == null)
				{
					effect2 = new ParticleEffect(@"materials\ensage_ui\particles\line.vpcf", unit);     
					effect2.SetControlPoint(1, unit.Position);
					effect2.SetControlPoint(2, FindVector(unit.Position, me.Rotation, 1200 + castrange));
                    effect2.SetControlPoint(3, new Vector3(100, 70, 10));
                    effect2.SetControlPoint(4, new Vector3(150, 255, 255));
                }
				else 
				{
					effect2.SetControlPoint(1, unit.Position);
					effect2.SetControlPoint(2, FindVector(unit.Position, me.Rotation, 1200 + castrange));
                    effect2.SetControlPoint(3, new Vector3(100, 70, 10));
                    effect2.SetControlPoint(4, new Vector3(150, 255, 255));
                } 
			}
			if (!me.HasModifier("modifier_teleporting") && effect2 != null)
			{
			   effect2.Dispose();
			   effect2 = null;
			}	
        }

        public static Vector3 FindVector(Vector3 first, double ret, float distance)
        {
            var retVector = new Vector3(first.X + (float) Math.Cos(Utils.DegreeToRadian(ret)) * distance,
                first.Y + (float) Math.Sin(Utils.DegreeToRadian(ret)) * distance, 100);

            return retVector;
        }		

        static void FindItems()
        {
            //Skills
            Laser = me.Spellbook.SpellQ;
            Rocket = me.Spellbook.SpellW;
            Refresh = me.Spellbook.SpellR;
            March = me.Spellbook.SpellE;
            //Items
            blink = me.FindItem("item_blink");
            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            atos = me.FindItem("item_rod_of_atos");
            sheep = me.FindItem("item_sheepstick");
            soulring = me.FindItem("item_soul_ring");
            ethereal = me.FindItem("item_ethereal_blade");
            shiva = me.FindItem("item_shivas_guard");
            ghost = me.FindItem("item_ghost");
            cyclone = me.FindItem("item_cyclone");
            forcestaff = me.FindItem("item_force_staff");
            glimmer = me.FindItem("item_glimmer_cape");
            bottle = me.FindItem("item_bottle");
            veil = me.FindItem("item_veil_of_discord");
            travel = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_travel_boots"));
        }

        static Vector2 HeroPositionOnScreen(Hero x)
        {
            Vector2 PicPosition;
            PicPosition = new Vector2(HUDInfo.GetHPbarPosition(x).X - 1, HUDInfo.GetHPbarPosition(x).Y - 40);
            return PicPosition;
        }

        static bool Ready_for_refresh()
        {
            if ((ghost != null && ghost.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name))
                || (soulring != null && soulring.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
                || (sheep != null && sheep.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                || (Laser != null && Laser.CanBeCasted() && Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Laser.Name))
                || (ethereal != null && ethereal.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                || (dagon != null && dagon.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                || (Rocket != null && Rocket.CanBeCasted() && Menu.Item("ComboSkills: ").GetValue<AbilityToggler>().IsEnabled(Rocket.Name))
                || (shiva != null && shiva.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                || (glimmer != null && glimmer.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)))
                return false;
            else
                return true;
        }

        static bool CanReflectDamage(Hero x)
        {
            if (x.Modifiers.Any(m => (m.Name == "modifier_item_blade_mail_reflect" ) || (m.Name == "modifier_nyx_assassin_spiked_carapace") || (m.Name == "modifier_item_lotus_orb_active")))
                return true;
            else
                return false;
        }
		
		static bool IsEulhexFind()
        {
            if ((me.FindItem("item_cyclone") != null && me.FindItem("item_cyclone").CanBeCasted()) || (me.FindItem("item_sheepstick") != null && me.FindItem("item_sheepstick").CanBeCasted()) )  
			  return true;
            else
              return false;
        } 	

        static bool IsLinkensProtected(Hero x)
        {
            if (x.Modifiers.Any(m => m.Name == "modifier_item_sphere_target") || x.FindItem("item_sphere") != null && x.FindItem("item_sphere").Cooldown <= 0)
                return true;
            else
                return false;
        } 
		
        private static bool IsCasted(Ability ability)
        {
            return ability.Level > 0 && ability.CooldownLength > 0 && Math.Ceiling(ability.CooldownLength).Equals(Math.Ceiling(ability.Cooldown));
        }
		
		static bool IsPhysDamageImune(Hero v)
        {
            if (me.Modifiers
                .Any(x =>
                x.Name == "modifier_tinker_laser_blind"
                || x.Name == "modifier_troll_warlord_whirling_axes_blind" 
				|| x.Name == "modifier_brewmaster_drunken_haze"
                || x.Name == "modifier_pugna_decrepify"
				|| x.Name == "modifier_item_ethereal_blade_ethereal")
                || v.Modifiers.Any(x => x.Name == "modifier_omniknight_guardian_angel"
									|| x.Name == "modifier_nyx_assassin_spiked_carapace"
									|| x.Name == "modifier_pugna_decrepify"
									|| x.Name == "modifier_windrunner_windrun"
									|| x.Name == "modifier_winter_wyverny_cold_embrace"
									|| x.Name == "modifier_ghost_state" 
									|| x.Name == "modifier_item_ethereal_blade_ethereal"
									)
                || (v.ClassId == ClassId.CDOTA_Unit_Hero_Tiny && v.Spellbook.SpellE.Level > 0)
				|| v.IsInvul()
				)
                return true;
            
            else
                return false;
        }

		static int manaprocast()
        {
            int manalaser = 0, manarocket = 0, manarearm = 0, manadagon = 0, manaveil = 0, manasheep = 0, manaethereal = 0, manashiva = 0, manasoulring = 0;
			int manacounter = 0; 
				
			if (Laser!=null && Laser.Level>0)
				manalaser = (int)(laser_mana[Laser.Level - 1]);
			else
				manalaser = 0;
			
			if (Rocket != null && Rocket.Level>0)
				manarocket = (int)(rocket_mana[Rocket.Level - 1]);
			else
				manarocket = 0;
				
			if (Refresh != null && Refresh.Level>0)
				manarearm = (int)(rearm_mana[Refresh.Level - 1]);
			else
				manarearm = 0;
				
			if (dagon != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
				manadagon = 180;
			else
				manadagon = 0;		
				
			if (ethereal != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
				manaethereal = 100;
			else
				manaethereal = 0;
				
			if (veil != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
				manaveil = 50;
			else
				manaveil = 0;
				
			if (sheep != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
				manasheep = 100;
			else
				manasheep = 0;
				
			if (shiva != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
				manashiva = 100;
			else
				manashiva = 0;

			if (soulring != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
				manasoulring = 150;
			else
				manasoulring = 0;

				
			manacounter = manalaser+manarocket+manadagon+manaethereal+manaveil+manasheep+manashiva-manasoulring;			
			return manacounter;
              
        }	

		static int manaonerocket()
        {
            int  manarocket = 0, manarearm = 0, manasoulring = 0;
			int manacounter = 0; 

			if (Rocket != null && Rocket.Level>0)
				manarocket = (rocket_mana[Rocket.Level - 1]);
			else
				manarocket = 0;
				
			if (Refresh != null && Refresh.Level>0)
				manarearm = (rearm_mana[Refresh.Level - 1]);
			else
				manarearm = 0;
				

			if (soulring != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
				manasoulring = 150;
			else
				manasoulring = 0;

				
			manacounter = manarocket-manasoulring;			
			return manacounter;
              
        }	

		static int manafactdamage(Hero en)
        {
            if (en != null && en.IsAlive && en.IsValid)
            {
			
				int manalaser = 0, manarocket = 0, manarearm = 0, manadagon = 0, dagondist = 0, manaveil = 0, manasheep = 0, manaethereal = 0, manashiva = 0, manasoulring = 0;
				int manacounter = 0; 
					
				if (Laser!=null &&  Laser.Level>0 && Laser.CanBeCasted())// && Laser.Level>0)
					manalaser = (int)(laser_mana[Laser.Level - 1]);
				else
					manalaser = 0;
				
				if (Rocket != null &&  Rocket.Level>0 && Rocket.CanBeCasted())// && Rocket.Level>0)
					manarocket = (int)(rocket_mana[Rocket.Level - 1]);
				else
					manarocket = 0;
					
				if (Refresh != null &&  Refresh.Level>0   && Refresh.CanBeCasted())// && Refresh.Level>0)
					manarearm = (int)(rearm_mana[Refresh.Level - 1]);
				else
					manarearm = 0;
					
				if (dagon != null && dagon.CanBeCasted())// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
				{
					dagondist = dagondistance[dagon.Level - 1];
					manadagon = 180;
				}
				else
				{
					manadagon = 0;		
					dagondist = 0;
				}	
				if (ethereal != null && ethereal.CanBeCasted())// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					manaethereal = 100;
				else
					manaethereal = 0;
					
				if (veil != null && veil.CanBeCasted() && !en.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff"))// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
					manaveil = 50;
				else
					manaveil = 0;
					
				if (sheep != null && sheep.CanBeCasted())// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
					manasheep = 100;
				else
					manasheep = 0;
					
				if (shiva != null && shiva.CanBeCasted())// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
					manashiva = 100;
				else
					manashiva = 0;

				if (soulring != null && soulring.CanBeCasted())// && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
					manasoulring = 150;
				else
					manasoulring = 0;

					
				manacounter = ((me.Distance2D(en)<650+castrange + ensage_error)? manalaser : 0 )+ ((me.Distance2D(en)<2500)? manarocket : 0) + ((me.Distance2D(en)<800+castrange + ensage_error)? manaethereal: 0) + ((me.Distance2D(en)<dagondist+castrange + ensage_error)? manadagon: 0) + ((me.Distance2D(en)<900 + ensage_error)? manashiva : 0)-manasoulring;  //factical mana consume in current range

				
				return manacounter;
			}
			else
				return 0;
              
        }

        static int ProcastCounter(Hero en)
		{
            if (!en.IsMagicImmune() && !en.IsInvul())
            {
				if (IsPhysDamageImune(en))
                    return (int)Math.Ceiling(en.Health / GetComboDamage());
				else
					return (int)Math.Ceiling(en.Health / GetComboDamage() - GetOneAutoAttackDamage(en) / en.Health );
            }
            else 
                return 999;
		}

		static int RktCount(Hero en)
		{
            if (!en.IsMagicImmune() && !en.IsInvul())
			{
				if (((int)((en.Health - GetComboDamage(en) + GetOneAutoAttackDamage(en)) / GetRocketDamage()))<=0)
					return 0;
				else
					return ((int)((en.Health - GetComboDamage(en) + GetOneAutoAttackDamage(en)) / GetRocketDamage()));
			}
			else 
                return 999;
		}

		static int OnlyRktCount(Hero en)
		{
            if (!en.IsMagicImmune() && !en.IsInvul())
				return ((int)(en.Health / GetRocketDamage()+1));
			else 
                return 999;
		}

		static int OnlyRktCountDmg(Hero en)
		{
            if (!en.IsMagicImmune() && !en.IsInvul())
            {
				return ((int)(en.Health / (int)GetRocketDamage() + 1) * (int)GetRocketDamage());
            }
			else 
                return 999;
		}
		
		static int HitCount(Hero en)
		{
			//if (!me.CanAttack() || en.IsAttackImmune() || en.IsInvul() || ((int)Math.Ceiling((en.Health - procastdamage)/hitDmg))<=0)
            if (me.CanAttack() && !en.IsAttackImmune() && !en.IsInvul())
            {
                /*
                if ((ethereal != null && ethereal.CanBeCasted())
                    || (ghost != null && ghost.CanBeCasted() && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name))
                    )
                    return ((int)Math.Ceiling((en.Health - procastdamage)/hitDmg));
                else
                    return ((int)Math.Ceiling((en.Health - procastdamage)/hitDmg)+1);*/
				if ((int)Math.Ceiling((en.Health - GetComboDamage(en) + 2 * GetOneAutoAttackDamage(en)) / GetOneAutoAttackDamage(en)) <= 0)
					return 0;
				else
					return ((int)Math.Ceiling((en.Health - GetComboDamage(en) + 2 * GetOneAutoAttackDamage(en)) / GetOneAutoAttackDamage(en)));

            }
            else 
                return 999;
		}
		
		static bool OneHitLeft(Hero en)
		{
			
			if(((en.Health < GetComboDamageByDistance(en)) && (en.Health > GetComboDamageByDistance(en) - GetOneAutoAttackDamage(en)))
				&& !IsPhysDamageImune(en)
				&& me.Distance2D(en) < me.GetAttackRange()+50)
				return true;
			else	
				return false;
			//return 	((en.Health-GetComboDamageByDistance(en)-hitDmg)<0 && Math.Abs(en.Health-GetComboDamageByDistance(en)-hitDmg)<hitDmg);
			
		}
		
        static void Information(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectManager.LocalHero;
            if (me == null)
                return;
            if (me.ClassId != ClassId.CDOTA_Unit_Hero_Tinker)
                return;

            //GetRocketDamage();
            //Console.WriteLine(GetLaserDamage().ToString());
            //var rocket = me.Spellbook.SpellW;
            //Console.WriteLine("Console Test");
            //var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));
            //Console.WriteLine(eblade.AbilitySpecialData.FirstOrDefault(x => x.Name == "blast_agility_multiplier").Value.ToString());
            //Console.WriteLine(eblade.AbilitySpecialData.FirstOrDefault(x => x.Name == "blast_damage_base").Value.ToString());

            Hero targetInf = null;

            targetInf = me.ClosestToMouseTarget(2000);
            FindItems();
            if (targetInf != null && targetInf.IsValid && !targetInf.IsIllusion && targetInf.IsAlive && targetInf.IsVisible)
            {
				if (Menu.Item("TargetCalculator").GetValue<bool>())
				{	
					var start = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(0, HUDInfo.GetHpBarSizeY() - 70);
					var starts = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(1, HUDInfo.GetHpBarSizeY() - 69);
					var start2 = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(0, HUDInfo.GetHpBarSizeY() - 90);
					var start2s = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(1, HUDInfo.GetHpBarSizeY() - 89);
					var start3 = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(0, HUDInfo.GetHpBarSizeY() - 110);
					var start3s = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(1, HUDInfo.GetHpBarSizeY() - 109);
					var start4 = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(-25, HUDInfo.GetHpBarSizeY() - 13);
					var start4s = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(-24, HUDInfo.GetHpBarSizeY() - 12);
					
					Drawing.DrawText(EZKill(targetInf) ? GetEZKillDamage(targetInf).ToString()+" ez" : GetEZKillDamage(targetInf).ToString(), starts, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					Drawing.DrawText(EZKill(targetInf) ? GetEZKillDamage(targetInf).ToString()+" ez" : GetEZKillDamage(targetInf).ToString(), start, new Vector2(21, 21), EZKill(targetInf) ? Color.Lime : Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					
					Drawing.DrawText((GetComboDamage(targetInf) + GetOneAutoAttackDamage(targetInf)).ToString(), start2s, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					Drawing.DrawText((GetComboDamage(targetInf) + GetOneAutoAttackDamage(targetInf)).ToString(), start2, new Vector2(21, 21), (targetInf.Health < (GetComboDamage(targetInf) + GetOneAutoAttackDamage(targetInf))) ? Color.Lime : Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);

					Drawing.DrawText(GetComboDamageByDistance(targetInf).ToString(), start3s, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					Drawing.DrawText(GetComboDamageByDistance(targetInf).ToString(), start3, new Vector2(21, 21), (targetInf.Health < GetComboDamageByDistance(targetInf)) ? Color.Lime : Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					
					Drawing.DrawText("x"+ProcastCounter(targetInf).ToString(), start4s, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					Drawing.DrawText("x"+ProcastCounter(targetInf).ToString(), start4, new Vector2(21, 21), Color.White, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					
					
					//Drawing.DrawText(manafactdamage(targetInf).ToString(), start3+30, new Vector2(21, 21), (me.Mana > manafactdamage(targetInf)) ? Color.Aqua : Color.Blue, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);

				}

				if (Menu.Item("HitCounter").GetValue<bool>())
				{	
					var hitcounter = HitCount(targetInf);
					var starthit = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(117, HUDInfo.GetHpBarSizeY() - 13);
					var starthits = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(118, HUDInfo.GetHpBarSizeY() - 12);
					Drawing.DrawText(hitcounter.ToString()+" hits", starthits, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					Drawing.DrawText(hitcounter.ToString()+" hits", starthit, new Vector2(21, 21), (hitcounter<=1)?Color.Lime:Color.White, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
				}

				if (Menu.Item("RocketCounter").GetValue<bool>() && Rocket.Level>0)
				{	
					var startrocket = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(117, HUDInfo.GetHpBarSizeY() + 6);
					var startrockets = HUDInfo.GetHPbarPosition(targetInf) + new Vector2(118, HUDInfo.GetHpBarSizeY() + 7);
                    Drawing.DrawText(RktCount(targetInf).ToString() + " rkts", startrockets, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
                    Drawing.DrawText(RktCount(targetInf).ToString() + " rkts", startrocket, new Vector2(21, 21), (RktCount(targetInf)<=1)?Color.Lime:Color.Yellow, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					if (Refresh != null && Refresh.Level>0)
					{
						Drawing.DrawText("          (x"+OnlyRktCount(targetInf).ToString()+") "/*+OnlyRktCountDmg(targetInf)*/, startrockets, new Vector2(21, 21), Color.Black, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
						Drawing.DrawText("          (x"+OnlyRktCount(targetInf).ToString()+") "/*+OnlyRktCountDmg(targetInf)*/, startrocket, new Vector2(21, 21), (Math.Ceiling((me.Mana-manaonerocket())/(manaonerocket()+rearm_mana[Refresh.Level - 1]))>=OnlyRktCount(targetInf))?Color.Lime:Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
					}
				}
			}  
			
			if (Menu.Item("Calculator").GetValue<bool>())
			{
				var coordX = Menu.Item("BarPosX").GetValue<Slider>().Value;
				var coordY = Menu.Item("BarPosY").GetValue<Slider>().Value;
				
				Drawing.DrawText("Full cast:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordX, HUDInfo.ScreenSizeY() / 2 + 210 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("Full cast:", new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordX, HUDInfo.ScreenSizeY() / 2 + 210 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				
				
				Drawing.DrawText("x1", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -240 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x1", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x2", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordX, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x2", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordX, HUDInfo.ScreenSizeY() / 2 + 285 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                Drawing.DrawText("x3", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText("x3", new Vector2(HUDInfo.ScreenSizeX() / 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                Drawing.DrawText(GetComboDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText(GetComboDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);
                Drawing.DrawText((2 * GetComboDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText((2 * GetComboDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 285 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);
                Drawing.DrawText((3 * GetComboDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText((3 * GetComboDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 200 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                if (Menu.Item("debug").IsActive())
                {
                    Drawing.DrawText("laser dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("laser dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetLaserDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetLaserDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("rocket dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("rocket dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetRocketDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetRocketDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("dagon dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("dagon dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 410 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetDagonDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetDagonDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 410 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("eblade dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("eblade dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 - 240 + coordX, HUDInfo.ScreenSizeY() / 2 + 435 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetEtherealBladeDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetEtherealBladeDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 - 100 + coordX, HUDInfo.ScreenSizeY() / 2 + 435 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("enemy magic res:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("enemy magic res:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(targetInf.MagicDamageResist.ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 280, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(targetInf.MagicDamageResist.ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 280, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("enemy combo dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("enemy combo dmg:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetComboDamage(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 280, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetComboDamage(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 280, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("enemy combo dmg by distance:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("enemy combo dmg by distance:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 410 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetComboDamageByDistance(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 420, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetComboDamageByDistance(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 420, HUDInfo.ScreenSizeY() / 2 + 410 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("my distance to enemy:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("my distance to enemy:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 50, HUDInfo.ScreenSizeY() / 2 + 435 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(me.Distance2D(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX + 420, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(me.Distance2D(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX + 420, HUDInfo.ScreenSizeY() / 2 + 435 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("EZKill?:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX - 600, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("EZKill?:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX - 600, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(EZKill(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX - 400, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(EZKill(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX - 400, HUDInfo.ScreenSizeY() / 2 + 360 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);

                    Drawing.DrawText("EZKill damage:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX - 600, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("EZKill damage:", new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX - 600, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
                    Drawing.DrawText(GetEZKillDamage(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + coordX - 400, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(GetEZKillDamage(targetInf).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + coordX - 400, HUDInfo.ScreenSizeY() / 2 + 385 + coordY), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);
                }

                Drawing.DrawText("dmg", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordX, HUDInfo.ScreenSizeY() / 2 + 232 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("dmg", new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordX, HUDInfo.ScreenSizeY() / 2 + 232 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
				

				Drawing.DrawText("mana", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 232 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("mana", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 232 + coordY), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				if (Refresh != null && Refresh.Level>0)
				{
					Drawing.DrawText(manaprocast().ToString()+" ("+(-manaprocast()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText(manaprocast().ToString()+" ("+(-manaprocast()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + coordY), new Vector2(30, 200),(me.Mana>manaprocast())? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((2*manaprocast()+rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(2*manaprocast()+rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((2*manaprocast()+rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(2*manaprocast()+rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordX , HUDInfo.ScreenSizeY() / 2 + 285 + coordY), new Vector2(30, 200), (me.Mana>(2*manaprocast()+rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((3*manaprocast()+2*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(3*manaprocast()+2*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((3*manaprocast()+2*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(3*manaprocast()+2*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 310 + coordY), new Vector2(30, 200), (me.Mana>(3*manaprocast()+2*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
				}
				else
				{
					Drawing.DrawText(manaprocast().ToString()+" ("+(-manaprocast()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordY), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText(manaprocast().ToString()+" ("+(-manaprocast()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordX, HUDInfo.ScreenSizeY() / 2 + 260 + coordY), new Vector2(30, 200), (me.Mana>manaprocast())? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
				}
			}
				
			if (Menu.Item("CalculatorRkt").GetValue<bool>())
			{
				var coordXr = Menu.Item("BarPosXr").GetValue<Slider>().Value;
				var coordYr = Menu.Item("BarPosYr").GetValue<Slider>().Value;
				
				Drawing.DrawText("Rockets:", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 210 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("Rockets:", new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 210 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				
				Drawing.DrawText("x1", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x1", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x2", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x2", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 285 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x3", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x3", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x4", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x4", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x5", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x5", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x6", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x6", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x7", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x7", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x8", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x8", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				Drawing.DrawText("x9", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("x9", new Vector2(HUDInfo.ScreenSizeX() / 2-240 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				

				Drawing.DrawText("dmg", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 232 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("dmg", new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 232 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);
				
				Drawing.DrawText(GetRocketDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText(GetRocketDamage().ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((2*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((2*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 285 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((3*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((3*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((4*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((4*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((5*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((5*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((6*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((6*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((7*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((7*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((8*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((8*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			
				Drawing.DrawText((9*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText((9*GetRocketDamage()).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + coordYr), new Vector2(30, 200), Color.LimeGreen, FontFlags.AntiAlias);			

							
				Drawing.DrawText("mana", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 232 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
				Drawing.DrawText("mana", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 232 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				


				if (Refresh != null && Refresh.Level>0)
				{
					Drawing.DrawText("               x"+ Math.Ceiling((me.Mana-manaonerocket())/(manaonerocket()+rearm_mana[Refresh.Level - 1] )).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 210 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText("               x"+ Math.Ceiling((me.Mana-manaonerocket())/(manaonerocket()+rearm_mana[Refresh.Level - 1] )).ToString(), new Vector2(HUDInfo.ScreenSizeX() / 2-200 + coordXr, HUDInfo.ScreenSizeY() / 2 + 210 + coordYr), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);			
				
				
					Drawing.DrawText(manaonerocket().ToString()+" ("+(-manaonerocket()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText(manaonerocket().ToString()+" ("+(-manaonerocket()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + coordYr), new Vector2(30, 200),(me.Mana>manaonerocket())? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((2*manaonerocket()+rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(2*manaonerocket()+rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 285 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((2*manaonerocket()+rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(2*manaonerocket()+rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr , HUDInfo.ScreenSizeY() / 2 + 285 + coordYr), new Vector2(30, 200), (me.Mana>(2*manaonerocket()+rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((3*manaonerocket()+2*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(3*manaonerocket()+2*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((3*manaonerocket()+2*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(3*manaonerocket()+2*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 310 + coordYr), new Vector2(30, 200), (me.Mana>(3*manaonerocket()+2*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((4*manaonerocket()+3*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(4*manaonerocket()+3*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((4*manaonerocket()+3*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(4*manaonerocket()+3*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 335 + coordYr), new Vector2(30, 200), (me.Mana>(4*manaonerocket()+3*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((5*manaonerocket()+4*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(5*manaonerocket()+4*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((5*manaonerocket()+4*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(5*manaonerocket()+4*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 360 + coordYr), new Vector2(30, 200), (me.Mana>(5*manaonerocket()+4*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((6*manaonerocket()+5*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(6*manaonerocket()+5*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((6*manaonerocket()+5*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(6*manaonerocket()+5*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 385 + coordYr), new Vector2(30, 200), (me.Mana>(6*manaonerocket()+5*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((7*manaonerocket()+6*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(7*manaonerocket()+6*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((7*manaonerocket()+6*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(7*manaonerocket()+6*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 410 + coordYr), new Vector2(30, 200), (me.Mana>(7*manaonerocket()+6*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((8*manaonerocket()+7*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(8*manaonerocket()+7*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((8*manaonerocket()+7*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(8*manaonerocket()+7*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 435 + coordYr), new Vector2(30, 200), (me.Mana>(8*manaonerocket()+7*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
					Drawing.DrawText((9*manaonerocket()+8*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(9*manaonerocket()+8*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText((9*manaonerocket()+8*rearm_mana[Refresh.Level - 1]).ToString()+" ("+(-(9*manaonerocket()+8*rearm_mana[Refresh.Level - 1])+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 460 + coordYr), new Vector2(30, 200), (me.Mana>(9*manaonerocket()+8*rearm_mana[Refresh.Level - 1]))? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
				
				}
				else
				{
					Drawing.DrawText(manaonerocket().ToString()+" ("+(-manaonerocket()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + 2 + coordYr), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
					Drawing.DrawText(manaonerocket().ToString()+" ("+(-manaonerocket()+(int)me.Mana).ToString()+")", new Vector2(HUDInfo.ScreenSizeX() / 2 -80 + coordXr, HUDInfo.ScreenSizeY() / 2 + 260 + coordYr), new Vector2(30, 200), (me.Mana>manaonerocket())? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);			
				}
			}

            if (Menu.Item("ComboModeDrawing").GetValue<bool>())
            {

                if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    //Drawing.DrawText(Menu.Item("Chase").GetValue<KeyBind>().Active == true ? "Chasing" : "Comboing", new Vector2(HUDInfo.ScreenSizeX() / 2 +2, HUDInfo.ScreenSizeY() / 2 + 160 +2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    //Drawing.DrawText(Menu.Item("Chase").GetValue<KeyBind>().Active == true ? "Chasing" : "Comboing", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 160), new Vector2(30, 200), Menu.Item("Chase").GetValue<KeyBind>().Active == true ? Color.Red : Color.LimeGreen, FontFlags.AntiAlias);
                    Drawing.DrawText(" ON!", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2 + 150, HUDInfo.ScreenSizeY() / 2 + 235 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText(" ON!", new Vector2(HUDInfo.ScreenSizeX() / 2 + 150, HUDInfo.ScreenSizeY() / 2 + 235), new Vector2(30, 200), Menu.Item("Chase").GetValue<KeyBind>().Active == true ? Color.Red : Color.LimeGreen, FontFlags.AntiAlias);

                }

                if (Game.IsKeyDown(Menu.Item("Rocket Spam Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    Drawing.DrawText("Rocket Spam!", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2, HUDInfo.ScreenSizeY() / 2 + 185 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("Rocket Spam!", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 185), new Vector2(30, 200), Color.Yellow, FontFlags.AntiAlias);
                }

                if (Game.IsKeyDown(Menu.Item("March Spam Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                {
                    Drawing.DrawText("March Spam!", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2, HUDInfo.ScreenSizeY() / 2 + 210 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText("March Spam!", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 210), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);

                }

                Drawing.DrawText(Menu.Item("Chase").GetValue<KeyBind>().Active == true ? "Chase Mode" : "Combo Mode", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2, HUDInfo.ScreenSizeY() / 2 + 235 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText(Menu.Item("Chase").GetValue<KeyBind>().Active == true ? "Chase Mode" : "Combo Mode", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 235), new Vector2(30, 200), Menu.Item("Chase").GetValue<KeyBind>().Active == true ? Color.Red : Color.LimeGreen, FontFlags.AntiAlias);

                Drawing.DrawText(Menu.Item("TargetLock").GetValue<StringList>().SelectedIndex == 0 ? "Target: Free" : "Target: Lock", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2, HUDInfo.ScreenSizeY() / 2 + 285 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                Drawing.DrawText(Menu.Item("TargetLock").GetValue<StringList>().SelectedIndex == 0 ? "Target: Free" : "Target: Lock", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 285), new Vector2(30, 200), Color.White, FontFlags.AntiAlias);

                if (Menu.Item("autoKillsteal").GetValue<bool>())
                {
                    Drawing.DrawText((Menu.Item("Chase").GetValue<KeyBind>().Active == true || !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) ? "KS: on" : "KS: off", new Vector2(HUDInfo.ScreenSizeX() / 2 + 2, HUDInfo.ScreenSizeY() / 2 + 260 + 2), new Vector2(30, 200), Color.Black, FontFlags.AntiAlias);
                    Drawing.DrawText((Menu.Item("Chase").GetValue<KeyBind>().Active == true || !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) ? "KS: on" : "KS: off", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2 + 260), new Vector2(30, 200), (Menu.Item("Chase").GetValue<KeyBind>().Active == true || !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)) ? Color.LimeGreen : Color.Red, FontFlags.AntiAlias);
                }
            }
		}

        public static float GetComboDamage()
        {
            var comboDamage = 0.0f;
            var totalMagicResistance = 0.0f;
            var etheral_blade_magic_reduction = 0.0f;
            var veil_of_discord_magic_reduction = 0.0f;
            var base_magic_res = 0.25f;

            var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

            if (eblade != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
            {
                etheral_blade_magic_reduction = 0.4f;
            }

            var veil = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_veil_of_discord"));

            if (veil != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
            {
                veil_of_discord_magic_reduction = 0.25f;
            }

            totalMagicResistance = ((1 - base_magic_res) * (1 + etheral_blade_magic_reduction) * (1 + veil_of_discord_magic_reduction));

            comboDamage = ((GetEtherealBladeDamage() + GetRocketDamage() + GetDagonDamage()) * totalMagicResistance) + GetLaserDamage();

            return comboDamage;
        }

        public static float GetComboDamage(Hero enemy)
        {
            var comboDamage = 0.0f;
            var totalMagicResistance = 0.0f;
            var etheral_blade_magic_reduction = 0.0f;
            var veil_of_discord_magic_reduction = 0.0f;

            var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

            if (eblade != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
            {
                etheral_blade_magic_reduction = 0.4f;
            }

            var veil = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_veil_of_discord"));

            if (veil != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
            {
                veil_of_discord_magic_reduction = 0.25f;
            }

            totalMagicResistance = ((1 - enemy.MagicDamageResist) * (1 + etheral_blade_magic_reduction) * (1 + veil_of_discord_magic_reduction));

            comboDamage = ((GetEtherealBladeDamage() + GetRocketDamage() + GetDagonDamage()) * totalMagicResistance) + GetLaserDamage();

            return comboDamage;
        }

        //EZKill will only consider if dagon, ethereal blade and veil will kill the enemy
        public static bool EZKill(Hero enemy)
        {
            if (enemy != null && enemy.IsAlive && enemy.IsValid)
            {
                var EZKillDamage = 0.0f;
                var totalMagicResistance = 0.0f;
                var etheral_blade_magic_reduction = 0.0f;
                var veil_of_discord_magic_reduction = 0.0f;

                var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

                if (eblade != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                {
                    etheral_blade_magic_reduction = 0.4f;
                }

                var veil = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_veil_of_discord"));

                if (veil != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
                {
                    veil_of_discord_magic_reduction = 0.25f;
                }

                totalMagicResistance = ((1 - enemy.MagicDamageResist) * (1 + etheral_blade_magic_reduction) * (1 + veil_of_discord_magic_reduction));

                EZKillDamage = (GetEtherealBladeDamage() + GetDagonDamage()) * totalMagicResistance;
                if (enemy.Health < EZKillDamage)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static float GetEZKillDamage(Hero enemy)
        {
            if (enemy != null && enemy.IsAlive && enemy.IsValid)
            {
                var EZKillDamage = 0.0f;
                var totalMagicResistance = 0.0f;
                var etheral_blade_magic_reduction = 0.0f;
                var veil_of_discord_magic_reduction = 0.0f;

                var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

                if (eblade != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                {
                    etheral_blade_magic_reduction = 0.4f;
                }

                var veil = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_veil_of_discord"));

                if (veil != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(veil.Name))
                {
                    veil_of_discord_magic_reduction = 0.25f;
                }

                totalMagicResistance = ((1 - enemy.MagicDamageResist) * (1 + etheral_blade_magic_reduction) * (1 + veil_of_discord_magic_reduction));

                return EZKillDamage = (GetEtherealBladeDamage() + GetDagonDamage()) * totalMagicResistance;
            }
            else
                return 0.0f;
        }

        public static float GetComboDamageByDistance(Hero enemy)
        {
            if (enemy != null && enemy.IsAlive && enemy.IsValid)
            {
                var comboDamageByDistance = 0.0f;
                var totalMagicResistance = 0.0f;
                var etheral_blade_magic_reduction = 0.0f;
                var veil_of_discord_magic_reduction = 0.0f;

                var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

                if (((eblade != null && eblade.CanBeCasted())
                    || (eblade != null && IsCasted(eblade)))
                    && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_ethereal_blade")
                    && !enemy.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_ethereal"))
                {
                    etheral_blade_magic_reduction = 0.4f;
                }

                var veil = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_veil_of_discord"));

                if (veil != null
                    && veil.CanBeCasted()
                    && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_veil_of_discord")
                    && !enemy.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff"))
                {
                    veil_of_discord_magic_reduction = 0.25f;
                }

                totalMagicResistance = ((1 - enemy.MagicDamageResist) * (1 + etheral_blade_magic_reduction) * (1 + veil_of_discord_magic_reduction));

                var dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));

                if (dagon != null
                    && dagon.CanBeCasted()
                    && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                    && (me.Distance2D(enemy) < dagon.AbilitySpecialData.First(x => x.Name == "#AbilityCastRange").GetValue(dagon.Level - 1) + castrange + ensage_error))
                {
                    comboDamageByDistance += GetDagonDamage() * totalMagicResistance;
                }

                if (((eblade != null && eblade.CanBeCasted())
                    || (eblade != null && IsCasted(eblade)))
                    && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_ethereal_blade")
                    && (me.Distance2D(enemy) < 800 + castrange + ensage_error))
                {
                    comboDamageByDistance += GetEtherealBladeDamage() * totalMagicResistance;
                }

                var laser = me.Spellbook.SpellQ;

                if (laser != null
                    && laser.Level > 0
                    && laser.CanBeCasted()
                    && (me.Distance2D(enemy) < 650 + castrange + ensage_error))
                {
                    comboDamageByDistance += GetLaserDamage();
                }

                //Distance Calculation
                var rocket = me.Spellbook.SpellW;

                if ((rocket != null && rocket.Level > 0 && rocket.CanBeCasted()) || (rocket != null && rocket.Level > 0 && IsCasted(rocket)))
                {
                    if (me.Distance2D(enemy) < 800 + castrange + ensage_error)
                        comboDamageByDistance += GetRocketDamage() * totalMagicResistance;
                    else if (me.Distance2D(enemy) >= 800 + castrange + ensage_error && me.Distance2D(enemy) < 1600 + castrange + ensage_error)
                        comboDamageByDistance += GetRocketDamage() * ((1 - enemy.MagicDamageResist) * (1 + veil_of_discord_magic_reduction));
                    else if (me.Distance2D(enemy) >= 1600 + castrange + ensage_error && me.Distance2D(enemy) < 2500)
                        comboDamageByDistance += GetRocketDamage() * ((1 - enemy.MagicDamageResist));
                }

                if (me.CanAttack()
                    && !enemy.IsAttackImmune()
                    && me.Distance2D(enemy) < me.GetAttackRange() + 50
                    && !enemy.IsInvul()
                    && !IsPhysDamageImune(enemy)
                    //&& (ethereal == null || !ethereal.CanBeCasted())
                    //&& (ghost == null || !ghost.CanBeCasted() || !Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name))
                    )
                {
                    comboDamageByDistance += (enemy.DamageTaken(me.BonusDamage + me.DamageAverage, DamageType.Physical, me));
                }

                return comboDamageByDistance;
            }
            else
            {
                return 0;
            }
        }

        public static float GetLaserDamage()
        {

            var laserDamage = 0.0f;
            var totalSpellAmp = 0.0f;

            var laser = me.Spellbook.SpellQ;
            if (laser.Level > 0)
            {
                laserDamage += laser.AbilitySpecialData.First(x => x.Name == "laser_damage").GetValue(laser.Level - 1);
            }

            var talent25 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_unique_tinker");
            if (talent25.Level > 0)
            {
                laserDamage += talent25.AbilitySpecialData.First(x => x.Name == "value").Value;
            }

            //Spell Amplification Calculation (addition)
            var talent15 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_spell_amplify_4");
            if (talent15.Level > 0)
            {
                totalSpellAmp += (talent15.AbilitySpecialData.First(x => x.Name == "value").Value) / 100.0f;
            }

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);
            if (aetherLens != null)
            {
                totalSpellAmp += (aetherLens.AbilitySpecialData.First(x => x.Name == "spell_amp").Value) / 100.0f;
            }

            totalSpellAmp += (100.0f + me.TotalIntelligence / 16.0f) / 100.0f;

            laserDamage *= totalSpellAmp;

            return laserDamage;
        }

        public static float GetRocketDamage()
        {
            var rocketDamage = 0.0f;
            var totalSpellAmp = 0.0f;

            var rocket = me.Spellbook.SpellW;

            if (rocket.Level > 0)
            {
                rocketDamage += rocket.AbilitySpecialData.First(x => x.Name == "#AbilityDamage").GetValue(rocket.Level - 1);
            }

            //Spell Amplification Calculation (addition)
            var talent15 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_spell_amplify_4");
            if (talent15.Level > 0)
            {
                totalSpellAmp += (talent15.AbilitySpecialData.First(x => x.Name == "value").Value) / 100.0f;
            }

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);
            if (aetherLens != null)
            {
                totalSpellAmp += (aetherLens.AbilitySpecialData.First(x => x.Name == "spell_amp").Value) / 100.0f;
            }

            totalSpellAmp += (100.0f + me.TotalIntelligence / 16.0f) / 100.0f;

            rocketDamage *= totalSpellAmp;

            return rocketDamage;
        }

        public static float GetDagonDamage()
        {
            var dagonDamage = 0.0f;
            var totalSpellAmp = 0.0f;

            var dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));

            if (dagon != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
            {
                dagonDamage += (dagon.AbilitySpecialData.FirstOrDefault(x => x.Name == "damage").GetValue(dagon.Level - 1));
            }

            //Spell Amplification Calculation (addition)
            var talent15 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_spell_amplify_4");
            if (talent15.Level > 0)
            {
                totalSpellAmp += (talent15.AbilitySpecialData.First(x => x.Name == "value").Value) / 100.0f;
            }

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);
            if (aetherLens != null)
            {
                totalSpellAmp += (aetherLens.AbilitySpecialData.First(x => x.Name == "spell_amp").Value) / 100.0f;
            }

            totalSpellAmp += (100.0f + me.TotalIntelligence / 16.0f) / 100.0f;

            dagonDamage *= totalSpellAmp;

            return dagonDamage;
        }

        public static float GetOneAutoAttackDamage(Hero enemy)
        {
            var OneAutoAttackDamage = 0.0f;

            if (me.CanAttack()
                && !enemy.IsAttackImmune()
                && !enemy.IsInvul()
                && !IsPhysDamageImune(enemy))
            {
                OneAutoAttackDamage += (enemy.DamageTaken(me.BonusDamage + me.DamageAverage, DamageType.Physical, me));
            }

            return OneAutoAttackDamage;
        }

        public static float GetEtherealBladeDamage()
        {
            var etherealBladeDamage = 0.0f;
            var totalSpellAmp = 0.0f;

            var eblade = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_ethereal_blade"));

            if (eblade != null && Menu.Item("ComboItems: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
            {
                etherealBladeDamage += ((me.TotalIntelligence * eblade.AbilitySpecialData.FirstOrDefault(x => x.Name == "blast_agility_multiplier").Value) + eblade.AbilitySpecialData.FirstOrDefault(x => x.Name == "blast_damage_base").Value);
            }

            //Spell Amplification Calculation (addition)
            var talent15 = me.Spellbook.Spells.First(x => x.Name == "special_bonus_spell_amplify_4");
            if (talent15.Level > 0)
            {
                totalSpellAmp += (talent15.AbilitySpecialData.First(x => x.Name == "value").Value) / 100.0f;
            }

            var aetherLens = me.Inventory.Items.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Item_Aether_Lens);
            if (aetherLens != null)
            {
                totalSpellAmp += (aetherLens.AbilitySpecialData.First(x => x.Name == "spell_amp").Value) / 100.0f;
            }

            totalSpellAmp += (100.0f + me.TotalIntelligence / 16.0f) / 100.0f;

            etherealBladeDamage *= totalSpellAmp;

            return etherealBladeDamage;
        }

        public static Vector3 GetClosestToVector(Vector3[] coords, Unit z)
        {
            var closestVector = coords.First();
            foreach (var v in coords.Where(v => closestVector.Distance2D(z) > v.Distance2D(z)))
                closestVector = v;
            return closestVector;
        }
                
        public static void ParticleDraw(EventArgs args)
        {
            //
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;

            if (me == null) return;

            for (int i = 0; i < TinkerCords.SafePos.Count(); ++i)
            {
                if (!iscreated)
                {
                    ParticleEffect effect = new ParticleEffect(EffectPath, TinkerCords.SafePos[i]);
                    effect.SetControlPoint(1, new Vector3(HIDE_AWAY_RANGE, 0, 0));
                    Effects.Add(effect);
                }
            }
            iscreated = true;
        }

        private class TinkerCords
        {
            public static readonly Vector3[]
            SafePos =
            {
            new Vector3(-7305, -5016, 384),
            new Vector3(-7328, -4768, 384),
            new Vector3(-7264, -4505, 384),
            new Vector3(-7136, -4384, 384),
            new Vector3(-7072, -1120, 384),
            new Vector3(-7072, -672, 384),
            new Vector3(-7200, -288, 384),
            new Vector3(-6880, 288, 384),
            new Vector3(-6944, 1568, 384),
            new Vector3(-6688, 3488, 384),
            new Vector3(-6752, 3616, 384),
            new Vector3(-6816, 3744, 384),
            new Vector3(-6816, 4448, 384),
            new Vector3(-5152, 5088, 384),
            new Vector3(-3936, 5536, 384),
            new Vector3(-5152, 6624, 384),
            new Vector3(-3680, 6624, 384),
            new Vector3(-2720, 6752, 384),
            new Vector3(-2720, 5536, 384),
            new Vector3(-1632, 6688, 384),
            new Vector3(-1056, 6752, 384),
            new Vector3(-736, 6816, 384),
            new Vector3(-992, 5536, 384),
            new Vector3(-1568, 5536, 384),
            new Vector3(608, 7008, 384),
            new Vector3(1632, 6752, 256),
            new Vector3(2336, 7136, 384),
            new Vector3(1568, 3040, 384),
            new Vector3(1824, 3296, 384),
            new Vector3(-2976, 480, 384),
            new Vector3(736, 1056, 256),
            new Vector3(928, 1248, 256),
            new Vector3(928, 1696, 256),
            new Vector3(2784, 992, 256),
            new Vector3(-2656, -1440, 256),
            new Vector3(-2016, -2464, 256),
            new Vector3(-2394, -3110, 256),
            new Vector3(-1568, -3232, 256),
            new Vector3(-2336, -4704, 256),
            new Vector3(-416, -7072, 384),
            new Vector3(2336, -5664, 384),
            new Vector3(2464, -5728, 384),
            new Vector3(2848, -5664, 384),
            new Vector3(2400, -6817, 384),
            new Vector3(3040, -6624, 384),
            new Vector3(4256, -6624, 384),
            new Vector3(4192, -6880, 384),
            new Vector3(5024, -5408, 384),
            new Vector3(5856, -6240, 384),
            new Vector3(6304, -6112, 384),
            new Vector3(6944, -5472, 384),
            new Vector3(7328, -5024, 384),
            new Vector3(7200, -3296, 384),
            new Vector3(7200, -2272, 384),
            new Vector3(6944, -992, 384),
            new Vector3(6816, -224, 384),
            new Vector3(7200, 480, 384),
            new Vector3(7584, 2080, 256),
            new Vector3(7456, 2784, 384),
            new Vector3(5344, 2528, 384),
            new Vector3(7200, 5536, 384),
            new Vector3(4192, 6944, 384),
            new Vector3(5472, 6752, 384),
            new Vector3(-6041, -6883, 384),
            new Vector3(-5728, -6816, 384),
            new Vector3(-5408, -7008, 384),
            new Vector3(-5088, -7072, 384),
            new Vector3(-4832, -7072, 384),
            new Vector3(-3744, -7200, 384)
        };
            public static readonly Vector3[]
            PanicPos =
            {

        };
        }
    }
}
