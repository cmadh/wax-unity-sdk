using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Request flag model.
    /// </summary>
    [System.Flags]
    public enum RequestFlag : byte
    {
        None = 0,
        Broadcast = 1 << 0,
        Background = 1 << 1
    }

    /// <summary>
    /// Request flag extensions.
    /// </summary>
    public static class RequestFlagExtensions
    {

        public static RequestFlag GetDefault()
        {
            return RequestFlag.Broadcast;
        }

        public static RequestFlag SetBroadcast(this RequestFlag flag, bool b)
        {
            if (b)
            {
                return flag |= RequestFlag.Broadcast;
            }
            else
            {
                return flag &= ~RequestFlag.Broadcast;
            }
        }

        public static RequestFlag SetBackground(this RequestFlag flag, bool b)
        {
            if (b)
            {
                return flag |= RequestFlag.Background;
            }
            else
            {
                return flag &= ~RequestFlag.Background;
            }
        }

        public static bool IsBroadcast(this RequestFlag flag)
        {
            return flag.HasFlag(RequestFlag.Broadcast);
            //return (flag & RequestFlag.Broadcast) != 0;
        }

        public static bool IsBackground(this RequestFlag flag)
        {
            return flag.HasFlag(RequestFlag.Background);
            //return (flag & RequestFlag.Background) != 0;
        }

        public static byte GetFlagValue(this RequestFlag flag)
        {
            return (byte)flag;
        }

    }
}