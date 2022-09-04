namespace StrattonStudios.EosioUnity
{

    public enum KeyType
    {
        K1 = 0,
        R1 = 1,
        WA = 2,
        Sha256x2 = 3
    }

    public static class KeyTypeExtensions
    {

        public static int ToIndex(this KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.K1:
                    return 0;
                case KeyType.R1:
                    return 1;
                case KeyType.WA:
                    return 2;
                default:
                    throw new System.Exception($"Unknown curve type: {keyType}");
            }
        }

        public static KeyType ToKeyType(this int index)
        {
            switch (index)
            {
                case 0:
                    return KeyType.K1;
                case 1:
                    return KeyType.R1;
                case 2:
                    return KeyType.WA;
                default:
                    throw new System.Exception("Unknown curve type");
            }
        }

    }

}