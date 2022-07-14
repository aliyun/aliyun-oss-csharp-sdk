using Aliyun.OSS.Model;
using System;
using System.Collections.Generic;
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
        Task<Bucket> CreateBucketAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Creates the bucket with specified storage class.
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="storageClass">Storage class.</param>
        Task<Bucket> CreateBucketAsync(string bucketName, StorageClass? storageClass, CancellationToken cancellation=default);

        /// <summary>
        /// Creates a bucket
        /// </summary>
        /// <returns>The bucket.</returns>
        /// <param name="createBucketRequest"><see cref="CreateBucketRequest"/></param>
        Task<Bucket> CreateBucketAsync(CreateBucketRequest createBucketRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes a empty bucket.If the bucket is not empty, this will fail.
        /// </summary>
        /// <param name="bucketName">The bucket name to delete</param>
        Task DeleteBucketAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// List all buckets under the current account.
        /// </summary>
        /// <returns>All <see cref="Bucket" /> instances</returns>
        Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken cancellation=default);

        /// <summary>
        /// Lists all buckets according to the ListBucketsRequest, which could have filters by prefix, marker, etc.
        /// </summary>
        /// <param name="listBucketsRequest"><see cref="ListBucketsRequest"/> instance</param>
        /// <returns><see cref="ListBucketsResult" /> instance</returns>
        Task<ListBucketsResult> ListBucketsAsync(ListBucketsRequest listBucketsRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket information.
        /// </summary>
        /// <returns>The bucket information.</returns>
        /// <param name="bucketName">Bucket name.</param>
        Task<BucketInfo> GetBucketInfoAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket stat.
        /// </summary>
        /// <returns>The bucket stat.</returns>
        /// <param name="bucketName">Bucket name.</param>
        Task<BucketStat> GetBucketStatAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <param name="acl"><see cref="CannedAccessControlList" /> instance</param>
        Task SetBucketAclAsync(string bucketName, CannedAccessControlList acl, CancellationToken cancellation=default);

        /// <summary>
        /// Sets the bucket ACL
        /// </summary>
        /// <param name="setBucketAclRequest"></param>
        Task SetBucketAclAsync(SetBucketAclRequest setBucketAclRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket ACL
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Bucket ACL<see cref="AccessControlList" /> instance</returns>
        Task<AccessControlList> GetBucketAclAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket location
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>bucket location</returns>
        Task<BucketLocationResult> GetBucketLocationAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket metadata
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns><see cref="BucketMetadata" />metadata</returns>
        Task<BucketMetadata> GetBucketMetadataAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets the CORS rules for the <see cref="Bucket" />
        /// </summary>
        /// <param name="setBucketCorsRequest"></param>
        Task SetBucketCorsAsync(SetBucketCorsRequest setBucketCorsRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the <see cref="Bucket" /> CORS rules.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>CORS rules</returns>
        Task<IList<CORSRule>> GetBucketCorsAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes the CORS rules on the <see cref="Bucket" />
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        Task DeleteBucketCorsAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> logging config
        /// OSS will log the access information on this bucket, according to the logging config
        /// The hourly log file will be stored in the target bucket.
        /// </summary>
        /// <param name="setBucketLoggingRequest"></param>
        Task SetBucketLoggingAsync(SetBucketLoggingRequest setBucketLoggingRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the bucket logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>The logging config result</returns>
        Task<BucketLoggingResult> GetBucketLoggingAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes the <see cref="Bucket" /> logging config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        Task DeleteBucketLoggingAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="setBucketWebSiteRequest"><see cref="SetBucketWebsiteRequest"/> instance</param>
        Task SetBucketWebsiteAsync(SetBucketWebsiteRequest setBucketWebSiteRequest, CancellationToken cancellation=default);


        /// <summary>
        /// Gets <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketWebsiteResult"/> instance</returns>
        Task<BucketWebsiteResult> GetBucketWebsiteAsync(string bucketName, CancellationToken cancellation=default);


        /// <summary>
        /// Deletes the <see cref="Bucket" /> static website config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        Task DeleteBucketWebsiteAsync(string bucketName, CancellationToken cancellation=default);


        /// <summary>
        /// Sets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="setBucketRefererRequest">The requests that contains the Referer whitelist</param>
        Task SetBucketRefererAsync(SetBucketRefererRequest setBucketRefererRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets the <see cref="Bucket" /> referer config
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>Referer config</returns>
        Task<RefererConfiguration> GetBucketRefererAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> lifecycle rule
        /// </summary>
        /// <param name="setBucketLifecycleRequest">the <see cref="SetBucketLifecycleRequest" /> instance</param>
        Task SetBucketLifecycleAsync(SetBucketLifecycleRequest setBucketLifecycleRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes the bucket's all lifecycle rules.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketLifecycleAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> lifecycle instance. 
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>Lifecycle list</returns>
        Task<IList<LifecycleRule>> GetBucketLifecycleAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="setBucketStorageCapacityRequest"><see cref="SetBucketStorageCapacityRequest"/> instance</param>
        Task SetBucketStorageCapacityAsync(SetBucketStorageCapacityRequest setBucketStorageCapacityRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> storage capacity
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketStorageCapacityResult"/> instance</returns>
        Task<GetBucketStorageCapacityResult> GetBucketStorageCapacityAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Checks if the bucket exists
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns>
        /// True when the bucket exists under the current user;
        /// Otherwise returns false.
        /// </returns>
        Task<bool> DoesBucketExistAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="setBucketPolicyRequest"><see cref="SetBucketPolicyRequest"/> instance</param>
        Task SetBucketPolicyAsync(SetBucketPolicyRequest setBucketPolicyRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> policy
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketPolicyResult"/> instance</returns>
        Task<GetBucketPolicyResult> GetBucketPolicyAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes <see cref="Bucket" /> policy.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketPolicyAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="setBucketTaggingRequest"><see cref="SetBucketTaggingRequest"/> instance</param>
        Task SetBucketTaggingAsync(SetBucketTaggingRequest setBucketTaggingRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes the bucket's tagging.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketTaggingAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes the bucket's tagging.
        /// </summary>
        /// <param name="deleteBucketTaggingRequest">DeleteBucketTaggingRequest.</param>
        Task DeleteBucketTaggingAsync(DeleteBucketTaggingRequest deleteBucketTaggingRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket tagging
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketTaggingResult"/> instance</returns>
        Task<GetBucketTaggingResult> GetBucketTaggingAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="setBucketRequestPaymentRequest"><see cref="SetBucketRequestPaymentRequest"/> instance</param>
        Task SetBucketRequestPaymentAsync(SetBucketRequestPaymentRequest setBucketRequestPaymentRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket request payment
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketRequestPaymentResult"/></returns>
        Task<GetBucketRequestPaymentResult> GetBucketRequestPaymentAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="setBucketEncryptionRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        Task SetBucketEncryptionAsync(SetBucketEncryptionRequest setBucketEncryptionRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes bucket encryption rule
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task DeleteBucketEncryptionAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket encryption rule
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="BucketEncryptionResult"/> instance</returns>
        Task<BucketEncryptionResult> GetBucketEncryptionAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="setBucketVersioningRequest"><see cref="SetBucketEncryptionRequest"/> instance</param>
        Task SetBucketVersioningAsync(SetBucketVersioningRequest setBucketVersioningRequest, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket versioning
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        /// <returns><see cref="GetBucketVersioningResult"/> instance</returns>
        Task<GetBucketVersioningResult> GetBucketVersioningAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// Sets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="SetBucketInventoryConfigurationRequest"/> instance</param>
        Task SetBucketInventoryConfigurationAsync(SetBucketInventoryConfigurationRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// Deletes <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="DeleteBucketInventoryConfigurationRequest"/> instance</param>
        Task DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="GetBucketInventoryConfigurationRequest"/> instance</param>
        /// <returns><see cref="GetBucketInventoryConfigurationResult"/> instance</returns>
        Task<GetBucketInventoryConfigurationResult> GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> bucket inventory configuration
        /// </summary>
        /// <param name="request"><see cref="ListBucketInventoryConfigurationRequest"/> instance</param>
        /// <returns><see cref="ListBucketInventoryConfigurationResult"/> instance</returns>
        Task<ListBucketInventoryConfigurationResult> ListBucketInventoryConfigurationAsync(ListBucketInventoryConfigurationRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// InitiateBucketWorm
        /// </summary>
        /// <returns><see cref="InitiateBucketWormResult"/> instance</returns>
        /// <param name="request"><see cref="InitiateBucketWormRequest"/> instance</param>
        Task<InitiateBucketWormResult> InitiateBucketWormAsync(InitiateBucketWormRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// Gets <see cref="Bucket" /> AbortBucketWorm
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        Task AbortBucketWormAsync(string bucketName, CancellationToken cancellation=default);

        /// <summary>
        /// CompleteBucketWorm
        /// </summary>
        /// <param name="request"><see cref="CompleteBucketWormRequest"/> instance</param>
        Task CompleteBucketWormAsync(CompleteBucketWormRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// ExtendBucketWorm
        /// </summary>
        /// <param name="request"><see cref="ExtendBucketWormRequest"/> instance</param>
        Task ExtendBucketWormAsync(ExtendBucketWormRequest request, CancellationToken cancellation=default);

        /// <summary>
        /// GetBucketWormResult
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <returns><see cref="GetBucketWormResult"/> instance</returns>
        Task<GetBucketWormResult> GetBucketWormAsync(string bucketName, CancellationToken cancellation=default);
        #endregion
    }
}
