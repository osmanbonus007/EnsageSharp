using System.Collections.Generic;

namespace BeAwarePlus.Data
{
    internal class ParticleToTexture
    {
        public Dictionary<string, string> ControlPoint_0 { get; } = new Dictionary<string, string>()
        {
            { "alchemist_acid_spray_cast", "alchemist_acid_spray" },
            { "alchemist_chemichalrage_effect", "alchemist_chemical_rage" },
            { "alchemist_unstableconc_bottles", "alchemist_unstable_concoction" },
            { "axe_culling_blade_boost", "axe_culling_blade" },
            { "centaur_double_edge", "centaur_double_edge" },
            { "doom_bringer_devour", "doom_bringer_devour" },
            { "espirit_rollingboulder", "earth_spirit_rolling_boulder" },
            { "earthshaker_totem_cast", "earthshaker_enchant_totem" },
            { "wisp_relocate_channel", "wisp_relocate" },
            { "legion_commander_press_hero", "legion_commander_press_the_attack" },
            { "lycan_summon_wolves_cast", "lycan_summon_wolves" },
            { "omniknight_purification_cast", "omniknight_purification" },
            { "omniknight_repel_cast", "omniknight_repel" },
            { "sven_spell_storm_bolt_lightning", "sven_storm_bolt" },
            { "sven_spell_gods_strength_ambient", "sven_gods_strength" },
            { "tidehunter_anchor_hero", "tidehunter_anchor_smash" },
            { "shredder_whirling_death", "shredder_whirling_death" },
            { "shredder_reactive_hit", "shredder_reactive_armor" },
            { "tiny_craggy_hit", "tiny_craggy_exterior" },
            { "treant_leech_seed.vpcf", "treant_leech_seed" },
            { "treant_overgrowth_cast", "treant_overgrowth" },
            { "tusk_walruspunch_start", "tusk_walrus_punch" },
            { "underlord_pit_cast", "abyssal_underlord_pit_of_malice" },
            { "abbysal_underlord_darkrift_ambient", "abyssal_underlord_dark_rift" },
            { "skeletonking_hellfireblast_warmup", "skeleton_king_hellfire_blast" },
            { "antimage_blade_hit", "antimage_mana_break" },
            { "arc_warden_magnetic_cast", "arc_warden_magnetic_field" },
            { "arc_warden_tempest_cast", "arc_warden_tempest_double" },
            { "arc_warden_flux_cast", "arc_warden_flux" },
            { "bounty_hunter_hand_r", "bounty_hunter_jinada" },
            { "broodmother_spin_web_cast", "broodmother_spin_web" },
            { "broodmother_hunger_buff", "broodmother_insatiable_hunger" },
            { "clinkz_windwalk", "clinkz_wind_walk" },
            { "drow_marksmanship_start", "drow_ranger_marksmanship" },
            { "ember_spirit_flameguard", "ember_spirit_flame_guard" },
            { "juggernaut_crit_tgt", "juggernaut_blade_dance" },
            { "juggernaut_omni_slash_tgt", "juggernaut_omni_slash" },
            { "lone_druid_bear_spawn", "lone_druid_spirit_bear" },
            { "lone_druid_true_form", "lone_druid_true_form" },
            { "true_form_lone_druid", "lone_druid_true_form_druid" },
            { "medusa_mana_shield_cast", "medusa_mana_shield" },
            { "medusa_mana_shield_end", "medusa_mana_shield" },
            { "monkey_king_strike_cast", "monkey_king_boundless_strike" },
            { "monkey_king_disguise", "monkey_king_mischief" },
            { "monkey_king_fur_army_cast", "monkey_king_wukongs_command" },
            { "monkey_king_quad_tap_hit", "monkey_king_jingu_mastery" },
            { "phantom_assassin_phantom_strike_end", "phantom_assassin_phantom_strike" },
            { "phantomlancer_spiritlance_caster", "phantom_lancer_spirit_lance" },
            { "phantom_lancer_doppleganger_aoe", "phantom_lancer_doppelwalk" },
            { "nevermore_necro_souls", "nevermore_necromastery" },
            { "mirana_moonlight_cast", "mirana_invis" },
            { "sandking_epicenter_tell", "sandking_epicenter" },
            { "slark_pounce_trail", "slark_pounce" },
            { "templar_assassin_refraction", "templar_assassin_refraction" },
            { "terrorblade_reflection_cast", "terrorblade_reflection" },
            { "ursa_overpower_buff", "ursa_overpower" },
            { "venomancer_venomous_gale_mouth", "venomancer_venomous_gale" },
            { "venomancer_ward_cast", "venomancer_plague_ward" },
            { "bane_sap", "bane_brain_sap" },
            { "batrider_flaming_lasso", "batrider_flaming_lasso" },
            { "chen_cast_1", "chen_penitence" },
            { "chen_cast_2", "chen_test_of_faith" },
            { "chen_cast_3", "chen_holy_persuasion" },
            { "death_prophet_silence_cast", "death_prophet_silence" },
            { "death_prophet_spiritsiphon", "death_prophet_spirit_siphon" },
            { "quas_orb", "invoker_wex" },
            { "wex_orb", "invoker_quas" },
            { "exort_orb", "invoker_exort" },
            { "invoker_cold_snap.vpcf", "invoker_cold_snap" },
            { "keeper_of_the_light_illuminate_charge.vpcf", "keeper_of_the_light_illuminate" },
            { "keeper_of_the_light_recall_cast", "keeper_of_the_light_recall" },
            { "lion_spell_impale_staff", "lion_impale" },
            { "lion_spell_voodoo_ambient", "lion_voodoo" },
            { "furion_force_of_nature_cast", "furion_force_of_nature" },
            { "furion_teleport.vpcf", "furion_teleportation" },
            { "furion_wrath_of_nature_cast", "furion_wrath_of_nature" },
            { "ogre_magi_fireblast_cast", "ogre_magi_fireblast" },
            { "ogre_magi_ignite_cast", "ogre_magi_ignite" },
            { "ogre_magi_bloodlust_cast", "ogre_magi_bloodlust" },
            { "oracle_fortune_cast.vpcf", "oracle_fortunes_end" },
            { "oracle_fatesedict_cast", "oracle_fates_edict" },
            { "queen_shadow_strike_body", "queenofpain_shadow_strike" },
            { "silencer_curse_cast", "silencer_curse_of_the_silent" },
            { "silencer_last_word_status_cast", "silencer_last_word" },
            { "silencer_global_silence.vpcf", "silencer_global_silence" },
            { "skywrath_mage_concussive_shot_cast", "skywrath_mage_concussive_shot" },
            { "stormspirit_ball_lightning", "storm_spirit_ball_lightning" },
            { "techies_remote_mine_plant", "techies_remote_mines" },
            { "tinker_missile_dud.vpcf", "tinker_heat_seeking_missile" },
            { "tinker_rearm", "tinker_rearm" },
            { "visage_grave_chill_caster", "visage_grave_chill" },
            { "warlock_rain_of_chaos_staff", "warlock_rain_of_chaos" },
            { "windrunner_spell_powershot_channel", "windrunner_powershot" },
            { "wyvern_arctic_burn_start", "winter_wyvern_arctic_burn" },
            { "wyvern_winters_curse_start", "winter_wyvern_winters_curse" },
            { "wyvern_cold_embrace_start", "winter_wyvern_cold_embrace" },
            { "zuus_lightning_bolt_start", "zuus_lightning_bolt" }
        };

