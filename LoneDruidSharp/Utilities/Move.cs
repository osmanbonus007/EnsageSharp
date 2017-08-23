using Ensage;
using SharpDX;

namespace LoneDruidSharpRewrite.Utilities
{
    public class Move
    {
        private readonly Sleeper sleeper;

        private readonly Unit unit;

        public Move(Unit unit)
        {
            this.sleeper = new Sleeper();
            this.unit = unit;
        }

        public void Pause(float duration)
        {
            this.sleeper.Sleep(duration);
        }

        public void ToPosition(Vector3 position)
        {
            this.Execute(position);
        }

        private void Execute(Vector3 position)
        {
            if (this.sleeper.Sleeping)
            {
                return;
            }
            this.unit.Move(position);
            this.sleeper.Sleep(200);
        }

    }
}
