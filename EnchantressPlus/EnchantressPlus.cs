using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.npc_dota_hero_enchantress;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

using log4net;

using PlaySharp.Toolkit.Logging;

namespace EnchantressPlus
{
    [ExportPlugin(
        name: "EnchantressPlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.1.0.0",
        units: HeroId.npc_dota_hero_enchantress)]
    internal class EnchantressPlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        private AbilityFactory AbilityFactory { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public EnchantressPlus([Import] IServiceContext context)
        {
            Context = context;
            AbilityFactory = context.AbilityFactory;
        }

        public enchantress_enchant Enchant { get; set; }

        public enchantress_natures_attendants NaturesAttendants { get; set; }

        public enchantress_impetus Impetus { get; set; }

        public Dagon Dagon
        {
            get
            {
                return Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;
            }
        }

        public Necronomicon Necronomicon
        {
            get
            {
                return Necronomicon1 ?? Necronomicon2 ?? (Necronomicon)Necronomicon3;
            }
        }

        [ItemBinding]
        public item_sheepstick Hex { get; set; }

        [ItemBinding]
        public item_orchid Orchid { get; set; }

        [ItemBinding]
        public item_bloodthorn Bloodthorn { get; set; }

        [ItemBinding]
        public item_rod_of_atos RodofAtos { get; set; }

        [ItemBinding]
        public item_veil_of_discord Veil { get; set; }

        [ItemBinding]
        public item_ethereal_blade Ethereal { get; set; }

        [ItemBinding]
        public item_dagon Dagon1 { get; set; }

        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }

        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }

        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }

        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }

        [ItemBinding]
        public item_necronomicon Necronomicon1 { get; set; }

        [ItemBinding]
        public item_necronomicon_2 Necronomicon2 { get; set; }

        [ItemBinding]
        public item_necronomicon_3 Necronomicon3 { get; set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; set; }

        [ItemBinding]
        public item_cyclone Eul { get; set; }

        [ItemBinding]
        public item_shivas_guard Shivas { get; set; }

        [ItemBinding]
        public item_hurricane_pike HurricanePike { get; set; }

        [ItemBinding]
        public item_heavens_halberd HeavensHalberd { get; set; }

        protected override void OnActivate()
        {
            Config = new Config(this);

            Enchant = AbilityFactory.GetAbility<enchantress_enchant>();
            NaturesAttendants = AbilityFactory.GetAbility<enchantress_natures_attendants>();
            Impetus = AbilityFactory.GetAbility<enchantress_impetus>();

            Context.Inventory.Attach(this);
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            Config?.Dispose();
        }
    }
}
