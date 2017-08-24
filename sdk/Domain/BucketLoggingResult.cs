/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket logging config
    /// </summary>
   public class BucketLoggingResult : GenericResult
    {
       /// <summary>
       /// Target bucket.
       /// </summary>
       public string TargetBucket { get; internal set; }

       /// <summary>
       /// Target logging file's prefix. If it's empty, the OSS system will name the file instead.
       /// </summary>
       public string TargetPrefix { get; internal set; }
    }
}
