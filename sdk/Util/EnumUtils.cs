/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;

namespace Aliyun.OSS.Util
{
    internal static class EnumUtils
    {
        private static readonly IDictionary<Enum, StringValueAttribute> StringValues =
            new Dictionary<Enum, StringValueAttribute>();

        public static string GetStringValue(Enum value)
        {
            string output;
            var type = value.GetType();

            if (StringValues.ContainsKey(value))
            {
                output = StringValues[value].Value;
            }
            else
            {
                var fi = type.GetField(value.ToString());
                var attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) 
                    as StringValueAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    output = attrs[0].Value;
                    // Put it in the cache.
                    lock(StringValues)
                    {
                        // Double check
                        if (!StringValues.ContainsKey(value))
                        {
                            StringValues.Add(value, attrs[0]);
                        }
                    }
                }
                else
                {
                    return value.ToString();
                }
            }

            return output;
        }
    }
}