        public Dictionary<string, string> ControlPoint_0Fix { get; } = new Dictionary<string, string>()
        {
            { "beastmaster_wildaxe.vpcf", "beastmaster_wild_axes" },
            { "beastmaster_call_bird", "beastmaster_call_of_the_wild" },
            { "beastmaster_call_boar", "beastmaster_call_of_the_wild_boar" },
            { "beastmaster_primal_roar.vpcf", "beastmaster_primal_roar" },
            { "/brewmaster_thunder_clap.vpcf", "brewmaster_thunder_clap" },
            { "brewmaster_earth_ambient_noportrait", "brewmaster_primal_split" },
            { "bristleback_quill_spray.vpcf", "bristleback_quill_spray" },
            { "bristleback_viscous_nasal_goo_debuff", "bristleback_viscous_nasal_goo" },
            { "chaos_knight_bolt_msg", "chaos_knight_chaos_bolt" },
            { "chaos_knight_reality_rift", "chaos_knight_reality_rift" },
            { "rattletrap_rocket_flare", "rattletrap_rocket_flare" },
            { "rattletrap_cog_ambient", "rattletrap_power_cogs" },
            { "rattletrap_hookshot", "rattletrap_hookshot" },
            { "doom_bringer_scorched_earth_debuff", "doom_bringer_scorched_earth" },
            { "doom_infernal_blade_impact", "doom_bringer_infernal_blade" },
            { "dragon_knight_dragon_tail_knightform", "dragon_knight_dragon_tail" },
            { "dragon_knight_dragon_tail_dragonform", "dragon_knight_dragon_tail" },
            { "espirit_stoneremnant", "earth_spirit_petrify" },
            { "espirit_geomagentic_grip_caster", "earth_spirit_geomagnetic_grip" },
            { "espirit_bouldersmash_caster", "earth_spirit_boulder_smash" },
            { "espirit_stone_explosion", "earth_spirit_magnetize" },
            { "elder_titan_earth_splitter", "elder_titan_earth_splitter" },
            { "huskar_life_break_cast", "huskar_life_break" },
            { "wisp_guardian_explosion", "wisp_spirits" },
            { "wisp_guardian_explosion_small", "wisp_spirits" },
            { "wisp_tether", "wisp_tether" },
            { "wisp_overcharge", "wisp_overcharge" },
            { "wisp_relocate_marker_endpoint", "wisp_relocate" },
            { "kunkka_spell_x_spot", "kunkka_x_marks_the_spot" },
            { "life_stealer_infest_emerge_bloody", "life_stealer_infest" },
            { "magnataur_reverse_polarity", "magnataur_reverse_polarity" },
            { "nightstalker_void_hit", "night_stalker_void" },
            { "phoenix_icarus_dive", "phoenix_icarus_dive" },
            { "phoenix_fire_spirit_launch", "phoenix_fire_spirits" },
            { "phoenix_sunray", "phoenix_sun_ray" },
            { "pudge_meathook", "pudge_meat_hook" },
            { "sandking_caustic_finale_debuff", "sandking_caustic_finale" },
            { "sandking_caustic_finale_explode", "sandking_caustic_finale" },
            { "spirit_breaker_greater_bash", "spirit_breaker_greater_bash" },
            { "tidehunter_spell_ravage.vpcf", "tidehunter_ravage" },
            { "shredder_timberchain", "shredder_timber_chain" },
            { "shredder_chakram_stay", "shredder_chakram" },
            { "tiny_avalanche", "tiny_avalanche" },
            { "tiny_toss_blur", "tiny_toss" },
            { "treant_eyesintheforest", "treant_eyes_in_the_forest" },
            { "treant_livingarmor", "treant_living_armor" },
            { "tusk_walruskick_tgt_status", "tusk_walrus_kick" },
            { "tusk_snowball_start", "tusk_snowball" },
            { "abyssal_underlord_firestorm_wave", "abyssal_underlord_firestorm" },
            { "abyssal_underlord_darkrift_target", "abyssal_underlord_dark_rift" },
            { "undying_tombstone", "undying_tombstone" },
            { "wraith_king_reincarnate", "skeleton_king_reincarnation" },
            { "antimage_blink_start", "antimage_blink" },
            { "antimage_manavoid", "antimage_mana_void" },
            { "arc_warden_wraith", "arc_warden_spark_wraith" },
            { "bloodseeker_bloodritual_ring", "bloodseeker_blood_bath" },
            { "bounty_hunter_windwalk", "bounty_hunter_wind_walk" },
            { "broodmother_spiderlings_spawn", "broodmother_spawn_spiderlings" },
            { "clinkz_death_pact", "clinkz_death_pact" },
            { "drow_hero_silence", "drow_ranger_silence" },
            { "ember_spirit_sleight_of_fist_cast", "ember_spirit_sleight_of_fist" },
            { "ember_spirit_remnant_dash", "ember_spirit_activate_fire_remnant" },
            { "faceless_void_chronosphere", "faceless_void_chronosphere" },
            { "gyro_calldown_first", "gyrocopter_call_down" },
            { "lone_druid_rabid_buff", "lone_druid_rabid" },
            { "lone_druid_battle_cry_buff", "lone_druid_true_form_battle_cry" },
            { "luna_lucent_beam_precast", "luna_lucent_beam" },
            { "luna_eclipse_precast", "luna_eclipse" },
            { "nyx_assassin_vendetta_start", "nyx_assassin_vendetta" },
            { "medusa_mystic_snake_cast", "medusa_mystic_snake" },
            { "meepo_earthbind_projectile_fx", "meepo_earthbind" },
            { "meepo_poof_start", "meepo_poof" },
            { "meepo_poof_end", "meepo_poof" },
            { "mirana_starfall_attack", "mirana_starfall" },
            { "morphling_waveform_dmg", "morphling_waveform" },
            { "morphling_replicate", "morphling_replicate" },
            { "phantom_assassin_crit_impact", "phantom_assassin_coup_de_grace" },
            { "razor_plasmafield", "razor_plasma_field" },
            { "riki_backstab", "riki_permanent_invisibility" },
            { "slark_dark_pact_pulses", "slark_dark_pact" },
            { "sniper_shrapnel_launch", "sniper_shrapnel" },
            { "sniper_headshot_slow", "sniper_headshot" },
            { "templar_assassin_psi_blade", "templar_assassin_psi_blades" },
            { "troll_warlord_whirling_axe_melee", "troll_warlord_whirling_axes_melee" },
            { "ursa_fury_swipes_debuff", "ursa_fury_swipes" },
            { "venomancer_poison_nova", "venomancer_poison_nova" },
            { "viper_poison_debuff", "viper_poison_attack" },
            { "weaver_shukuchi_start", "weaver_shukuchi" },
            { "weaver_shukuchi_damage", "weaver_shukuchi" },
            { "weaver_timelapse", "weaver_time_lapse" },
            { "ancient_ice_vortex", "ancient_apparition_ice_vortex" },
            { "ancient_apparition_chilling_touch", "ancient_apparition_chilling_touch" },
            { "ancient_apparition_ice_blast_final", "ancient_apparition_ice_blast" },
            { "ancient_apparition_cold_feet_marker", "ancient_apparition_cold_feet" },
            { "batrider_stickynapalm_impact", "batrider_sticky_napalm" },
            { "batrider_firefly", "batrider_firefly" },
            { "chen_cast_4", "chen_hand_of_god" },
            { "maiden_crystal_nova", "crystal_maiden_crystal_nova" },
            { "maiden_freezing_field_snow", "crystal_maiden_freezing_field" },
            { "maiden_frostbite_buff", "crystal_maiden_frostbite" },
            { "dark_seer_vacuum", "dark_seer_vacuum" },
            { "dark_seer_wall_of_replica", "dark_seer_wall_of_replica" },
            { "dark_seer_ion_shell_damage", "dark_seer_ion_shell" },
            { "dazzle_shadow_wave", "dazzle_shadow_wave" },
            { "dazzle_weave", "dazzle_weave" },
            { "death_prophet_carrion_swarm", "death_prophet_carrion_swarm" },
            { "death_prophet_spirit_glow", "death_prophet_exorcism" },
            { "disruptor_thunder_strike_buff", "disruptor_thunder_strike" },
            { "furion_sprout", "furion_sprout" },
            { "enchantress_natures_attendants_heal", "enchantress_natures_attendants" },
            { "enigma_demonic_conversion", "enigma_demonic_conversion" },
            { "enigma_midnight_pulse", "enigma_midnight_pulse" },
            { "invoker_chaos_meteor_fly", "invoker_chaos_meteor" },
            { "invoker_emp", "invoker_emp" },
            { "invoker_ice_wall.vpcf", "invoker_ice_wall" },
            { "jakiro_dual_breath_ice", "jakiro_dual_breath" },
            { "jakiro_ice_path.vpcf", "jakiro_ice_path" },
            { "jakiro_liquid_fire_explosion", "jakiro_liquid_fire" },
            { "keeper_of_the_light_illuminate_charge_spirit_form", "keeper_of_the_light_illuminate" },
            { "keeper_of_the_light_blinding_light_aoe", "keeper_of_the_light_blinding_light" },
            { "keeper_of_the_light_recall_poof", "keeper_of_the_light_recall" },
            { "keeper_chakra_magic", "keeper_of_the_light_chakra_magic" },
            { "keeper_mana_leak_cast", "keeper_of_the_light_mana_leak" },
            { "leshrac_pulse_nova.vpcf", "leshrac_pulse_nova" },
            { "leshrac_lightning_bolt", "leshrac_lightning_storm" },
            { "leshrac_diabolic_edict", "leshrac_diabolic_edict" },
            { "lich_dark_ritual", "lich_dark_ritual" },
            { "lich_slowed_cold", "lich_chain_frost" },
            { "lina_spell_laguna_blade", "lina_laguna_blade" },
            { "lion_spell_finger_of_death", "lion_finger_of_death" },
            { "obsidian_destroyer_sanity_eclipse_area", "obsidian_destroyer_sanity_eclipse" },
            { "obsidian_destroyer_prison_start", "obsidian_destroyer_astral_imprisonment" },
            { "puck_dreamcoil.vpcf", "puck_dream_coil" },
            { "puck_illusory_orb_blink_out", "puck_illusory_orb" },
            { "pugna_life_drain", "pugna_life_drain" },
            { "pugna_life_give", "pugna_life_drain" },
            { "pugna_netherblast.vpcf", "pugna_nether_blast" },
            { "queen_blink_start", "queenofpain_blink" },
            { "rubick_fade_bolt_head", "rubick_fade_bolt" },
            { "rubick_telekinesis.vpcf", "rubick_telekinesis" },
            { "shadow_demon_demonic_purge_cast", "shadow_demon_demonic_purge" },
            { "shadowshaman_shackle", "shadow_shaman_shackles" },
            { "shadowshaman_ether_shock", "shadow_shaman_ether_shock" },
            { "skywrath_mage_ancient_seal_debuff", "skywrath_mage_ancient_seal" },
            { "stormspirit_overload_discharge", "storm_spirit_overload" },
            { "tinker_missile.vpcf", "tinker_heat_seeking_missile" },
            { "warlock_upheaval", "warlock_upheaval" },
            { "windrunner_shackleshot_pair_tree", "windrunner_shackleshot" },
            { "witchdoctor_ward_skull", "witch_doctor_death_ward" },
            { "witchdoctor_maledict_aoe", "witch_doctor_maledict" },
            { "zuus_arc_lightning_head", "zuus_arc_lightning" }
        };

