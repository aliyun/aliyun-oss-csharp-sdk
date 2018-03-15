/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class SelectObjectResponseDeserializer : GetObjectResponseDeserializer
    {
        private readonly SelectObjectRequest _getObjectRequest;
        private readonly IServiceClient _serviceClient;

        public SelectObjectResponseDeserializer(SelectObjectRequest getObjectRequest, IServiceClient serviceClient)
            : base(getObjectRequest, serviceClient)
        {
            _getObjectRequest = getObjectRequest;
            _serviceClient = serviceClient;
        }

        public override OssObject Deserialize(ServiceResponse xmlStream)
        {
            OssObject ossObj = base.Deserialize(xmlStream);
            ossObj.ResponseStream = new OssSelectStream(ossObj.Content);

            return ossObj;
        }
    }
}
