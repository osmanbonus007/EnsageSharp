using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace VisagePlus.Features
{
    internal class FamiliarsLowHP
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

        public FamiliarsLowHP(Config config)
        {
            Config = config;
            Context = config.VisagePlus.Context;

            UpdateManager.Subscribe(LowHP, 100);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(LowHP);
        }

        private void LowHP()
        {
            var Familiars =
                EntityManager<Unit>.Entities.Where(
                    x =>
                    x.IsValid &&
                    x.IsAlive &&
                    x.IsControllable &&
                    x.Team == Context.Owner.Team &&
                    x.Name.Contains("npc_dota_visage_familiar")).ToArray();

            foreach (var Familiar in Familiars)
            {
                var FamiliarsStoneForm = Familiar.GetAbilityById(AbilityId.visage_summon_familiars_stone_form);

                if (Familiar.Health * 100 / Familiar.MaximumHealth <= Config.FamiliarsLowHPItem.Value 
                    && AbilityExtensions.CanBeCasted(FamiliarsStoneForm))
                {
                    FamiliarsStoneForm.UseAbility();
                }
            }
        }
    }
}