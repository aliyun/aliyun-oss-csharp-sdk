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
    internal class StreamReadTracker
    {
        private object sender;
        private EventHandler<StreamTransferProgressArgs> callback;
        private long contentLength;
        private long totalBytesRead;
        private long totalIncrementTransferred;
        private long progressUpdateInterval;

        internal StreamReadTracker(object sender, EventHandler<StreamTransferProgressArgs> callback, 
                                   long contentLength, long totalBytesRead, long progressUpdateInterval)
        {
            this.sender = sender;
            this.callback = callback;
            this.contentLength = contentLength;
            this.totalBytesRead = totalBytesRead;
            this.progressUpdateInterval = progressUpdateInterval;
        }

        public void ReadProgress(int bytesRead)
        {
            if (callback == null)
                return;

            // Invoke the progress callback only if bytes read > 0
            if (bytesRead > 0)
            {
                totalBytesRead += bytesRead;
                totalIncrementTransferred += bytesRead;

                if (totalIncrementTransferred >= this.progressUpdateInterval ||
                    totalBytesRead == contentLength)
                {
                    OssUtils.InvokeInBackground(
                                        callback,
                                        new StreamTransferProgressArgs(totalIncrementTransferred, totalBytesRead, contentLength),
                                        sender);
                    totalIncrementTransferred = 0;
                }
            }
        }

        public void UpdateProgress(float progress)
        {
            int bytesRead = (int)((long)(progress * contentLength) - totalBytesRead);
            ReadProgress(bytesRead);
        }

    }
}
