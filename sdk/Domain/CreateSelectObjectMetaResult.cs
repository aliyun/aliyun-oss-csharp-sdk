/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.Collections.Generic;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get CreateSelectObjectMetaResult.
    /// </summary>
    public class CreateSelectObjectMetaResult : GenericResult
    {
        public ulong Offset { get; internal set; }

        public ulong TotalScannedbytes { get; internal set; }

        public uint  Status { get; internal set; }

        public uint SplitsCount { get; internal set; }

        public ulong RowsCount { get; internal set; }

        public uint ColsCount { get; internal set; }

        /// <summary>
        /// Gets or sets the ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; internal set; }
    }
}

