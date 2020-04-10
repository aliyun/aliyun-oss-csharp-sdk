/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Model;

namespace Aliyun.OSS 
{
    /// <summary>
    /// The Object Storage Service (OSS) entry point interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OSS is the highly scalable, secure, inexpensive and reliable cloud storage service.
    /// This interface is to access all the functionality OSS provides.
    /// The same functionality could be done in web console.
    /// Multimedia sharing web app, network disk, or enterprise data backup app could be easily built based on OSS.
    /// </para>
    /// <para>
    /// OSS website：http://www.aliyun.com/product/oss
    /// </para>
    /// </remarks>
    public interface IOss
    {
        #region Switch Credentials & Endpoint

        /// <summary>
        /// Switches the user credentials
        /// </summary>
        /// <param name="creds">The credential instance</param>
        void SwitchCredentials(ICredentials creds);

        /// <summary>
        /// Sets the endpoint
        /// </summary>
        /// <param name="endpoint">Endpoint value</param>
        void SetEndpoint(Uri endpoint);

        #endregion

        #region Bucket Operations

        /// <summary>
        /// Creates a new bucket
        /// </summary>
        /// <param name="bucketName">The bucket name. It must be globably unique.</param>
        /// <returns><see cref="Bucket" /> instance</returns>
        Bucket CreateBucket(string bucketName);

        /// <summary>
        /// Creates the bucket with specified storage class.
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="storageClass">Storage class.</param>
        Bucket CreateBucket(string bucketName, StorageClass? storageClass);

        /// <summary>
        /// Creates a bucket
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="createBucketRequest"><see cref="CreateBucketRequest"/></param>
        Bucket CreateBucket(CreateBucketRequest createBucketRequest);

        /// <summary>
        /// Deletes a empty bucket.If the bucket is not empty, this will fail.
        /// </summary>
        /// <param name="bucketName">The bucket name to delete</param>
        void DeleteBucket(string bucketName);

        /// <summary>
        /// List all buckets under the current account.
        /// </summary>
        /// <returns>All <see cref="Bucket" /> instances</returns>
        IEnumerable<Bucket> ListBuckets();

        /// <summary>
        /// Lists all buckets according to the ListBucketsRequest, which could have filters by prefix, marker, etc.
        /// </summary>
        /// <param name="listBucketsRequest"><see cref="ListBucketsRequest"/> instance</param>
        /// <returns><see cref="ListBucketsResult" /> instance</returns>
        ListBucketsResult ListBuckets(ListBucketsRequest listBucketsRequest);

        /// <summary>
        /// Gets the bucket information.
        /// </summary>
        /// <returns>The bucket information.</returns>
        /// <param name="bucketName">Bucket name.</param>
        BucketInfo GetBucketInfo(string bucketName);

        /// <summary>
        /// Gets the bucket stat.
        /// </summary>
        /// <returns>The bucket stat.</returns>
        /// <param name="bucketName">Bucket name.</param>
        BucketStat GetBucketStat(string bucketName);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="acl"><see cref="CannedAccessControlList" /> instance</param>
        void SetBucketAcl(string bucketName, CannedAccessControlList acl);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="setBucketAclRequest"></param>
        void SetBucketAcl(SetBucketAclRequest setBucketAclRequest);

        /// <summary>
        /// Gets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Bucket ACL<see cref="AccessControlList" /> instance</returns>
        AccessControlList GetBucketAcl(string bucketName);

        /// <summary>
        /// Gets the bucket location
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>bucket location</returns>
        BucketLocationResult GetBucketLocation(string bucketName);

        /// <summary>
        /// Gets the bucket metadata
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns><see cref="BucketMetadata" />metadata</returns>
        BucketMetadata GetBucketMetadata(string bucketName);

        /// <summary>
        /// Sets the CORS rules for the <see cref="Bucket" />
        /// </summary>
        /// <param name="setBucketCorsRequest"></param>
        void SetBucketCors(SetBucketCorsRequest setBucketCorsRequest);

        /// <summary>
        /// Gets the <see cref="Bucket" /> CORS rules.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>CORS rules</returns>
        IList<CORSRule> GetBucketCors(string bucketName);

