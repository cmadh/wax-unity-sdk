namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Signature provider interface for ESR.
    /// </summary>
    public interface ISignatureProvider
    {

        Signature Sign(string message);

    }

}