        public Dictionary<string, string> ControlPoint_1 { get; } = new Dictionary<string, string>()
        {
            { "centaur_stampede_cast", "centaur_stampede" },
            { "rattletrap_battery_shrapnel", "rattletrap_battery_assault" },
            { "kunkka_spell_tidebringer", "kunkka_tidebringer" },
            { "legion_commander_courage_hit", "legion_commander_moment_of_courage" },
            { "lycan_howl_cast.vpcf", "lycan_howl" },
            { "magnataur_shockwave_cast", "magnataur_shockwave" },
            { "magnataur_skewer", "magnataur_skewer" },
            { "monkey_king_jump_trail", "monkey_king_tree_dance" },
            { "phoenix_supernova_start", "phoenix_supernova" },
            { "wraith_king_vampiric_aura_lifesteal", "skeleton_king_vampiric_aura" },
            { "ursa_enrage_buff", "ursa_enrage" },
            { "medusa_stone_gaze_active", "medusa_stone_gaze" },
            { "viper_viper_strike_warmup", "viper_viper_strike" },
            { "oracle_purifyingflames_cast", "oracle_false_promise" },
            { "techies_blast_off_trail", "techies_suicide" },
            { "warlock_fatal_bonds_hit_parent", "warlock_fatal_bonds" },
            { "zuus_thundergods_wrath_start", "zuus_thundergods_wrath" }
        };

