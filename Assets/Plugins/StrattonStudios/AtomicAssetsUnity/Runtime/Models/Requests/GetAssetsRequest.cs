using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetAssetsRequest
    {

        [QueryParameter("owner")]
        public string Owner;

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("template_id")]
        public int? TemplateId;

        [QueryParameter("match")]
        public string Match;

        [QueryParameter("collection_blacklist")]
        public List<string> CollectionBlacklist;

        [QueryParameter("collection_whitelist")]
        public List<string> CollectionWhitelist;

        [QueryParameter("only_duplicate_templates")]
        public bool? OnlyDuplicateTemplates;

        [QueryParameter("authorized_account")]
        public string AuthorizedAccount;

        [QueryParameter("hide_offers")]
        public bool? HideOffers;

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