using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage.Common.Threading;
using Ensage.SDK.Extensions;

using SkywrathMagePlus;

namespace SkywrathMage.Features
{
    internal class LinkenBreaker
    {
        private SkywrathMagePlusConfig Config { get; }

        public LinkenBreaker(SkywrathMagePlusConfig config)
        {
            Config = config;
        }

        public async Task Breaker(CancellationToken token, SkywrathMageCombo Combo)
        {
            var LinkenBreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                z => Config.LinkenBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);

            foreach (var Order in LinkenBreakerChanger.ToList())
            {
                // Medallion
                if (Combo.Medallion != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.Medallion.Item.Name == Order.Key
                    && Combo.Medallion.CanBeCasted)
                {
                    Combo.Medallion.UseAbility(Combo.Target);
                    await Await.Delay(Combo.Medallion.GetCastDelay(Combo.Target), token);
                }

                // Eul
                if (Combo.Eul != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.Eul.Item.Name == Order.Key
                    && Combo.Eul.CanBeCasted)
                {
                    Combo.Eul.UseAbility(Combo.Target);
                    await Await.Delay(Combo.Eul.GetCastDelay(Combo.Target), token);
                }

                // ForceStaff
                if (Combo.ForceStaff != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.ForceStaff.Item.Name == Order.Key
                    && Combo.ForceStaff.CanBeCasted)
                {
                    Combo.ForceStaff.UseAbility(Combo.Target);
                    await Await.Delay(Combo.ForceStaff.GetCastDelay(Combo.Target), token);
                }

                // Orchid
                if (Combo.Orchid != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.Orchid.Item.Name == Order.Key
                    && Combo.Orchid.CanBeCasted)
                {
                    Combo.Orchid.UseAbility(Combo.Target);
                    await Await.Delay(Combo.Orchid.GetCastDelay(Combo.Target), token);
                }

                // Bloodthorn
                if (Combo.Bloodthorn != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.Bloodthorn.Item.Name == Order.Key
                    && Combo.Bloodthorn.CanBeCasted)
                {
                    Combo.Bloodthorn.UseAbility(Combo.Target);
                    await Await.Delay(Combo.Bloodthorn.GetCastDelay(Combo.Target), token);
                }

                // RodofAtos
                if (Combo.RodofAtos != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.RodofAtos.Item.Name == Order.Key
                    && Combo.RodofAtos.CanBeCasted)
                {
                    Combo.RodofAtos.UseAbility(Combo.Target);
                    await Await.Delay(Combo.RodofAtos.GetCastDelay(Combo.Target), token);
                }

                // AncientSeal
                if (Combo.AncientSeal != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.AncientSeal.Ability.Name == Order.Key
                    && Combo.AncientSeal.CanBeCasted)
                {
                    Combo.AncientSeal.UseAbility(Combo.Target);
                    await Await.Delay(Combo.AncientSeal.GetCastDelay(Combo.Target), token);
                }

                // Hex
                if (Combo.Hex != null
                    && Combo.Target.IsLinkensProtected()
                    && Combo.Hex.Item.Name == Order.Key
                    && Combo.Hex.CanBeCasted)
                {
                    Combo.Hex.UseAbility(Combo.Target);
                    await Await.Delay(Combo.Hex.GetCastDelay(Combo.Target), token);
                }
            }
        }
    }
}
