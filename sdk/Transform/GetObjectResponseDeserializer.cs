/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class GetObjectResponseDeserializer : ResponseDeserializer<OssObject, OssObject>
    {
        private readonly GetObjectRequest _getObjectRequest;
        private readonly IServiceClient _serviceClient;

        public GetObjectResponseDeserializer(GetObjectRequest getObjectRequest, IServiceClient serviceClient)
            : base(null)
        {
            _getObjectRequest = getObjectRequest;
            _serviceClient = serviceClient;
        }

        public override OssObject Deserialize(ServiceResponse xmlStream)
        {
            OssObject ossObject =  new OssObject(_getObjectRequest.Key)
            {
                BucketName = _getObjectRequest.BucketName,
                ResponseStream = xmlStream.Content,
                Metadata = DeserializerFactory.GetFactory()
                    .CreateGetObjectMetadataResultDeserializer().Deserialize(xmlStream)
            };

            DeserializeGeneric(xmlStream, ossObject);

            var conf = OssUtils.GetClientConfiguration(_serviceClient);
            var originalStream = ossObject.ResponseStream;
            var streamLength = ossObject.ContentLength;

            // setup progress
            var callback = _getObjectRequest.StreamTransferProgress;
            if (callback != null)
            {
                originalStream = OssUtils.SetupProgressListeners(originalStream, streamLength, conf.ProgressUpdateInterval, _serviceClient, callback);
                ossObject.ResponseStream = originalStream;
            }

            // wrap response stream in MD5Stream
            if (conf.EnalbeMD5Check)
            {
                byte[] expectedHashDigest = null;
                if (xmlStream.Headers.ContainsKey(HttpHeaders.ContentMd5)) 
                {
                    var md5OfResponse = xmlStream.Headers[HttpHeaders.ContentMd5];
                    expectedHashDigest = Convert.FromBase64String(md5OfResponse);
                }
                var hashStream = new MD5Stream(originalStream, expectedHashDigest, streamLength);
                ossObject.ResponseStream = hashStream;
            }

            return ossObject;
        }
    }
}
