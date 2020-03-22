/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;
using System.Collections.Generic;

namespace Aliyun.OSS.Transform
{
    internal class DeleteObjectVersionsRequestSerializer : RequestSerializer<DeleteObjectVersionsRequest, DeleteObjectVersionsRequestModel>
    {
        public DeleteObjectVersionsRequestSerializer(ISerializer<DeleteObjectVersionsRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(DeleteObjectVersionsRequest request)
        {
            var model = new DeleteObjectVersionsRequestModel
            {
                Quiet = request.Quiet,
                ObjectToDels = new DeleteObjectVersionsRequestModel.ObjectToDel[request.Objects.Count]
            };
            
            for (var i = 0; i < request.Objects.Count; i++)
            {
                model.ObjectToDels[i] = new DeleteObjectVersionsRequestModel.ObjectToDel
                {
                    Key = request.Objects[i].Key,
                    VersionId = request.Objects[i].VersionId
                };
            };

            return ContentSerializer.Serialize(model);
        }
    }
}
