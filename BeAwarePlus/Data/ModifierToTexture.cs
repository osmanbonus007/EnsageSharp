using System.Collections.Generic;

namespace BeAwarePlus.Data
{
    internal class ModifierToTexture
    {
        public Dictionary<string, string> ModifierAllyList { get; } = new Dictionary<string, string>()
        {
            { "modifier_spirit_breaker_charge_of_darkness_vision", "spirit_breaker_charge_of_darkness" },
            { "modifier_nevermore_presence", "nevermore_dark_lord" },
            { "modifier_sniper_assassinate", "sniper_assassinate" },
            { "modifier_bounty_hunter_track", "bounty_hunter_track" },
            { "modifier_bloodseeker_thirst_vision", "bloodseeker_thirst" }
        };

        public Dictionary<string, string> ModifierEnemyList { get; } = new Dictionary<string, string>()
        {
            { "modifier_spirit_breaker_charge_of_darkness", "spirit_breaker_charge_of_darkness" }
        };

        public Dictionary<string, int> ModifierOthersList { get; } = new Dictionary<string, int>()
        {
            { "modifier_rune_haste", 10000 },
            { "modifier_rune_regen", 10000 },
            { "modifier_rune_arcane", 10000 },
            { "modifier_rune_doubledamage", 10000 },
            { "modifier_rune_invis", 3000 },
            { "modifier_item_invisibility_edge_windwalk", 3000 },
            { "modifier_item_shadow_amulet_fade", 3000 },
            { "modifier_item_silver_edge_windwalk", 3000 },
            { "modifier_item_gem_of_true_sight", 15000 },
            { "modifier_item_divine_rapier", 15000 }
        };
    }
}
