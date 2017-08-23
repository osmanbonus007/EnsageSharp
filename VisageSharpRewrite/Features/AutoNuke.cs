using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System.Linq;
using VisageSharpRewrite.Abilities;

namespace VisageSharpRewrite.Features
{
    public class AutoNuke
    {

        private SoulAssumption soulAssumption
        {
            get
            {
                return Variables.soulAssumption;
            }
        }

        public AutoNuke()
        {

        }

        public void Execute(Hero me)
        {
            var hasLens = me.HasItem(ClassId.CDOTA_Item_Aether_Lens);
            // Auto Kill steal
            var NearbyEnemy = ObjectManager.GetEntities<Hero>().Where(x => !x.IsMagicImmune() && x.IsAlive
                                                                           && !x.IsIllusion && x.Team != me.Team
                                                                           && x.Distance2D(me) <= (hasLens ? 1080 : 900) + 100);
            if (NearbyEnemy == null) return;
            var MinHpTargetNearbyEnemy = NearbyEnemy.OrderBy(x => x.Health).FirstOrDefault();
            if (MinHpTargetNearbyEnemy == null) return;
            
            var killables = NearbyEnemy.Where(x => x.Health <= soulAssumption.Damage(x, hasLens));
            
            if (killables.Count() == 0)
            {
                var SoulAssumpCharges = me.Modifiers.Where(x => x.Name == "modifier_visage_soul_assumption").FirstOrDefault();
                if (SoulAssumpCharges == null) return;
                if(soulAssumption.HasMaxCharges(me) && soulAssumption.CanbeCastedOn(MinHpTargetNearbyEnemy, hasLens))
                {
                    if (Utils.SleepCheck("soulassumption"))
                    {
                        soulAssumption.Use(MinHpTargetNearbyEnemy);
                        Utils.Sleep(200, "soulassumption");
                    }
                }
            }
            else
            {
                var killableTarget = killables.FirstOrDefault();               
                if (soulAssumption.CanbeCastedOn(killableTarget, hasLens))
                {
                    if (Utils.SleepCheck("soulassumption"))
                    {
                        soulAssumption.Use(killableTarget);
                        Utils.Sleep(100, "soulassumption");
                    }
                }
            }

        }

        public void KillSteal(Hero me)
        {
            var hasLens = me.HasItem(ClassId.CDOTA_Item_Aether_Lens);
            var NearbyEnemy = ObjectManager.GetEntities<Hero>().Where(x => !x.IsMagicImmune() && x.IsAlive
                                                                           && !x.IsIllusion && x.Team != me.Team
                                                                           && x.Distance2D(me) <= (hasLens ? 1080 : 900) + 100);
            if (NearbyEnemy == null) return;
            var MinHpTargetNearbyEnemy = NearbyEnemy.OrderBy(x => x.Health).FirstOrDefault();
            if (MinHpTargetNearbyEnemy == null) return;

            var killables = NearbyEnemy.Where(x => x.Health <= soulAssumption.Damage(x, hasLens));
            if (killables.Count() == 0) return;
            var killableTarget = killables.FirstOrDefault();
            if (soulAssumption.CanbeCastedOn(killableTarget, hasLens))
            {
                if (Utils.SleepCheck("soulassumption"))
                {
                    soulAssumption.Use(killableTarget);
                    Utils.Sleep(200, "soulassumption");
                }
            }
        }



    }
}
