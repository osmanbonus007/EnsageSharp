using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_skywrath_mage;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

using log4net;

using PlaySharp.Toolkit.Logging;

namespace SkywrathMagePlus
{
    [ExportPlugin(
        name: "SkywrathMagePlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.2.1.1",
        units: HeroId.npc_dota_hero_skywrath_mage)]
    internal class SkywrathMagePlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public SkywrathMagePlus([Import] IServiceContext context)
        {
            Context = context;
            AbilityFactory = context.AbilityFactory;
        }

        private AbilityFactory AbilityFactory { get; }

        public skywrath_mage_arcane_bolt ArcaneBolt { get; set; }

        public skywrath_mage_concussive_shot ConcussiveShot { get; set; }

        public skywrath_mage_ancient_seal AncientSeal { get; set; }

        public skywrath_mage_mystic_flare MysticFlare { get; set; }

        public Dagon Dagon
        {
            get
            {
                return Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;
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
        public item_force_staff ForceStaff { get; set; }

        [ItemBinding]
        public item_cyclone Eul { get; set; }

        protected override void OnActivate()
        {
            Config = new Config(this);

            ArcaneBolt = AbilityFactory.GetAbility<skywrath_mage_arcane_bolt>();
            ConcussiveShot = AbilityFactory.GetAbility<skywrath_mage_concussive_shot>();
            AncientSeal = AbilityFactory.GetAbility<skywrath_mage_ancient_seal>();
            MysticFlare = AbilityFactory.GetAbility<skywrath_mage_mystic_flare>();

            Context.Inventory.Attach(this);
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            Config?.Dispose();
        }
    }
}
