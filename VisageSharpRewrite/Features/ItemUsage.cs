using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using Ensage.Items;
using System.Collections.Generic;
using System.Linq;

namespace VisageSharpRewrite.Features
{
    public class ItemUsage
    {
        private List<Item> items;

        private bool hasLens;

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        public ItemUsage()
        {
            this.UpdateItems();
        }

        public void UpdateItems()
        {
            if (Variables.Hero == null || !Variables.Hero.IsValid)
            {
                return;
            }
            this.items = Variables.Hero.Inventory.Items.ToList();
            this.hasLens = me.HasItem(ClassId.CDOTA_Item_Aether_Lens);
            var powerTreads = this.items.FirstOrDefault(x => x.StoredName() == "item_power_treads");
            if (powerTreads != null)
            {
                Variables.PowerTreadsSwitcher = new PowerTreadsSwitcher(powerTreads as PowerTreads);
            }
        }

        public void Medalion(Hero target)
        {
            Item Medalion = me.FindItem("item_medallion_of_courage");
            if (Medalion == null) return;
            bool MedalionCond = !target.IsMagicImmune() && target.Distance2D(me) <= Medalion.CastRange + (hasLens? 200 : 0) + 100 && Medalion.CanBeCasted();
            if (!MedalionCond) return;
            if (Utils.SleepCheck("MedalionCond"))
            {
                Medalion.UseAbility(target);
                Utils.Sleep(100, "MedalionCond");
            }
        }

        public void SolarCrest(Hero target)
        {
            Item SolarCrest = me.FindItem("item_solar_crest");
            if (SolarCrest == null) return;
            bool SolarCrestCond = !target.IsMagicImmune() && target.Distance2D(me) <= SolarCrest.CastRange + (hasLens ? 200 : 0) + 100 && SolarCrest.CanBeCasted();
            if (!SolarCrestCond) return;
            if (Utils.SleepCheck("SolarCrest"))
            {
                SolarCrest.UseAbility(target);
                Utils.Sleep(100, "SolarCrest");
            }
        }

        public void RodOfAtos(Hero target)
        {
            Item RodOfAtos = me.FindItem("item_rod_of_atos");
            if (RodOfAtos == null) return;
            bool RodOfAtosCond = !target.IsMagicImmune() && target.Distance2D(me) <= RodOfAtos.CastRange + (hasLens ? 200 : 0) + 100 && RodOfAtos.CanBeCasted();
            if (!RodOfAtosCond) return;
            if (Utils.SleepCheck("RodOfAtos"))
            {
                RodOfAtos.UseAbility(target);
                Utils.Sleep(100, "RodOfAtos");
            }
        }

        public void UseVeil(Hero target)
        {
            Item Veil = me.FindItem("item_veil_of_discord");
            if (Veil == null) return;
            bool VeilCond = target.Distance2D(me) <= Veil.CastRange + 100 && Veil.CanBeCasted();
            if (!VeilCond) return;
            if (Utils.SleepCheck("Veil"))
            {
                Veil.UseAbility(target.Position);
                Utils.Sleep(100, "Veil");
            }
        }

        public void UseOrchid(Hero target)
        {
            Item Orchid = me.FindItem("item_orchid");
            if (Orchid == null) return;
            bool OrchidCond = !target.IsMagicImmune() && target.Distance2D(me) <= Orchid.CastRange + (hasLens ? 200 : 0) + 100 && Orchid.CanBeCasted();
            if (!OrchidCond) return;
            if (Utils.SleepCheck("Orchid"))
            {
                Orchid.UseAbility(target);
                Utils.Sleep(100, "Orchid");
            }
        }

        public void UseBloodthorn(Hero target)
        {
            Item Bloodthorn = me.FindItem("item_bloodthorn");
            if (Bloodthorn == null) return;
            bool BloodthornCond = !target.IsMagicImmune() && target.Distance2D(me) <= Bloodthorn.CastRange + (hasLens ? 200 : 0) + 100 && Bloodthorn.CanBeCasted();
            if (!BloodthornCond) return;
            if (Utils.SleepCheck("Bloodthorn"))
            {
                Bloodthorn.UseAbility(target);
                Utils.Sleep(100, "Bloodthorn");
            }
        }

        public void UseSheepstick(Hero target)
        {
            Item Sheepstick = me.FindItem("item_sheepstick");
            if (Sheepstick == null) return;
            bool SheepstickCond = !target.IsMagicImmune() && target.Distance2D(me) <= Sheepstick.CastRange + (hasLens ? 200 : 0) + 100 && Sheepstick.CanBeCasted();
            if (!SheepstickCond) return;
            if (Utils.SleepCheck("Bloodthorn"))
            {
                Sheepstick.UseAbility(target);
                Utils.Sleep(100, "Bloodthorn");
            }
        }



        public void OffensiveItem(Hero target)
        {
            UseSheepstick(target);
            UseBloodthorn(target);
            UseOrchid(target);
            UseVeil(target);
            Medalion(target);
            SolarCrest(target);
            RodOfAtos(target);
        }



    }
}
