using System.Collections;
using System.Collections.Generic;

using StrattonStudios.Networking;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class GetOffersRequest
    {

        [QueryParameter("account")]
        public string Account;

        [QueryParameter("sender")]
        public string Sender;

        [QueryParameter("recipient")]
        public string Recipient;

        [QueryParameter("state")]
        public string State;

        [QueryParameter("is_recipient_contract")]
        public bool? IsRecipientContract;

        [QueryParameter("asset_Id")]
        public string AssetId;

        [QueryParameter("template_name")]
        public string TemplateId;

        [QueryParameter("schema_name")]
        public string SchemaName;

        [QueryParameter("collection_name")]
        public string CollectionName;

        [QueryParameter("account_whitelist")]
        public string AccountWhitelist;

        [QueryParameter("account_blacklist")]
        public string AccountBlacklist;

        [QueryParameter("sender_asset_whitelist")]
        public string SenderAssetWhitelist;

        [QueryParameter("sender_asset_blacklist")]
        public string SenderAssetBlacklist;

        [QueryParameter("recipient_asset_whitelist")]
        public string RecipientAssetWhitelist;

        [QueryParameter("recipient_asset_blacklist")]
        public string RecipientAssetBlacklist;

        [QueryParameter("collection_blacklist")]
        public List<string> CollectionBlacklist;

        [QueryParameter("collection_whitelist")]
        public List<string> CollectionWhitelist;

        [QueryParameter("ids")]
        public List<string> Ids;

        [QueryParameter("account")]
        public string LowerBound;

        [QueryParameter("upper_bound")]
        public string UpperBound;

        [QueryParameter("before")]
        public int? efore;

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