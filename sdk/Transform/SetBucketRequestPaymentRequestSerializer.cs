/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketRequestPaymentRequestSerializer : RequestSerializer<SetBucketRequestPaymentRequest, RequestPaymentConfiguration>
    {
        public SetBucketRequestPaymentRequestSerializer(ISerializer<RequestPaymentConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(SetBucketRequestPaymentRequest request)
        {
            var model = new RequestPaymentConfiguration();
            model.Payer = request.Payer;
            return ContentSerializer.Serialize(model);
        }
    }
}

