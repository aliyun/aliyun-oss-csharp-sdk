/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket static website configuration
    /// </summary>
    public class SetBucketWebsiteRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Index page
        /// </summary>
        public string IndexDocument { get; private set; }

        /// <summary>
        /// Error page
        /// </summary>
        public string ErrorDocument { get; private set; }

        /// <summary>
        /// Website configuration in xml format
        /// </summary>
        public string Configuration { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketWebsiteRequest" />.
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />bucket name</param>
        /// <param name="indexDocument">index page</param>
        /// <param name="errorDocument">error page</param>
        public SetBucketWebsiteRequest(string bucketName, string indexDocument, string errorDocument)
        {
            BucketName = bucketName;
            IndexDocument = indexDocument;
            ErrorDocument = errorDocument;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketWebsiteRequest" />.
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />bucket name</param>
        /// <param name="configuration">website configuration in xml format</param>
        public SetBucketWebsiteRequest(string bucketName, string configuration)
        {
            BucketName = bucketName;
            Configuration = configuration;
        }
    }
}
