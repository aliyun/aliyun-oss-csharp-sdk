/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class DeleteObjectsResultDeserializer : ResponseDeserializer<DeleteObjectsResult, DeleteObjectsResult>
    {
        public DeleteObjectsResultDeserializer(IDeserializer<Stream, DeleteObjectsResult> contentDeserializer)
                 : base(contentDeserializer)
        { }

        public override DeleteObjectsResult Deserialize(ServiceResponse xmlStream)
        {
            var deleteObjectsResult = new DeleteObjectsResult();

            if (int.Parse(xmlStream.Headers[HttpHeaders.ContentLength]) != 0)
            {
                deleteObjectsResult = ContentDeserializer.Deserialize(xmlStream.Content);
            }

            DeserializeGeneric(xmlStream, deleteObjectsResult);

            return deleteObjectsResult;
        }
    }
}
