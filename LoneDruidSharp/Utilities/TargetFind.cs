using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using SharpDX;
using System.Linq;

namespace LoneDruidSharpRewrite.Utilities
{
    public class TargetFind
    {
        private readonly DotaTexture heroIcon;

        private readonly Sleeper sleeper;

        private Vector2 iconSize;

        public bool locked;

        public TargetFind()
        {
            this.sleeper = new Sleeper();
            this.heroIcon = Drawing.GetTexture("materials/ensage_ui/miniheroes/lone_druid");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public Hero Target { get; private set; }

        public void DrawTarget()
        {
            if(this.Target == null || !this.Target.IsVisible || !this.Target.IsAlive)
            {
                return;
            }

            Vector2 screenPosition;
            if (
                !Drawing.WorldToScreen(
                    this.Target.Position + new Vector3(0, 0, this.Target.HealthBarOffset / 3),
                    out screenPosition))
            {
                return;
            }

            screenPosition += new Vector2(-this.iconSize.X, 0);
            Drawing.DrawRect(screenPosition, this.iconSize, this.heroIcon);
            if (this.locked)
            {
                
                Drawing.DrawText(
                    "LOCKED",
                    screenPosition + new Vector2(this.iconSize.X + 2, 1),
                    new Vector2((float)(this.iconSize.X * 0.85)),
                    new Color(255, 150, 110),
                    FontFlags.AntiAlias | FontFlags.Additive);
            }

        }
        public void Find()
        {
            if (this.sleeper.Sleeping)
            {
                return;
            }

            if(this.locked && this.Target != null && this.Target.IsAlive)
            {
                return;
            }
            this.Target =
                Heroes.GetByTeam(Variable.EnemyTeam)
                    .Where(
                        x =>
                        x.IsValid && x.IsAlive && !x.IsIllusion && x.IsVisible
                        && x.Distance2D(Game.MousePosition) < 2000)
                    .MinOrDefault(x => x.Distance2D(Game.MousePosition));
            this.sleeper.Sleep(100);
        }

        public void LockTarget()
        {
            this.locked = true;
        }

        public void UnlockTarget()
        {
            this.locked = false;
        }
    }
}
