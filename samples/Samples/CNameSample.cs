/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of CNAME.
    /// </summary>
    public static class CNameSample
    {
        const string accessKeyId = "<your access key id>";
        const string accessKeySecret= "<your access key secret>";
        const string endpoint = "<valid host name>";

        const string bucketName = "<your bucket name>";
        const string key = "<your key>";
        const string fileToUpload = "<your local file path>";

        static ClientConfiguration clientConfig = new ClientConfiguration();
        static OssClient client = null;

        static CNameSample()
        {
            clientConfig.IsCname = true;
            client = new OssClient(new Uri(endpoint), accessKeyId, accessKeySecret, clientConfig);
        }

        public static void CNameOperation()
        {
            PutObject();
            DeleteObject();
            SetBucketAcl();
        }

        public static void PutObject() 
        {
            client.PutObject(bucketName, key, fileToUpload);
        }

        public static void DeleteObject()
        {
            client.DeleteObject(bucketName, key);
        }

        public static void SetBucketAcl()
        {
            client.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);
        }
    }
}
