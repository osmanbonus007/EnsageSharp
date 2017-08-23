using System.Collections.Generic;

namespace BeAwarePlus.Data
{
    internal class EntityToTexture
    {
        public Dictionary<string, string> EntityTexture { get; } = new Dictionary<string, string>()
        {
            { "invoker", "invoker_sun_strike" },
            { "tinker", "tinker_march_of_the_machines" },
            { "kunkka", "kunkka_torrent" },
            { "tusk", "tusk_frozen_sigil" },
            { "monkey_king", "monkey_king_primal_spring" },
            { "riki", "riki_smoke_screen" },
            { "disruptor", "disruptor_kinetic_field" },
            { "enigma", "enigma_black_hole" },
            { "leshrac", "leshrac_split_earth" },
            { "lina", "lina_light_strike_array" },
            { "skywrath_mage", "skywrath_mage_mystic_flare" },
            { "night_stalker", "night_stalker_darkness" },
            { "gyrocopter", "gyrocopter_homing_missile" },
            { "juggernaut", "juggernaut_healing_ward" },
            { "slark", "slark_shadow_dance" },
            { "templar_assassin", "templar_assassin_psionic_trap" },
            { "pugna", "pugna_nether_ward" },
            { "shadow_shaman", "shadow_shaman_mass_serpent_ward" },
            { "storm_spirit", "storm_spirit_static_remnant" }
        };

        public Dictionary<uint, string> EntityVisionTexture { get; } = new Dictionary<uint, string>()
        {
            { 500, "mirana_arrow" },
            { 200, "tusk_ice_shards" },
            { 325, "skywrath_mage_arcane_bolt" },
            { 450, "phantom_assassin_stifling_dagger" }
        };
    }
}
