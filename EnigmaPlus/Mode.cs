using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.npc_dota_hero_enigma;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

using SharpDX;
using Ensage.SDK.Renderer.Particle;

namespace EnigmaPlus
{
    internal class EnigmaPlusCombo : KeyPressOrbwalkingModeAsync
    {
        private uint Elsecount { get; set; }

        private PredictionOutput Output { get; set; }

        private AbilityFactory AbilityFactory { get; }

        private Config Config { get; }

        private ITargetSelectorManager TargetSelector { get; }

        private IPredictionManager Prediction { get; }

        private Unit Target { get; set;}

        private Unit OffTarget { get; set; }

        private enigma_midnight_pulse MidnightPulse { get; set; }

        private enigma_black_hole BlackHole { get; set; }

        private Item Lens { get; set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; set; }

        [ItemBinding]
        public item_blink Blink { get; set; }

        [ItemBinding]
        public item_veil_of_discord Veil { get; set; }

        [ItemBinding]
        public item_black_king_bar BKB { get; set; }

        [ItemBinding]
        public item_shivas_guard Shivas { get; set; }

        [ItemBinding]
        public item_glimmer_cape GlimmerCape { get; set; }

        [ItemBinding]
        public item_refresher Refresher { get; set; }

        [ItemBinding]
        public item_soul_ring SoulRing { get; set; }

        [ItemBinding]
        public item_arcane_boots ArcaneBoots { get; set; }

        [ItemBinding]
        public item_guardian_greaves Guardian { get; set; }

        public EnigmaPlusCombo(
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
            if (TargetSelector.IsActive)
            {
                Target = TargetSelector.Active.GetTargets().FirstOrDefault();
            }
            
            if (Target != null)
            {
                if (Output.Unit != null && Output.AoeTargetsHit.Count >= (Config.AmountItem.Value == 1 ? 0 : Config.AmountItem.Value)
                    && (BlackHole == null || !BlackHole.Ability.IsChanneling))
                {
                    Elsecount = 0;

                    // Blink
                    if (Blink != null
                        && BlackHole != null
                        && BlackHole.IsReady
                        && Owner.Distance2D(Output.CastPosition) <= Blink.CastRange 
                        + (Lens != null ? Lens.GetCastRange() : 0)
                        && Config.ItemsToggler.Value.IsEnabled(Blink.Item.Name)
                        && Blink.CanBeCasted
                        && Owner.Distance2D(Output.CastPosition) >= 400)
                    {
                        Blink.UseAbility(Output.CastPosition);
                        await Await.Delay(Blink.GetCastDelay(Target.Position), token);
                    }

                    if (Blink == null || !Blink.CanBeCasted || !Config.ItemsToggler.Value.IsEnabled(Blink.Item.Name) || Owner.Distance2D(Output.CastPosition) <= 400)
                    {
                        UseItem(token);
                    }

                    // BlackHole
                    if (BlackHole != null
                        && Config.AbilityToggler.Value.IsEnabled(BlackHole.Ability.Name)
                        && BlackHole.CanBeCasted
                        && Elsecount == 6)
                    {
                        BlackHole.UseAbility(Output.CastPosition);
                        await Await.Delay(BlackHole.GetCastDelay(Output.CastPosition), token);
                    }
                    else
                    {
                        Elsecount += 1;
                    }

                    // Refresher
                    if (Refresher != null
                        && Config.ItemsToggler.Value.IsEnabled(Refresher.Item.Name)
                        && Refresher.CanBeCasted 
                        && BlackHole != null && !BlackHole.CanBeCasted && !BlackHole.Ability.IsChanneling
                        && Elsecount == 7)
                    {
                        Refresher.UseAbility();
                        await Await.Delay(Refresher.GetCastDelay(), token);
                    }
                }

                if (Target.IsAttackImmune() || Target.IsInvulnerable() && (BlackHole == null || !BlackHole.Ability.IsChanneling))
                {
                    Orbwalker.Move(Game.MousePosition);
                }
                else if (BlackHole == null || !BlackHole.Ability.IsChanneling)
                {
                    Orbwalker.OrbwalkTo(Target);
                }
            }
            else if (BlackHole == null || !BlackHole.Ability.IsChanneling)
            {
                Orbwalker.Move(Game.MousePosition);
            }
        }

