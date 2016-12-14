/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Authentication;

namespace Aliyun.OSS 
{
    /// <summary>
    /// 阿里云对象存储服务（Object Storage Service， OSS）的访问接口。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 阿里云对象存储服务（Object Storage Service，简称OSS），是阿里云对外提供的海量，安全，低成本，
    /// 高可靠的云存储服务。用户可以通过简单的REST接口，在任何时间、任何地点上传和下载数据，
    /// 也可以使用WEB页面对数据进行管理。
    /// 基于OSS，用户可以搭建出各种多媒体分享网站、网盘、个人企业数据备份等基于大规模数据的服务。
    /// </para>
    /// <para>
    /// OSS的web体验地址：http://www.aliyun.com/product/oss
    /// </para>
    /// </remarks>
    public interface IOss
    {
        #region Switch Credentials & Endpoint

        /// <summary>
        /// 更换用户账号信息。
        /// </summary>
        /// <param name="creds">用户账号信息。</param>
        void SwitchCredentials(ICredentials creds);

        /// <summary>
        /// 设置访问OSS的Endpoint。
        /// </summary>
        /// <param name="endpoint">Endpoint值</param>
        void SetEndpoint(Uri endpoint);

        #endregion

        #region Bucket Operations

        /// <summary>
        /// 在OSS中创建一个新的Bucket。
        /// </summary>
        /// <param name="bucketName">要创建的Bucket的名称。</param>
        /// <returns><see cref="Bucket" />对象。</returns>
        Bucket CreateBucket(string bucketName);

        /// <summary>
        /// 在OSS中删除一个Bucket。
        /// </summary>
        /// <param name="bucketName">要删除的Bucket的名称。</param>
        void DeleteBucket(string bucketName);

        /// <summary>
        /// 返回请求者拥有的所有<see cref="Bucket" />的列表。
        /// </summary>
        /// <returns>请求者拥有的所有<see cref="Bucket" />的列表。</returns>
        IEnumerable<Bucket> ListBuckets();

        /// <summary>
        /// 分页返回请求者拥有的<see cref="Bucket" />的列表。
        /// </summary>
        /// <param name="listBucketsRequest"><see cref="ListBucketsRequest"/>对象</param>
        /// <returns><see cref="ListBucketsResult" /><see cref="Bucket"/>信息的列表</returns>
        ListBucketsResult ListBuckets(ListBucketsRequest listBucketsRequest);

        /// <summary>
        /// 设置指定<see cref="Bucket" />的访问权限<see cref="AccessControlList" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="acl"><see cref="CannedAccessControlList" />枚举中的访问权限。</param>
        void SetBucketAcl(string bucketName, CannedAccessControlList acl);

        /// <summary>
        /// 设置指定<see cref="Bucket" />的访问权限<see cref="AccessControlList" />。
        /// </summary>
        /// <param name="setBucketAclRequest"></param>
        void SetBucketAcl(SetBucketAclRequest setBucketAclRequest);

        /// <summary>
        /// 获取指定<see cref="Bucket" />的访问权限<see cref="AccessControlList" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns>访问权限<see cref="AccessControlList" />的实例。</returns>
        AccessControlList GetBucketAcl(string bucketName);


        /// <summary>
        /// 设置指定<see cref="Bucket" />的跨域资源共享(CORS)的规则，并覆盖原先所有的CORS规则。
        /// </summary>
        /// <param name="setBucketCorsRequest"></param>
        void SetBucketCors(SetBucketCorsRequest setBucketCorsRequest);

        /// <summary>
        /// 获取指定<see cref="Bucket" />的CORS规则。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <returns>跨域资源共享规则列表</returns>
        IList<CORSRule> GetBucketCors(string bucketName);

        /// <summary>
        /// 关闭指定<see cref="Bucket" />对应的CORS功能并清空所有规则。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        void DeleteBucketCors(string bucketName);

        /// <summary>
        /// 设置<see cref="Bucket" />的访问日志记录功能。
        /// 这个功能开启后，OSS将自动记录访问这个<see cref="Bucket" />请求的详细信息，并按照用户指定的规则，
        /// 以小时为单位，将访问日志作为一个Object写入用户指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="setBucketLoggingRequest"></param>
        void SetBucketLogging(SetBucketLoggingRequest setBucketLoggingRequest);

