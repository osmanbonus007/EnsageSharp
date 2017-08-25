using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_skywrath_mage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

using SharpDX;

namespace SkywrathMagePlus
{
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private bool WRadius { get; set; }

        private int Count { get; set; }

        private AbilityFactory AbilityFactory { get; }

        private Config Config { get; }

        private ITargetSelectorManager TargetSelector { get; }

        private IPredictionManager Prediction { get; }

        private Hero WShow { get; set; }

        public Unit Target { get; set;}

        private Unit OffTarget { get; set; }

        public skywrath_mage_arcane_bolt ArcaneBolt { get; set; }

        private skywrath_mage_concussive_shot ConcussiveShot { get; set; }

        public skywrath_mage_ancient_seal AncientSeal { get; set; }

        private skywrath_mage_mystic_flare MysticFlare { get; set; }

        public Dagon Dagon
        {
            get
            {
                return Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;
            }
        }

        [ItemBinding]
        public item_sheepstick Hex { get; set; }

        [ItemBinding]
        public item_orchid Orchid { get; set; }

        [ItemBinding]
        public item_bloodthorn Bloodthorn { get; set; }

        [ItemBinding]
        public item_rod_of_atos RodofAtos { get; set; }

        [ItemBinding]
        public item_veil_of_discord Veil { get; set; }

        [ItemBinding]
        public item_ethereal_blade Ethereal { get; set; }

