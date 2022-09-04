using System;

using StrattonStudios.EosioUnity;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AuthorityAccount
    {

        public PermissionLevel permission;

        public Int32 weight;
    }

}