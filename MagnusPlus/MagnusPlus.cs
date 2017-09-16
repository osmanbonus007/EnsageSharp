using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using Ensage.SDK.Abilities.npc_dota_hero_magnataur;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

using PlaySharp.Toolkit.Logging;
using log4net;

namespace MagnusPlus
{
    [ExportPlugin(
        name: "MagnusPlus",
        mode: StartupMode.Auto,
        author: "YEEEEEEE", 
        version: "1.0.0.0",
        units: HeroId.npc_dota_hero_magnataur)]
    internal class MagnusPlus : Plugin
    {
        private Config Config { get; set; }

        public IServiceContext Context { get; }

        public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public MagnusPlus([Import] IServiceContext context)
        {
            Context = context;
            AbilityFactory = context.AbilityFactory;
        }

        private AbilityFactory AbilityFactory { get; }

        public magnataur_shockwave Shockwave { get; set; }

        public magnataur_empower Empower { get; set; }

        public magnataur_skewer Skewer { get; set; }

        public magnataur_reverse_polarity ReversePolarity { get; set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; set; }

        [ItemBinding]
        public item_blink Blink { get; set; }

        [ItemBinding]
        public item_black_king_bar BKB { get; set; }

        [ItemBinding]
        public item_shivas_guard Shivas { get; set; }

        [ItemBinding]
        public item_refresher Refresher { get; set; }

        [ItemBinding]
        public item_arcane_boots ArcaneBoots { get; set; }

        [ItemBinding]
        public item_guardian_greaves Guardian { get; set; }

        protected override void OnActivate()
        {
            Config = new Config(this);

            Shockwave = AbilityFactory.GetAbility<magnataur_shockwave>();
            Empower = AbilityFactory.GetAbility<magnataur_empower>();
            Skewer = AbilityFactory.GetAbility<magnataur_skewer>();
            ReversePolarity = AbilityFactory.GetAbility<magnataur_reverse_polarity>();

            Context.Inventory.Attach(this);

            Context.TargetSelector.Config.ShowTargetParticle.Item.SetValue(false);
        }

        protected override void OnDeactivate()
        {
            Context.TargetSelector.Config.ShowTargetParticle.Item.SetValue(true);

            Context.Inventory.Detach(this);

            Config?.Dispose();
        }
    }
}
