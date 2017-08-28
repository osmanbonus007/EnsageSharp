using System.ComponentModel.Composition;
using System.Linq;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Helpers;
using Ensage.SDK.Extensions;
using Ensage.SDK.Renderer;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

namespace BeAwarePlus
{
    [ExportPlugin("BeAwarePlus", StartupMode.Auto, "YEEEEEEE", "3.0.1.0")]
    internal class BeAwarePlus : Plugin
    {
        private BeAwarePlusConfig Config { get; set; }

        public IRendererManager Render { get; }

        public IEntityContext<Unit> Context { get; }

        private Hero CheckEnemyBlink { get; set; }

        [ImportingConstructor]
        public BeAwarePlus(
            [Import] IRendererManager render, 
            [Import] IEntityContext<Unit> context)
        {
            Render = render;
            Context = context;
        }

        protected override void OnActivate()
        {
            Config = new BeAwarePlusConfig(this);

            Unit.OnModifierAdded += OnModifierEvent;
            Entity.OnParticleEffectAdded += OnParticleEvent;
            ObjectManager.OnAddEntity += OnEntityEvent;
        }

        protected override void OnDeactivate()
        {
            Unit.OnModifierAdded -= OnModifierEvent;
            Entity.OnParticleEffectAdded -= OnParticleEvent;
            ObjectManager.OnAddEntity -= OnEntityEvent;

            Config?.Dispose();
        }

        private void OnModifierEvent(Unit sender, ModifierChangedEventArgs args)
        {
            if (Config.ModifierToTexture.ModifierAllyList.Any(x => args.Modifier.Name == x.Key))
            {
                if (sender.Team == Context.Owner.Team)
                {
                    Config.Modifiers.ModifierAlly(sender, args);
                }
            }
            else if (Config.ModifierToTexture.ModifierEnemyList.Any(x => args.Modifier.Name == x.Key))
            {
                if (sender.Team != Context.Owner.Team)
                {
                    Config.Modifiers.ModifierEnemy(sender, args);
                }
            }
            else if (Config.ModifierToTexture.ModifierOthersList.Any(x => args.Modifier.Name == x.Key))
            {
                if (sender.Team != Context.Owner.Team && Utils.SleepCheck(args.Modifier.TextureName))
                {
                    var Tick = Config.ModifierToTexture.ModifierOthersList.FirstOrDefault(
                        x => 
                        args.Modifier.Name == x.Key).Value;

                    Config.Modifiers.ModifierOthers(sender, args);
                    Utils.Sleep(Tick, args.Modifier.TextureName);
                }
            }
        }

        private void OnEntityEvent(EntityEventArgs args)
        {
            if (args.Entity.Name.Contains("npc_dota_base"))
            {
                var Unit = args.Entity as Unit;

                if (Config.EntityToTexture.EntityVisionTexture.Any(x => Unit.DayVision == x.Key))
                {
                    if (Unit.DayVision == 200 && EntityManager<Hero>.Entities.Any(x => 
                    x.HeroId == HeroId.npc_dota_hero_invoker)
                    || Unit.DayVision == 500 && !EntityManager<Hero>.Entities.Any(x => 
                    x.HeroId == HeroId.npc_dota_hero_mirana))
                    {
                        return;
                    }
                        
                    var AbilityTexturName = Config.EntityToTexture.EntityVisionTexture.FirstOrDefault(x => 
                    Unit.DayVision == x.Key).Value;

                    var Hero = EntityManager<Hero>.Entities.FirstOrDefault(x =>
                    x.Spellbook.Spells.Any(v =>
                    v.Name.Contains(AbilityTexturName)));

                   Config.Entities.Entity(
                        Hero, 
                        args, 
                        AbilityTexturName);
                }
            }

            if (args.Entity.Name.Contains("npc_dota_thinker")
                || args.Entity.Name.Contains("tusk_frozen_sigil")
                || args.Entity.Name.Contains("gyrocopter_homing_missile")
                || args.Entity.Name.Contains("juggernaut_healing_ward")
                || args.Entity.Name.Contains("slark_visual")
                || args.Entity.Name.Contains("templar_assassin_psionic_trap")
                || args.Entity.Name.Contains("pugna_nether_ward")
                || args.Entity.Name.Contains("shadow_shaman_ward")
                || args.Entity.Name.Contains("stormspirit_remnant"))
            {
                if (Config.EntityToTexture.EntityTexture.Any(x => args.Entity.Owner.Name.Contains(x.Key)))
                {
                    var AbilityTexturName = Config.EntityToTexture.EntityTexture.FirstOrDefault(x =>
                    args.Entity.Owner.Name.Contains(x.Key)).Value;

                    Config.Entities.Entity(
                        args.Entity.Owner as Hero, 
                        args, 
                        AbilityTexturName);
                }
            }
        }
        private void OnParticleEvent(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (args.Name.Contains("blink_dagger_end"))
            {
                CheckEnemyBlink = sender as Hero;
            }

            /*if ((args.Name.Contains("sven_spell_great_cleave.vpcf")
                || args.Name.Contains("magnataur_empower_cleave_effect.vpcf")
                || args.Name.Contains("battlefury_cleave.vpcf")
                || args.Name.Contains("tiny_grow_cleave.vpcf")) //TEST   */                              

            // GetControlPoint 0
            if (Config.ParticleToTexture.ControlPoint_0.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_0.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        Config.ParticleSpells.Spells(
                            sender as Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(0));
                    });
            }

