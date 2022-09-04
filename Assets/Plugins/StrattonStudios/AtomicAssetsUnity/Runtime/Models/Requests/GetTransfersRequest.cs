using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetTransfersRequest
    {

        [QueryParameter("account")]
        public string Account;

        [QueryParameter("sender")]
        public string Sender;

        [QueryParameter("recipient")]
        public string Recipient;

        [QueryParameter("asset_id")]
        public string AssetId;

        [QueryParameter("template_id")]
        public string TemplateId;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("collection_black_list")]
        public List<string> CollectionBlacklist;

        [QueryParameter("collection_whitelist")]
        public List<string> CollectionWhitelist;

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