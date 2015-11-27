/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
