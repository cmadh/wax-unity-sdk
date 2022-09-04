using System.Collections.Generic;


namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// EOSIO Signing Request interface.
    /// </summary>
    public interface IESRRequest : IJsonModel
    {

        List<Action> GetRawActions();

        List<object> ToVariant();

    }

}