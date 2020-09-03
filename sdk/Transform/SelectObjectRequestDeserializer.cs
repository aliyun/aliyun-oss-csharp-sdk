/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SelectObjectRequestDeserializer : ResponseDeserializer<OssObject, OssObject>
    {
        private readonly SelectObjectRequest _request;

        public SelectObjectRequestDeserializer(SelectObjectRequest selectObjectRequest)
            : base(null)
        {
            _request = selectObjectRequest;
        }

        public override OssObject Deserialize(ServiceResponse xmlStream)
        {
            OssObject ossObject = new OssObject(_request.Key)
            {
                BucketName = _request.BucketName,
                ResponseStream = xmlStream.Content,
                Metadata = DeserializerFactory.GetFactory()
                    .CreateGetObjectMetadataResultDeserializer().Deserialize(xmlStream)
            };
            DeserializeGeneric(xmlStream, ossObject);

            if (xmlStream.Headers.ContainsKey("x-oss-select-output-raw") &&
                xmlStream.Headers["x-oss-select-output-raw"].Equals("false"))
            {
                var stream = new SelectObjectStream(xmlStream.Content);
                ossObject.ResponseStream = stream;
            }

            return ossObject;
        }
    }
}
