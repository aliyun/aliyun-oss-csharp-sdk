/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using System.IO;
using Aliyun.OSS.Common.Internal;
namespace Aliyun.OSS.Common.Communication
{
    // this is to workaround the HttpClient behavior that it will dispose the request's content after the send.
    // While this behavior does not work with current code base as the caller suppose the stream can be re-used.
    internal class StreamWeakReferece : WrapperStream
    {
        public StreamWeakReferece(Stream stream) : base(stream)
        {
        }

        public override void Close() // does not close the underlying stream
        {
        }
    }
}
