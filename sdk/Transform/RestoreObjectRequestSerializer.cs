/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class RestoreObjectRequestSerializer : RequestSerializer<RestoreObjectRequest, RestoreRequestModel>
    {
        public RestoreObjectRequestSerializer(ISerializer<RestoreRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(RestoreObjectRequest request)
        {
            var model = new RestoreRequestModel();
            model.Days = request.Days;
            if (request.Tier != null) { 
                model.JobParameter = new RestoreRequestModel.JobParameters();
                model.JobParameter.Tier = request.Tier.Value;
            }
            return ContentSerializer.Serialize(model);
        }
    }
}

