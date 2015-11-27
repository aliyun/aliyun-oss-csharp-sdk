/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 设置指定Bucket的Referer白名单和是否允许referer字段为空的请求。
    /// </summary>
    public class SetBucketRefererRequest
    {
        private readonly IList<string> _refererList = new List<string>();  
        
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 指定是否允许referer字段为空的请求访问。
        /// </summary>
        public bool AllowEmptyReferer { get; private set; }
         
        /// <summary>
        /// 获取或者设置需要删除的referer列表。
        /// </summary>
        public IList<string> RefererList
        {
            get { return _refererList; }
        }

        /// <summary>
        /// 使用空的referer列表、允许referer为空的请求访问的构造函数。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject"/></param>
        public SetBucketRefererRequest(string bucketName)
            : this(bucketName, null, true)
        { }

        /// <summary>
        /// 使用指定的referer列表、允许referer为空的请求访问的构造函数。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject"/></param>
        /// <param name="refererList">referer列表</param>
        public SetBucketRefererRequest(string bucketName, IList<string> refererList)
            : this(bucketName, refererList, true)
        { }

        /// <summary>
        /// 使用指定的referer列表、是否允许referer为空的请求访问的构造函数。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject"/></param>
        /// <param name="refererList">referer列表</param>
        /// <param name="allowEmptyReferer">是否允许referer为空的请求访问</param>
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
        /// 清空referer列表。
        /// </summary>
        public void ClearRefererList()
        {
            RefererList.Clear();
        }
    }
}
