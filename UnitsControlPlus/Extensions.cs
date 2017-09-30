using System.Collections.Generic;
using System.Linq;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Extensions;

using SharpDX;

namespace UnitsControlPlus
{
    internal class Extensions
    {
        public int GetDelay { get; } = 100 + (int)Game.Ping;

        private float LastCastAttempt { get; set; }

        public bool CanBeCasted(Ability ability, Unit unit)
        {
            if (ability == null)
            {
                return false;
            }

            if (ability.Level == 0 || ability.IsHidden || ability.Cooldown > 0)
            {
                return false;
            }

            if (unit.Mana < ability.ManaCost)
            {
                return false;
            }

            var isItem = ability is Item;
            if (unit.IsStunned() || unit.IsMuted() || !isItem && unit.IsSilenced())
            {
                return false;
            }

            if ((Game.RawGameTime - LastCastAttempt) < 0.8f)
            {
                return false;
            }

            return true;
        }

        public bool CanHit(Ability ability, Unit unit, Unit target)
        {
            if (unit.Distance2D(target) < ability.CastRange)
            {
                return true;
            }

            return false;
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

        public bool UseAbility(Ability ability, Unit unit, Unit target)
        {
            if (!CanBeCasted(ability, unit))
            {
                return false;
            }

            var result = ability.UseAbility(target);
            if (result)
            {
                LastCastAttempt = Game.RawGameTime;
            }

            return result;
        }

        public bool UseAbility(Ability ability, Unit unit, Vector3 position)
        {
            if (!CanBeCasted(ability, unit))
            {
                return false;
            }

            var result = ability.UseAbility(position);
            if (result)
            {
                LastCastAttempt = Game.RawGameTime;
            }

            return result;
        }

        private HashSet<NetworkActivity> AttackActivities { get; } = new HashSet<NetworkActivity>
        {
            NetworkActivity.Attack,
            NetworkActivity.Attack2,
            NetworkActivity.AttackEvent
        };

        public bool Attack(Unit unit, Unit target)
        {
            if (unit.IsChanneling() || target.IsInvulnerable() || target.IsAttackImmune())
            {
                return false;
            }

            if (!AttackActivities.Any(x => x == unit.NetworkActivity) 
                || unit.Name == "npc_dota_neutral_prowler_shaman"
                || unit.Name == "npc_dota_neutral_prowler_acolyte")
            {
                if (Utils.SleepCheck($"Attack{unit.Handle}"))
                {
                    Utils.Sleep(800, $"Attack{unit.Handle}");

                    return unit.Attack(target);
                }
            }

            return false;
        }

        public bool Move(Unit unit, Unit target)
        {
            if (unit.IsChanneling() || unit.IsRooted())
            {
                return false;
            }

            if (Utils.SleepCheck($"Move{unit.Handle}"))
            {
                Utils.Sleep(800, $"Move{unit.Handle}");

                return unit.Move(target.Position);
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
                Utils.Sleep(800, $"Move{unit.Handle}");

                return unit.Move(position);
            }

            return false;
        }

        public bool Follow(Unit unit, Unit target)
        {
            if (Utils.SleepCheck($"Follow{unit.Handle}"))
            {
                Utils.Sleep(800, $"Follow{unit.Handle}");

                return unit.Move(target.Position);
            }

            return false;
        }
    }
}
