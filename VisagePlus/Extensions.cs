using Ensage;
using Ensage.Common;
using Ensage.SDK.Extensions;

using SharpDX;

namespace VisagePlus
{
    internal class Extensions
    {
        public int GetDelay { get; } = 100 + (int)Game.Ping;

        private float LastCastAttempt { get; set; }

        public bool CanBeCasted(Ability ability, Unit unit)
        {
            if (ability.IsHidden || ability.Cooldown > 0)
            {
                return false;
            }

            if (unit.IsStunned() || unit.IsMuted() || unit.IsSilenced())
            {
                return false;
            }

            if ((Game.RawGameTime - LastCastAttempt) < 0.8f)
            {
                return false;
            }

            return true;
        }

        public bool UseAbility(Ability ability, Unit unit)
        {
            if (!CanBeCasted(ability, unit))
            {
                return false;
            }

            var result = ability.UseAbility();
            if (result)
            {
                LastCastAttempt = Game.RawGameTime;
            }

            return result;
        }

        public bool Attack(Unit unit, Unit target)
        {
            if (Utils.SleepCheck($"Attack{unit.Handle}"))
            {
                Utils.Sleep(200, $"Attack{unit.Handle}");

                return unit.Attack(target);
            }

            return false;
        }

        public bool Move(Unit unit, Vector3 position)
        {
            if (unit.IsRooted())
            {
                return false;
            }

            if (Utils.SleepCheck($"Move{unit.Handle}"))
            {
                Utils.Sleep(200, $"Move{unit.Handle}");

                return unit.Move(position);
            }

            return false;
        }

        public bool Follow(Unit unit, Unit target)
        {
            if (unit.IsRooted())
            {
                return false;
            }

            if (Utils.SleepCheck($"Follow{unit.Handle}"))
            {
                Utils.Sleep(200, $"Follow{unit.Handle}");

                return unit.Follow(target);
            }

            return false;
        }

        public bool SmartStone(Unit target)
        {
            return target.HasModifier("modifier_teleporting");
        }
    }
}
