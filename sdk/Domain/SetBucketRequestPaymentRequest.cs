/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket request payment.
    /// </summary>
    public class SetBucketRequestPaymentRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the request payment
        /// </summary>
        public RequestPayer Payer { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketRequestPaymentRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="payer">request payer</param>
        public SetBucketRequestPaymentRequest(string bucketName, RequestPayer payer)
        {
            BucketName = bucketName;
            Payer = payer;
        }
    }

}