        [ItemBinding]
        public item_dagon Dagon1 { get; set; }

        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }

        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }

        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }

        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; set; }

        [ItemBinding]
        public item_cyclone Eul { get; set; }

        [ItemBinding]
        public item_medallion_of_courage Medallion { get; set; }

        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
            AbilityFactory = context.AbilityFactory;

            TargetSelector = context.TargetSelector;
            Prediction = context.Prediction;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (Config.TargetItem.Value.SelectedValue.Contains("Lock") 
                && (Target == null || !Target.IsValid || !Target.IsAlive))
            {
                if (!TargetSelector.IsActive)
                {
                    TargetSelector.Activate();
                }

                if (TargetSelector.IsActive)
                {
                    Target = TargetSelector.Active.GetTargets().FirstOrDefault();
                }

                if (Target != null)
                {
                    if (TargetSelector.IsActive)
                    {
                        TargetSelector.Deactivate();
                    }
                }
            }
            else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && TargetSelector.IsActive)
            {
                Target = TargetSelector.Active.GetTargets().FirstOrDefault();
            }

            if (Target != null && !Target.HasModifier("modifier_item_blade_mail_reflect"))
            {
                if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !AntimageShield())
                {
                    // AncientSeal
                    if (AncientSeal != null
                        && Config.AbilityToggler.Value.IsEnabled(AncientSeal.Ability.Name)
                        && AncientSeal.CanBeCasted)
                    {
                        AncientSeal.UseAbility(Target);
                        await Await.Delay(AncientSeal.GetCastDelay(Target), token);
                    }

                    // ConcussiveShot
                    if (ConcussiveShot != null
                        && Config.AbilityToggler.Value.IsEnabled(ConcussiveShot.Ability.Name)
                        && (!Config.WTargetItem
                        || (Target == WShow
                        || (WShow != null && Target.Distance2D(WShow) <= 250)))
                        && ConcussiveShot.CanBeCasted
                        && Owner.Distance2D(Target.Position) <= Config.WRadiusItem.Value + 25)
                    {
                        ConcussiveShot.UseAbility();
                        await Await.Delay(ConcussiveShot.GetCastDelay(), token);
                    }

                    // ArcaneBolt
                    if (ArcaneBolt != null
                        && Config.AbilityToggler.Value.IsEnabled(ArcaneBolt.Ability.Name)
                        && ArcaneBolt.CanBeCasted)
                    {
                        ArcaneBolt.UseAbility(Target);
                        await Await.Delay(ArcaneBolt.GetCastDelay(Target), token);
                    }

                    // MysticFlare
                    if (MysticFlare != null
                        && Config.AbilityToggler.Value.IsEnabled(MysticFlare.Ability.Name)
                        && MysticFlare.CanBeCasted && ActiveMysticFlare(Target))
                    {
                        var CheckHero = EntityManager<Hero>.Entities.Where(
                            x =>
                            !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Owner.Team &&
                            x.Distance2D(Owner) <= MysticFlare.CastRange);

                        var UltimateScepter = Owner.GetItemById(AbilityId.item_ultimate_scepter) != null;
                        var DubleMysticFlare = UltimateScepter && CheckHero.Count() == 1;

                        var Input =
                            new PredictionInput(
                                Owner,
                                Target,
                                0,
                                float.MaxValue,
                                MysticFlare.CastRange,
                                DubleMysticFlare ? -250 : -100,
                                PredictionSkillshotType.SkillshotCircle,
                                true)
                            {
                                CollisionTypes = CollisionTypes.None
                            };

                        var Output = Prediction.GetPrediction(Input);

                        MysticFlare.UseAbility(Output.CastPosition);
                        await Await.Delay(MysticFlare.GetCastDelay(Target), token);
                    }

                    await Config.ComboItems.Items(token, this);
                }
                else
                {
                    await Config.LinkenBreaker.Breaker(token, this);
                }
                
                if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    Orbwalker.Move(Game.MousePosition);
                }
                else
                {
                    Orbwalker.OrbwalkTo(Target);
                }
            }
            else
            {
                Orbwalker.Move(Game.MousePosition);
            }
        }

        public bool AntimageShield()
        {
            var Shield = Target.GetAbilityById(AbilityId.antimage_spell_shield);

            return Shield != null 
                && Shield.Cooldown == 0
                && Shield.Level > 0
                && Target.GetItemById(AbilityId.item_ultimate_scepter) != null;
        }

        private bool ActiveMysticFlare(Unit Target)
        {
            return Target.MovementSpeed < 240
                || Target.IsStunned()
                || Target.HasModifier("modifier_rod_of_atos_debuff")
                && (Target.HasModifier("modifier_skywrath_mage_concussive_shot_slow")
                || Target.HasModifier("modifier_sheepstick_debuff"));
        }

        private void OnUpdate()
        {
            if (Config.ComboRadiusItem.Value)
            {
                Context.Particle.DrawRange(
                    Owner, 
                    "ComboRadius", 
                    ArcaneBolt.CastRange, 
                    Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("ComboRadius");
            }

            if (WRadius)
            {
                Count += 1;
                WRadius = Count != 50;

                Context.Particle.AddOrUpdate(
                    Owner,
                    "CShotRadius",
                    "materials/ensage_ui/particles/range_display_mod.vpcf",
                    ParticleAttachment.AbsOriginFollow,
                    true,
                    1,
                    new Vector3(Config.WRadiusItem.Value, 255, 0),
                    2,
                    new Vector3(0, 255, 255));
            }
            else
            {
                Context.Particle.Remove("CShotRadius");
            }

            WShow = EntityManager<Hero>.Entities.OrderBy(
                x => 
                x.Distance2D(Owner)).FirstOrDefault(
                x => 
                !x.IsIllusion &&   
                x.IsAlive &&     
                x.IsVisible &&    
                x.IsValid &&
                x.Team != Owner.Team &&
                x.Distance2D(Owner) <= ConcussiveShot.Radius - 25);

            if (Config.WDrawItem.Value 
                && WShow != null 
                && ConcussiveShot
                && ConcussiveShot.Ability.Cooldown <= 1)
            {
                Context.Particle.AddOrUpdate(
                    WShow, 
                    "ConcussiveShot",
                    "particles/units/heroes/hero_skywrath_mage/skywrath_mage_concussive_shot.vpcf", 
                    ParticleAttachment.AbsOrigin,
                    false,
                    0,
                    WShow.Position + new Vector3(0, 200, WShow.HealthBarOffset),
                    1,
                    WShow.Position + new Vector3(0, 200, WShow.HealthBarOffset),
                    2,
                    new Vector3(1000));
            }
            else
            {
                Context.Particle.Remove("ConcussiveShot");
            }

            if (TargetSelector.IsActive)
            {
                OffTarget = TargetSelector.Active.GetTargets().FirstOrDefault();
            }

            if ((Target != null || OffTarget != null) && !Config.SpamKeyItem.Value)
            {
                Context.Particle.DrawTargetLine(
                    Owner, 
                    "Target", 
                    Target != null ? Target.Position : OffTarget.Position, 
                    Target != null ? Color.Red : Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (!CanExecute && Target != null)
            {
                if (!TargetSelector.IsActive)
                {
                    TargetSelector.Activate();
                }

                Target = null;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Config.WRadiusItem.PropertyChanged += CShotRadiusChanged;

            Context.Inventory.Attach(this);

            UpdateManager.Subscribe(OnUpdate, 25);

            ArcaneBolt = AbilityFactory.GetAbility<skywrath_mage_arcane_bolt>();
            ConcussiveShot = AbilityFactory.GetAbility<skywrath_mage_concussive_shot>();
            AncientSeal = AbilityFactory.GetAbility<skywrath_mage_ancient_seal>();
            MysticFlare = AbilityFactory.GetAbility<skywrath_mage_mystic_flare>();
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            Config.WRadiusItem.PropertyChanged -= CShotRadiusChanged;

            base.OnDeactivate();
        }

        private void CShotRadiusChanged(object sender, PropertyChangedEventArgs e)
        {
            WRadius = true;
            Count = 0;
        }
    }
}
