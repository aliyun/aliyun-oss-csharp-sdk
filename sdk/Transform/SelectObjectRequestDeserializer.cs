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
        private readonly SelectObjectRequest _selectObjectRequest;

        public SelectObjectRequestDeserializer(SelectObjectRequest selectObjectRequest)
            : base(null)
        {
            _selectObjectRequest = selectObjectRequest;
        }

        public override OssObject Deserialize(ServiceResponse xmlStream)
        {
            OssObject ossObject = new OssObject(_selectObjectRequest.Key)
            {
                BucketName = _selectObjectRequest.BucketName,
                ResponseStream = xmlStream.Content,
                Metadata = DeserializerFactory.GetFactory()
                    .CreateGetObjectMetadataResultDeserializer().Deserialize(xmlStream)
            };
            if (!_selectObjectRequest.OutputFormats.OutputRawData)
            {
                var stream = new SelectObjectStream(xmlStream.Content);
                ossObject.ResponseStream = stream;
            }

            DeserializeGeneric(xmlStream, ossObject);
            return ossObject;
        }
    }
}
