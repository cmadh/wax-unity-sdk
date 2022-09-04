using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.EosioUnity.Signing
{

    public class DefaultAbiProvider : IAbiProvider
    {

        protected EosioClient eos;

        public DefaultAbiProvider(EosioClient eos)
        {
            this.eos = eos;
        }

        public async UniTask<Abi> GetAbi(string account)
        {
            return await this.eos.GetAbi(account);
        }

    }

}