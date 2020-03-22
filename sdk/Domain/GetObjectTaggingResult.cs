/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.Collections.Generic;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's tagging.
    /// </summary>
    public class GetObjectTaggingResult : GenericResult
    {
        /// <summary>
        /// The bucket tagging.
        /// </summary>
        private readonly IList<Tag> tags = new List<Tag>();

        internal void Addtag(Tag tag)
        {
            tags.Add(tag);
        }

        public IList<Tag> Tags
        {
            get { return tags; }
        }

        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string VersionId { get; internal set; }
    }
}
