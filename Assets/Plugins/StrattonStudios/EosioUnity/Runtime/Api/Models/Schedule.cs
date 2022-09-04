using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Schedule
    {

        public UInt32? version;

        public List<ScheduleProducers> producers;
    }

}