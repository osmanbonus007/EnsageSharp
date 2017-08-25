using System.Collections.Generic;
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
        private Config Config { get; }

        private IOrderedEnumerable<KeyValuePair<string, uint>> BreakerChanger { get; set; }

        public LinkenBreaker(Config config)
        {
            Config = config;
        }

        public async Task Breaker(CancellationToken token, Mode Mode)
        {
            if (Mode.Target.IsLinkensProtected())
            {
                BreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                z => Config.LinkenBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }
            else if (Mode.AntimageShield())
            {
                BreakerChanger = Config.AntimageBreakerChanger.Value.Dictionary.Where(
                z => Config.AntimageBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }
            
            foreach (var Order in BreakerChanger.ToList())
            {
                // Medallion
                if (Mode.Medallion != null
                    && Mode.Medallion.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.Medallion.CanBeCasted)
                {
                    Mode.Medallion.UseAbility(Mode.Target);
                    await Await.Delay(Mode.Medallion.GetCastDelay(Mode.Target), token);
                }

                // Eul
                if (Mode.Eul != null
                    && Mode.Eul.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.Eul.CanBeCasted)
                {
                    Mode.Eul.UseAbility(Mode.Target);
                    await Await.Delay(Mode.Eul.GetCastDelay(Mode.Target), token);
                }

                // ForceStaff
                if (Mode.ForceStaff != null
                    && Mode.ForceStaff.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.ForceStaff.CanBeCasted)
                {
                    Mode.ForceStaff.UseAbility(Mode.Target);
                    await Await.Delay(Mode.ForceStaff.GetCastDelay(Mode.Target), token);
                }

                // Orchid
                if (Mode.Orchid != null
                    && Mode.Orchid.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.Orchid.CanBeCasted)
                {
                    Mode.Orchid.UseAbility(Mode.Target);
                    await Await.Delay(Mode.Orchid.GetCastDelay(Mode.Target), token);
                }

                // Bloodthorn
                if (Mode.Bloodthorn != null
                    && Mode.Bloodthorn.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.Bloodthorn.CanBeCasted)
                {
                    Mode.Bloodthorn.UseAbility(Mode.Target);
                    await Await.Delay(Mode.Bloodthorn.GetCastDelay(Mode.Target), token);
                }

                // RodofAtos
                if (Mode.RodofAtos != null
                    && Mode.RodofAtos.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.RodofAtos.CanBeCasted)
                {
                    Mode.RodofAtos.UseAbility(Mode.Target);
                    await Await.Delay(Mode.RodofAtos.GetCastDelay(Mode.Target), token);
                }

                // ArcaneBolt
                if (Mode.ArcaneBolt != null
                    && Mode.ArcaneBolt.Ability.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.ArcaneBolt.CanBeCasted)
                {
                    Mode.ArcaneBolt.UseAbility(Mode.Target);
                    await Await.Delay(Mode.ArcaneBolt.GetCastDelay(Mode.Target), token);
                }

                // Hex
                if (Mode.Hex != null
                    && Mode.Hex.Item.Name == Order.Key
                    && (Mode.Target.IsLinkensProtected() || Mode.AntimageShield())
                    && Mode.Hex.CanBeCasted)
                {
                    Mode.Hex.UseAbility(Mode.Target);
                    await Await.Delay(Mode.Hex.GetCastDelay(Mode.Target), token);
                }
            }
        }
    }
}
