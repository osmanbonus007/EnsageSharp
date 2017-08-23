using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using VisageSharpRewrite.Abilities;
using VisageSharpRewrite.Features;
using VisageSharpRewrite.Utilities;

namespace VisageSharpRewrite
{
    public class VisageSharp
    {

        private static Hero Me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private static List<Unit> Familiars
        {
            get
            {
                return Variables.Familiars;
            }
        }

        private bool pause;

        private FamiliarAutoLast familiarAutoLastHit;

        private TalentAbuse talentAbuse;

        private AutoNuke autoNuke;

        private Follow follow;

        private DrawText drawText;

        private TargetFind targetFind;

        private Combo combo;

        private bool FollowHasLock;

        private bool LashitHasLock;

        private bool NukeLock;

        private Hero Target
        {
            get
            {
                return this.targetFind.Target;
            }
        }

        public VisageSharp()
        {
            this.familiarAutoLastHit = new FamiliarAutoLast();
        }


        public void OnDraw()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            drawText.DrawAutoLastHit(Variables.InAutoLasthiMode);
            drawText.DrawAutoNuke(Variables.AutoSoulAumptionOn);
            drawText.DrawTextCombo(Variables.ComboOn);
            drawText.DrawFollow(Variables.FollowMode);
            if (!Variables.ComboOn)
            {
                combo.DisableParticleEffect();
                return;                
            }
            if (this.Target == null) return;
            combo.DrawTarget(Target);
            combo.DrawParticleEffect(Target);          
            this.targetFind.DrawTarget();

        }

        public void OnLoad()
        {
            Variables.Hero = ObjectManager.LocalHero;
            this.pause = Variables.Hero.ClassId != ClassId.CDOTA_Unit_Hero_Visage;
            if (this.pause) return;
            Variables.MenuManager = new MenuManager(Me.Name);
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar") && unit.IsAlive).ToList();
            Variables.graveChill = new GraveChill(Me.Spellbook.Spell1);
            Variables.soulAssumption = new SoulAssumption(Me.Spellbook.Spell2);
            Variables.familiarControl = new FamiliarControl();
            Variables.MenuManager.Menu.AddToMainMenu();
            Variables.EnemyTeam = Me.GetEnemyTeam();
            this.familiarAutoLastHit = new FamiliarAutoLast();
            this.autoNuke = new AutoNuke();
            this.follow = new Follow();
            this.drawText = new DrawText();
            this.targetFind = new TargetFind();
            this.combo = new Combo();
            this.talentAbuse = new TalentAbuse();
            Game.PrintMessage(
                "VisageSharp" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " loaded");
        }

        public void OnUpdate_TalentAbuse()
        {
            if (Game.IsPaused)
            {
                //this.pause = Game.IsPaused;
                return;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            if (!Variables.InAutoLasthiMode) return;
            talentAbuse.Execute();
        }

        public void OnUpdate_AutoLastHit()
        {
            if (Game.IsPaused)
            {
                //this.pause = Game.IsPaused;
                return;
            }
            
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            if (!Variables.InAutoLasthiMode) return;
            if (!Variables.Familiars.Any(x => x != null) || !Variables.Familiars.Any(x => x.IsAlive) || !Variables.Familiars.Any(x => x.IsValid))
            {
                return;
            }
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar") && unit.IsAlive).ToList();

            if (Familiars == null) return;
            familiarAutoLastHit.Execute(Familiars);             
        }

        public void OnUpdate_AutoNuke()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            var EnemyNearMe = ObjectManager.GetEntities<Hero>().Any(x => x.IsAlive
                                                                          && x.Team != Me.Team
                                                                          && Me.Distance2D(x) <= 1500);
            if (!EnemyNearMe) return;
            if (!Variables.AutoSoulAumptionOn)
            {
                return;
            }
            autoNuke.Execute(Me);
        }

