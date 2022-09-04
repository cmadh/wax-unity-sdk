using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetCollectionsRequest
    {

        [QueryParameter("author")]
        public string Author;

        [QueryParameter("match")]
        public string Match;

        [QueryParameter("authorized_account")]
        public string AuthorizedAccount;

        [QueryParameter("notify_account")]
        public string NotifyAccount;

        [QueryParameter("collection_blacklist")]
        public List<string> CollectionBlacklist;

        [QueryParameter("collection_whitelist")]
        public List<string> CollectionWhitelist;

        [QueryParameter("ids")]
        public List<string> Ids;

        [QueryParameter("lower_bound")]
        public string LowerBound;

        [QueryParameter("upper_bound")]
        public string UpperBound;

        [QueryParameter("before")]
        public int? Before;

        [QueryParameter("after")]
        public int? After;

        [QueryParameter("page")]
        public int? Page;

        [QueryParameter("limit")]
        public int? Limit;

        [QueryParameter("order")]
        public OrderMode? Order;

        [QueryParameter("sort")]
        public string Sort;

    }

}