        /// <summary>
        /// 查看<see cref="Bucket" />的访问日志配置。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns>访问日志记录信息</returns>
        BucketLoggingResult GetBucketLogging(string bucketName);
        
        /// <summary>
        /// 关闭<see cref="Bucket" />的访问日志记录功能。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        void DeleteBucketLogging(string bucketName);

        /// <summary>
        /// 将一个<see cref="Bucket" />设置成静态网站托管模式。
        /// </summary>
        /// <param name="setBucketWebSiteRequest">静态网站托管状态<see cref="SetBucketWebsiteRequest"/></param>
        void SetBucketWebsite(SetBucketWebsiteRequest setBucketWebSiteRequest);
        

        /// <summary>
        /// 获取<see cref="Bucket" />的静态网站托管状态。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns>静态网站托管状态<see cref="BucketWebsiteResult"/></returns>
        BucketWebsiteResult GetBucketWebsite(string bucketName);


        /// <summary>
        /// 关闭<see cref="Bucket" />的静态网站托管模式。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        void DeleteBucketWebsite(string bucketName);


        /// <summary>
        /// 设置<see cref="Bucket" />的Referer访问白名单和是否允许referer字段为空。
        /// </summary>
        /// <param name="setBucketRefererRequest">包含Referer白名单的<see cref="SetBucketRefererRequest"/>对象</param>
        void SetBucketReferer(SetBucketRefererRequest setBucketRefererRequest);

        /// <summary>
        /// 查看<see cref="Bucket" />的Referer配置。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns>Referer配置</returns>
        RefererConfiguration GetBucketReferer(string bucketName);

        /// <summary>
        /// 设置<see cref="Bucket" />的Lifecycle规则。
        /// </summary>
        /// <param name="setBucketLifecycleRequest">请求参数。</param>
        void SetBucketLifecycle(SetBucketLifecycleRequest setBucketLifecycleRequest);

        /// <summary>
        /// 查看<see cref="Bucket" />的Lifecycle规则列表。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <returns>Lifecycle规则列表。</returns>
        IList<LifecycleRule> GetBucketLifecycle(string bucketName);

        /// <summary>
        /// 判断指定的<see cref="Bucket" />是否存在。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns>
        /// 当Bucket存在且当前用户为该Bucket的拥有者，返回True；
        /// 当Bucket存在且当前用户不是该Bucket的拥有者，返回True；
        /// 否则返回False。
        /// </returns>
        bool DoesBucketExist(string bucketName);

        #endregion

        #region Object Operations

        /// <summary>
        /// 列出指定<see cref="Bucket" />下<see cref="OssObject" />的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        ObjectListing ListObjects(string bucketName);

        /// <summary>
        /// 开始异步列出指定<see cref="Bucket" />下<see cref="OssObject" />的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginListObjects(string bucketName, AsyncCallback callback, object state);

        /// <summary>
        /// 列出指定<see cref="Bucket" />下其Key以prefix为前缀<see cref="OssObject" />
        /// 的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="prefix">限定返回的<see cref="OssObject.Key" />必须以此作为前缀。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        ObjectListing ListObjects(string bucketName, string prefix);

        /// <summary>
        /// 开始异步列出指定<see cref="Bucket" />下其Key以prefix为前缀<see cref="OssObject" />
        /// 的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="prefix">限定返回的<see cref="OssObject.Key" />必须以此作为前缀。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginListObjects(string bucketName, string prefix, AsyncCallback callback, object state);

        /// <summary>
        /// 列出指定<see cref="Bucket" />下<see cref="OssObject" />的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="listObjectsRequest">请求参数。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        ObjectListing ListObjects(ListObjectsRequest listObjectsRequest);

        /// <summary>
        /// 开始异步列出指定<see cref="Bucket" />下<see cref="OssObject" />的摘要信息<see cref="OssObjectSummary" />。
        /// </summary>
        /// <param name="listObjectsRequest">请求参数。</param>
        /// <returns><see cref="OssObject" />的列表信息。</returns>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginListObjects(ListObjectsRequest listObjectsRequest, AsyncCallback callback, Object state);

        /// <summary>
        /// 等待挂起的异步列出指定<see cref="Bucket" />的摘要信息<see cref="OssObjectSummary" />操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns><see cref="ObjectListing"/>对象。</returns>
        ObjectListing EndListObjects(IAsyncResult asyncResult);


        /// <summary>
        /// 上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(string bucketName, string key, Stream content);

