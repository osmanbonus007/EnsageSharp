using System.Threading;
using System.Threading.Tasks;

using Ensage.Common.Threading;
using Ensage.SDK.Extensions;

using SkywrathMagePlus;

namespace SkywrathMage.Features
{
    internal class ComboItems
    {
        private SkywrathMagePlusConfig Config { get; }


        public ComboItems(SkywrathMagePlusConfig config)
        {
            Config = config;
        }

        public async Task Items(CancellationToken token, SkywrathMageCombo Combo)
        {
            // Hex
            if (Combo.Hex != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.Hex.Item.Name)
                && Combo.Hex.CanBeCasted)
            {
                Combo.Hex.UseAbility(Combo.Target);
                await Await.Delay(Combo.Hex.GetCastDelay(Combo.Target), token);
            }

            // Orchid
            if (Combo.Orchid != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.Orchid.Item.Name)
                && Combo.Orchid.CanBeCasted)
            {
                Combo.Orchid.UseAbility(Combo.Target);
                await Await.Delay(Combo.Orchid.GetCastDelay(Combo.Target), token);
            }

            // Bloodthorn
            if (Combo.Bloodthorn != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.Bloodthorn.Item.Name)
                && Combo.Bloodthorn.CanBeCasted)
            {
                Combo.Bloodthorn.UseAbility(Combo.Target);
                await Await.Delay(Combo.Bloodthorn.GetCastDelay(Combo.Target), token);
            }

            // RodofAtos
            if (Combo.RodofAtos != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.RodofAtos.Item.Name)
                && Combo.RodofAtos.CanBeCasted)
            {
                Combo.RodofAtos.UseAbility(Combo.Target);
                await Await.Delay(Combo.RodofAtos.GetCastDelay(Combo.Target), token);
            }

            // Veil
            if (Combo.Veil != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.Veil.Item.Name)
                && Combo.Veil.CanBeCasted)
            {
                Combo.Veil.UseAbility(Combo.Target.Position);
                await Await.Delay(Combo.Veil.GetCastDelay(Combo.Target), token);
            }

            // Ethereal
            if (Combo.Ethereal != null
                && Config.ItemsToggler.Value.IsEnabled(Combo.Ethereal.Item.Name)
                && Combo.Ethereal.CanBeCasted)
            {
                Combo.Ethereal.UseAbility(Combo.Target);
                await Await.Delay(Combo.Ethereal.GetCastDelay(Combo.Target), token);
            }

            // Dagon
            if (Combo.Dagon != null
                && Config.ItemsToggler.Value.IsEnabled("item_dagon_5")
                && Combo.Dagon.CanBeCasted
                && (Combo.AncientSeal == null || (Combo.Target.HasModifier("modifier_skywrath_mage_ancient_seal") && !Combo.AncientSeal.CanBeCasted)
                || !Config.AbilityToggler.Value.IsEnabled(Combo.AncientSeal.Ability.Name))
                && (Combo.Ethereal == null || (Combo.Target.IsEthereal() && !Combo.Ethereal.CanBeCasted)
                || !Config.ItemsToggler.Value.IsEnabled(Combo.Ethereal.Item.Name)))
            {
                Combo.Dagon.UseAbility(Combo.Target);
                await Await.Delay(Combo.Dagon.GetCastDelay(Combo.Target), token);
            }
        }
    }
}
