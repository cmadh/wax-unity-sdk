using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class SerializableAction
    {

        public string account;

        public string name;

        public List<SerializablePermissionLevel> authorization;

        public object data;

        public string hex_data;
    }

}