using System.Linq;

using Ensage;
using Ensage.Common;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace VisagePlus.Features
{
    internal class FamiliarsCombo
    {
        private Config Config { get; }

        private VisagePlus Main { get; }

        private IServiceContext Context { get; }

        private Sleeper FamiliarsSleeper { get; }

        public FamiliarsCombo(Config config)
        {
            Config = config;
            Main = config.VisagePlus;
            Context = config.VisagePlus.Context;

            FamiliarsSleeper = new Sleeper();
        }

        public void Familiars(Hero Target)
        {
            var Familiars =
                EntityManager<Unit>.Entities.Where(
                    x =>
                    x.IsValid &&
                    x.IsAlive &&
                    x.IsControllable &&
                    x.Team == Context.Owner.Team &&
                    (x.Name.Contains("npc_dota_visage_familiar") ||
                    x.Name.Contains("npc_dota_necronomicon_warrior") ||
                    x.Name.Contains("npc_dota_necronomicon_archer"))).ToArray();

            foreach (var Familiar in Familiars)
            {
                if (Target != null)
                {
                    var StunDebuff = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                    var HexDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_sheepstick_debuff");
                    var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);
                    var ManaBurn = Familiar.GetAbilityById(AbilityId.necronomicon_archer_mana_burn);

                    // FamiliarsStoneForm
                    if (FamiliarsStoneForm != null &&
                        Config.AbilityToggler.Value.IsEnabled(FamiliarsStoneForm.Name)
                        && AbilityExtensions.CanBeCasted(FamiliarsStoneForm)
                        && Familiar.Distance2D(Target) <= 100
                        && (StunDebuff == null || StunDebuff.RemainingTime <= 0.5)
                        && (HexDebuff == null || HexDebuff.RemainingTime <= 0.5)
                        && !FamiliarsSleeper.Sleeping)
                    {
                        FamiliarsStoneForm.UseAbility();
                        FamiliarsSleeper.Sleep(FamiliarsStoneForm.GetAbilitySpecialData("stun_duration") * 1000 - 200);
                    }

                    // Necronomicon Mana Burn
                    if (ManaBurn != null
                        && AbilityExtensions.CanBeCasted(ManaBurn))
                    {
                        ManaBurn.UseAbility(Target);
                    }

                    if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                    {
                        if (Utils.SleepCheck($"IsAttack{Familiar.Handle}"))
                        {
                            Familiar.Move(Target.Position);
                            Utils.Sleep(200, $"IsAttack{Familiar.Handle}");
                        }
                    }
                    else 
                    if (Target != null 
                        && !AbilityExtensions.CanBeCasted(FamiliarsStoneForm)
                        || ((StunDebuff != null && StunDebuff.RemainingTime >= 0.5)
                        || (HexDebuff != null && HexDebuff.RemainingTime >= 0.5)))
                    {
                        Familiar.Attack(Target);
                    }
                    else if (Utils.SleepCheck($"Target{Familiar.Handle}"))
                    {
                        Familiar.Move(Target.Position);
                        Utils.Sleep(200, $"Target{Familiar.Handle}");
                    }
                }
                else if (Utils.SleepCheck($"Move{Familiar.Handle}"))
                {
                    Familiar.Move(Game.MousePosition);
                    Utils.Sleep(200, $"Move{Familiar.Handle}");
                }
            }
        }
    }
}
