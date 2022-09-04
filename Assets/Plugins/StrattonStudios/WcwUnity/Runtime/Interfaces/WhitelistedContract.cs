using System.Collections.Generic;

namespace StrattonStudios.WcwUnity
{

    public class WhitelistedContract
    {

        public string Contract { get; set; }

        public string Domain { get; set; }

        public List<string> Recipients { get; set; } = new List<string>();

    }

}