        public void OnUpdate_Follow()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar") && unit.IsAlive && unit.Team == Me.Team).ToList();
            if (Variables.FollowMode)
            {
                if (Utils.SleepCheck("follow"))
                {
                    follow.Execute(Familiars);
                    Utils.Sleep(1000, "follow");
                }
            }
        }

        public void OnUpdate_LowHP()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar") && unit.IsAlive && unit.Team == Me.Team).ToList();

            if (Utils.SleepCheck("lowHP"))
            {
                foreach (var LowHP in Familiars)
                {
                    if (LowHP.Health <= Variables.MenuManager.FamiliarsLowHP.GetValue<Slider>().Value
                        && LowHP.Spellbook.SpellQ.CanBeCasted())
                    {
                        LowHP.Spellbook.SpellQ.UseAbility();
                    }
                    
                }
                Utils.Sleep(200, "lowHP");
            }
        }

        public void OnUpdate_Combo()
        {
            /*
            if (this.pause || ((Variables.Hero == null && Variables.Familiars == null) || (!Variables.Hero.IsAlive && !Variables.Familiars.Any(x => x.IsAlive)))) return;
            */
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            if (!Variables.ComboOn)
            {
                this.targetFind.UnlockTarget();
            }
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar") && unit.IsAlive && unit.Team == Me.Team).ToList();
            if (Variables.ComboOn)
            {
                combo.Execute(Me, Target, Familiars);
            }



        }

        public void OnUpdate_MenuControl()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            this.targetFind.Find();
            Variables.Familiars = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name.Contains("npc_dota_visage_familiar")).ToList();
            //lasthit mode will disable follow mode

            
            if (LashitHasLock)
            {
                if (Variables.InAutoLasthiMode)
                {
                    if (Variables.FollowMode)
                    {
                        Variables.MenuManager.FamiliarFollowMenu.SetValue(new KeyBind(Variables.MenuManager.FamiliarFollowMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                    }
                }
                LashitHasLock = false;
            }

            //follow mode with disable lasthit mode
            if (Variables.FollowMode)
            {
                if (Variables.InAutoLasthiMode)
                {
                    Variables.MenuManager.AutoFamiliarLastHitMenu.SetValue(new KeyBind(Variables.MenuManager.AutoFamiliarLastHitMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                LashitHasLock = true;
            }

            //disable autonuke in Combo mode and return back with AutoNuke on
            if (NukeLock)
            {
                if (!Variables.AutoSoulAumptionOn)
                {
                    Variables.MenuManager.AutoSoulAssumpMenu.SetValue(new KeyBind(Variables.MenuManager.AutoSoulAssumpMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, true));
                    
                }
                NukeLock = false;
            }

            if (Variables.ComboOn)
            {
                //disable familiar auto last hit
                //disable follow mode
                if (Variables.AutoSoulAumptionOn)
                {
                    Variables.MenuManager.AutoSoulAssumpMenu.SetValue(new KeyBind(Variables.MenuManager.AutoSoulAssumpMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                if (Variables.InAutoLasthiMode && Variables.familiarControl.AnyFamiliarNearMe(Familiars, 2000))
                {
                    Variables.MenuManager.AutoFamiliarLastHitMenu.SetValue(new KeyBind(Variables.MenuManager.AutoFamiliarLastHitMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                if (Variables.FollowMode)
                {
                    Variables.MenuManager.FamiliarFollowMenu.SetValue(new KeyBind(Variables.MenuManager.FamiliarFollowMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                FollowHasLock = true;
                NukeLock = true;
            }
            else
            {
                if (FollowHasLock) // Follow has lock to return to Follow Mode from Combo
                {
                    if (!Variables.InAutoLasthiMode && !Variables.FollowMode && !Variables.ComboOn && Variables.familiarControl.AnyFamiliarNearMe(Familiars, 1500))
                    {
                        //auto switch to follow mode
                        //Variables.MenuManager.FamiliarFollowMenu.SetValue(new KeyBind(Variables.MenuManager.FamiliarFollowMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, true));
                    }
                    //release lock;
                    FollowHasLock = false; 
                }
            }

        }



        public void OnWndProc(WndEventArgs args)
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            /*
            if (this.Target == null || !this.Target.IsValid)
            {
                return;
            }
            */
        }

        public void Player_OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive || Familiars == null)
            {
                return;
            }
            //unlock target
            this.targetFind.UnlockTarget();
            if (!Variables.InAutoLasthiMode)
            {
                //reset autoattack mode
                familiarAutoLastHit.PlayerExecution();
            }
            //refinements on follow mode, familiar will duplicate Hero movement if familiars are next the heros, instead of simply following.
            if (sender.Equals(ObjectManager.LocalPlayer))
            {
                /*if (Variables.FollowMode)
                {
                    follow.PlayerExecution(args, Familiars);
                }*/
            }              
            
            //diable particile effect
            if (Variables.ComboOn)
            {
                combo.PlayerExecution(Target);
            }
        }

        public void OnClose()
        {
            this.pause = true;
            if (Variables.MenuManager != null)
            {
                Variables.MenuManager.Menu.RemoveFromMainMenu();
            }

            Variables.PowerTreadsSwitcher = null;
            Variables.familiarControl = null;
            Variables.Familiars = null;
        }




    }
}
