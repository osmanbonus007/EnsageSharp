using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;

namespace SkywrathMagePlus.Features
{
    internal class LinkenBreaker
    {
        private Config Config { get; }

        private SkywrathMagePlus Main { get; set; }

        private IOrderedEnumerable<KeyValuePair<string, uint>> BreakerChanger { get; set; }

        public LinkenBreaker(Config config)
        {
            Config = config;
            Main = config.SkywrathMagePlus;
        }

        public async Task Breaker(CancellationToken token, Hero Target)
        {
            if (Target.IsLinkensProtected())
            {
                BreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                z => Config.LinkenBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }
            else if (Config.Data.AntimageShield(Target))
            {
                BreakerChanger = Config.AntimageBreakerChanger.Value.Dictionary.Where(
                z => Config.AntimageBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }

            if (BreakerChanger == null)
            {
                return;
            }
            
            foreach (var Order in BreakerChanger.ToList())
            {
                // Eul
                if (Main.Eul != null
                    && Main.Eul.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Eul.CanBeCasted
                    && Main.Eul.CanHit(Target))
                {
                    Main.Eul.UseAbility(Target);
                    await Await.Delay(Main.Eul.GetCastDelay(Target), token);
                }

                // ForceStaff
                if (Main.ForceStaff != null
                    && Main.ForceStaff.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.ForceStaff.CanBeCasted
                    && Main.ForceStaff.CanHit(Target))
                {
                    Main.ForceStaff.UseAbility(Target);
                    await Await.Delay(Main.ForceStaff.GetCastDelay(Target), token);
                }

                // Orchid
                if (Main.Orchid != null
                    && Main.Orchid.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Orchid.CanBeCasted
                    && Main.Orchid.CanHit(Target))
                {
                    Main.Orchid.UseAbility(Target);
                    await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                }

                // Bloodthorn
                if (Main.Bloodthorn != null
                    && Main.Bloodthorn.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Bloodthorn.CanBeCasted
                    && Main.Bloodthorn.CanHit(Target))
                {
                    Main.Bloodthorn.UseAbility(Target);
                    await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                }

                // RodofAtos
                if (Main.RodofAtos != null
                    && Main.RodofAtos.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.RodofAtos.CanBeCasted
                    && Main.RodofAtos.CanHit(Target))
                {
                    Main.RodofAtos.UseAbility(Target);
                    await Await.Delay(Main.RodofAtos.GetCastDelay(Target), token);
                }

                // ArcaneBolt
                if (Main.ArcaneBolt != null
                    && Main.ArcaneBolt.Ability.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.ArcaneBolt.CanBeCasted
                    && Main.ArcaneBolt.CanHit(Target))
                {
                    Main.ArcaneBolt.UseAbility(Target);
                    await Await.Delay(Main.ArcaneBolt.GetCastDelay(Target), token);
                }

                // AncientSeal
                if (Main.AncientSeal != null
                    && Main.AncientSeal.Ability.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.AncientSeal.CanBeCasted
                    && Main.AncientSeal.CanHit(Target))
                {
                    Main.AncientSeal.UseAbility(Target);
                    await Await.Delay(Main.AncientSeal.GetCastDelay(Target), token);
                }

                // Hex
                if (Main.Hex != null
                    && Main.Hex.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Hex.CanBeCasted
                    && Main.Hex.CanHit(Target))
                {
                    Main.Hex.UseAbility(Target);
                    await Await.Delay(Main.Hex.GetCastDelay(Target), token);
                }
            }
        }
    }
}
