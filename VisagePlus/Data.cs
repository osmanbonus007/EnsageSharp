using Ensage;
using Ensage.SDK.Extensions;

namespace VisagePlus
{
    internal class Data
    {
        public bool SmartStone(Unit target)
        {
            return target.HasModifier("modifier_teleporting");
        }

        public bool CancelCombo(Unit target)
        {
            return target.HasModifier("modifier_eul_cyclone")
                || target.HasModifier("modifier_brewmaster_storm_cyclone")
                || target.HasModifier("modifier_shadow_demon_disruption")
                || target.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                || target.HasModifier("modifier_tusk_snowball_movement")
                || target.HasModifier("modifier_invoker_tornado")
                || target.HasModifier("modifier_winter_wyvern_winters_curse");
        }
    }
}
