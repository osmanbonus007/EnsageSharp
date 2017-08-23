namespace VisageSharpRewrite.Utilities
{
    public class Sleeper
    {
        private float lastSleepTickCount;

        public Sleeper()
        {
            this.lastSleepTickCount = 0;
        }

        public bool Sleeping
        {
            get
            {
                return Variables.TickCount < this.lastSleepTickCount;
            }
        }

        public void Sleep(float duration)
        {
            this.lastSleepTickCount = Variables.TickCount + duration;
        }
    }
}
