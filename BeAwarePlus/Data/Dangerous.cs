using System.Collections.Generic;

namespace BeAwarePlus.Data
{
    internal class Dangerous
    {
        public List<string> DangerousSpellList { get; } = new List<string>()
        {
            { "ancient_apparition_ice_blast" },
            { "mirana_invis" },
            { "sandking_epicenter" },
            { "furion_teleportation" },
            { "furion_wrath_of_nature" },
            { "alchemist_unstable_concoction" },
            { "bounty_hunter_wind_walk" },
            { "clinkz_wind_walk" },
            { "nyx_assassin_vendetta" },
            { "wisp_relocate" },
            { "morphling_replicate" },
            { "ursa_enrage" },
            { "abyssal_underlord_dark_rift" },
            { "mirana_arrow" },
            { "monkey_king_primal_spring" },
            { "spirit_breaker_charge_of_darkness" }
        };

        public List<string> DangerousItemList { get; } = new List<string>()
        {
            { "item_smoke_of_deceit" },
            { "item_glimmer_cape" },
            { "item_invis_sword" },
            { "item_shadow_amulet" },
            { "item_silver_edge" },
            { "item_gem" },
            { "item_rapier" },
            { "rune_haste" },
            { "rune_regen" },
            { "rune_arcane" },
            { "rune_doubledamage" },
            { "rune_invis" }
        };
    }
}