        /// <summary>
        /// Deletes the CORS rules on the <see cref="Bucket" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        void DeleteBucketCors(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> logging config
        /// OSS will log the access information on this bucket, according to the logging config
        /// The hourly log file will be stored in the target bucket.
        /// </summary>
        /// <param name="setBucketLoggingRequest"></param>
        void SetBucketLogging(SetBucketLoggingRequest setBucketLoggingRequest);

        /// <summary>
        /// Gets the bucket logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>The logging config result</returns>
        BucketLoggingResult GetBucketLogging(string bucketName);
        
        /// <summary>
        /// Deletes the <see cref="Bucket" /> logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        void DeleteBucketLogging(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="setBucketWebSiteRequest"><see cref="SetBucketWebsiteRequest"/> instance</param>
        void SetBucketWebsite(SetBucketWebsiteRequest setBucketWebSiteRequest);
        

        /// <summary>
        /// Gets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketWebsiteResult"/> instance</returns>
        BucketWebsiteResult GetBucketWebsite(string bucketName);


        /// <summary>
        /// Deletes the <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        void DeleteBucketWebsite(string bucketName);


        /// <summary>
        /// Sets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="setBucketRefererRequest">The requests that contains the Referer whitelist</param>
        void SetBucketReferer(SetBucketRefererRequest setBucketRefererRequest);

