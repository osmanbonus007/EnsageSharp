namespace LoneDruidSharpRewrite.Utilities
{
    class Sleeper
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
                return Variable.TickCount < this.lastSleepTickCount;
            }
        }

        public void Sleep(float duration)
        {
            this.lastSleepTickCount = Variable.TickCount + duration;
        }





    }
}
