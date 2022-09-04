
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Utilities;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    public class IdentityProof
    {

        public ChainId chainId;

        public string scope;

        public string expiration;

        public PermissionLevel signer;

        public string signature;

        public Transaction GetTransaction()
        {
            var action = new Action()
            {
                Account = new AccountName(""),
                Name = new ActionName("identity"),
                Authorization = new List<PermissionLevel>() { this.signer },
                Data = new ActionData(new IdentityV2(this.signer).ToDictionary())
            };
            return new Transaction()
            {
                RefBlockNum = 0,
                RefBlockPrefix = 0,
                Expiration = this.expiration,
                Actions = new List<Action>() { action }
            };
        }

        public async UniTask<string> Recover()
        {
            var transactionDigest = await GetTransaction().SigningDigest(null, this.chainId.GetChainId());
            return RecoverDigest(transactionDigest);
        }

        public string RecoverDigest(string digest)
        {
            var pubKey = RecoverUtility.CheckSignedMessage(HexUtility.FromHexString(digest), this.signature);
            var pub = KeyUtility.ConvertPublicKeyBinaryToString(pubKey);
            return pub;
        }

        public bool Verify(Authority auth, uint currentTime)
        {
            //SerializationHelper.DateToTimePointSec(System.DateTime.Parse(this.expiration));
            var expire = System.DateTimeOffset.Parse(this.expiration, System.Globalization.CultureInfo.InvariantCulture).ToUnixTimeMilliseconds();
            return (currentTime < expire && HasPermission(auth, this.signature));

        }

        public int KeyWeight(Authority auth, string publicKey)
        {
            var weight = auth.keys.Find(x => x.key == publicKey);
            return weight != null ? weight.weight : 0;
        }

        public bool HasPermission(Authority auth, string publicKey, bool includePartial = false)
        {
            var threshold = includePartial ? 1 : auth.threshold;
            var weight = KeyWeight(auth, publicKey);
            return weight >= threshold;
        }

    }

}