        public Dictionary<string, string> ControlPoint_1Fix { get; } = new Dictionary<string, string>()
        {
            { "axe_beserkers_call_owner", "axe_berserkers_call" },
            { "elder_titan_ancestral_spirit_ambient", "elder_titan_ancestral_spirit" },
            { "legion_commander_odds.vpcf", "legion_commander_overwhelming_odds" },
            { "shredder_chakram_return", "shredder_return_chakram" },
            { "ember_spirit_fire_remnant", "ember_spirit_fire_remnant" },
            { "faceless_void_time_walk_preimage", "faceless_void_time_walk" },
            { "lone_druid_savage_roar", "lone_druid_savage_roar" },
            { "riki_blink_strike", "riki_blink_strike" },
            { "furion_teleport_end.vpcf", "furion_teleportation" },
            { "enchantress_enchant", "enchantress_enchant" },
            { "oracle_false_promise_heal.vpcf", "oracle_purifying_flames" },
            { "oracle_false_promise_cast.vpcf", "oracle_purifying_flames" }
        };

        public Dictionary<string, string> ControlPoint_2 { get; } = new Dictionary<string, string>()
        {
            { "sven_spell_warcry", "sven_warcry" }
        };

        public Dictionary<string, string> ControlPoint_2Fix { get; } = new Dictionary<string, string>()
        {
            { "centaur_warstomp", "centaur_hoof_stomp" },
            { "undying_decay", "undying_decay" },
            { "shadow_demon_soul_catcher_v2_projected_ground", "shadow_demon_soul_catcher" }
        };

