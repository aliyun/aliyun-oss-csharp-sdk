/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Internal
{
    internal class StreamReadTracker : StreamTransferTracker
    {
        internal StreamReadTracker(object sender, EventHandler<StreamTransferProgressArgs> callback, 
                                   long contentLength, long totalBytesRead, long progressUpdateInterval)
            :base(sender, callback, contentLength, totalBytesRead, progressUpdateInterval)
        {
        }

        public void ReadProgress(int bytesRead)
        {
            TransferredProgress(bytesRead);
        }
    }
}
