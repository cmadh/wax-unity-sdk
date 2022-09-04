using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Interface storage adapters should implement.
    /// </summary>
    /// <remarks>
    /// Storage adapters are responsible for persisting <see cref="LinkSession"/>s and can optionally be
    /// passed to the <see cref="Link"/> constructor to auto-persist sessions.
    /// </remarks>
    public interface ILinkStorage
    {

        /// <summary>
        /// Write string to storage at key. Should overwrite existing values without error.
        /// </summary>
        /// <param name="key">The key to write</param>
        /// <param name="data">The data to write</param>
        UniTask Write(string key, string data);

        /// <summary>
        /// Read key from storage. Should return <c>null</c> if key can not be found.
        /// </summary>
        /// <param name="key">The key to read</param>
        /// <returns>Returns the loaded data if exists</returns>
        UniTask<string> Read(string key);

        /// <summary>
        /// Delete key from storage. Should not error if deleting non-existing key.
        /// </summary>
        /// <param name="key">The key to remove</param>
        UniTask Remove(string key);

    }

}