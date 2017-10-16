using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_zuus;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

using log4net;

using PlaySharp.Toolkit.Logging;

namespace ZeusPlus
{
    [ExportPlugin(
        name: "ZeusPlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.0.0.1",
        units: HeroId.npc_dota_hero_zuus)]
    internal class ZeusPlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        private AbilityFactory AbilityFactory { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public ZeusPlus([Import] IServiceContext context)
        {
            Context = context;
            AbilityFactory = context.AbilityFactory;
        }

        public zuus_arc_lightning ArcLightning { get; set; }

        public zuus_lightning_bolt LightningBolt { get; set; }

        public zuus_static_field StaticField { get; set; }

        public zuus_cloud Nimbus { get; set; }

        public zuus_thundergods_wrath ThundergodsWrath { get; set; }

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

        [ItemBinding]
        public item_shivas_guard Shivas { get; set; }

        [ItemBinding]
        public item_blink Blink { get; set; }

        protected override void OnActivate()
        {
            ArcLightning = AbilityFactory.GetAbility<zuus_arc_lightning>();
            LightningBolt = AbilityFactory.GetAbility<zuus_lightning_bolt>();
            StaticField = AbilityFactory.GetAbility<zuus_static_field>();
            Nimbus = AbilityFactory.GetAbility<zuus_cloud>();
            ThundergodsWrath = AbilityFactory.GetAbility<zuus_thundergods_wrath>();

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