        /// <summary>
        /// 开始异步上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, Stream content,
            AsyncCallback callback, Object state);

        /// <summary>
        /// 上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(string bucketName, string key, Stream content, ObjectMetadata metadata);

        /// <summary>
        /// 开始异步上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, Stream content, ObjectMetadata metadata,
            AsyncCallback callback, Object state);

        /// <summary>
        /// 上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(string bucketName, string key, string fileToUpload);

        /// <summary>
        /// 开始异步上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload,
            AsyncCallback callback, Object state);

        /// <summary>
        /// 上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata);

        /// <summary>
        /// 开始异步上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata,
            AsyncCallback callback, Object state);

        /// <summary>
        ///  等待挂起的异步上传指定的<see cref="OssObject" />到指定的<see cref="Bucket" />操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns><see cref="PutObjectResult"/>对象。</returns>
        PutObjectResult EndPutObject(IAsyncResult asyncResult);

        /// <summary>
        /// 已废弃，请使用ResumableUploadObject。
        /// 上传指定的大文件：<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// 如果上传的文件大小小于或等于分片大小，则会使用普通上传，只需上传一次即可。
        /// 如果上传文件大小大于分片大小，则会使用分片上传。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>,
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        [Obsolete("PutBigObject is deprecated, please use ResumableUploadObject instead")]
        PutObjectResult PutBigObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, long? partSize = null);

        /// <summary>
        /// 已废弃，请使用ResumableUploadObject。
        /// 上传指定的大文件：<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// 如果上传的文件大小小于或等于分片大小，则会使用普通上传，只需上传一次即可。
        /// 如果上传文件大小大于分片大小，则会使用分片上传。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>,
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        [Obsolete("PutBigObject is deprecated, please use ResumableUploadObject instead")]
        PutObjectResult PutBigObject(string bucketName, string key, Stream content, ObjectMetadata metadata, long? partSize = null); 

        /// <summary>
        /// 使用URL签名方式上传指定文件。
        /// </summary>
        /// <param name="signedUrl">PUT请求类型的URL签名。</param>
        /// <param name="fileToUpload">上传文件的路径。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(Uri signedUrl, string fileToUpload);

        /// <summary>
        /// 使用URL签名方式上传指定输入流。
        /// </summary>
        /// <param name="signedUrl">PUT请求类型的URL签名。</param>
        /// <param name="content">请求输入流。</param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult PutObject(Uri signedUrl, Stream content);

        /// <summary>
        /// 自动分片后按片上传，支持断点续传。
        /// 如果上传的文件大小小于或等于分片大小，则会使用普通上传，只需上传一次即可。
        /// 如果上传文件大小大于分片大小，则会使用分片上传。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="fileToUpload">指定上传文件的路径。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="checkpointDir">保存断点续传中间状态文件的目录，如果指定了，则具有断点续传功能，否则每次都会重新上传</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult ResumableUploadObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, string checkpointDir, long? partSize = null);

        /// <summary>
        /// 自动分片后按片上传，支持断点续传。
        /// 如果上传的文件大小小于或等于分片大小，则会使用普通上传，只需上传一次即可。
        /// 如果上传文件大小大于分片大小，则会使用分片上传。
        /// </summary>
        /// <param name="bucketName">指定的<see cref="Bucket" />名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="OssObject.Key" />。</param>
        /// <param name="content"><see cref="OssObject" />的<see cref="OssObject.Content" />。</param>
        /// <param name="metadata"><see cref="OssObject" />的元信息。</param>
        /// <param name="checkpointDir">保存断点续传中间状态文件的目录，如果指定了，则具有断点续传功能，否则每次都会重新上传</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>,
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="PutObjectResult" />实例。</returns>
        PutObjectResult ResumableUploadObject(string bucketName, string key, Stream content, ObjectMetadata metadata, string checkpointDir, long? partSize = null);

        /// <summary>
        /// 追加指定的内容到指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="request"><see cref="AppendObjectRequest" />的实例</param>
        /// <returns><see cref="AppendObjectResult" />实例</returns>
        AppendObjectResult AppendObject(AppendObjectRequest request);

        /// <summary>
        /// 追加指定的内容到指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="request"><see cref="AppendObjectRequest" />的实例</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginAppendObject(AppendObjectRequest request, AsyncCallback callback, Object state);

        /// <summary>
        /// 等待挂起的异步追加<see cref="OssObject" />操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns><see cref="AppendObjectResult"/>对象。</returns>
        AppendObjectResult EndAppendObject(IAsyncResult asyncResult);

        /// <summary>
        /// 从指定的<see cref="Bucket" />中获取指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="bucketName">要获取的<see cref="OssObject"/>所在的<see cref="Bucket" />的名称。</param>
        /// <param name="key">要获取的<see cref="OssObject"/>的<see cref="OssObject.Key"/>。</param>
        /// <returns><see cref="OssObject" />实例。</returns>
        OssObject GetObject(string bucketName, string key);

        /// <summary>
        /// 使用URL签名方式获取指定的<see cref="OssObject"/>
        /// </summary>
        /// <param name="signedUrl">GET请求类型的URL签名</param>
        /// <returns><see cref="OssObject"/>实例。</returns>
        OssObject GetObject(Uri signedUrl);

        /// <summary>
        /// 从指定的<see cref="Bucket" />中获取满足请求参数<see cref="GetObjectRequest"/>的<see cref="OssObject" />。
        /// </summary>
        /// <param name="getObjectRequest">请求参数。</param>
        /// <returns><see cref="OssObject" />实例。使用后需要释放此对象以释放HTTP连接。</returns>
        OssObject GetObject(GetObjectRequest getObjectRequest);

        /// <summary>
        /// 开始从指定的<see cref="Bucket" />中异步获取满足请求参数<see cref="GetObjectRequest"/>的<see cref="OssObject" />。
        /// </summary>
        /// <param name="getObjectRequest">请求参数。</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginGetObject(GetObjectRequest getObjectRequest, AsyncCallback callback, Object state);

        /// <summary>
        /// 开始获取满足Bucket Name, Object Key条件的<see cref="OssObject"/>
        /// </summary>
        /// <param name="bucketName">获取的bucket的名称</param>
        /// <param name="key">获取的object的key</param>
        /// <param name="callback">用户自定义委托对象</param>
        /// <param name="state">用户自定义状态对象</param>
        /// <returns>异步请求的对象引用</returns>
        IAsyncResult BeginGetObject(string bucketName, string key, AsyncCallback callback, Object state);

        /// <summary>
        /// 等待挂起的异步获取<see cref="OssObject" />操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns><see cref="OssObject"/>对象。</returns>
        OssObject EndGetObject(IAsyncResult asyncResult);

        /// <summary>
        /// 从指定的<see cref="Bucket" />中获取指定的<see cref="OssObject" />，
        /// 并导出到指定的输出流。
        /// </summary>
        /// <param name="getObjectRequest">请求参数。</param>
        /// <param name="output">输出流。</param>
        /// <returns>导出<see cref="OssObject" />的元信息。</returns>
        ObjectMetadata GetObject(GetObjectRequest getObjectRequest, Stream output);

        /// <summary>
        /// 获取<see cref="OssObject" />的元信息。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        /// <returns><see cref="OssObject" />的元信息。</returns>
        ObjectMetadata GetObjectMetadata(string bucketName, string key);

        /// <summary>
        /// 删除指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        void DeleteObject(string bucketName, string key);

        /// <summary>
        /// 批量删除指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="deleteObjectsRequest">请求参数</param>
        /// <returns>返回结果</returns>
        DeleteObjectsResult DeleteObjects(DeleteObjectsRequest deleteObjectsRequest);
        
        /// <summary>
        /// 复制一个Object。
        /// </summary>
        /// <param name="copyObjectRequst">请求参数</param>
        /// <returns>返回的结果</returns>
        CopyObjectResult CopyObject(CopyObjectRequest copyObjectRequst);

        /// <summary>
        /// 开始异步复制一个Object。
        /// </summary>
        /// <param name="copyObjectRequst">请求参数</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginCopyObject(CopyObjectRequest copyObjectRequst, AsyncCallback callback, Object state);

        /// <summary>
        /// 等待挂起的异步复制指定的<see cref="OssObject" />操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns><see cref="CopyObjectResult"/>对象。</returns>
        CopyObjectResult EndCopyResult(IAsyncResult asyncResult);

        /// <summary>
        /// 已废弃，请使用ResumableCopyObject。
        /// 拷贝指定的大文件：<see cref="OssObject" />到指定的<see cref="Bucket" />。
        /// 如果拷贝的文件大小小于或等于分片大小，则会使用普通拷贝，只需拷贝一次即可。
        /// 如果拷贝文件大小大于分片大小，则会使用分片拷贝。
        /// </summary>
        /// <param name="copyObjectRequest">请求对象</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>。
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="CopyObjectResult" />实例。</returns>
        [Obsolete("CopyBigObject is deprecated, please use ResumableCopyObject instead")]
        CopyObjectResult CopyBigObject(CopyObjectRequest copyObjectRequest, long? partSize = null);

        /// <summary>
        /// 自动分片后按片拷贝，支持断点续传。
        /// 如果拷贝的文件大小小于或等于分片大小，则会使用普通拷贝，只需拷贝一次即可。
        /// 如果拷贝文件大小大于分片大小，则会使用分片拷贝。
        /// </summary>
        /// <param name="copyObjectRequest">请求对象</param>
        /// <param name="checkpointDir">保存断点续传中间状态文件的目录，如果指定了，则具有断点续传功能，否则每次都会重新拷贝</param>
        /// <param name="partSize">分片大小，如果用户不指定，则使用<see cref="Util.OssUtils.DefaultPartSize"/>。
        /// 如果用户指定的partSize小于<see cref="Util.OssUtils.PartSizeLowerLimit"/>，这会调整到<see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// </param>
        /// <returns><see cref="CopyObjectResult" />实例。</returns>
        CopyObjectResult ResumableCopyObject(CopyObjectRequest copyObjectRequest, string checkpointDir, long? partSize = null);

        /// <summary>
        /// 修改文件的元数据
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        /// <param name="newMeta">修改后的文件元数据</param>
        void ModifyObjectMeta(string bucketName, string key, ObjectMetadata newMeta);

        /// <summary>
        /// 判断指定的<see cref="Bucket"/>下是否存在指定的<see cref="OssObject" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        /// <returns>如果存在则返回True，否则返回False。</returns>
        bool DoesObjectExist(string bucketName, string key);

        /// <summary>
        /// 设置文件的访问控制权限<see cref="CannedAccessControlList" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        /// <param name="acl"><see cref="CannedAccessControlList" />枚举中的访问权限。</param>
        void SetObjectAcl(string bucketName, string key, CannedAccessControlList acl);

        /// <summary>
        /// 设置文件的访问控制权限<see cref="CannedAccessControlList" />。
        /// </summary>
        /// <param name="setObjectAclRequest"></param>
        void SetObjectAcl(SetObjectAclRequest setObjectAclRequest);

        /// <summary>
        /// 获取文件的访问控制权限<see cref="AccessControlList" />。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject.Key" />。</param>
        /// <returns>访问控制列表<see cref="AccessControlList" />的实例。</returns>
        AccessControlList GetObjectAcl(string bucketName, string key);

        #endregion
        
        #region Generate URL
        
        /// <summary>
        /// 生成一个签名的Uri。
        /// </summary>
        /// <param name="generatePresignedUriRequest">请求参数</param>
        /// <returns>访问<see cref="OssObject" />的Uri。</returns>
        Uri GeneratePresignedUri(GeneratePresignedUriRequest generatePresignedUriRequest);
        
        /// <summary>
        /// 使用默认过期时间（自现在起15分钟后）生成一个用HTTP GET方法访问<see cref="OssObject" />的Uri。
        /// </summary>
        /// <param name="bucketName">Bucket名称。</param>
        /// <param name="key">Object的Key</param>
        /// <returns>访问<see cref="OssObject" />的Uri。</returns>
        Uri GeneratePresignedUri(string bucketName, string key);

        /// <summary>
        /// 使用指定过期时间生成一个用HTTP GET方法访问<see cref="OssObject" />的Uri。
        /// </summary>
        /// <param name="bucketName">Bucket名称。</param>
        /// <param name="key">Object的Key</param>
        /// <param name="expiration">Uri的超时时间。</param>
        /// <returns>访问<see cref="OssObject" />的Uri。</returns>
        Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration);
        
        
        /// <summary>
        /// 使用默认过期时间（自现在起15分钟后）生成一个用指定方法访问<see cref="OssObject" />的Uri。
        /// </summary>
        /// <param name="bucketName">Bucket名称。</param>
        /// <param name="key">Object的Key</param>
        /// <param name="method">访问Uri的方法</param>
        /// <returns>访问<see cref="OssObject" />的Uri。</returns>
        Uri GeneratePresignedUri(string bucketName, string key, SignHttpMethod method);

        /// <summary>
        /// 使用指定过期时间生成一个用指定方法访问<see cref="OssObject" />的Uri。
        /// </summary>
        /// <param name="bucketName">Bucket名称。</param>
        /// <param name="key">Object的Key</param>
        /// <param name="expiration">Uri的超时时间。</param>
        /// <param name="method">访问Uri的方法</param>
        /// <returns>访问<see cref="OssObject" />的Uri。</returns>
        Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration, SignHttpMethod method);
        
        #endregion

        #region Generate Post Policy

        /// <summary>
        /// 生成Post请求的policy表单域。
        /// </summary>
        /// <param name="expiration">policy过期时间。</param>
        /// <param name="conds">policy条件列表。</param>
        /// <returns>policy字符串。</returns>
        string GeneratePostPolicy(DateTime expiration, PolicyConditions conds);

        #endregion

        #region Multipart Operations
        /// <summary>
        /// 列出所有执行中的Multipart Upload事件
        /// </summary>
        /// <param name="listMultipartUploadsRequest">请求参数</param>
        /// <returns><see cref="MultipartUploadListing" />的列表信息。</returns>
        MultipartUploadListing ListMultipartUploads(ListMultipartUploadsRequest listMultipartUploadsRequest);
        
        /// <summary>
        /// 初始化一个Multipart Upload事件
        /// </summary>
        /// <param name="initiateMultipartUploadRequest">请求参数</param>
        /// <returns>初始化结果</returns>
        InitiateMultipartUploadResult InitiateMultipartUpload(InitiateMultipartUploadRequest initiateMultipartUploadRequest);
        
        /// <summary>
        /// 中止一个Multipart Upload事件
        /// </summary>
        /// <param name="abortMultipartUploadRequest">请求参数</param>
        void AbortMultipartUpload(AbortMultipartUploadRequest abortMultipartUploadRequest);
        
        /// <summary>
        /// 上传某分块的数据
        /// </summary>
        /// <param name="uploadPartRequest">请求参数</param>
        /// <returns>分块上传结果</returns>
        UploadPartResult UploadPart(UploadPartRequest uploadPartRequest);

        /// <summary>
        /// 开始异步上传某分块的数据。
        /// </summary>
        /// <param name="uploadPartRequest">请求参数</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginUploadPart(UploadPartRequest uploadPartRequest, AsyncCallback callback, object state);

        /// <summary>
        /// 等待挂起的异步上传某分块的数据操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns>分块上传结果</returns>
        UploadPartResult EndUploadPart(IAsyncResult asyncResult);


        /// <summary>
        /// 从某一已存在的Object中拷贝数据来上传某分块。
        /// </summary>
        /// <param name="uploadPartCopyRequest">请求参数</param>
        /// <returns>分块上传结果。</returns>
        UploadPartCopyResult UploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest);

        /// <summary>
        /// 开始以某一已存在的Object中异步拷贝数据来上传某分块。
        /// </summary>
        /// <param name="uploadPartCopyRequest">请求参数</param>
        /// <param name="callback">用户自定义委托对象。</param>
        /// <param name="state">用户自定义状态对象。</param>
        /// <returns>异步请求的对象引用。</returns>
        IAsyncResult BeginUploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest, AsyncCallback callback, object state);

        /// <summary>
        /// 等待挂起的异步拷贝某分块的数据操作的完成。
        /// </summary>
        /// <param name="asyncResult">对所等待的挂起异步请求的引用。</param>
        /// <returns>分块上传结果。</returns>
        UploadPartCopyResult EndUploadPartCopy(IAsyncResult asyncResult);


        /// <summary>
        /// 列出已经上传成功的Part
        /// </summary>
        /// <param name="listPartsRequest">请求参数</param>
        /// <returns><see cref="PartListing" />的列表信息。</returns>
        PartListing ListParts(ListPartsRequest listPartsRequest);
 
        /// <summary>
        /// 完成分块上传
        /// </summary>
        /// <param name="completeMultipartUploadRequest">请求参数</param>
        /// <returns><see cref="CompleteMultipartUploadResult" />的列表信息。</returns>        
        CompleteMultipartUploadResult CompleteMultipartUpload(CompleteMultipartUploadRequest completeMultipartUploadRequest);
        
        #endregion
    }
}
