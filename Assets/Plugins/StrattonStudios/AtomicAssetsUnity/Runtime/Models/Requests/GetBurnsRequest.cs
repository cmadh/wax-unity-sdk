using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetBurnsRequest
    {

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("template_id")]
        public string TemplateId;

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

        [QueryParameter("page")]
        public int? Page;

        [QueryParameter("limit")]
        public int? Limit;

        [QueryParameter("order")]
        public OrderMode? Order;

    }

}