using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// EISUI `time_point`, `time_point_sec` and `block_timestamp_type` utilities.
    /// </summary>
    /// <remarks>
    /// The EOSIO time point is based on microseconds.
    /// </remarks>
    public static class EosioTimeUtility
    {

        /// <summary>
        /// The EPOCH date time in UTC.
        /// </summary>
        public static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Public Methods

        /// <summary>
        /// Converts <see cref="DateTime"/> to EOSIO `time_point` (milliseconds since epoch)
        /// </summary>
        /// <param name="date">The date to convert</param>
        /// <returns>Returns the time point</returns>
        public static ulong ConvertDateToTimePoint(DateTime date)
        {
            var span = (date - EpochUtc);
            return (ulong)(span.Ticks / TimeSpan.TicksPerMillisecond);
        }

        /// <summary>
        /// Converts EOSIO `time_point` (milliseconds since epoch) to <see cref="DateTime"/>
        /// </summary>
        /// <param name="ticks">The EOSIO `time_point` to convert</param>
        /// <returns>Returns the date time corresponding to the time point</returns>
        public static DateTime ConvertTimePointToDate(long ticks)
        {
            return new DateTime(ticks + EpochUtc.Ticks);
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> to `block_timestamp_type` (half-seconds since a different epoch)
        /// </summary>
        /// <param name="value">The date to convert</param>
        /// <returns>Returns the block timestamp type</returns>
        public static uint ConvertDateToBlockTimestamp(DateTime value)
        {
            var span = (value - EpochUtc);
            return (uint)((ulong)Math.Round((double)(span.Ticks / TimeSpan.TicksPerMillisecond - 946684800000) / 500) & 0xffffffff);
        }

        /// <summary>
        /// Converts EOSIO `block_timestamp_type` (half-seconds since a different epoch) to <see cref="DateTime"/>
        /// </summary>
        /// <param name="slot">The EOSIO `block_timestamp_type` to convert</param>
        /// <returns>Returns the date time corresponding to the block timestamp type</returns>
        public static DateTime ConvertBlockTimestampToDate(uint slot)
        {
            return new DateTime(slot * TimeSpan.TicksPerMillisecond * 500 + 946684800000 + new DateTime(1970, 1, 1).Ticks);
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> to EOSIO `time_point_sec` (seconds since epoch)
        /// </summary>
        /// <param name="value">The date to convert</param>
        /// <returns>Returns the time point seconds</returns>
        public static uint ConvertDateToTimePointSec(DateTime value)
        {
            var span = (value - new DateTime(1970, 1, 1));
            return (uint)((span.Ticks / TimeSpan.TicksPerSecond) & 0xffffffff);
        }

        /// <summary>
        /// Converts EOSIO `time_point_sec` (seconds since epoch) to <see cref="DateTime"/>
        /// </summary>
        /// <param name="secs">The EOSIO `time_point_sec` to convert</param>
        /// <returns>Returns the date time corresponding to the time point seconds</returns>
        public static DateTime ConvertTimePointSecToDate(uint secs)
        {
            return new DateTime(secs * TimeSpan.TicksPerSecond + new DateTime(1970, 1, 1).Ticks);
        }

        /// <summary>
        /// Gets the expiration date time from the timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp</param>
        /// <param name="expireSeconds">The expiration time</param>
        /// <returns>Returns the date time as string</returns>
        public static string GetExpirationTime(uint timestamp, uint expireSeconds = 60)
        {
            return ConvertTimePointSecToDate(timestamp + expireSeconds).ToString();
        }

        /// <summary>
        /// Converts UNIX timestamp to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="timestamp">The UNIX timestamp</param>
        /// <returns>Returns the date time corresponding to the UNIX timestamp</returns>
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to UNIX timestamp.
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns>Returns the UNIX timestamp corresponding to the date</returns>
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        #endregion

    }

}