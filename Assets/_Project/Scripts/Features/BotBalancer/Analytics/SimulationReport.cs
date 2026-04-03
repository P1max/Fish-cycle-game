using System;
using System.Collections.Generic;

namespace Features.BotBalancer.Analytics
{
    [Serializable]
    public class SimulationReport
    {
        public string ProfileName;
        public List<IntervalSnapshot> Snapshots = new();
    }

    [Serializable]
    public class IntervalSnapshot
    {
        public float TimeSecond;
        public int CoinsBalance;
        public int FishesCount;
        public int DeadCount;
        public int BoughtCount;
        public int BornCount;
        public int FeederUsedCount;
        public int CoinsCollectedAmount;
    }
}