using System.Collections.Generic;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// EOSIO Resolved Signing Request.
    /// </summary>
    public class ResolvedSigningRequest
    {

        public readonly SigningRequest SigningRequest;
        public readonly PermissionLevel Signer;
        public readonly Transaction Transaction;
        public readonly byte[] SerializedTransaction;

        public static async UniTask<ResolvedSigningRequest> FromPayload(JObject payload, SigningRequestEncodingOptions options)
        {
            var request = SigningRequest.From((string)payload["req"], options);
            var abis = await request.FetchAbis();
            var transaction = new TransactionContext();
            transaction.RefBlockNum = ushort.Parse((string)payload["rbn"]);
            transaction.RefBlockPrefix = uint.Parse((string)payload["rid"]);
            transaction.Expiration = System.DateTime.Parse((string)payload["ex"], System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.ffffffZ", System.Globalization.CultureInfo.InvariantCulture);
            if (payload.ContainsKey("cid"))
            {
                transaction.SetChain((string)payload["cid"]);
            }
            else
            {
                transaction.SetChain(request.GetChainId().GetChainId());
            }
            return await request.Resolve(abis, new PermissionLevel((string)payload["sa"], (string)payload["sp"]), transaction);
        }

        public ResolvedSigningRequest(SigningRequest request, PermissionLevel signer, Transaction transaction, byte[] serializedTransaction)
        {
            this.SigningRequest = request;
            this.Signer = signer;
            this.Transaction = transaction;
            this.SerializedTransaction = serializedTransaction;
        }

        public byte[] GetSerializedTransaction()
        {
            var result = new byte[this.SerializedTransaction.Length];
            System.Array.Copy(this.SerializedTransaction, result, this.SerializedTransaction.Length);
            return result;
        }

        public string GetTransactionId()
        {
            try
            {
                byte[] sha256 = Sha256Manager.GetHash(this.SerializedTransaction);
                // Could use: SerializationHelper.ByteArrayToHexString(sha256);
                return HexUtility.ToHexString(sha256);
                //return EncodingHelper.ByteArrayToString(sha256);
            }
            catch (System.Exception e)
            {
                throw new EosioException("Failed to get SHA256 message digest", e);
            }
        }

        public ResolvedCallback GetCallback(List<string> signatures)
        {
            return GetCallback(signatures, -1);
        }

        public ResolvedCallback GetCallback(List<string> signatures, long blockNum)
        {
            if (string.IsNullOrEmpty(this.SigningRequest.GetCallback()))
                throw new EosioException("Callback is null or empty");

            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add(ResolvedCallback.SIG, signatures[0]);
            payload.Add(ResolvedCallback.TX, GetTransactionId());
            payload.Add(ResolvedCallback.RBN, this.Transaction.RefBlockNum.ToString());
            payload.Add(ResolvedCallback.RID, this.Transaction.RefBlockPrefix.ToString());
            payload.Add(ResolvedCallback.EX, this.Transaction.Expiration);
            payload.Add(ResolvedCallback.REQ, this.SigningRequest.Encode());
            payload.Add(ResolvedCallback.SA, this.Signer.Actor.Value);
            payload.Add(ResolvedCallback.SP, this.Signer.Permission.Value);

            for (int i = 1; i < signatures.Count; i++)
                payload.Add(ResolvedCallback.SIG + i, signatures[i]);

            if (blockNum != -1)
                payload.Add(ResolvedCallback.BN, blockNum.ToString());


            var callback = this.SigningRequest.GetCallback();
            System.Text.RegularExpressions.Regex.Replace(callback, "(\\{\\{([a-z0-9]+)\\}\\})", m =>
            {
                string text = m.Groups[0].Value;
                text = text.Substring(2);
                text = text.Substring(0, text.Length - 2);
                return payload.ContainsKey(text) ? payload[text] : "";
            });

            //Pattern pattern = Pattern.compile("(\\{\\{([a-z0-9]+)\\}\\})");
            //Matcher matcher = pattern.matcher(this.gSigningRequest.GetCallback());
            //StringBuffer url = new StringBuffer(this.gSigningRequest.GetCallback().length());
            //while (matcher.find())
            //{
            //    string text = matcher.group(0);
            //    text = text.substring(2);
            //    text = text.substring(0, text.length() - 2);
            //    matcher.appendReplacement(url, payload.containsKey(text) ? String.valueOf(payload.get(text)) : "");
            //}
            //matcher.appendTail(url);
            //string callbackUrl = url.toString();
            return new ResolvedCallback(callback, this.SigningRequest.GetRequestFlag().IsBackground(), payload);
        }

        public IdentityProof GetIdentityProof(string signature)
        {
            if (!this.SigningRequest.IsIdentity())
            {
                throw new System.Exception("Not a identity request");
            }
            var proof = new IdentityProof();
            proof.chainId = this.SigningRequest.GetChainId();
            proof.expiration = this.Transaction.Expiration;
            proof.signer = this.Signer;
            proof.signature = signature;
            return proof;
        }

    }

}