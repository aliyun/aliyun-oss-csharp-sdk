/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */
 
using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定完成Delete Multiple Objects的请求参数。
    /// </summary>
    public class DeleteObjectsRequest
    {
        private readonly IList<string> _keys = new List<string>();
        private string _encodingType;

        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// 获取或设置请求结果的返回模式（详细模式或静默模式）。
        /// </summary>
        public bool Quiet { get; private set; }
        
        /// <summary>
        /// 获取或者设置需要删除的key列表。
        /// </summary>
        public IList<string> Keys
        {
            get { return _keys; }
        }
         
        /// <summary>
        /// 获取或者设置encoding-type的值，默认是url。
        /// </summary>
        public string EncodingType
        {
            get
            {
                return _encodingType ??  HttpUtils.UrlEncodingType;
            }
            set
            {
                _encodingType = value;
            }
        }

        /// <summary>
        /// 使用静默方式的构造函数。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject"/></param>
        /// <param name="keys">预删除的Object列表</param>
        public DeleteObjectsRequest(string bucketName, IList<string> keys)
            : this(bucketName, keys, true)
        { }

        /// <summary>
        /// 使用指定的请求结果返回模式的构造函数。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject"/></param>
        /// <param name="keys">预删除的Object列表</param>
        /// <param name="quiet">true表示静默模式，false表示详细模式</param>
        public DeleteObjectsRequest(string bucketName, IList<string> keys, bool quiet)
        {
            if (keys == null)
                throw new ArgumentException("The list of keys to be deleted should not be null");
            if (keys.Count <= 0)
                throw new ArgumentException("No any keys specified.");
            if (keys.Count > OssUtils.DeleteObjectsUpperLimit)
                throw new ArgumentException("Count of objects to be deleted exceeds upper limit");

            BucketName = bucketName;
            foreach (var key in keys)
            {
                OssUtils.CheckObjectKey(key);
                Keys.Add(key);
            }
            Quiet = quiet;
        }
    }
}
