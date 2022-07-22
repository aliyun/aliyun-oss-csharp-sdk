using Aliyun.OSS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aliyun.OSS
{
    public partial interface IOss
    {
        #region Bucket Operations

        /// <summary>
        /// Creates a new bucket
        /// </summary>
        /// <param name="bucketName">The bucket name. It must be globably unique.</param>
        /// <returns><see cref="Bucket" /> instance</returns>
        Task<Bucket> CreateBucketAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Creates the bucket with specified storage class.
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="storageClass">Storage class.</param>
        Task<Bucket> CreateBucketAsync(string bucketName, StorageClass? storageClass, CancellationToken cancellation = default);

        /// <summary>
        /// Creates a bucket
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="createBucketRequest"><see cref="CreateBucketRequest"/></param>
        Task<Bucket> CreateBucketAsync(CreateBucketRequest createBucketRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes a empty bucket.If the bucket is not empty, this will fail.
        /// </summary>
        /// <param name="bucketName">The bucket name to delete</param>
        Task DeleteBucketAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// List all buckets under the current account.
        /// </summary>
        /// <returns>All <see cref="Bucket" /> instances</returns>
        Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken cancellation = default);

        /// <summary>
        /// Lists all buckets according to the ListBucketsRequest, which could have filters by prefix, marker, etc.
        /// </summary>
        /// <param name="listBucketsRequest"><see cref="ListBucketsRequest"/> instance</param>
        /// <returns><see cref="ListBucketsResult" /> instance</returns>
        Task<ListBucketsResult> ListBucketsAsync(ListBucketsRequest listBucketsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket information.
        /// </summary>
        /// <returns>The bucket information.</returns>
        /// <param name="bucketName">Bucket name.</param>
        Task<BucketInfo> GetBucketInfoAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket stat.
        /// </summary>
        /// <returns>The bucket stat.</returns>
        /// <param name="bucketName">Bucket name.</param>
        Task<BucketStat> GetBucketStatAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="acl"><see cref="CannedAccessControlList" /> instance</param>
        Task SetBucketAclAsync(string bucketName, CannedAccessControlList acl, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="setBucketAclRequest"></param>
        Task SetBucketAclAsync(SetBucketAclRequest setBucketAclRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Bucket ACL<see cref="AccessControlList" /> instance</returns>
        Task<AccessControlList> GetBucketAclAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket location
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>bucket location</returns>
        Task<BucketLocationResult> GetBucketLocationAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket metadata
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns><see cref="BucketMetadata" />metadata</returns>
        Task<BucketMetadata> GetBucketMetadataAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the CORS rules for the <see cref="Bucket" />
        /// </summary>
        /// <param name="setBucketCorsRequest"></param>
        Task SetBucketCorsAsync(SetBucketCorsRequest setBucketCorsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the <see cref="Bucket" /> CORS rules.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>CORS rules</returns>
        Task<IList<CORSRule>> GetBucketCorsAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the CORS rules on the <see cref="Bucket" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        Task DeleteBucketCorsAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> logging config
        /// OSS will log the access information on this bucket, according to the logging config
        /// The hourly log file will be stored in the target bucket.
        /// </summary>
        /// <param name="setBucketLoggingRequest"></param>
        Task SetBucketLoggingAsync(SetBucketLoggingRequest setBucketLoggingRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the bucket logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>The logging config result</returns>
        Task<BucketLoggingResult> GetBucketLoggingAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the <see cref="Bucket" /> logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        Task DeleteBucketLoggingAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="setBucketWebSiteRequest"><see cref="SetBucketWebsiteRequest"/> instance</param>
        Task SetBucketWebsiteAsync(SetBucketWebsiteRequest setBucketWebSiteRequest, CancellationToken cancellation = default);


        /// <summary>
        /// Gets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketWebsiteResult"/> instance</returns>
        Task<BucketWebsiteResult> GetBucketWebsiteAsync(string bucketName, CancellationToken cancellation = default);


        /// <summary>
        /// Deletes the <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        Task DeleteBucketWebsiteAsync(string bucketName, CancellationToken cancellation = default);


        /// <summary>
        /// Sets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="setBucketRefererRequest">The requests that contains the Referer whitelist</param>
        Task SetBucketRefererAsync(SetBucketRefererRequest setBucketRefererRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Referer config</returns>
        Task<RefererConfiguration> GetBucketRefererAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> lifecycle rule
        /// </summary>
        /// <param name="setBucketLifecycleRequest">the <see cref="SetBucketLifecycleRequest" /> instance</param>
        Task SetBucketLifecycleAsync(SetBucketLifecycleRequest setBucketLifecycleRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the bucket's all lifecycle rules.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketLifecycleAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> lifecycle instance. 
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>Lifecycle list</returns>
        Task<IList<LifecycleRule>> GetBucketLifecycleAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="setBucketStorageCapacityRequest"><see cref="SetBucketStorageCapacityRequest"/> instance</param>
        Task SetBucketStorageCapacityAsync(SetBucketStorageCapacityRequest setBucketStorageCapacityRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketStorageCapacityResult"/> instance</returns>
        Task<GetBucketStorageCapacityResult> GetBucketStorageCapacityAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Checks if the bucket exists
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>
        /// True when the bucket exists under the current user;
        /// Otherwise returns false.
        /// </returns>
        Task<bool> DoesBucketExistAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="setBucketPolicyRequest"><see cref="SetBucketPolicyRequest"/> instance</param>
        Task SetBucketPolicyAsync(SetBucketPolicyRequest setBucketPolicyRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketPolicyResult"/> instance</returns>
        Task<GetBucketPolicyResult> GetBucketPolicyAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes <see cref="Bucket" /> policy.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketPolicyAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="setBucketTaggingRequest"><see cref="SetBucketTaggingRequest"/> instance</param>
        Task SetBucketTaggingAsync(SetBucketTaggingRequest setBucketTaggingRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the bucket's tagging.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketTaggingAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the bucket's tagging.
        /// </summary>
        /// <param name="deleteBucketTaggingRequest">DeleteBucketTaggingRequest.</param>
        Task DeleteBucketTaggingAsync(DeleteBucketTaggingRequest deleteBucketTaggingRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketTaggingResult"/> instance</returns>
        Task<GetBucketTaggingResult> GetBucketTaggingAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="setBucketRequestPaymentRequest"><see cref="SetBucketRequestPaymentRequest"/> instance</param>
        Task SetBucketRequestPaymentAsync(SetBucketRequestPaymentRequest setBucketRequestPaymentRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketRequestPaymentResult"/></returns>
        Task<GetBucketRequestPaymentResult> GetBucketRequestPaymentAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="setBucketEncryptionRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        Task SetBucketEncryptionAsync(SetBucketEncryptionRequest setBucketEncryptionRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes bucket encryption rule
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketEncryptionAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketEncryptionResult"/> instance</returns>
        Task<BucketEncryptionResult> GetBucketEncryptionAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="setBucketVersioningRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        Task SetBucketVersioningAsync(SetBucketVersioningRequest setBucketVersioningRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketVersioningResult"/> instance</returns>
        Task<GetBucketVersioningResult> GetBucketVersioningAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="SetBucketInventoryConfigurationRequest"/> instance</param>
        Task SetBucketInventoryConfigurationAsync(SetBucketInventoryConfigurationRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="DeleteBucketInventoryConfigurationRequest"/> instance</param>
        Task DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="GetBucketInventoryConfigurationRequest"/> instance</param>
        /// <returns><see cref="GetBucketInventoryConfigurationResult"/> instance</returns>
        Task<GetBucketInventoryConfigurationResult> GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="ListBucketInventoryConfigurationRequest"/> instance</param>
        /// <returns><see cref="ListBucketInventoryConfigurationResult"/> instance</returns>
        Task<ListBucketInventoryConfigurationResult> ListBucketInventoryConfigurationAsync(ListBucketInventoryConfigurationRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// InitiateBucketWorm
        /// </summary>
        /// <returns><see cref="InitiateBucketWormResult"/> instance</returns>
        /// <param name="request"><see cref="InitiateBucketWormRequest"/> instance</param>
        Task<InitiateBucketWormResult> InitiateBucketWormAsync(InitiateBucketWormRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="Bucket" /> AbortBucketWorm
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task AbortBucketWormAsync(string bucketName, CancellationToken cancellation = default);

        /// <summary>
        /// CompleteBucketWorm
        /// </summary>
        /// <param name="request"><see cref="CompleteBucketWormRequest"/> instance</param>
        Task CompleteBucketWormAsync(CompleteBucketWormRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// ExtendBucketWorm
        /// </summary>
        /// <param name="request"><see cref="ExtendBucketWormRequest"/> instance</param>
        Task ExtendBucketWormAsync(ExtendBucketWormRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// GetBucketWormResult
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <returns><see cref="GetBucketWormResult"/> instance</returns>
        Task<GetBucketWormResult> GetBucketWormAsync(string bucketName, CancellationToken cancellation = default);
        #endregion

        #region Object Operations

        /// <summary>
        /// Lists all objects under the <see cref="Bucket" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="OssObject" /> list</returns>
        Task<ObjectListing> ListObjectsAsync(string bucketName, CancellationToken cancellation = default);


        /// <summary>
        /// Lists object with specified prefix
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="prefix"><see cref="OssObject.Key" /> prefix</param>
        /// <returns><see cref="OssObject" /> instances list</returns>
        Task<ObjectListing> ListObjectsAsync(string bucketName, string prefix, CancellationToken cancellation = default);


        /// <summary>
        /// Lists objects according to the ListObjectsRequest.
        /// The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="listObjectsRequest"><see cref="ListObjectsRequest" /> instance</param>
        /// <returns><see cref="OssObject" /> list</returns>
        Task<ObjectListing> ListObjectsAsync(ListObjectsRequest listObjectsRequest, CancellationToken cancellation = default);


        /// <summary>
        /// Lists object vesions according to the ListObjectVersionsRequest.
        /// The returned object is type of OssObjectSummary.
        /// </summary>
        /// <param name="listObjectVersionsRequest"><see cref="ListObjectVersionsRequest" /> instance</param>
        /// <returns><see cref="OssObject" /> list</returns>
        Task<ObjectVersionList> ListObjectVersionsAsync(ListObjectVersionsRequest listObjectVersionsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Puts object to the specified bucket with specified object key.
        /// </summary>
        /// <param name="bucketName">specified bucket name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(string bucketName, string key, Stream content, CancellationToken cancellation = default);


        /// <summary>
        /// Uploads the content to object under the specified bucket and object key.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="content"><see cref="OssObject.Content" /></param>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(string bucketName, string key, Stream content, ObjectMetadata metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Upload a <see cref="OssObject" /> according to <see cref="PutObjectRequest" />.
        /// </summary>
        /// <param name="putObjectRequest"><see cref="PutObjectRequest" />instance</param>
        /// <returns><see cref="PutObjectResult" />instance</returns>
        Task<PutObjectResult> PutObjectAsync(PutObjectRequest putObjectRequest, CancellationToken cancellation = default);


        /// <summary>
        /// Uploads a local file to OSS under the specified bucket
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file path to upload</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(string bucketName, string key, string fileToUpload, CancellationToken cancellation = default);

        /// <summary>
        /// Uploads a local file with specified metadata to OSS.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="fileToUpload">local file path</param>
        /// <param name="metadata"><see cref="OssObject" />metadata</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, CancellationToken cancellation = default);



        /// <summary>
        /// Uploads the file via the signed url.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="fileToUpload">File to upload</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(Uri signedUrl, string fileToUpload, CancellationToken cancellation = default);

        /// <summary>
        /// Uploads the instream via the signed url.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="content">content stream</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        Task<PutObjectResult> PutObjectAsync(Uri signedUrl, Stream content, CancellationToken cancellation = default);

        /// <summary>
        /// Uploads the file via the signed url with the metadata.
        /// </summary>
        /// <param name="signedUrl">The signed url</param>
        /// <param name="fileToUpload">Local file path</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        Task<PutObjectResult> PutObjectAsync(Uri signedUrl, string fileToUpload, ObjectMetadata metadata, CancellationToken cancellation = default);

        /// <summary>
        /// Uploads the stream via the signed url with the metadata.
        /// </summary>
        /// <param name="signedUrl">Signed url</param>
        /// <param name="content">content stream</param>
        /// <returns><see cref="PutObjectResult" /> instance</returns>
        /// <param name="metadata"><see cref="OssObject" /> metadata</param>
        Task<PutObjectResult> PutObjectAsync(Uri signedUrl, Stream content, ObjectMetadata metadata, CancellationToken cancellation = default);


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
        [Obsolete("CopyBigObject is deprecated, please use ResumableUploadObjectAsync instead")]
        Task<CopyObjectResult> CopyBigObjectAsync(CopyObjectRequest copyObjectRequest, long? partSize = null, string checkpointDir = null, CancellationToken cancellation = default);

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
        Task<PutObjectResult> ResumableUploadObjectAsync(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                              EventHandler<StreamTransferProgressArgs> streamTransferProgress = null, CancellationToken cancellation = default);

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
        Task<PutObjectResult> ResumableUploadObjectAsync(string bucketName, string key, Stream content, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                              EventHandler<StreamTransferProgressArgs> streamTransferProgress = null, CancellationToken cancellation = default);

        /// <summary>
        /// Resumables the upload object.
        /// The request.UploadStream will be disposed once the call finishes.
        /// </summary>
        /// <returns>The upload object.</returns>
        /// <param name="request">Upload Request.</param>
        Task<PutObjectResult> ResumableUploadObjectAsync(UploadObjectRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Appends object to OSS according to the <see cref="AppendObjectRequest" />
        /// </summary>
        /// <param name="request"><see cref="AppendObjectRequest" /> instance</param>
        /// <returns><see cref="AppendObjectResult" /> result</returns>
        Task<AppendObjectResult> AppendObjectAsync(AppendObjectRequest request, CancellationToken cancellation = default);


        /// <summary>
        /// Creates the symlink of the target object
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="symlink">Symlink.</param>
        /// <param name="target">Target.</param>
        /// <returns><see cref="CreateSymlinkResult"/> instance</returns>
        Task<CreateSymlinkResult> CreateSymlinkAsync(string bucketName, string symlink, string target, CancellationToken cancellation = default);

        /// <summary>
        /// Creates the symlink of the target object
        /// </summary>
        /// <param name="createSymlinkRequest">Create symlink request.</param>
        /// <returns><see cref="CreateSymlinkResult"/> instance</returns>
        Task<CreateSymlinkResult> CreateSymlinkAsync(CreateSymlinkRequest createSymlinkRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the target file of the symlink.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="symlink">Symlink </param>
        /// <returns>OssSymlink object</returns>
        Task<OssSymlink> GetSymlinkAsync(string bucketName, string symlink, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the target file of the symlink.
        /// </summary>
        /// <param name="getSymlinkRequest">Get symlink request.</param>
        /// <returns>OssSymlink object</returns>
        Task<OssSymlink> GetSymlinkAsync(GetSymlinkRequest getSymlinkRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets object
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key"><see cref="OssObject.Key"/></param>
        /// <returns><see cref="OssObject" /> instance</returns>
        Task<OssObject> GetObjectAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Gets object via signed url
        /// </summary>
        /// <param name="signedUrl">The signed url of HTTP GET method</param>
        /// <returns><see cref="OssObject"/> instance</returns>
        Task<OssObject> GetObjectAsync(Uri signedUrl, CancellationToken cancellation = default);

        /// <summary>
        /// Gets object via the bucket name and key name in the <see cref="GetObjectRequest" /> instance.
        /// </summary>
        /// <param name="getObjectRequest"> The request parameter</param>
        /// <returns><see cref="OssObject" /> instance. The caller needs to dispose the object.</returns>
        Task<OssObject> GetObjectAsync(GetObjectRequest getObjectRequest, CancellationToken cancellation = default);


        /// <summary>
        /// Gets the object and assign the data to the stream.
        /// </summary>
        /// <param name="getObjectRequest">request parameter</param>
        /// <param name="output">output stream</param>
        /// <returns><see cref="OssObject" /> metadata</returns>
        Task<ObjectMetadata> GetObjectAsync(GetObjectRequest getObjectRequest, Stream output, CancellationToken cancellation = default);

        /// <summary>
        /// Download a file.
        /// Internally it may use multipart download in case the file is big
        /// </summary>
        /// <returns>The metadata object</returns>
        /// <param name="request">DownloadObjectRequest instance</param>
        Task<ObjectMetadata> ResumableDownloadObjectAsync(DownloadObjectRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="OssObject" />metadata</returns>
        Task<ObjectMetadata> GetObjectMetadataAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="request">GetObjectMetadataRequest instance</param>
        /// <returns><see cref="OssObject" />metadata</returns>
        Task<ObjectMetadata> GetObjectMetadataAsync(GetObjectMetadataRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets <see cref="OssObject" /> metadata.
        /// </summary>
        /// <param name="request">GetObjectMetadataRequest instance</param>
        /// <returns><see cref="OssObject" />metadata</returns>
        Task<ObjectMetadata> GetSimplifiedObjectMetadataAsync(GetObjectMetadataRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes <see cref="OssObject" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="DeleteObjectResult" />instance</returns>
        Task<DeleteObjectResult> DeleteObjectAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes <see cref="OssObject" />
        /// </summary>
        /// <param name="deleteObjectRequest">the request parameter</param>
        /// <returns><see cref="DeleteObjectResult" />instance</returns>
        Task<DeleteObjectResult> DeleteObjectAsync(DeleteObjectRequest deleteObjectRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes multiple objects
        /// </summary>
        /// <param name="deleteObjectsRequest">the request parameter</param>
        /// <returns>delete object result</returns>
        Task<DeleteObjectsResult> DeleteObjectsAsync(DeleteObjectsRequest deleteObjectsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes multiple objects with version id
        /// </summary>
        /// <param name="deleteObjectVersionsRequest">the request parameter</param>
        /// <returns>delete object result</returns>
        Task<DeleteObjectVersionsResult> DeleteObjectVersionsAsync(DeleteObjectVersionsRequest deleteObjectVersionsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// copy an object to another one in OSS.
        /// </summary>
        /// <param name="copyObjectRequst">The request parameter</param>
        /// <returns>copy object result</returns>
        Task<CopyObjectResult> CopyObjectAsync(CopyObjectRequest copyObjectRequst, CancellationToken cancellation = default);


        /// <summary>
        /// Resumable object copy.
        /// If the file size is less than part size, normal file upload is used; otherwise multipart upload is used.
        /// </summary>
        /// <param name="copyObjectRequest">request parameter</param>
        /// <param name="checkpointDir">checkpoint file folder </param>
        /// <param name="partSize">The part size. 
        /// </param>
        /// <returns><see cref="CopyObjectResult" /> instance</returns>
        Task<CopyObjectResult> ResumableCopyObjectAsync(CopyObjectRequest copyObjectRequest, string checkpointDir, long? partSize = null, CancellationToken cancellation = default);

        /// <summary>
        /// Modify the object metadata. 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <param name="newMeta">new metadata</param>
        /// <param name="checkpointDir">check point folder. It must be specified to store the checkpoint information</param>
        /// <param name="partSize">Part size, it's no less than <see cref="Util.OssUtils.DefaultPartSize"/>
        /// </param>
        Task ModifyObjectMetaAsync(string bucketName, string key, ObjectMetadata newMeta, long? partSize = null, string checkpointDir = null, CancellationToken cancellation = default);

        /// <summary>
        /// Checks if the object exists
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns>true:object exists;false:otherwise</returns>
        Task<bool> DoesObjectExistAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the object ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /> key</param>
        /// <param name="acl"><see cref="CannedAccessControlList" /> instance</param>
        Task SetObjectAclAsync(string bucketName, string key, CannedAccessControlList acl, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the object ACL
        /// </summary>
        /// <param name="setObjectAclRequest"></param>
        Task SetObjectAclAsync(SetObjectAclRequest setObjectAclRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the object ACL 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="AccessControlList" /> instance</returns>
        Task<AccessControlList> GetObjectAclAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the object ACL
        /// </summary>
        /// <param name="getObjectAclRequest"></param>
        Task<AccessControlList> GetObjectAclAsync(GetObjectAclRequest getObjectAclRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Restores the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        Task<RestoreObjectResult> RestoreObjectAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Restores the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="restoreObjectRequest"></param>
        Task<RestoreObjectResult> RestoreObjectAsync(RestoreObjectRequest restoreObjectRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the object tagging
        /// </summary>
        /// <param name="request"><see cref="SetObjectTaggingRequest" /> instance</param>
        Task SetObjectTaggingAsync(SetObjectTaggingRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the object tagging 
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        /// <returns><see cref="GetObjectTaggingResult" /> instance</returns>
        Task<GetObjectTaggingResult> GetObjectTaggingAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the object tagging
        /// </summary>
        /// <param name="request"><see cref="GetObjectTaggingRequest" /> instance</param>
        /// <returns><see cref="GetObjectTaggingResult" /> instance</returns>
        Task<GetObjectTaggingResult> GetObjectTaggingAsync(GetObjectTaggingRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes object tagging
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="key"><see cref="OssObject.Key" /></param>
        Task DeleteObjectTaggingAsync(string bucketName, string key, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes the object tagging
        /// </summary>
        /// <param name="request"><see cref="DeleteObjectTaggingRequest" /> instance</param>
        Task DeleteObjectTaggingAsync(DeleteObjectTaggingRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the contents of a object based on a SQL statement. 
        /// </summary>
        /// <param name="request"><see cref="SelectObjectRequest" /> instance</param>
        Task<OssObject> SelectObjectAsync(SelectObjectRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Creates the meta of a select object
        /// </summary>
        /// <param name="request"><see cref="CreateSelectObjectMetaRequest" /> instance</param>
        Task<CreateSelectObjectMetaResult> CreateSelectObjectMetaAsync(CreateSelectObjectMetaRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Processes the object
        /// </summary>
        /// <param name="request"><see cref="ProcessObjectRequest" /> instance</param>
        /// <returns><see cref="ProcessObjectRequest" /> instance</returns>
        Task<ProcessObjectResult> ProcessObjectAsync(ProcessObjectRequest request, CancellationToken cancellation = default);

        #endregion

        #region Multipart Operations
        /// <summary>
        /// Lists ongoing multipart uploads 
        /// </summary>
        /// <param name="listMultipartUploadsRequest">request parameter</param>
        /// <returns><see cref="MultipartUploadListing" /> instance</returns>
        Task<MultipartUploadListing> ListMultipartUploadsAsync(ListMultipartUploadsRequest listMultipartUploadsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Initiate a multipart upload
        /// </summary>
        /// <param name="initiateMultipartUploadRequest">request parameter</param>
        /// <returns><see cref="InitiateMultipartUploadResult"/> instance</returns>
        Task<InitiateMultipartUploadResult> InitiateMultipartUploadAsync(InitiateMultipartUploadRequest initiateMultipartUploadRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Aborts a multipart upload
        /// </summary>
        /// <param name="abortMultipartUploadRequest">request parameter</param>
        Task AbortMultipartUploadAsync(AbortMultipartUploadRequest abortMultipartUploadRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Uploads a part
        /// </summary>
        /// <param name="uploadPartRequest">request parameter</param>
        /// <returns><see cref="UploadPartResult" /> instance</returns>
        Task<UploadPartResult> UploadPartAsync(UploadPartRequest uploadPartRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Copy an existing object as one part of a multipart upload.
        /// </summary>
        /// <param name="uploadPartCopyRequest">request parameter</param>
        /// <returns><see cref="UploadPartCopyResult"/> instance</returns>
        Task<UploadPartCopyResult> UploadPartCopyAsync(UploadPartCopyRequest uploadPartCopyRequest, CancellationToken cancellation = default);


        /// <summary>
        /// Lists successfully uploaded parts of a specific upload id
        /// </summary>
        /// <param name="listPartsRequest">request parameter</param>
        /// <returns><see cref="PartListing" /> instance</returns>
        Task<PartListing> ListPartsAsync(ListPartsRequest listPartsRequest, CancellationToken cancellation = default);

        /// <summary>
        /// Completes a multipart upload. 
        /// </summary>
        /// <param name="completeMultipartUploadRequest">the request parameter</param>
        /// <returns><see cref="CompleteMultipartUploadResult" /> instance</returns>        
        Task<CompleteMultipartUploadResult> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest completeMultipartUploadRequest, CancellationToken cancellation = default);

        #endregion

        #region Live Channel
        /// <summary>
        /// Creates a live channel
        /// </summary>
        /// <param name="request"><see cref="CreateLiveChannelRequest" /> instance</param>
        /// <returns><see cref="CreateLiveChannelResult" /> instance</returns>        
        Task<CreateLiveChannelResult> CreateLiveChannelAsync(CreateLiveChannelRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Lists live channels
        /// </summary>
        /// <param name="request"><see cref="ListLiveChannelRequest" /> instance</param>
        /// <returns><see cref="ListLiveChannelResult" /> instance</returns>        
        Task<ListLiveChannelResult> ListLiveChannelAsync(ListLiveChannelRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Deletes a live channel
        /// </summary>
        /// <param name="request"><see cref="DeleteLiveChannelRequest" /> instance</param>
        Task DeleteLiveChannelAsync(DeleteLiveChannelRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Sets the live channel status
        /// </summary>
        /// <param name="request"><see cref="SetLiveChannelStatusRequest" /> instance</param>
        Task SetLiveChannelStatusAsync(SetLiveChannelStatusRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the live channel information
        /// </summary>
        /// <param name="request"><see cref="GetLiveChannelInfoRequest" /> instance</param>
        /// <returns><see cref="GetLiveChannelInfoResult" /> instance</returns>        
        Task<GetLiveChannelInfoResult> GetLiveChannelInfoAsync(GetLiveChannelInfoRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the live channel status
        /// </summary>
        /// <param name="request"><see cref="GetLiveChannelStatRequest" /> instance</param>
        /// <returns><see cref="GetLiveChannelStatResult" /> instance</returns>        
        Task<GetLiveChannelStatResult> GetLiveChannelStatAsync(GetLiveChannelStatRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets the live channel history
        /// </summary>
        /// <param name="request"><see cref="GetLiveChannelHistoryRequest" /> instance</param>
        /// <returns><see cref="GetLiveChannelHistoryResult" /> instance</returns>        
        Task<GetLiveChannelHistoryResult> GetLiveChannelHistoryAsync(GetLiveChannelHistoryRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Creates a vod playlist
        /// </summary>
        /// <param name="request"><see cref="PostVodPlaylistRequest" /> instance</param>
        Task PostVodPlaylistAsync(PostVodPlaylistRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Gets a vod playlist
        /// </summary>
        /// <param name="request"><see cref="GetVodPlaylistRequest" /> instance</param>
        /// <returns><see cref="GetVodPlaylistResult" /> instance</returns>        
        Task<GetVodPlaylistResult> GetVodPlaylistAsync(GetVodPlaylistRequest request, CancellationToken cancellation = default);

        #endregion

    }
}
