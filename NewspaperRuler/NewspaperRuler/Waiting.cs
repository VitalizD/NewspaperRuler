using System;

namespace NewspaperRuler
{
    public class Waiting
    {
        private int remainingTicks;
        private readonly Action execute;

        public Waiting(Action actionAfterWaiting) => execute = actionAfterWaiting;

        public void EveryTick()
        {
            if (remainingTicks > 0)
            {
                remainingTicks--;
                if (remainingTicks == 0)
                    execute();
            }
        }

        public void WaitAndExecute(int ticks) => remainingTicks = ticks;

        public void Cancel() => remainingTicks = 0;
    }
}
