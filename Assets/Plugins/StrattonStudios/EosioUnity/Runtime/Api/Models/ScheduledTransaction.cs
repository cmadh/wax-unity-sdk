using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class ScheduledTransaction
    {

        public string trx_id;

        public string sender;

        public string sender_id;

        public string payer;

        public DateTime? delay_until;

        public DateTime? expiration;

        public DateTime? published;

        public Object transaction;
    }

}