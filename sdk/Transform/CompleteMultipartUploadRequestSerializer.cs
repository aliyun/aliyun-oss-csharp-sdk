/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class CompleteMultipartUploadRequestSerializer 
        : RequestSerializer<CompleteMultipartUploadRequest, CompleteMultipartUploadRequestModel>
    {
        public CompleteMultipartUploadRequestSerializer(ISerializer<CompleteMultipartUploadRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }
        
        public override Stream Serialize(CompleteMultipartUploadRequest request)
        {
            var model = new CompleteMultipartUploadRequestModel();
            var modelParts = new List<CompleteMultipartUploadRequestModel.CompletePart>();
            foreach (var part in request.PartETags)
            {
                var modelPart = new CompleteMultipartUploadRequestModel.CompletePart
                {
                    ETag = "\"" + part.ETag + "\"",
                    PartNumber = part.PartNumber
                };
                modelParts.Add(modelPart);
            }
            model.Parts = modelParts.ToArray();
            return ContentSerializer.Serialize(model);
        }
    }
}
