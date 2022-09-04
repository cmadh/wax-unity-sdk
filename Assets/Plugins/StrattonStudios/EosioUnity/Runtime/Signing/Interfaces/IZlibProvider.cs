namespace StrattonStudios.EosioUnity.Signing
{

    public interface IZlibProvider
    {

        byte[] DecompressByteArray(byte[] data);


        byte[] CompressByteArray(byte[] data);

    }

}