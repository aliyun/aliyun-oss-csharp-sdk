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
    /// Description of DeleteObjectVersionsResult.
    /// </summary>
    public class DeleteObjectVersionsResult : GenericResult
    {
        private readonly IList<DeletedObjectSummary> _deletedObjectSummaries = new List<DeletedObjectSummary>();

        /// <summary>
        /// The iterator of <see cref="DeletedObjectSummary" /> that meet the requirements in the DeleteObjectVersionsRequest.
        /// </summary>
        public IEnumerable<DeletedObjectSummary> DeletedObjectSummaries
        {
            get { return _deletedObjectSummaries; }
        }

        /// <summary>
        /// gets or sets EncodingType
        /// </summary>
        public string EncodingType { get; set; }


        internal void AddDeletedObjectSummary(DeletedObjectSummary summary)
        {
            _deletedObjectSummaries.Add(summary);
        }
    }
}