        /// <summary>
        /// Gets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Referer config</returns>
        RefererConfiguration GetBucketReferer(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> lifecycle rule
        /// </summary>
        /// <param name="setBucketLifecycleRequest">the <see cref="SetBucketLifecycleRequest" /> instance</param>
        void SetBucketLifecycle(SetBucketLifecycleRequest setBucketLifecycleRequest);

        /// <summary>
        /// Deletes the bucket's all lifecycle rules.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        void DeleteBucketLifecycle(string bucketName);

        /// <summary>
        /// Gets <see cref="Bucket" /> lifecycle instance. 
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>Lifecycle list</returns>
        IList<LifecycleRule> GetBucketLifecycle(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="setBucketStorageCapacityRequest"><see cref="SetBucketStorageCapacityRequest"/> instance</param>
        void SetBucketStorageCapacity(SetBucketStorageCapacityRequest setBucketStorageCapacityRequest);

        /// <summary>
        /// Gets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketStorageCapacityResult"/> instance</returns>
        GetBucketStorageCapacityResult GetBucketStorageCapacity(string bucketName);

        /// <summary>
        /// Checks if the bucket exists
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>
        /// True when the bucket exists under the current user;
        /// Otherwise returns false.
        /// </returns>
        bool DoesBucketExist(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="setBucketPolicyRequest"><see cref="SetBucketPolicyRequest"/> instance</param>
        void SetBucketPolicy(SetBucketPolicyRequest setBucketPolicyRequest);

        /// <summary>
        /// Gets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketPolicyResult"/> instance</returns>
        GetBucketPolicyResult GetBucketPolicy(string bucketName);

        /// <summary>
        /// Deletes <see cref="Bucket" /> policy.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        void DeleteBucketPolicy(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="setBucketTaggingRequest"><see cref="SetBucketTaggingRequest"/> instance</param>
        void SetBucketTagging(SetBucketTaggingRequest setBucketTaggingRequest);

        /// <summary>
        /// Deletes the bucket's tagging.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        void DeleteBucketTagging(string bucketName);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketTaggingResult"/> instance</returns>
        GetBucketTaggingResult GetBucketTagging(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="setBucketRequestPaymentRequest"><see cref="SetBucketRequestPaymentRequest"/> instance</param>
        void SetBucketRequestPayment(SetBucketRequestPaymentRequest setBucketRequestPaymentRequest);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketRequestPaymentResult"/></returns>
        GetBucketRequestPaymentResult GetBucketRequestPayment(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="setBucketEncryptionRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        void SetBucketEncryption(SetBucketEncryptionRequest setBucketEncryptionRequest);

        /// <summary>
        /// Deletes bucket encryption rule
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        void DeleteBucketEncryption(string bucketName);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketEncryptionResult"/> instance</returns>
        BucketEncryptionResult GetBucketEncryption(string bucketName);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="setBucketVersioningRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        void SetBucketVersioning(SetBucketVersioningRequest setBucketVersioningRequest);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketVersioningResult"/> instance</returns>
        GetBucketVersioningResult GetBucketVersioning(string bucketName);

        #endregion

        #region Object Operations

        /// <summary>
        /// Lists all objects under the <see cref="Bucket" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="OssObject" /> list</returns>
        ObjectListing ListObjects(string bucketName);

        /// <summary>
        /// Begins the async call to list objects.The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="OssObject" />list</returns>
        /// <param name="callback">callback when the list is done</param>
        /// <param name="state">state object in the callback</param>
        /// <returns>IAsyncResult instance.</returns>
        IAsyncResult BeginListObjects(string bucketName, AsyncCallback callback, object state);

        /// <summary>
        /// Lists object with specified prefix
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="prefix"><see cref="OssObject.Key" /> prefix</param>
        /// <returns><see cref="OssObject" /> instances list</returns>
        ObjectListing ListObjects(string bucketName, string prefix);

        /// <summary>
        /// Begins the async call to list objects under the specified bucket and prefix
        /// The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="prefix"><see cref="OssObject.Key" /> prefix</param>
        /// <returns><see cref="OssObject" /> list</returns>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance</returns>
        IAsyncResult BeginListObjects(string bucketName, string prefix, AsyncCallback callback, object state);

        /// <summary>
        /// Lists objects according to the ListObjectsRequest.
        /// The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="listObjectsRequest"><see cref="ListObjectsRequest" /> instance</param>
        /// <returns><see cref="OssObject" /> list</returns>
        ObjectListing ListObjects(ListObjectsRequest listObjectsRequest);

        /// <summary>
        /// Begins the async call to list objects under the specified <see cref="Bucket" /> with specified filters in <see cref="ListObjectsRequest" />
        /// </summary>
        /// <param name="listObjectsRequest"><see cref="ListObjectsRequest"/> instance</param>
        /// <returns><see cref="OssObject" /> list</returns>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state object</param>
        IAsyncResult BeginListObjects(ListObjectsRequest listObjectsRequest, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the async call of listing objects.
        /// </summary>
        /// <param name="asyncResult">The asyncResult instance returned by BeginListObjects call</param>
        /// <returns><see cref="ObjectListing"/> instance</returns>
        ObjectListing EndListObjects(IAsyncResult asyncResult);

        /// <summary>
        /// Lists object vesions according to the ListObjectVersionsRequest.
        /// The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="listObjectVersionsRequest"><see cref="ListObjectVersionsRequest" /> instance</param>
        /// <returns><see cref="OssObject" /> list</returns>
        ObjectVersionList ListObjectVersions(ListObjectVersionsRequest listObjectVersionsRequest);

        /// <summary>
        /// Puts object to the specified bucket with specified object key.
        /// </summary>
        /// <param name="bucketName">specified bucket name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(string bucketName, string key, Stream content);

        /// <summary>
        /// Begins the async call of uploading object to specified bucket.
        /// </summary>
        /// <param name="bucketName">target <see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>The IAsyncResult instance for EndPutObject()</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, Stream content,
            AsyncCallback callback, Object state);

        /// <summary>
        /// Uploads the content to object under the specified bucket and object key.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(string bucketName, string key, Stream content, ObjectMetadata metadata);

        /// <summary>
        /// Upload a <see cref="OssObject" /> according to <see cref="PutObjectRequest" />.
        /// </summary>
        /// <param name="putObjectRequest"><see cref="PutObjectRequest" />instance</param>
        /// <returns><see cref="PutObjectResult" />instance</returns>
        PutObjectResult PutObject(PutObjectRequest putObjectRequest);

        /// <summary>
        /// Begins the async call to upload object
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance for EndPutObject()</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, Stream content, ObjectMetadata metadata,
            AsyncCallback callback, Object state);

        /// <summary>
        /// Begins the async call to upload object
        /// </summary>
        /// <param name="putObjectRequest"><see cref="PutObjectRequest" /> instance</param>
        /// <param name="callback">callback object</param>
        /// <param name="state">state object</param>
        /// <returns>IAsyncResult instance for EndPutObject()</returns>
        IAsyncResult BeginPutObject(PutObjectRequest putObjectRequest, AsyncCallback callback, object state);

        /// <summary>
        /// Uploads a local file to OSS under the specified bucket
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file path to upload</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(string bucketName, string key, string fileToUpload);

        /// <summary>
        /// Begins the async call to upload local file to OSS under the specified bucket.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file path to upload</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAyncResult instance</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload,
            AsyncCallback callback, Object state);

        /// <summary>
        /// Uploads a local file with specified metadata to OSS.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file path</param>
        /// <param name="metadata"><see cref="OssObject" />metadata</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata);

        /// <summary>
        /// Begins the async call to upload object with specified metadata.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file to upload</param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance for EndPutObject</returns>
        IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata,
            AsyncCallback callback, Object state);

        /// <summary>
        ///  Ends the async call to upload the object.
        ///  When it's called, the actual upload has already been done.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult instance</param>
        /// <returns><see cref="PutObjectResult"/> instance</returns>
        PutObjectResult EndPutObject(IAsyncResult asyncResult);

        /// <summary>
        /// Deprecated method.Please use ResumableUploadObject.
        /// Uploads the specified file with optional part size.
        /// If the file size is not bigger than the part size, then use normal file upload.
        /// Otherwise use multipart upload.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">target object key</param>
        /// <param name="fileToUpload">local file path to upload</param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="partSize">Part size.If it's not specified, then use <see cref="Util.OssUtils.DefaultPartSize"/>.
        /// If the part size is less than <see cref="Util.OssUtils.PartSizeLowerLimit"/>, it will be changed to <see cref="Util.OssUtils.PartSizeLowerLimit"/> automatically.
        /// </param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        [Obsolete("PutBigObject is deprecated, please use ResumableUploadObject instead")]
        PutObjectResult PutBigObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, long? partSize = null);

        /// <summary>
        /// Deprecated method. Use ResumableUploadObject instead.
        /// Upload the specified file to OSS.
        /// If the file size is same or less than the part size, use normal file upload instead.
        /// Otherwise it will use multipart file upload.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="partSize">Part size. If it's not specified or the value is less than <see cref="Util.OssUtils.PartSizeLowerLimit"/>, 
        /// then use <see cref="Util.OssUtils.DefaultPartSize"/> instead.
        /// </param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        [Obsolete("PutBigObject is deprecated, please use ResumableUploadObject instead")]
        PutObjectResult PutBigObject(string bucketName, string key, Stream content, ObjectMetadata metadata, long? partSize = null); 

        /// <summary>
        /// Uploads the file via the signed url.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="fileToUpload">File to upload</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(Uri signedUrl, string fileToUpload);

        /// <summary>
        /// Uploads the instream via the signed url.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="content">content stream</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult PutObject(Uri signedUrl, Stream content);

        /// <summary>
        /// Uploads the file via the signed url with the metadata.
        /// </summary>
        /// <param name="signedUrl">The signed url</param>
        /// <param name="fileToUpload">Local file path</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        PutObjectResult PutObject(Uri signedUrl, string fileToUpload, ObjectMetadata metadata);

        /// <summary>
        /// Uploads the stream via the signed url with the metadata.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="content">content stream</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        PutObjectResult PutObject(Uri signedUrl, Stream content, ObjectMetadata metadata);

        /// <summary>
        /// Resumable file upload. It automaticlly uses multipart upload upon big file and also support resume upload after a failed upload.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> instance</param>
        /// <param name="key"><see cref="OssObject.Key" /> instance</param>
        /// <param name="fileToUpload">file to upload</param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="checkpointDir">Check point dir. If it's not specified, then no checkpoint file is saved and thus resumable file upload is not supported.</param>
        /// <param name="partSize">Part size. If it's not specified, or the size is smaller than <see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// then <see cref="Util.OssUtils.DefaultPartSize"/> is used instead.
        /// </param>
        /// <returns><see cref="PutObjectResult" /> instance </returns>
        PutObjectResult ResumableUploadObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                              EventHandler<StreamTransferProgressArgs> streamTransferProgress = null);

