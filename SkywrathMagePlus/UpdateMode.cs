using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;
using Ensage.SDK.Renderer.Particle;

using SharpDX;

namespace SkywrathMagePlus
{
    internal class UpdateMode
    {
        private Config Config { get; }

        private SkywrathMagePlus Main { get; }

        private IServiceContext Context { get; }

        public Hero WShow { get; set; }

        private Hero OffTarget { get; set; }

        private bool WRadius { get; set; }

        private int Count { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
            Main = config.SkywrathMagePlus;
            Context = config.SkywrathMagePlus.Context;

            UpdateManager.Subscribe(OnUpdate, 25);
            Config.WRadiusItem.PropertyChanged += CShotRadiusChanged;
        }

        public void Dispose()
        {
            Config.WRadiusItem.PropertyChanged -= CShotRadiusChanged;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void CShotRadiusChanged(object sender, PropertyChangedEventArgs e)
        {
            WRadius = true;
            Count = 0;
        }

        private void OnUpdate()
        {
            if (Config.ComboRadiusItem)
            {
                Context.Particle.DrawRange(
                    Context.Owner,
                    "ComboRadius",
                    Main.ArcaneBolt.CastRange,
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
                    Context.Owner,
                    "CShotRadius",
                    "materials/ensage_ui/particles/range_display_mod.vpcf",
                    ParticleAttachment.AbsOriginFollow,
                    RestartType.FullRestart,
                    1,
                    new Vector3(Config.WRadiusItem, 255, 0),
                    2,
                    new Vector3(0, 255, 255));
            }
            else
            {
                Context.Particle.Remove("CShotRadius");
            }

            WShow = EntityManager<Hero>.Entities.OrderBy(
                x => x.Distance2D(Context.Owner)).FirstOrDefault(
                x => !x.IsIllusion &&
                x.IsAlive &&
                x.IsVisible &&
                x.IsValid &&
                x.Team != Context.Owner.Team &&
                x.Distance2D(Context.Owner) <= Main.ConcussiveShot.Radius - 25);

            if (Config.WDrawItem
                && WShow != null
                && Main.ConcussiveShot
                && Main.ConcussiveShot.Ability.Cooldown <= 1)
            {
                Context.Particle.AddOrUpdate(
                    WShow,
                    "ConcussiveShot",
                    "particles/units/heroes/hero_skywrath_mage/skywrath_mage_concussive_shot.vpcf",
                    ParticleAttachment.AbsOrigin,
                    RestartType.None,
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

            if (Context.TargetSelector.IsActive 
                || OffTarget == null || !OffTarget.IsValid || !OffTarget.IsAlive)
            {
                OffTarget = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (OffTarget != null && !Config.SpamKeyItem)
            {
                Context.Particle.DrawTargetLine(
                    Context.Owner,
                    "Target",
                    OffTarget.Position,
                    Config.Mode.CanExecute ? Color.Red : Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (!Config.Mode.CanExecute && Config.Mode.Target != null)
            {
                if (!Context.TargetSelector.IsActive)
                {
                    Context.TargetSelector.Activate();
                }

                Config.Mode.Target = null;
            }
        }
    }
}