            // GetControlPoint 0 Fix
            if (Config.ParticleToTexture.ControlPoint_0Fix.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_0Fix.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsValid &&
                            !x.IsIllusion &&
                            x.Spellbook.Spells.Any(v =>
                            v.Name.Contains(AbilityTexturName)));

                        Config.ParticleSpells.Spells(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(0));
                    });
            }

            // GetControlPoint 1
            if (Config.ParticleToTexture.ControlPoint_1.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_1.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        Config.ParticleSpells.Spells(
                            sender as Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(1));
                    });
            }

            // GetControlPoint 1 Fix
            if (Config.ParticleToTexture.ControlPoint_1Fix.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_1Fix.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsValid &&
                            !x.IsIllusion &&
                            x.Spellbook.Spells.Any(v =>
                            v.Name.Contains(AbilityTexturName)));

                        Config.ParticleSpells.Spells(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(1));
                    });
            }

            // GetControlPoint 2
            if (Config.ParticleToTexture.ControlPoint_2.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_2.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        Config.ParticleSpells.Spells(
                            sender as Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(2));
                    });
            }

            // GetControlPoint 2 Fix
            if (Config.ParticleToTexture.ControlPoint_2Fix.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_2Fix.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsValid &&
                            !x.IsIllusion &&
                            x.Spellbook.Spells.Any(v =>
                            v.Name.Contains(AbilityTexturName)));

                        Config.ParticleSpells.Spells(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(2));
                    });
            }

            // GetControlPoint 5
            if (Config.ParticleToTexture.ControlPoint_5.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_5.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        Config.ParticleSpells.Spells(
                            sender as Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(5));
                    });
            }

            // GetControlPoint 5 Fix
            if (Config.ParticleToTexture.ControlPoint_5Fix.Any(
                x => args.Name.Contains(x.Key)))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_5Fix.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsValid &&
                            !x.IsIllusion &&
                            x.Spellbook.Spells.Any(v =>
                            v.Name.Contains(AbilityTexturName)));

                        Config.ParticleSpells.Spells(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(5));
                    });
            }

            // GetControlPoint 1 Plus
            if (Config.ParticleToTexture.ControlPoint_1Plus.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ControlPoint_1Plus.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;
                        if (args.ParticleEffect.GetControlPoint(0).ToVector2()
                        == args.ParticleEffect.GetControlPoint(1).ToVector2())
                        {
                            return;
                        }

                        Config.ParticleSpells.Spells(
                            sender as Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(1));
                    },
                    1);
            }

            // Items
            if (Config.ParticleToTexture.Items.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.Items.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        if (AbilityTexturName.Contains("item_blink"))
                        {
                            Config.ParticleItems.Items(
                                CheckEnemyBlink,
                                args.Name,
                                AbilityTexturName,
                                args.ParticleEffect.GetControlPoint(0));
                        }

                        else if (AbilityTexturName.Contains("item_refresher"))
                        {
                            Config.ParticleItems.Items(
                                sender as Hero,
                                args.Name,
                                AbilityTexturName,
                                args.ParticleEffect.GetControlPoint(0));
                        }

                        else if (AbilityTexturName.Contains("item_bfury"))
                        {
                            if (args.ParticleEffect.GetControlPoint(0).ToVector2()
                            == args.ParticleEffect.GetControlPoint(1).ToVector2())
                            {
                                return;
                            }

                            Config.ParticleItems.Items(
                                sender as Hero,
                                args.Name,
                                AbilityTexturName,
                                args.ParticleEffect.GetControlPoint(1));
                        }
                        else 

                        if (AbilityTexturName.Contains("item_pipe")
                        || AbilityTexturName.Contains("item_hood_of_defiance")
                        || AbilityTexturName.Contains("item_crimson_guard"))
                        {
                            Config.ParticleItems.Items(
                                sender as Hero,
                                args.Name,
                                AbilityTexturName,
                                args.ParticleEffect.GetControlPoint(1)); 
                        }

                        
                    },
                    1);
            }

            // Items Semi Null CP0
            if (Config.ParticleToTexture.ItemsSemiNullCP0.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ItemsSemiNullCP0.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                            x =>
                            x.IsValid &&
                            !x.IsIllusion &&
                            x.Inventory.Items.Any(v => 
                            v.Name.Contains(AbilityTexturName) 
                            && v?.Cooldown / v?.CooldownLength * 100 >= 99));

                        Config.ParticleItems.ItemsNull(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(0));
                    },
                    1);
            }

            // Items Semi Null CP1
            if (Config.ParticleToTexture.ItemsSemiNullCP1.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ItemsSemiNullCP1.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var Hero = EntityManager<Hero>.Entities.FirstOrDefault(
                               x =>
                               x.IsValid &&
                               !x.IsIllusion &&
                               x.Inventory.Items.Any(v =>
                               v.Name.Contains(AbilityTexturName)
                               && v?.Cooldown / v?.CooldownLength * 100 >= 99));

                        Config.ParticleItems.ItemsNull(
                            Hero,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(1));
                    },
                    1);
            }

            // Item Smoke
            if (Config.ParticleToTexture.ItemsNullCP0.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ItemsNullCP0.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        var IgnorAllySmoke = EntityManager<Hero>.Entities.Any(
                            x =>
                            x.Team == Context.Owner.Team &&
                            x.HasModifier("modifier_smoke_of_deceit"));

                        if (!IgnorAllySmoke)
                        {
                            Config.ParticleItems.ItemsNull(
                                null,
                                args.Name,
                                AbilityTexturName,
                                args.ParticleEffect.GetControlPoint(0));
                        }
                    },
                    20);
            }

            if (Config.ParticleToTexture.ItemsNullCP1.Any(
                x => args.Name.Contains(x.Key)))
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        var AbilityTexturName = Config.
                        ParticleToTexture.ItemsNullCP1.FirstOrDefault(
                            x => args.Name.Contains(x.Key)).Value;

                        Config.ParticleItems.ItemsNull(
                            null,
                            args.Name,
                            AbilityTexturName,
                            args.ParticleEffect.GetControlPoint(1));
                    },
                    1);
            }

            //Town Portall Scrol Teleport Start 
            if (args.Name.Contains("/teleport_start.vpcf") || args.Name.Contains("/teleport_end.vpcf"))
            {
                DelayAction.Add(
                    -1,
                    () =>
                    {
                        var ParticleColor = args.ParticleEffect.GetControlPoint(2);

                        if (ParticleColor.IsZero || args.ParticleEffect.GetControlPoint(0).IsZero)
                        {
                            return;
                        }

                        var Hero = ObjectManager.GetPlayerById((uint)Config.
                            Colors.Vector3ToID.FindIndex(
                            x =>
                            x == ParticleColor)).Hero;

                        Config.ParticleTeleport.Teleport(
                            Hero,
                            args.ParticleEffect.GetControlPoint(0),
                            "item_tpscroll",
                            ParticleColor,
                            args.Name.Contains("/teleport_end.vpcf"));
                    });
            }
        }
    }
}