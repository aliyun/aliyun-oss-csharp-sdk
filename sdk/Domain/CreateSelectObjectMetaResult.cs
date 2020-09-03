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
    /// The result class of the operation to create the meta of select object.
    /// </summary>
    public class CreateSelectObjectMetaResult : GenericResult
    {
        public long Offset { get; internal set; }

        public long TotalScannedBytes { get; internal set; }

        public int  Status { get; internal set; }

        public long SplitsCount { get; internal set; }

        public long RowsCount { get; internal set; }

        public long ColumnsCount { get; internal set; }

        /// <summary>
        /// Gets or sets the ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; internal set; }
    }
}

