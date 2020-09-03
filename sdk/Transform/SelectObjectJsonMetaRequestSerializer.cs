/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;
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

            var inputFormat = (CreateSelectObjectMetaJSONInputFormat)request.InputFormat;

            model.InputFormat = new CreateSelectObjectMetaInputFormatModel();
            model.InputFormat.CompressionTypeInfo = request.InputFormat.CompressionType;

            model.InputFormat.JSON = new CreateSelectObjectMetaInputFormatModel.JSONModel();

            model.InputFormat.JSON.Type = inputFormat.Type;

            model.OverwriteIfExists = request.OverwriteIfExists;

            return ContentSerializer.Serialize(model);
        }
    }
}

