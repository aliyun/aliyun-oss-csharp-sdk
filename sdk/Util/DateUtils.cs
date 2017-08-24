/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;

namespace Aliyun.OSS.Util
{
    internal static class DateUtils
    {
        private const string Rfc822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";
        private const string Iso8601DateFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

        /// <summary>
        /// Format an instance of <see cref="DateTime" /> to a GMT format string.
        /// </summary>
        public static string FormatRfc822Date(DateTime dtime)
        {
            return dtime.ToUniversalTime().ToString(Rfc822DateFormat,
                               CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format a GMT format string to an instance of <see cref="DateTime" />.
        /// </summary>
        /// <returns></returns>
        public static DateTime ParseRfc822Date(String s)
        {
            return DateTime.SpecifyKind(
                DateTime.ParseExact(s, Rfc822DateFormat, CultureInfo.InvariantCulture),
                DateTimeKind.Utc);
        }

        /// <summary>
        /// Format an instance of <see cref="DateTime" /> to string in iso-8601 format.
        /// </summary>
        public static string FormatIso8601Date(DateTime dtime)
        {
            
            return dtime.ToUniversalTime().ToString(Iso8601DateFormat,
                               CultureInfo.CurrentCulture);
        }
    }
}
