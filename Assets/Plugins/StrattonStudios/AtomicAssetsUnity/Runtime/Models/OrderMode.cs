using StrattonStudios.Networking;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public enum OrderMode
    {

        [QueryParameter("asc")]
        Ascending,
        [QueryParameter("desc")]
        Descending,

    }

}