        public Dictionary<string, string> ControlPoint_5 { get; } = new Dictionary<string, string>()
        {
            { "visage_soul_assumption_beams", "visage_soul_assumption" }
        };

        public Dictionary<string, string> ControlPoint_5Fix { get; } = new Dictionary<string, string>()
        {
            { "nevermore_wings", "nevermore_requiem" }
        };

        public Dictionary<string, string> ControlPoint_1Plus { get; } = new Dictionary<string, string>()
        {
            { "sven_spell_great_cleave", "sven_great_cleave" },
            { "magnataur_empower_cleave_effect", "magnataur_empower" }
        };

        public Dictionary<string, string> Items { get; } = new Dictionary<string, string>()
        {
            { "battlefury_cleave", "item_bfury" },
            { "blink_dagger_start", "item_blink" },
            { "refresher", "item_refresher" },
            { "vanguard_active_launch", "item_crimson_guard" },
            { "pipe_of_insight.vpcf", "item_hood_of_defiance" },
            { "pipe_of_insight_launch", "item_pipe" }
        };

        public Dictionary<string, string> ItemsNullCP0 { get; } = new Dictionary<string, string>()
        {
            { "smoke_of_deceit.vpcf", "item_smoke_of_deceit" }
        };

        public Dictionary<string, string> ItemsNullCP1 { get; } = new Dictionary<string, string>()
        {
            { "dagon.vpcf", "item_dagon" }
        };

        public Dictionary<string, string> ItemsSemiNullCP0 { get; } = new Dictionary<string, string>()
        {
            { "urn_of_shadows.vpcf", "item_urn_of_shadows" },
            { "glimmer_cape_initial_flash", "item_glimmer_cape" },
            { "veil_of_discord.vpcf", "item_veil_of_discord" },
            { "necronomicon_spawn_warrior", "item_necronomicon" },
        };

        public Dictionary<string, string> ItemsSemiNullCP1 { get; } = new Dictionary<string, string>()
        {
            { "hand_of_midas", "item_hand_of_midas" },
            { "iron_talon_active", "item_iron_talon" },
        };
    }
}
