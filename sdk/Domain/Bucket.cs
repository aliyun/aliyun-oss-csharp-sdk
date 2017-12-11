/*
///Copyright (C) Alibaba Cloud Computing
///All rights reserved.
///
 */

using System;
using System.Globalization;

namespace Aliyun.OSS
{
    ///<summary>
    ///Bucket is the OSS namespace, which could be thought as storage space.
    ///</summary>
    /// <remarks>
    ///<para>
    ///Bucket is globally unique across the whole OSS and is immutable. Every object must be stored at one and only one bucket.
    ///An application, such as picture sharing website, could have one or more bucket. And each account could only create up to 10 buckets.
    ///But in every bucket, there's no limit in terms of data size and object count.
    ///</para>
    ///<para>
    ///Bucket naming rules
    ///<list type="">
    /// <item>Can only have lowercase letter, number or dash (-)</item>
    /// <item>Can only start with lowercase letter or number</item>
    /// <item>The length must be between 3 and 63</item>
    ///</list>
    ///</para>
    /// </remarks>
    public class Bucket
    {
        /// <summary>
        /// Bucket location getter/setter
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Bucket <see cref="Owner" /> getter/setter
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// Bucket creation time getter/setter
        /// </summary>
        public DateTime CreationDate { get; internal set; }

        /// <summary>
        /// Creats a new <see cref="Bucket" /> instance with the specified name.
        /// </summary>
        /// <param name="name">Bucket name</param>
        internal Bucket(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Returns the bucket's serialization information in string.
        /// </summary>
        /// <returns>The serialization information in string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "OSS Bucket [Name={0}], [Location={1}] [Owner={2}], [CreationTime={3}]",
                                 Name, Location, Owner, CreationDate);
        }
    }
}
