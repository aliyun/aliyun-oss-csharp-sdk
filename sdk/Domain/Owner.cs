/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// The owner of the OSS object.
    /// </summary>
    [XmlRoot("Owner")]
    public class Owner
    {
        /// <summary>
        /// Gets or sets the owner Id.
        /// </summary>
        [XmlElement("ID")]
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the owner name.
        /// </summary>
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="Owner" />---internal only,.
        /// </summary>
        internal Owner()
        { }
        
        /// <summary>
        /// Creates a new instance of <see cref="Owner" /> with owner id and name.
        /// </summary>
        /// <param name="id">Owner id.</param>
        /// <param name="displayName">Owner display name</param>
        internal Owner(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets <see cref="Owner"/> serialization result in string.
        /// </summary>
        /// <returns><see cref="Owner"/> serialization result in string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "[Owner Id={0}, DisplayName={1}]",
                                 Id ?? string.Empty,
                                 DisplayName ?? string.Empty);
        }
    }
}
