/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Transform
{
    internal class GetObjectResponseDeserializer : ResponseDeserializer<OssObject, OssObject>
    {
        private readonly GetObjectRequest _getObjectRequest;

        public GetObjectResponseDeserializer(GetObjectRequest getObjectRequest)
            : base(null)
        {
            _getObjectRequest = getObjectRequest;
        }

        public override OssObject Deserialize(ServiceResponse xmlStream)
        {
            return new OssObject(_getObjectRequest.Key)
            {
                BucketName = _getObjectRequest.BucketName,
                Content = xmlStream.Content,
                Metadata = DeserializerFactory.GetFactory()
                    .CreateGetObjectMetadataResultDeserializer().Deserialize(xmlStream)
            };
        }
    }
}