        /// <summary>
        /// Resumable file upload. It automaticlly uses multipart upload upon big file and also support resume upload after a failed upload.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" />. Content is disposed after the call finishes.</param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <param name="checkpointDir">Check point dir. If it's not specified, then no checkpoint file is saved and thus resumable file upload is not supported.</param>
        /// <param name="partSize">Part size. If it's not specified, or the size is smaller than <see cref="Util.OssUtils.PartSizeLowerLimit"/>
        /// then <see cref="Util.OssUtils.DefaultPartSize"/> is used instead.
        /// </param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        PutObjectResult ResumableUploadObject(string bucketName, string key, Stream content, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                              EventHandler<StreamTransferProgressArgs> streamTransferProgress = null);

        /// <summary>
        /// Resumables the upload object.
        /// The request.UploadStream will be disposed once the call finishes.
        /// </summary>
        /// <returns>The upload object.</returns>
        /// <param name="request">Upload Request.</param>
        PutObjectResult ResumableUploadObject(UploadObjectRequest request);

        /// <summary>
        /// Appends object to OSS according to the <see cref="AppendObjectRequest" />
        /// </summary>
        /// <param name="request"><see cref="AppendObjectRequest" /> instance</param>
        /// <returns><see cref="AppendObjectResult" /> result</returns>
        AppendObjectResult AppendObject(AppendObjectRequest request);

