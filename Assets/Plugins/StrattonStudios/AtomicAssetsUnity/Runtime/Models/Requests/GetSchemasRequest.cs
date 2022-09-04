using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetSchemasRequest
    {

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("authorized_account")]
        public string AuthorizedAccount;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("match")]
        public string Match;

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
        public OrderMode? OrderMode;

        [QueryParameter("sort")]
        public string Sort;

    }

}