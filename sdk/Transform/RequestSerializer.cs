/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;

namespace Aliyun.OSS.Transform
{
    internal abstract class RequestSerializer<TRequest, TModel> : ISerializer<TRequest, Stream>
    {
        protected ISerializer<TModel, Stream> ContentSerializer { get; private set; }

        public RequestSerializer(ISerializer<TModel, Stream> contentSerializer)
        {
            ContentSerializer = contentSerializer;
        }

        public abstract Stream Serialize(TRequest request);
    }
}
