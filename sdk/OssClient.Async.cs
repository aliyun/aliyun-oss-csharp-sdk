using Aliyun.OSS.Commands;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Model;
using Aliyun.OSS.Properties;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aliyun.OSS
{
    public partial class OssClient : IOss
    {
        #region Bucket Operations
        /// <inheritdoc/>
        public async Task AbortBucketWormAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = AbortBucketWormCommand.Create(_serviceClient, _endpoint,
                                       CreateContext(HttpMethod.Delete, bucketName, null),
                                       bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task CompleteBucketWormAsync(CompleteBucketWormRequest request, CancellationToken cancellation = default)
        {
            var cmd = CompleteBucketWormCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Post, request.BucketName, null),
                                                      request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task<Bucket> CreateBucketAsync(string bucketName, CancellationToken cancellation = default)
        {
            return await CreateBucketAsync(bucketName, null, cancellation);
        }

        /// <inheritdoc/>
        public async Task<Bucket> CreateBucketAsync(string bucketName, StorageClass? storageClass, CancellationToken cancellation = default)
        {
            var cmd = CreateBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, bucketName, null),
                                                 bucketName, storageClass);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }

            return new Bucket(bucketName);
        }

        /// <inheritdoc/>
        public async Task<Bucket> CreateBucketAsync(CreateBucketRequest createBucketRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(createBucketRequest);

            var cmd = CreateBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, createBucketRequest.BucketName, null),
                                                 createBucketRequest.BucketName, createBucketRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }

            return new Bucket(createBucketRequest.BucketName);
        }

        /// <inheritdoc/>
        public async Task DeleteBucketAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Delete, bucketName, null),
                                                 bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketCorsAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Delete, bucketName, null),
                                                    bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketEncryptionAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                       CreateContext(HttpMethod.Delete, bucketName, null),
                                       bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketInventoryConfigurationCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Delete, request.BucketName, null),
                                                    request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketLifecycleAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketLifecycleCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, bucketName, null),
                                                   bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketLoggingAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Delete, bucketName, null),
                                                        bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketPolicyAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Delete, bucketName, null),
                                                    bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketTaggingAsync(string bucketName, CancellationToken cancellation = default)
        {
            await DeleteBucketTaggingAsync(new DeleteBucketTaggingRequest(bucketName),cancellation);
        }

        /// <inheritdoc/>
        public async Task DeleteBucketTaggingAsync(DeleteBucketTaggingRequest deleteBucketTaggingRequest, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, deleteBucketTaggingRequest.BucketName, null),
                                                   deleteBucketTaggingRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task DeleteBucketWebsiteAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = DeleteBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                       CreateContext(HttpMethod.Delete, bucketName, null),
                                                       bucketName);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DoesBucketExistAsync(string bucketName, CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "bucketName");
            if (!OssUtils.IsBucketNameValid(bucketName))
                throw new ArgumentException(OssResources.BucketNameInvalid, "bucketName");

            try
            {
                await GetBucketAclAsync(bucketName,cancellation);
            }
            catch (OssException e)
            {
                if (e.ErrorCode.Equals(OssErrorCode.NoSuchBucket))
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task ExtendBucketWormAsync(ExtendBucketWormRequest request, CancellationToken cancellation = default)
        {
            var cmd = ExtendBucketWormCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Post, request.BucketName, null),
                                                      request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task<AccessControlList> GetBucketAclAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketAclCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<IList<CORSRule>> GetBucketCorsAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketEncryptionResult> GetBucketEncryptionAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketInfo> GetBucketInfoAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketInfoCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, bucketName, null), bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketInventoryConfigurationResult> GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, CancellationToken cancellation = default)
        {
            var cmd = GetBucketInventoryConfigurationCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, request.BucketName, null),
                                                    request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<IList<LifecycleRule>> GetBucketLifecycleAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketLifecycleCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Get, bucketName, null),
                                                      bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketLocationResult> GetBucketLocationAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketLocationCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Get, bucketName, null),
                                                      bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketLoggingResult> GetBucketLoggingAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, bucketName, null),
                                                    bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketMetadata> GetBucketMetadataAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketMetadataCommand.Create(_serviceClient, _endpoint,
                                          CreateContext(HttpMethod.Head, bucketName, null),
                                          bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketPolicyResult> GetBucketPolicyAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<RefererConfiguration> GetBucketRefererAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketRefererCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Get, bucketName, null),
                                                     bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketRequestPaymentResult> GetBucketRequestPaymentAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketRequestPaymentCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketStat> GetBucketStatAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketStatCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, bucketName, null), bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketStorageCapacityResult> GetBucketStorageCapacityAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketStorageCapacityCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketTaggingResult> GetBucketTaggingAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketVersioningResult> GetBucketVersioningAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketVersioningCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, bucketName, null),
                                     bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<BucketWebsiteResult> GetBucketWebsiteAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, bucketName, null),
                                                    bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetBucketWormResult> GetBucketWormAsync(string bucketName, CancellationToken cancellation = default)
        {
            var cmd = GetBucketWormCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, bucketName, null),
                                     bucketName);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<InitiateBucketWormResult> InitiateBucketWormAsync(InitiateBucketWormRequest request, CancellationToken cancellation = default)
        {
            var cmd = InitiateBucketWormCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Post, request.BucketName, null),
                                                      request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<ListBucketInventoryConfigurationResult> ListBucketInventoryConfigurationAsync(ListBucketInventoryConfigurationRequest request, CancellationToken cancellation = default)
        {
            var cmd = ListBucketInventoryConfigurationCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, request.BucketName, null),
                                                    request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken cancellation = default)
        {
            var cmd = ListBucketsCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Get, null, null), null);
            var result = await cmd.ExecuteAsync(cancellation);
            return result.Buckets;
        }

        /// <inheritdoc/>
        public async Task<ListBucketsResult> ListBucketsAsync(ListBucketsRequest listBucketsRequest, CancellationToken cancellation = default)
        {
            var cmd = ListBucketsCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Get, null, null), listBucketsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task SetBucketAclAsync(string bucketName, CannedAccessControlList acl, CancellationToken cancellation = default)
        {
            var setBucketAclRequest = new SetBucketAclRequest(bucketName, acl);
            await SetBucketAclAsync(setBucketAclRequest, cancellation);
        }

        /// <inheritdoc/>
        public async Task SetBucketAclAsync(SetBucketAclRequest setBucketAclRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketAclRequest);

            var cmd = SetBucketAclCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Put, setBucketAclRequest.BucketName, null),
                                                setBucketAclRequest.BucketName, setBucketAclRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketCorsAsync(SetBucketCorsRequest setBucketCorsRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketCorsRequest);
            var cmd = SetBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, setBucketCorsRequest.BucketName, null),
                                                 setBucketCorsRequest.BucketName,
                                                 setBucketCorsRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketEncryptionAsync(SetBucketEncryptionRequest setBucketEncryptionRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketEncryptionRequest);

            var cmd = SetBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketEncryptionRequest.BucketName, null),
                                                      setBucketEncryptionRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketInventoryConfigurationAsync(SetBucketInventoryConfigurationRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = SetBucketInventoryConfigurationCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Put, request.BucketName, null),
                                                    request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketLifecycleAsync(SetBucketLifecycleRequest setBucketLifecycleRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketLifecycleRequest);
            if (setBucketLifecycleRequest.LifecycleRules.Count == 0)
            {
                throw new ArgumentException("SetBucketLifecycleRequest must have at least one LifecycleRule.");
            }

            var cmd = SetBucketLifecycleCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketLifecycleRequest.BucketName, null),
                                                      setBucketLifecycleRequest.BucketName,
                                                      setBucketLifecycleRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketLoggingAsync(SetBucketLoggingRequest setBucketLoggingRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketLoggingRequest);
            var cmd = SetBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Put, setBucketLoggingRequest.BucketName, null),
                                                    setBucketLoggingRequest.BucketName,
                                                    setBucketLoggingRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketPolicyAsync(SetBucketPolicyRequest setBucketPolicyRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketPolicyRequest);

            var cmd = SetBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Put, setBucketPolicyRequest.BucketName, null),
                                                setBucketPolicyRequest.BucketName, setBucketPolicyRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketRefererAsync(SetBucketRefererRequest setBucketRefererRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketRefererRequest);
            var cmd = SetBucketRefererCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Put, setBucketRefererRequest.BucketName, null),
                                                    setBucketRefererRequest.BucketName,
                                                    setBucketRefererRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketRequestPaymentAsync(SetBucketRequestPaymentRequest setBucketRequestPaymentRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketRequestPaymentRequest);

            var cmd = SetBucketRequestPaymentCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketRequestPaymentRequest.BucketName, null),
                                                      setBucketRequestPaymentRequest.BucketName,
                                                      setBucketRequestPaymentRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketStorageCapacityAsync(SetBucketStorageCapacityRequest setBucketStorageCapacityRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketStorageCapacityRequest);
            var cmd = SetBucketStorageCapacityCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Put, setBucketStorageCapacityRequest.BucketName, null),
                                                             setBucketStorageCapacityRequest.BucketName,
                                                             setBucketStorageCapacityRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketTaggingAsync(SetBucketTaggingRequest setBucketTaggingRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketTaggingRequest);

            var cmd = SetBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketTaggingRequest.BucketName, null),
                                                      setBucketTaggingRequest.BucketName,
                                                      setBucketTaggingRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketVersioningAsync(SetBucketVersioningRequest setBucketVersioningRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketVersioningRequest);

            var cmd = SetBucketVersioningCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketVersioningRequest.BucketName, null),
                                                      setBucketVersioningRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetBucketWebsiteAsync(SetBucketWebsiteRequest setBucketWebSiteRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(setBucketWebSiteRequest);
            var cmd = SetBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Put, setBucketWebSiteRequest.BucketName, null),
                                                     setBucketWebSiteRequest.BucketName,
                                                     setBucketWebSiteRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        #endregion
    }
}
