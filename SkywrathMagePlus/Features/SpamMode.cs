using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;
using Ensage.SDK.Extensions;

using SharpDX;

using Config = SkywrathMagePlus.Config;
using Mode = SkywrathMagePlus.Mode;

namespace SkywrathMage
{
    internal class SpamMode
    {
        private Config Config { get; }

        private Mode Mode { get; }

        private IServiceContext Context { get; }

        private TaskHandler Handler { get; set; }

        private Unit Target { get; set; }

        public SpamMode(Config config, Mode mode)
        {
            Config = config;
            Mode = mode;
            Context = config.SkywrathMagePlus.Context;

            config.SpamKeyItem.PropertyChanged += SpamKeyChanged;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);
        }

        public void Dispose()
        {
            Config.SpamKeyItem.PropertyChanged -= SpamKeyChanged;
        }

        private void SpamKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.SpamKeyItem.Value)
            {
                Handler.RunAsync();
            }
            else
            {
                Handler?.Cancel();

                if (Target != null)
                {
                    if (!Context.TargetSelector.IsActive)
                    {
                        Context.TargetSelector.Activate();
                    }

                    Target = null;
                }

                Context.Particle.Remove("SpamTarget");
            }
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            if (Config.TargetItem.Value.SelectedValue.Contains("Lock")
                && (Target == null || !Target.IsValid || !Target.IsAlive))
            {
                if (!Context.TargetSelector.IsActive)
                {
                    Context.TargetSelector.Activate();
                }

                if (Context.TargetSelector.IsActive)
                {
                    Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault();
                }

                if (Target != null)
                {
                    if (Context.TargetSelector.IsActive)
                    {
                        Context.TargetSelector.Deactivate();
                    }
                }
            }
            else 
            
            if (Config.TargetItem.Value.SelectedValue.Contains("Default") 
                && Context.TargetSelector.IsActive)
            {
                Target = Context.TargetSelector.Active.GetTargets().FirstOrDefault();
            }

            if (Target != null)
            {
                if (!Target.IsMagicImmune())
                {
                    Context.Particle.DrawTargetLine(
                        Context.Owner,
                        "SpamTarget",
                        Target.Position,
                        Color.Green);

                    // ArcaneBolt
                    if (Mode.ArcaneBolt != null
                        && Config.AbilityToggler.Value.IsEnabled(Config.SkywrathMagePlus.Mode.ArcaneBolt.Ability.Name)
                        && Mode.ArcaneBolt.CanBeCasted)
                    {
                        Mode.ArcaneBolt.UseAbility(Target);
                        await Await.Delay(Mode.ArcaneBolt.GetCastDelay(), token);
                    }
                }
                if (Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    Context.Orbwalker.Move(Game.MousePosition);
                }
                else
                {
                    Context.Orbwalker.OrbwalkTo(Target);
                }
            }
            else
            {
                Context.Orbwalker.Move(Game.MousePosition);
                Context.Particle.Remove("SpamTarget");
            }
        }
    }
}