        /// <summary>
        /// Begins the async call to append object to OSS.
        /// </summary>
        /// <param name="request"><see cref="AppendObjectRequest" /> instance</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">state object</param>
        /// <returns>IAsyncResut instance for EndAppendObject call</returns>
        IAsyncResult BeginAppendObject(AppendObjectRequest request, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the async call to append object to OSS. WHen it's called, the actual upload has been done.
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult instance returned from BeginAppendObjet</param>
        /// <returns><see cref="AppendObjectResult"/> instance</returns>
        AppendObjectResult EndAppendObject(IAsyncResult asyncResult);

        /// <summary>
        /// Creates the symlink of the target object
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="symlink">Symlink.</param>
        /// <param name="target">Target.</param>
        /// <returns><see cref="CreateSymlinkResult"/> instance</returns>
        CreateSymlinkResult CreateSymlink(string bucketName, string symlink, string target);

        /// <summary>
        /// Creates the symlink of the target object
        /// </summary>
        /// <param name="createSymlinkRequest">Create symlink request.</param>
        /// <returns><see cref="CreateSymlinkResult"/> instance</returns>
        CreateSymlinkResult CreateSymlink(CreateSymlinkRequest createSymlinkRequest);

        /// <summary>
        /// Gets the target file of the symlink.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="symlink">Symlink </param>
        /// <returns>OssSymlink object</returns>
        OssSymlink GetSymlink(string bucketName, string symlink);

        /// <summary>
        /// Gets the target file of the symlink.
        /// </summary>
        /// <param name="getSymlinkRequest">Get symlink request.</param>
        /// <returns>OssSymlink object</returns>
        OssSymlink GetSymlink(GetSymlinkRequest getSymlinkRequest);

        /// <summary>
        /// Gets object
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key"><see cref="OssObject.Key"/></param>
        /// <returns><see cref="OssObject" /> instance</returns>
        OssObject GetObject(string bucketName, string key);

        /// <summary>
        /// Gets object via signed url
        /// </summary>
        /// <param name="signedUrl">The signed url of HTTP GET method</param>
        /// <returns><see cref="OssObject"/> instance</returns>
        OssObject GetObject(Uri signedUrl);

        /// <summary>
        /// Gets object via the bucket name and key name in the <see cref="GetObjectRequest" /> instance.
        /// </summary>
        /// <param name="getObjectRequest"> The request parameter</param>
        /// <returns><see cref="OssObject" /> instance. The caller needs to dispose the object.</returns>
        OssObject GetObject(GetObjectRequest getObjectRequest);

        /// <summary>
        /// Begins the async call to get object according to the <see cref="GetObjectRequest"/> instance.
        /// </summary>
        /// <param name="getObjectRequest"> request parameter</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance for EndGetObject()</returns>
        IAsyncResult BeginGetObject(GetObjectRequest getObjectRequest, AsyncCallback callback, Object state);

        /// <summary>
        /// Begins the async call to get object by the bucket and key information.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">state instance</param>
        /// <returns>ISyncResult instance</returns>
        IAsyncResult BeginGetObject(string bucketName, string key, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the async call to get the object.
        /// </summary>
        /// <param name="asyncResult">The AsyncResult instance returned from BeginGetObject()</param>
        /// <returns><see cref="OssObject"/> instance</returns>
        OssObject EndGetObject(IAsyncResult asyncResult);

        /// <summary>
        /// Gets the object and assign the data to the stream.
        /// </summary>
        /// <param name="getObjectRequest">request parameter</param>
        /// <param name="output">output stream</param>
        /// <returns><see cref="OssObject" /> metadata</returns>
        ObjectMetadata GetObject(GetObjectRequest getObjectRequest, Stream output);

        /// <summary>
        /// Download a file.
        /// Internally it may use multipart download in case the file is big
        /// </summary>
        /// <returns>The metadata object</returns>
        /// <param name="request">DownloadObjectRequest instance</param>
        ObjectMetadata ResumableDownloadObject(DownloadObjectRequest request);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="OssObject" />metadata</returns>
        ObjectMetadata GetObjectMetadata(string bucketName, string key);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="request">GetObjectMetadataRequest instance</param>
        /// <returns><see cref="OssObject" />metadata</returns>
        ObjectMetadata GetObjectMetadata(GetObjectMetadataRequest request);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="request">GetObjectMetadataRequest instance</param>
        /// <returns><see cref="OssObject" />metadata</returns>
        ObjectMetadata GetSimplifiedObjectMetadata(GetObjectMetadataRequest request);

        /// <summary>
        /// Deletes <see cref="OssObject" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="DeleteObjectResult" />instance</returns>
        DeleteObjectResult DeleteObject(string bucketName, string key);

        /// <summary>
        /// Deletes <see cref="OssObject" />
        /// </summary>
        /// <param name="deleteObjectRequest">the request parameter</param>
        /// <returns><see cref="DeleteObjectResult" />instance</returns>
        DeleteObjectResult DeleteObject(DeleteObjectRequest deleteObjectRequest);

        /// <summary>
        /// Deletes multiple objects
        /// </summary>
        /// <param name="deleteObjectsRequest">the request parameter</param>
        /// <returns>delete object result</returns>
        DeleteObjectsResult DeleteObjects(DeleteObjectsRequest deleteObjectsRequest);

        /// <summary>
        /// Deletes multiple objects with version id
        /// </summary>
        /// <param name="deleteObjectVersionsRequest">the request parameter</param>
        /// <returns>delete object result</returns>
        DeleteObjectVersionsResult DeleteObjectVersions(DeleteObjectVersionsRequest deleteObjectVersionsRequest);

        /// <summary>
        /// copy an object to another one in OSS.
        /// </summary>
        /// <param name="copyObjectRequst">The request parameter</param>
        /// <returns>copy object result</returns>
        CopyObjectResult CopyObject(CopyObjectRequest copyObjectRequst);

        /// <summary>
        /// Begins the async call to copy an object
        /// </summary>
        /// <param name="copyObjectRequst">the request parameter</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>The IAsyncResult instance for EndCopyObject()</returns>
        IAsyncResult BeginCopyObject(CopyObjectRequest copyObjectRequst, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the async call to copy an object.
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult instance returned from BeginCopyObject()</param>
        /// <returns><see cref="CopyObjectResult"/> instance</returns>
        CopyObjectResult EndCopyResult(IAsyncResult asyncResult);

        /// <summary>
        /// Deprecated. Use ResumableCopyObject instead.
        /// Copy the specified file with optional checkpoint support.
        /// </summary>
        /// <param name="copyObjectRequest">the request parameter</param>
        /// <param name="partSize">part size. If the part size is not specified, or less than <see cref="Util.OssUtils.DefaultPartSize"/>,
        /// <see cref="Util.OssUtils.PartSizeLowerLimit"/> will be used instead.
        /// </param>
        /// <param name="checkpointDir">The checkpoint file folder. If it's not specified, checkpoint information is not stored and resumnable upload will not be supported in this case.</param>
        /// <returns><see cref="CopyObjectResult" /> instance.</returns>
        [Obsolete("CopyBigObject is deprecated, please use ResumableCopyObject instead")]
        CopyObjectResult CopyBigObject(CopyObjectRequest copyObjectRequest, long? partSize = null, string checkpointDir = null);

        /// <summary>
        /// Resumable object copy.
        /// If the file size is less than part size, normal file upload is used; otherwise multipart upload is used.
        /// </summary>
        /// <param name="copyObjectRequest">request parameter</param>
        /// <param name="checkpointDir">checkpoint file folder </param>
        /// <param name="partSize">The part size. 
        /// </param>
        /// <returns><see cref="CopyObjectResult" /> instance</returns>
        CopyObjectResult ResumableCopyObject(CopyObjectRequest copyObjectRequest, string checkpointDir, long? partSize = null);

        /// <summary>
        /// Modify the object metadata. 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="newMeta">new metadata</param>
        /// <param name="checkpointDir">check point folder. It must be specified to store the checkpoint information</param>
        /// <param name="partSize">Part size, it's no less than <see cref="Util.OssUtils.DefaultPartSize"/>
        /// </param>
        void ModifyObjectMeta(string bucketName, string key, ObjectMetadata newMeta, long? partSize = null, string checkpointDir = null);

        /// <summary>
        /// Checks if the object exists
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns>true:object exists;false:otherwise</returns>
        bool DoesObjectExist(string bucketName, string key);

        /// <summary>
        /// Sets the object ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /> key</param>
        /// <param name="acl"><see cref="CannedAccessControlList" /> instance</param>
        void SetObjectAcl(string bucketName, string key, CannedAccessControlList acl);

        /// <summary>
        /// Sets the object ACL
        /// </summary>
        /// <param name="setObjectAclRequest"></param>
        void SetObjectAcl(SetObjectAclRequest setObjectAclRequest);

        /// <summary>
        /// Gets the object ACL 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="AccessControlList" /> instance</returns>
        AccessControlList GetObjectAcl(string bucketName, string key);

        /// <summary>
        /// Gets the object ACL
        /// </summary>
        /// <param name="getObjectAclRequest"></param>
        AccessControlList GetObjectAcl(GetObjectAclRequest getObjectAclRequest);

        /// <summary>
        /// Restores the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        RestoreObjectResult RestoreObject(string bucketName, string key);

        /// <summary>
        /// Restores the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="restoreObjectRequest"></param>
        RestoreObjectResult RestoreObject(RestoreObjectRequest restoreObjectRequest);

        /// <summary>
        /// Sets the object tagging
        /// </summary>
        /// <param name="request"><see cref="SetObjectTaggingRequest" /> instance</param>
        void SetObjectTagging(SetObjectTaggingRequest request);

        /// <summary>
        /// Gets the object tagging 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="GetObjectTaggingResult" /> instance</returns>
        GetObjectTaggingResult GetObjectTagging(string bucketName, string key);

        /// <summary>
        /// Gets the object tagging
        /// </summary>
        /// <param name="request"><see cref="GetObjectTaggingRequest" /> instance</param>
        /// <returns><see cref="GetObjectTaggingResult" /> instance</returns>
        GetObjectTaggingResult GetObjectTagging(GetObjectTaggingRequest request);

        /// <summary>
        /// Deletes object tagging
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        void DeleteObjectTagging(string bucketName, string key);

        /// <summary>
        /// Deletes the object tagging
        /// </summary>
        /// <param name="request"><see cref="DeleteObjectTaggingRequest" /> instance</param>
        void DeleteObjectTagging(DeleteObjectTaggingRequest request);

        #endregion

        #region Generate URL

        /// <summary>
        /// Generates a signed url
        /// </summary>
        /// <param name="generatePresignedUriRequest">request parameter</param>
        /// <returns>The signed url. The user could use this url to access the object directly</returns>
        Uri GeneratePresignedUri(GeneratePresignedUriRequest generatePresignedUriRequest);
        
        /// <summary>
        /// Generates the signed url with default expiration time (15 min) that supports HTTP GET method.
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="key">Object key</param>
        /// <returns>Signed uri</returns>
        Uri GeneratePresignedUri(string bucketName, string key);

        /// <summary>
        /// Generates the pre-signed GET url with specified expiration time
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="expiration">Uri expiration time</param>
        /// <returns>signed url</returns>
        Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration);
        
        
        /// <summary>
        /// Generates the pre-signed url with specified expiration time that supports the specified HTTP method
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="method">HTTP method</param>
        /// <returns>signed url</returns>
        Uri GeneratePresignedUri(string bucketName, string key, SignHttpMethod method);

        /// <summary>
        /// Generates the presigned url with specified method and specified expiration time.
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="expiration">Uri expiration time</param>
        /// <param name="method">HTTP method</param>
        /// <returns>signed url</returns>
        Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration, SignHttpMethod method);
        
        #endregion

        #region Generate Post Policy

        /// <summary>
        /// Generates the post policy
        /// </summary>
        /// <param name="expiration">policy expiration time</param>
        /// <param name="conds">policy conditions</param>
        /// <returns>policy string</returns>
        string GeneratePostPolicy(DateTime expiration, PolicyConditions conds);

        #endregion

        #region Multipart Operations
        /// <summary>
        /// Lists ongoing multipart uploads 
        /// </summary>
        /// <param name="listMultipartUploadsRequest">request parameter</param>
        /// <returns><see cref="MultipartUploadListing" /> instance</returns>
        MultipartUploadListing ListMultipartUploads(ListMultipartUploadsRequest listMultipartUploadsRequest);
        
        /// <summary>
        /// Initiate a multipart upload
        /// </summary>
        /// <param name="initiateMultipartUploadRequest">request parameter</param>
        /// <returns><see cref="InitiateMultipartUploadResult"/> instance</returns>
        InitiateMultipartUploadResult InitiateMultipartUpload(InitiateMultipartUploadRequest initiateMultipartUploadRequest);
        
        /// <summary>
        /// Aborts a multipart upload
        /// </summary>
        /// <param name="abortMultipartUploadRequest">request parameter</param>
        void AbortMultipartUpload(AbortMultipartUploadRequest abortMultipartUploadRequest);
        
        /// <summary>
        /// Uploads a part
        /// </summary>
        /// <param name="uploadPartRequest">request parameter</param>
        /// <returns><see cref="UploadPartResult" /> instance</returns>
        UploadPartResult UploadPart(UploadPartRequest uploadPartRequest);

        /// <summary>
        /// Begins the async call to upload a part
        /// </summary>
        /// <param name="uploadPartRequest">request parameter</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance for EndUploadPart()</returns>
        IAsyncResult BeginUploadPart(UploadPartRequest uploadPartRequest, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the async call to upload a part.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult instance returned from BeginUploadPart()</param>
        /// <returns><see cref="UploadPartResult" /> instance</returns>
        UploadPartResult EndUploadPart(IAsyncResult asyncResult);


        /// <summary>
        /// Copy an existing object as one part of a multipart upload.
        /// </summary>
        /// <param name="uploadPartCopyRequest">request parameter</param>
        /// <returns><see cref="UploadPartCopyResult"/> instance</returns>
        UploadPartCopyResult UploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest);

        /// <summary>
        /// Begins the async call to copy an existing object as one part of a multipart upload.
        /// </summary>
        /// <param name="uploadPartCopyRequest">request parameter</param>
        /// <param name="callback">callback instance</param>
        /// <param name="state">callback state</param>
        /// <returns>IAsyncResult instance for EndUploadPartCopy()</returns>
        IAsyncResult BeginUploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the async call to copy an existing object as one part of a multipart upload.
        /// </summary>
        /// <param name="asyncResult">IAsyncResult instance</param>
        /// <returns>The upload result</returns>
        UploadPartCopyResult EndUploadPartCopy(IAsyncResult asyncResult);


        /// <summary>
        /// Lists successfully uploaded parts of a specific upload id
        /// </summary>
        /// <param name="listPartsRequest">request parameter</param>
        /// <returns><see cref="PartListing" /> instance</returns>
        PartListing ListParts(ListPartsRequest listPartsRequest);
 
        /// <summary>
        /// Completes a multipart upload. 
        /// </summary>
        /// <param name="completeMultipartUploadRequest">the request parameter</param>
        /// <returns><see cref="CompleteMultipartUploadResult" /> instance</returns>        
        CompleteMultipartUploadResult CompleteMultipartUpload(CompleteMultipartUploadRequest completeMultipartUploadRequest);
        
        #endregion
    }
}
