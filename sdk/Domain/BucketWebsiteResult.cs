/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's static website config.
    /// </summary>
   public class BucketWebsiteResult : GenericResult
    {
        /// <summary>
        /// The index page for the static website.
        /// </summary>
       public string IndexDocument { get; internal set; }

        /// <summary>
        /// The error page for the static website.
        /// </summary>
       public string ErrorDocument { get; internal set; }
    }
}
