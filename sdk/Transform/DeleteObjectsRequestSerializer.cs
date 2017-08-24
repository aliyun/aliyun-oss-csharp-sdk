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
    internal class DeleteObjectsRequestSerializer : RequestSerializer<DeleteObjectsRequest, DeleteObjectsRequestModel>
    {
        public DeleteObjectsRequestSerializer(ISerializer<DeleteObjectsRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(DeleteObjectsRequest request)
        {
            var newKeys = new List<DeleteObjectsRequestModel.ObjectToDel>();
            foreach (var key in request.Keys)
            {
                newKeys.Add(new DeleteObjectsRequestModel.ObjectToDel { Key = key });
            }

            var model = new DeleteObjectsRequestModel
            {
                Quiet = request.Quiet,
                Keys = newKeys.ToArray()
            };
            return ContentSerializer.Serialize(model);
        }
    }
}
