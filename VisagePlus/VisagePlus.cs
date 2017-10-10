using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.npc_dota_hero_visage;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

using log4net;

using PlaySharp.Toolkit.Logging;

namespace VisagePlus
{
    [ExportPlugin(
        name: "VisagePlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.2.0.0",
        units: HeroId.npc_dota_hero_visage)]
    internal class VisagePlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        private AbilityFactory AbilityFactory { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public VisagePlus([Import] IServiceContext context)
        {
            Context = context;
            AbilityFactory = context.AbilityFactory;
        }

        public visage_grave_chill GraveChill { get; set; }

        public visage_soul_assumption SoulAssumption { get; set; }

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
        public item_medallion_of_courage Medallion { get; set; }

        [ItemBinding]
        public item_solar_crest SolarCrest { get; set; }

        [ItemBinding]
        public item_armlet Armlet { get; set; }

        [ItemBinding]
        public item_shivas_guard Shivas { get; set; }

        [ItemBinding]
        public item_hurricane_pike HurricanePike { get; set; }

        [ItemBinding]
        public item_heavens_halberd HeavensHalberd { get; set; }

        protected override void OnActivate()
        {
            GraveChill = AbilityFactory.GetAbility<visage_grave_chill>();
            SoulAssumption = AbilityFactory.GetAbility<visage_soul_assumption>();

            Context.Inventory.Attach(this);

            Config = new Config(this);
        }

        protected override void OnDeactivate()
        {
            Config?.Dispose();

            Context.Inventory.Detach(this);
        }
    }
}
