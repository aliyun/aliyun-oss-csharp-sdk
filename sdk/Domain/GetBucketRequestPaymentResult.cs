/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's request payment.
    /// </summary>
    public class GetBucketRequestPaymentResult : GenericResult
    {
        /// <summary>
        /// Gets the request payment
        /// </summary>
        public RequestPayer Payer { get; set; }
    }
}
