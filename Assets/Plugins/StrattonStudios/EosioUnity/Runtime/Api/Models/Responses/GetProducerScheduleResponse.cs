using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetProducerScheduleResponse
    {

        public Schedule active;

        public Schedule pending;

        public Schedule proposed;
    }

}