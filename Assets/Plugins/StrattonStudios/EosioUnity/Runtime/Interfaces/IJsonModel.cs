using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Specifies the object as JSON model.
    /// </summary>
    public interface IJsonModel
    {

        string ToJSON();

    }

}