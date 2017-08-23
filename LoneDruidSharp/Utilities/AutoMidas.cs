using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System.Linq;

namespace LoneDruidSharpRewrite.Utilities
{
    public class AutoMidas
    {
        private Hero me
        {
            get
            {
                return Variable.Hero;
            }
        }

        private Unit bear
        {
            get
            {
                return Variable.Bear;
            }
        }

        private Item midas;

        private Unit midasTarget;

        public AutoMidas()
        {
        }

        private bool HasMidasOn(Unit unit)
        {
            return unit.Inventory.Items.Any(x => x.Name == "item_hand_of_midas" && x.CanBeCasted());
        }

        private void FindMidasOn(Unit unit)
        {
            if (HasMidasOn(unit))
            {
                this.midas = unit.FindItem("item_hand_of_midas");         
            }
        }

        public void getMidasCreeps(Unit src)
        {
            if (src == null) return;
            var midasTarget = ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                !x.IsMagicImmune() && x.Team != Variable.Bear.Team &&
                                (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral) && x.IsSpawned && x.IsAlive &&
                                 !x.IsMagicImmune() &&
                                 x.Distance2D(src) <= 600 + 100)
                                .OrderByDescending(x => x.Health).DefaultIfEmpty(null).FirstOrDefault();

            if(midasTarget == null)
            {
                this.midasTarget = null;
                return;
            }
            this.midasTarget = midasTarget;
        }

        public void Use(Item midas, Unit src, Unit target)
        {
            if (src == null) return;
            if (midas == null) return;
            var UseCond = !src.IsChanneling() && src.IsAlive;
            if (!UseCond) return;
            if (target == null) return;
            if (Utils.SleepCheck("midas"))
            {
                midas.UseAbility(target);
                Utils.Sleep(1000, "midas");
            }
        }

        public void Execute()
        {
            if (HasMidasOn(me))
            {
                FindMidasOn(me);
                getMidasCreeps(me);
                Use(this.midas, me, this.midasTarget);
            }
            if (HasMidasOn(bear))
            {
                FindMidasOn(bear);
                getMidasCreeps(bear);
                Use(this.midas, bear, this.midasTarget);
            }
        }
    }
}
