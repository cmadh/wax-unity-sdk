using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Permission
    {

        public string perm_name;

        public string parent;

        public Authority required_auth;
    }

}