        private async void UseItem(CancellationToken token)
        {
            
            // SoulRing
            if (SoulRing != null
                && Config.ItemsToggler.Value.IsEnabled(SoulRing.Item.Name)
                && SoulRing.CanBeCasted)
            {
                SoulRing.UseAbility();
                await Await.Delay(SoulRing.GetCastDelay(), token);
            }
            else
            {
                Elsecount += 1;
            }

            // MidnightPulse
            if (MidnightPulse != null
                && Config.AbilityToggler.Value.IsEnabled(MidnightPulse.Ability.Name)
                && MidnightPulse.CanBeCasted)
            {
                MidnightPulse.UseAbility(Output.CastPosition);
                await Await.Delay(MidnightPulse.GetCastDelay(Output.CastPosition), token);
            }
            else
            {
                Elsecount += 1;
            }

            // GlimmerCape
            if (GlimmerCape != null
                && Config.ItemsToggler.Value.IsEnabled(GlimmerCape.Item.Name)
                && GlimmerCape.CanBeCasted)
            {
                GlimmerCape.UseAbility(Owner);
                await Await.Delay(GlimmerCape.GetCastDelay(Owner), token);
            }
            else
            {
                Elsecount += 1;
            }

            // BKB
            if (BKB != null
                && Config.ItemsToggler.Value.IsEnabled(BKB.Item.Name)
                && BKB.CanBeCasted)
            {
                BKB.UseAbility();
                await Await.Delay(BKB.GetCastDelay(), token);
            }
            else
            {
                Elsecount += 1;
            }

            // Shivas
            if (Shivas != null
                && Config.ItemsToggler.Value.IsEnabled(Shivas.Item.Name)
                && Shivas.CanBeCasted)
            {
                Shivas.UseAbility();
                await Await.Delay(Shivas.GetCastDelay(), token);
            }
            else
            {
                Elsecount += 1;
            }

            // ArcaneBoots
            if (ArcaneBoots != null
                && Config.ItemsToggler.Value.IsEnabled(ArcaneBoots.Item.Name)
                && ArcaneBoots.CanBeCasted
                && Owner.Mana * 100 / Owner.MaximumMana <= 92
                && Elsecount == 5)
            {
                ArcaneBoots.UseAbility();
                await Await.Delay(ArcaneBoots.GetCastDelay(), token);
            }

            // Guardian
            if (Guardian != null
                && Config.ItemsToggler.Value.IsEnabled(Guardian.Item.Name)
                && Guardian.CanBeCasted
                && Owner.Mana * 100 / Owner.MaximumMana <= 92
                && Elsecount == 5)
            {
                Guardian.UseAbility();
                await Await.Delay(Guardian.GetCastDelay(), token);
            }

            // Veil
            if (Veil != null
                && Config.ItemsToggler.Value.IsEnabled(Veil.Item.Name)
                && Veil.CanBeCasted)
            {
                Veil.UseAbility(Output.CastPosition);
                await Await.Delay(Veil.GetCastDelay(Output.CastPosition), token);
            }
            else
            {
                Elsecount += 1;
            }
        }

        private void OnUpdate()
        {
            if (TargetSelector.IsActive)
            {
                OffTarget = TargetSelector.Active.GetTargets().FirstOrDefault();
                Context.Particle.Remove("Text");
            }
            else
            {
                Context.Particle.AddOrUpdate(
                    Owner,
                    "Text",
                    "materials/ensage_ui/particles/text.vpcf",
                    ParticleAttachment.AbsOrigin,
                    RestartType.None,
                    0,
                    Owner.Position - new Vector3(0, 200, 0),
                    1,
                    new Vector3(121, 8611111, 231651),
                    2,
                    new Vector3(111, 1111121, 115111),
                    3,
                    new Vector3(113, 1151114, 111111),
                    4,
                    new Vector3(111, 1111111, 111118),
                    5,
                    new Vector3(111, 1111511, 111111),
                    6,
                    new Vector3(111, 1111111, 111111),
                    7,
                    new Vector3(511, 1111111, 111111),
                    10,
                    new Vector3(50, 16, 0),
                    11,
                    new Vector3(255, 0, 0));
            }

            var Targets =
                EntityManager<Hero>.Entities.OrderBy(x => x == OffTarget).Where(
                    x => x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion).ToList();

            if (OffTarget != null)
            {
                var Input =
                    new PredictionInput(
                        Owner,
                        OffTarget,
                        1,
                        float.MaxValue,
                        2000,
                        420,
                        PredictionSkillshotType.SkillshotCircle,
                        true,
                        Targets)
                    {
                        CollisionTypes = CollisionTypes.None
                    };

                Output = Prediction.GetPrediction(Input);
            }
            
            if (OffTarget != null
                && OffTarget.Distance2D(Output.CastPosition) <= 1000 && Output.AoeTargetsHit.Count >= (Config.AmountItem.Value == 1 ? 0 : Config.AmountItem.Value))
            {
                Context.Particle.AddOrUpdate(
                    Owner,
                    "BlackHoleRadius",
                    "particles/ui_mouseactions/drag_selected_ring.vpcf",
                    ParticleAttachment.AbsOrigin,
                    RestartType.None,
                    0,
                    Output.CastPosition,
                    1,
                    Color.Aqua,
                    2,
                    420 * 1.1f);
            }
            else
            {
                Context.Particle.Remove("BlackHoleRadius");
            }

            Lens = Owner.GetItemById(AbilityId.item_aether_lens);

            if (Blink != null && Config.ComboRadiusItem.Value)
            {
                Context.Particle.DrawRange(
                    Owner, 
                    "ComboRadius",
                    Blink.CastRange + (Lens != null ? Lens.GetCastRange() : 0), 
                    Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("ComboRadius");
            }

            if (Target != null || OffTarget != null)
            {
                Context.Particle.DrawTargetLine(
                    Owner, 
                    "Target",
                    Target != null ? Target.Position : (Output != null && Output.AoeTargetsHit.Count 
                    >= (Config.AmountItem.Value == 1 ? 0 : Config.AmountItem.Value)) 
                    ? Output.CastPosition : OffTarget.Position, 
                    Target != null ? Color.Red : Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (!CanExecute && Target != null)
            {
                Target = null;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Context.Inventory.Attach(this);

            UpdateManager.Subscribe(OnUpdate, 25);

            MidnightPulse = AbilityFactory.GetAbility<enigma_midnight_pulse>();
            BlackHole = AbilityFactory.GetAbility<enigma_black_hole>();
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            UpdateManager.Unsubscribe(OnUpdate);

            base.OnDeactivate();
        }
    }
}
