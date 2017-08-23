using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System;

namespace LoneDruidSharpRewrite.Features.Orbwalk
{
    public class AttackAnimationTracker
    {
        private NetworkActivity lastUnitActivity;

        private float nextUnitAttackEnd;

        private float nextUnitAttackRelease;

        protected AttackAnimationTracker(Unit unit)
        {
            this.Unit = unit;
            Events.OnUpdate += this.Track;
        }

        public Unit Unit { get; set; }


        public bool CanAttack(Entity target = null, float bonusWindupMs = 0)
        {
            if (this.Unit == null || !this.Unit.IsValid)
            {
                return false;
            }

            var turnTime = 0d;
            if (target != null)
            {
                turnTime = this.Unit.GetTurnTime(target)
                           + (Math.Max(this.Unit.Distance2D(target) - this.Unit.GetAttackRange() - 100, 0)
                              / this.Unit.MovementSpeed);
            }

            return this.nextUnitAttackEnd - Game.Ping - (turnTime * 1000) - 75 + bonusWindupMs < Utils.TickCount;
        }

        public bool CanCancelAttack(float delay = 0f)
        {
            if (this.Unit == null || !this.Unit.IsValid)
            {
                return true;
            }

            var time = Utils.TickCount;
            var cancelTime = this.nextUnitAttackRelease - Game.Ping - delay + 50;
            return time >= cancelTime;
        }

        private void Track(EventArgs args)
        {
            if (this.Unit == null || !this.Unit.IsValid)
            {
                Events.OnUpdate -= this.Track;
                return;
            }

            if (!Game.IsInGame || Game.IsPaused)
            {
                return;
            }

            if (this.Unit.NetworkActivity == this.lastUnitActivity)
            {
                return;
            }

            this.lastUnitActivity = this.Unit.NetworkActivity;
            if (!this.Unit.IsAttacking())
            {
                if (this.CanCancelAttack())
                {
                    return;
                }

                this.lastUnitActivity = 0;
                this.nextUnitAttackEnd = 0;
                this.nextUnitAttackRelease = 0;
                return;
            }

            this.nextUnitAttackEnd = (float)(Utils.TickCount + (UnitDatabase.GetAttackRate(this.Unit) * 1000));
            this.nextUnitAttackRelease = (float)(Utils.TickCount + (UnitDatabase.GetAttackPoint(this.Unit) * 1000));

        }
    }
}
