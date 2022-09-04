using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetTemplatesRequest
    {

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("issued_supply")]
        public int? IssuedSupply;

        [QueryParameter("min_issued_supply")]
        public int? MinIssuedSupply;

        [QueryParameter("max_issued_supply")]
        public int? MaxIssuedSupply;

        [QueryParameter("has_assets")]
        public bool? HasAssets;

        [QueryParameter("max_supply")]
        public int? MaxSupply;

        [QueryParameter("is_burnable")]
        public bool? IsBurnable;

        [QueryParameter("is_transferable")]
        public bool? IsTransferable;

        [QueryParameter("authorized_account")]
        public string AuthorizedAccount;

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
        public OrderMode? Order;

        [QueryParameter("sort")]
        public string Sort;

    }

}