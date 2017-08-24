/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request of the operation to set bucket referer.
    /// </summary>
    public class SetBucketRefererRequest
    {
        private readonly IList<string> _refererList = new List<string>();  
        
        /// <summary>
        /// Gets the bucket name.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the flag of allowing empty referer.
        /// </summary>
        public bool AllowEmptyReferer { get; private set; }
         
        /// <summary>
        /// Gets the referer list.
        /// </summary>
        public IList<string> RefererList
        {
            get { return _refererList; }
        }

        /// <summary>
        /// Creates the instance of SetBucketRefererRequest
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public SetBucketRefererRequest(string bucketName)
            : this(bucketName, null, true)
        { }

        /// <summary>
        /// Creates the instance of <see cref="SetBucketRefererRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="refererList">referer list </param>
        public SetBucketRefererRequest(string bucketName, IList<string> refererList)
            : this(bucketName, refererList, true)
        { }

        /// <summary>
        /// Creates the instance of <see cref="SetBucketRefererRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="refererList">referer list</param>
        /// <param name="allowEmptyReferer">allowEmptyReferer flag</param>
        public SetBucketRefererRequest(string bucketName, IEnumerable<string> refererList, 
            bool allowEmptyReferer)
        {            
            if (refererList != null)
            {
                foreach (var referer in refererList)
                {
                    if (string.IsNullOrEmpty(referer))
                        continue;
                    _refererList.Add(referer);
                }
            }

            BucketName = bucketName;
            AllowEmptyReferer = allowEmptyReferer;
        }

        /// <summary>
        /// Clears the referer list.
        /// </summary>
        public void ClearRefererList()
        {
            RefererList.Clear();
        }
    }
}
