/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketRequestPaymentResultDeserializer : ResponseDeserializer<GetBucketRequestPaymentResult, RequestPaymentConfiguration>
    {
        public GetBucketRequestPaymentResultDeserializer(IDeserializer<Stream, RequestPaymentConfiguration> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketRequestPaymentResult Deserialize(ServiceResponse xmlStream)
        {
            GetBucketRequestPaymentResult result = new GetBucketRequestPaymentResult();

            var mode = ContentDeserializer.Deserialize(xmlStream.Content);

            result.Payer = mode.Payer;

            this.DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

