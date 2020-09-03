/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;
using System;
using System.Text;
namespace Aliyun.OSS.Transform
{
    internal class SelectObjectJsonMetaRequestSerializer : RequestSerializer<CreateSelectObjectMetaRequest, JsonMetaRequestModel>
    {
        public SelectObjectJsonMetaRequestSerializer(ISerializer<JsonMetaRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(CreateSelectObjectMetaRequest request)
        {
            var model = new JsonMetaRequestModel();

            model.InputFormats = new CreateSelectObjectMetaRequestModel.InputFormat();
            model.InputFormats.CompressionTypes = request.InputFormats.CompressionTypes;

            model.InputFormats.JSONInputs = new CreateSelectObjectMetaRequestModel.InputFormat.JSONInput();
            model.InputFormats.JSONInputs.Types = request.InputFormats.JSONInputs.Types;

            model.OverwriteIfExists = request.OverwriteIfExists;

            return ContentSerializer.Serialize(model);
        }
    }
}

