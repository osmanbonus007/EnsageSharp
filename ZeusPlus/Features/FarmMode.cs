using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Service;

namespace ZeusPlus.Features
{
    internal class FarmMode : AttackOrbwalkingModeAsync
    {
        private MenuManager Menu { get; }

        private ZeusPlus Main { get; }

        public FarmMode(Config config, IServiceContext context) : base(context, "Zeus Farm Mode", 32, false, false, false, false, true, true)
        {
            Menu = config.Menu;
            Main = config.Main;
        }

        protected override void OnActivate()
        {
            Menu.FarmKeyItem.Item.ValueChanged += FarmkeyChanged;

            UpdateManager.BeginInvoke(
                () =>
                {
                    Config.Key.Item.ValueChanged += FarmSDKkeyChanged;
                });

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            Config.Key.Item.ValueChanged -= FarmSDKkeyChanged;
            Menu.FarmKeyItem.Item.ValueChanged -= FarmkeyChanged;

            base.OnDeactivate();
        }

        private void FarmSDKkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var keyCode = e.GetNewValue<KeyBind>().Key;

            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            Menu.FarmKeyItem.Item.ValueChanged -= FarmkeyChanged;
            Menu.FarmKeyItem.Item.SetValue(new KeyBind(keyCode));
            Menu.FarmKeyItem.Item.ValueChanged += FarmkeyChanged;
        }

        private void FarmkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var keyCode = e.GetNewValue<KeyBind>().Key;

            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            Config.Key.Item.ValueChanged -= FarmSDKkeyChanged;
            Config.Key.Item.SetValue(new KeyBind(keyCode));
            Config.Key.Item.ValueChanged += FarmSDKkeyChanged;
        }

        public async override Task ExecuteAsync(CancellationToken token)
        {
            var ArcLightning = Main.ArcLightning;
            var unit = EntityManager<Unit>.Entities.FirstOrDefault(x =>
                                                                   (x.NetworkName == "CDOTA_BaseNPC_Creep_Neutral" ||
                                                                   x.NetworkName == "CDOTA_BaseNPC_Invoker_Forged_Spirit" ||
                                                                   x.NetworkName == "CDOTA_BaseNPC_Warlock_Golem" ||
                                                                   x.NetworkName == "CDOTA_BaseNPC_Creep" ||
                                                                   x.NetworkName == "CDOTA_BaseNPC_Creep_Lane" ||
                                                                   x.NetworkName == "CDOTA_BaseNPC_Creep_Siege" ||
                                                                   x.NetworkName == "CDOTA_Unit_Hero_Beastmaster_Boar" ||
                                                                   x.NetworkName == "CDOTA_Unit_SpiritBear" ||
                                                                   x.NetworkName == "CDOTA_Unit_Broodmother_Spiderling") &&
                                                                   x.IsValid &&
                                                                   x.IsVisible &&
                                                                   x.IsAlive &&
                                                                   x.IsSpawned &&
                                                                   !x.IsIllusion &&
                                                                   x.IsEnemy(Owner) &&
                                                                   x.Distance2D(Owner) < ArcLightning.CastRange &&
                                                                   x.Health < ArcLightning.GetDamage(x));
            // Arc Lightning
            if (unit != null && ArcLightning.CanBeCasted && ArcLightning.CanHit(unit))
            {
                ArcLightning.UseAbility(unit);
                await Await.Delay(ArcLightning.GetCastDelay(unit), token);
            }
            else
            {
                Orbwalker.OrbwalkTo(Selector.GetTarget());
            }
        }
    }
}
