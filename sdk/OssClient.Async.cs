using Aliyun.OSS.Commands;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Model;
using Aliyun.OSS.Properties;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Aliyun.OSS.Common.Communication.ExecutionContext;

namespace Aliyun.OSS
{
    public partial class OssClient
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

        #region Object Operations
        /// <inheritdoc/>    
        public async Task<AppendObjectResult> AppendObjectAsync(AppendObjectRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);

            var cmd = AppendObjectCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, request.BucketName, request.Key),
                                                 request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CopyObjectResult> CopyBigObjectAsync(CopyObjectRequest copyObjectRequest, long? partSize = null, string checkpointDir = null, CancellationToken cancellation = default)
        {
            return await ResumableCopyObjectAsync(copyObjectRequest, checkpointDir, partSize, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CopyObjectResult> CopyObjectAsync(CopyObjectRequest copyObjectRequst, CancellationToken cancellation)
        {
            ThrowIfNullRequest(copyObjectRequst);
            var cmd = CopyObjectCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, copyObjectRequst.DestinationBucketName, copyObjectRequst.DestinationKey),
                                               copyObjectRequst);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CreateSelectObjectMetaResult> CreateSelectObjectMetaAsync(CreateSelectObjectMetaRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);

            var cmd = CreateSelectObjectMetaCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, request.BucketName, request.Key),
                                                      request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CreateSymlinkResult> CreateSymlinkAsync(string bucketName, string symlink, string target, CancellationToken cancellation)
        {
            var cmd = CreateSymlinkCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, bucketName, symlink),
                                                  new CreateSymlinkRequest(bucketName, symlink, target));
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CreateSymlinkResult> CreateSymlinkAsync(CreateSymlinkRequest createSymlinkRequest, CancellationToken cancellation)
        {
            var cmd = CreateSymlinkCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, createSymlinkRequest.BucketName, createSymlinkRequest.Symlink),
                                                  createSymlinkRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<DeleteObjectResult> DeleteObjectAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await DeleteObjectAsync(new DeleteObjectRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<DeleteObjectResult> DeleteObjectAsync(DeleteObjectRequest deleteObjectRequest, CancellationToken cancellation)
        {
            var cmd = DeleteObjectCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Delete, deleteObjectRequest.BucketName, deleteObjectRequest.Key),
                                                deleteObjectRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<DeleteObjectsResult> DeleteObjectsAsync(DeleteObjectsRequest deleteObjectsRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(deleteObjectsRequest);

            var cmd = DeleteObjectsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, deleteObjectsRequest.BucketName, null),
                                                 deleteObjectsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task DeleteObjectTaggingAsync(string bucketName, string key, CancellationToken cancellation)
        {
            await DeleteObjectTaggingAsync(new DeleteObjectTaggingRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task DeleteObjectTaggingAsync(DeleteObjectTaggingRequest request, CancellationToken cancellation)
        {
            var cmd = DeleteObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, request.BucketName, request.Key),
                                                   request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>    
        public async Task<DeleteObjectVersionsResult> DeleteObjectVersionsAsync(DeleteObjectVersionsRequest deleteObjectVersionsRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(deleteObjectVersionsRequest);

            var cmd = DeleteObjectVersionsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, deleteObjectVersionsRequest.BucketName, null),
                                                 deleteObjectVersionsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<bool> DoesObjectExistAsync(string bucketName, string key, CancellationToken cancellation)
        {
            try
            {
                var cmd = HeadObjectCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Head, bucketName, key),
                                                  bucketName, key);

                using (await cmd.ExecuteAsync(cancellation))
                {
                    // Do nothing
                }
            }
            catch (OssException e)
            {
                if (e.ErrorCode == OssErrorCode.NoSuchBucket ||
                    e.ErrorCode == OssErrorCode.NoSuchKey)
                {
                    return false;
                }

                // Rethrow
                throw;
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                // Rethrow
                throw;
            }
#if NETCOREAPP2_0
            catch (System.Net.Http.HttpRequestException ex2)
            {
                if (ex2.Message.Contains("404"))
                {
                    return false;
                }

                throw;
            }
#endif
            return true;
        }

        /// <inheritdoc/>    
        public async Task<AccessControlList> GetObjectAclAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await GetObjectAclAsync(new GetObjectAclRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<AccessControlList> GetObjectAclAsync(GetObjectAclRequest getObjectAclRequest, CancellationToken cancellation)
        {
            var cmd = GetObjectAclCommand.Create(_serviceClient,
                                                 _endpoint,
                                                 CreateContext(HttpMethod.Get, getObjectAclRequest.BucketName, getObjectAclRequest.Key),
                                                 getObjectAclRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<OssObject> GetObjectAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await GetObjectAsync(new GetObjectRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<OssObject> GetObjectAsync(Uri signedUrl, CancellationToken cancellation)
        {
            // prepare request
            var request = new ServiceRequest
            {
                Method = HttpMethod.Get,
                Endpoint = OssUtils.GetEndpointFromSignedUrl(signedUrl),
                ResourcePath = OssUtils.GetResourcePathFromSignedUrl(signedUrl),
                ParametersInUri = true
            };
            var parameters = OssUtils.GetParametersFromSignedUrl(signedUrl);
            foreach (var param in parameters)
            {
                request.Parameters.Add(param.Key, param.Value);
            }

            // prepare context
            var context = new ExecutionContext();
            context.Signer = null;
            context.Credentials = null;
            context.ResponseHandlers.Add(new ErrorResponseHandler());

            // get response
            var serviceResponse = await _serviceClient.SendAsync(request, context, cancellation);

            // build result
            var getObjectRequest = new GetObjectRequest(null, null);
            var ResponseDeserializer = new GetObjectResponseDeserializer(getObjectRequest, _serviceClient);
            return ResponseDeserializer.Deserialize(serviceResponse);
        }

        /// <inheritdoc/>    
        public async Task<OssObject> GetObjectAsync(GetObjectRequest getObjectRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(getObjectRequest);

            var cmd = GetObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Get, getObjectRequest.BucketName, getObjectRequest.Key),
                                             getObjectRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectMetadata> GetObjectAsync(GetObjectRequest getObjectRequest, Stream output, CancellationToken cancellation)
        {
            var ossObject = await GetObjectAsync(getObjectRequest, cancellation);
            using (ossObject.Content)
            {
                await IoUtils.WriteToAsync(ossObject.Content, output);
            }
            return ossObject.Metadata;
        }

        /// <inheritdoc/>    
        public async Task<ObjectMetadata> GetObjectMetadataAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await GetObjectMetadataAsync(new GetObjectMetadataRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectMetadata> GetObjectMetadataAsync(GetObjectMetadataRequest request, CancellationToken cancellation)
        {
            var cmd = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Head, request.BucketName, request.Key),
                                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<GetObjectTaggingResult> GetObjectTaggingAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await GetObjectTaggingAsync(new GetObjectTaggingRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<GetObjectTaggingResult> GetObjectTaggingAsync(GetObjectTaggingRequest request, CancellationToken cancellation)
        {
            var cmd = GetObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, request.BucketName, request.Key),
                                                             request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectMetadata> GetSimplifiedObjectMetadataAsync(GetObjectMetadataRequest request, CancellationToken cancellation)
        {
            var cmd = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Head, request.BucketName, request.Key),
                                                     request, true);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<OssSymlink> GetSymlinkAsync(string bucketName, string symlink, CancellationToken cancellation)
        {
            var cmd = GetSymlinkCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, bucketName, symlink),
                                               new GetSymlinkResultDeserializer(),
                                               new GetSymlinkRequest(bucketName, symlink));
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<OssSymlink> GetSymlinkAsync(GetSymlinkRequest getSymlinkRequest, CancellationToken cancellation)
        {
            var cmd = GetSymlinkCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, getSymlinkRequest.BucketName, getSymlinkRequest.Key),
                                               new GetSymlinkResultDeserializer(),
                                               getSymlinkRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectListing> ListObjectsAsync(string bucketName, CancellationToken cancellation)
        {
            return await ListObjectsAsync(bucketName, null, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectListing> ListObjectsAsync(string bucketName, string prefix, CancellationToken cancellation)
        {
            var listObjectsRequest = new ListObjectsRequest(bucketName)
            {
                Prefix = prefix
            };
            return await ListObjectsAsync(listObjectsRequest,cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectListing> ListObjectsAsync(ListObjectsRequest listObjectsRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(listObjectsRequest);
            var cmd = ListObjectsCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, listObjectsRequest.BucketName, null),
                                               listObjectsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectVersionList> ListObjectVersionsAsync(ListObjectVersionsRequest listObjectVersionsRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(listObjectVersionsRequest);
            var cmd = ListObjectVersionsCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, listObjectVersionsRequest.BucketName, null),
                                               listObjectVersionsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task ModifyObjectMetaAsync(string bucketName, string key, ObjectMetadata newMeta, long? partSize, string checkpointDir, CancellationToken cancellation)
        {
            var copyObjectRequest = new CopyObjectRequest(bucketName, key, bucketName, key)
            {
                NewObjectMetadata = newMeta
            };
            await CopyBigObjectAsync(copyObjectRequest, partSize, checkpointDir, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ProcessObjectResult> ProcessObjectAsync(ProcessObjectRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);

            var cmd = ProcessObjectCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Post, request.BucketName, request.Key),
                                                      request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(string bucketName, string key, Stream content, CancellationToken cancellation)
        {
            return await PutObjectAsync(bucketName, key, content, null, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(string bucketName, string key, Stream content, ObjectMetadata metadata, CancellationToken cancellation)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest(bucketName, key, content, metadata);
            return await PutObjectAsync(putObjectRequest, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(PutObjectRequest putObjectRequest, CancellationToken cancellation)
        {
            ObjectMetadata metadata = putObjectRequest.Metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(putObjectRequest.Key, null, ref metadata);
            putObjectRequest.Metadata = metadata;

            var cmd = PutObjectCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, putObjectRequest.BucketName, putObjectRequest.Key),
                                              putObjectRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(string bucketName, string key, string fileToUpload, CancellationToken cancellation)
        {
            return await PutObjectAsync(bucketName, key, fileToUpload, null, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, CancellationToken cancellation)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            metadata = metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref metadata);

            PutObjectResult result;
            using (Stream content = File.OpenRead(fileToUpload))
            {
                result = await PutObjectAsync(bucketName, key, content, metadata, cancellation);
            }
            return result;
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(Uri signedUrl, string fileToUpload, CancellationToken cancellation)
        {
            return await PutObjectAsync(signedUrl, fileToUpload, null, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(Uri signedUrl, Stream content, CancellationToken cancellation)
        {
            return await PutObjectAsync(signedUrl, content, null, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(Uri signedUrl, string fileToUpload, ObjectMetadata metadata, CancellationToken cancellation)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            PutObjectResult result;
            using (Stream content = File.OpenRead(fileToUpload))
            {
                result = await PutObjectAsync(signedUrl, content, metadata, cancellation);
            }
            return result;
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> PutObjectAsync(Uri signedUrl, Stream content, ObjectMetadata metadata, CancellationToken cancellation)
        {
            // prepare request
            var request = new ServiceRequest
            {
                Method = HttpMethod.Put,
                Endpoint = OssUtils.GetEndpointFromSignedUrl(signedUrl),
                ResourcePath = OssUtils.GetResourcePathFromSignedUrl(signedUrl),
                ParametersInUri = true
            };
            var parameters = OssUtils.GetParametersFromSignedUrl(signedUrl);
            foreach (var param in parameters)
            {
                request.Parameters.Add(param.Key, param.Value);
            }
            request.Content = content;

            // populate headers
            if (metadata != null)
            {
                //prevent to be assigned default value in metadata.Populate
                if (metadata.ContentType == null)
                {
                    request.Headers[HttpHeaders.ContentType] = "";
                }
                metadata.Populate(request.Headers);
            }
            if (!request.Headers.ContainsKey(HttpHeaders.ContentLength))
            {
                request.Headers[HttpHeaders.ContentLength] = content.Length.ToString();
            }

            // prepare context
            var context = new ExecutionContext();
            context.Signer = null;
            context.Credentials = null;
            if (ObjectMetadata.HasCallbackHeader(metadata))
            {
                context.ResponseHandlers.Add(new CallbackResponseHandler());
            }
            else
            {
                context.ResponseHandlers.Add(new ErrorResponseHandler());
            }

            ClientConfiguration config = OssUtils.GetClientConfiguration(_serviceClient);
            if (config.EnableCrcCheck)
            {
                var hashStream = new Crc64Stream(request.Content, null, request.Content.Length);
                request.Content = hashStream;
                context.ResponseHandlers.Add(new Crc64CheckHandler(hashStream));
            }

            // get response
            var serviceResponse = await _serviceClient.SendAsync(request, context, cancellation);

            // build result
            var putObjectRequest = new PutObjectRequest(null, null, null, metadata);
            var ResponseDeserializer = new PutObjectResponseDeserializer(putObjectRequest);
            return ResponseDeserializer.Deserialize(serviceResponse);
        }

        /// <inheritdoc/>    
        public async Task<RestoreObjectResult> RestoreObjectAsync(string bucketName, string key, CancellationToken cancellation)
        {
            return await RestoreObjectAsync(new RestoreObjectRequest(bucketName, key), cancellation);
        }

        /// <inheritdoc/>    
        public async Task<RestoreObjectResult> RestoreObjectAsync(RestoreObjectRequest restoreObjectRequest, CancellationToken cancellation)
        {
            ExecutionContext context = CreateContext(HttpMethod.Post, restoreObjectRequest.BucketName, restoreObjectRequest.Key);
            var cmd = RestoreObjectCommand.Create(_serviceClient,
                                                  _endpoint,
                                                  context,
                                                  restoreObjectRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task<CopyObjectResult> ResumableCopyObjectAsync(CopyObjectRequest copyObjectRequest, string checkpointDir, long? partSize, CancellationToken cancellation)
        {
            return await ResumableCopyObjectAsync(copyObjectRequest, checkpointDir, partSize, cancellation);
        }

        /// <inheritdoc/>    
        public async Task<ObjectMetadata> ResumableDownloadObjectAsync(DownloadObjectRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);
            ThrowIfNullRequest(request.BucketName);
            ThrowIfNullRequest(request.Key);
            ThrowIfNullRequest(request.DownloadFile);

            if (!Directory.GetParent(request.DownloadFile).Exists)
            {
                throw new ArgumentException(String.Format("Invalid file path {0}. The parent folder does not exist.", request.DownloadFile));
            }

            var metaRequest = new GetObjectMetadataRequest(request.BucketName, request.Key)
            {
                RequestPayer = request.RequestPayer,
                VersionId = request.VersionId
            };
            ObjectMetadata objectMeta = await this.GetObjectMetadataAsync(metaRequest,cancellation);
            var fileSize = objectMeta.ContentLength;

            // Adjusts part size
            long actualPartSize = AdjustPartSize(request.PartSize);
            var config = OssUtils.GetClientConfiguration(_serviceClient);
            if (fileSize <= actualPartSize)
            {
                using (Stream fs = File.Open(request.DownloadFile, FileMode.Create))
                {
                    using (var ossObject = await GetObjectAsync(request.ToGetObjectRequest(), cancellation))
                    {
                        var streamWrapper = ossObject.Content;
                        try
                        {
                            if (config.EnalbeMD5Check && !string.IsNullOrEmpty(objectMeta.ContentMd5))
                            {
                                byte[] expectedHashDigest = Convert.FromBase64String(objectMeta.ContentMd5); ;
                                streamWrapper = new MD5Stream(ossObject.Content, expectedHashDigest, fileSize);
                            }
                            else if (config.EnableCrcCheck && !string.IsNullOrEmpty(objectMeta.Crc64))
                            {
                                if (ulong.TryParse(objectMeta.Crc64, out var crcVal))
                                {
                                    byte[] expectedHashDigest = BitConverter.GetBytes(crcVal);
                                    streamWrapper = new Crc64Stream(ossObject.Content, expectedHashDigest, fileSize);
                                }
                            }

                            if (request.StreamTransferProgress != null)
                            {
                                streamWrapper = this.SetupProgressListeners(streamWrapper,
                                                                            objectMeta.ContentLength,
                                                                            0,
                                                                            config.ProgressUpdateInterval,
                                                                            request.StreamTransferProgress);
                            }
                            await ResumableDownloadManager.WriteToAsync(streamWrapper, fs,cancellation);
                        }
                        finally
                        {
                            if (!Object.Equals(streamWrapper, fs))
                            {
                                streamWrapper.Dispose();
                            }
                        }
                    }
                }

                return objectMeta;
            }

            ResumableDownloadContext resumableContext = this.LoadResumableDownloadContext(request.BucketName, request.Key, request.VersionId, objectMeta, request.CheckpointDir, actualPartSize);
            ResumableDownloadManager resumableDownloadManager = new ResumableDownloadManager(this, ((RetryableServiceClient)_serviceClient).MaxRetryTimes, config);
            resumableDownloadManager.ResumableDownloadWithRetry(request, resumableContext);
            resumableContext.Clear();
            return objectMeta;
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> ResumableUploadObjectAsync(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, string checkpointDir, long? partSize, EventHandler<StreamTransferProgressArgs> streamTransferProgress, CancellationToken cancellation)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            // calculates content-type
            metadata = metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref metadata);

            using (var fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await ResumableUploadObjectAsync(bucketName, key, fs, metadata, checkpointDir, partSize, streamTransferProgress,cancellation);
            }
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> ResumableUploadObjectAsync(string bucketName, string key, Stream content, ObjectMetadata metadata, string checkpointDir, long? partSize, EventHandler<StreamTransferProgressArgs> streamTransferProgress, CancellationToken cancellation)
        {
            UploadObjectRequest request = new UploadObjectRequest(bucketName, key, content);
            request.CheckpointDir = checkpointDir;
            request.PartSize = partSize;
            request.StreamTransferProgress = streamTransferProgress;
            request.Metadata = metadata;

            return await ResumableUploadObjectAsync(request,cancellation);
        }

        /// <inheritdoc/>    
        public async Task<PutObjectResult> ResumableUploadObjectAsync(UploadObjectRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);
            ThrowIfNullRequest(request.BucketName);
            ThrowIfNullRequest(request.Key);
            if (string.IsNullOrEmpty(request.UploadFile) && request.UploadStream == null)
            {
                throw new ArgumentException("Parameter request.UploadFile or request.UploadStream must not be null.");
            }

            if (request.UploadStream != null && !request.UploadStream.CanSeek)
            {
                throw new ArgumentException("Parameter request.UploadStream must be seekable---for nonseekable stream, please call UploadObject instead.");
            }

            // calculates content-type
            if (request.Metadata == null)
            {
                request.Metadata = new ObjectMetadata();
            }

            ObjectMetadata metadata = request.Metadata;
            SetContentTypeIfNull(request.Key, null, ref metadata);

            // Adjust part size
            long actualPartSize = AdjustPartSize(request.PartSize);

            // If the file size is less than the part size, upload it directly.
            long fileSize = 0;
            Stream uploadSteam = null;

            if (request.UploadStream != null)
            {
                fileSize = request.UploadStream.Length;
                uploadSteam = request.UploadStream;
            }
            else
            {
                fileSize = new System.IO.FileInfo(request.UploadFile).Length;
                uploadSteam = new FileStream(request.UploadFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            if (fileSize <= actualPartSize)
            {
                try
                {
                    var putObjectRequest = new PutObjectRequest(request.BucketName, request.Key, uploadSteam, metadata)
                    {
                        StreamTransferProgress = request.StreamTransferProgress,
                        RequestPayer = request.RequestPayer,
                        TrafficLimit = request.TrafficLimit
                    };
                    return await PutObjectAsync(putObjectRequest, cancellation);
                }
                finally
                {
                    uploadSteam.Dispose();
                }
            }

            var resumableContext = LoadResumableUploadContext(request.BucketName, request.Key, uploadSteam,
                                                              request.CheckpointDir, actualPartSize);

            if (resumableContext.UploadId == null)
            {
                var initRequest = new InitiateMultipartUploadRequest(request.BucketName, request.Key, metadata)
                {
                    RequestPayer = request.RequestPayer
                };

                var initResult = InitiateMultipartUpload(initRequest);
                resumableContext.UploadId = initResult.UploadId;
            }

            int maxRetry = ((RetryableServiceClient)_serviceClient).MaxRetryTimes;
            ClientConfiguration config = OssUtils.GetClientConfiguration(_serviceClient);
            ResumableUploadManager uploadManager = new ResumableUploadManager(this, maxRetry, config);
            uploadManager.ResumableUploadWithRetry(request, resumableContext);

            // Completes the upload
            var completeRequest = new CompleteMultipartUploadRequest(request.BucketName, request.Key, resumableContext.UploadId)
            {
                RequestPayer = request.RequestPayer
            };
            if (metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback))
            {
                var callbackMetadata = new ObjectMetadata();
                callbackMetadata.AddHeader(HttpHeaders.Callback, metadata.HttpMetadata[HttpHeaders.Callback]);
                completeRequest.Metadata = callbackMetadata;
            }

            foreach (var part in resumableContext.PartContextList)
            {
                if (part == null || !part.IsCompleted)
                {
                    throw new OssException("Not all parts are completed.");
                }

                completeRequest.PartETags.Add(part.PartETag);
            }

            PutObjectResult result = CompleteMultipartUpload(completeRequest);
            resumableContext.Clear();

            return result;
        }

        /// <inheritdoc/>    
        public async Task<OssObject> SelectObjectAsync(SelectObjectRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);

            var cmd = SelectObjectCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, request.BucketName, request.Key),
                                                      request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>    
        public async Task SetObjectAclAsync(string bucketName, string key, CannedAccessControlList acl, CancellationToken cancellation)
        {
            var setObjectAclRequest = new SetObjectAclRequest(bucketName, key, acl);
            await SetObjectAclAsync(setObjectAclRequest,cancellation);
        }

        /// <inheritdoc/>    
        public async Task SetObjectAclAsync(SetObjectAclRequest setObjectAclRequest, CancellationToken cancellation)
        {
            ThrowIfNullRequest(setObjectAclRequest);

            var cmd = SetObjectAclCommand.Create(_serviceClient,
                                                 _endpoint,
                                                 CreateContext(HttpMethod.Put, setObjectAclRequest.BucketName, setObjectAclRequest.Key),
                                                 setObjectAclRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>    
        public async Task SetObjectTaggingAsync(SetObjectTaggingRequest request, CancellationToken cancellation)
        {
            ThrowIfNullRequest(request);

            var cmd = SetObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, request.BucketName, request.Key),
                                                      request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }


        #endregion

        #region Multipart Operations
        /// <inheritdoc/>
        public async Task<MultipartUploadListing> ListMultipartUploadsAsync(ListMultipartUploadsRequest listMultipartUploadsRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(listMultipartUploadsRequest);
            var cmd = ListMultipartUploadsCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Get, listMultipartUploadsRequest.BucketName, null),
                                                        listMultipartUploadsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<InitiateMultipartUploadResult> InitiateMultipartUploadAsync(InitiateMultipartUploadRequest initiateMultipartUploadRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(initiateMultipartUploadRequest);
            var cmd = InitiateMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.Post, initiateMultipartUploadRequest.BucketName, initiateMultipartUploadRequest.Key),
                                                           initiateMultipartUploadRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task AbortMultipartUploadAsync(AbortMultipartUploadRequest abortMultipartUploadRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(abortMultipartUploadRequest);
            var cmd = AbortMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Delete, abortMultipartUploadRequest.BucketName, abortMultipartUploadRequest.Key),
                                                        abortMultipartUploadRequest);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>        
        public async Task<UploadPartResult> UploadPartAsync(UploadPartRequest uploadPartRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(uploadPartRequest);
            var cmd = UploadPartCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, uploadPartRequest.BucketName, uploadPartRequest.Key),
                                              uploadPartRequest);
            return await cmd.ExecuteAsync(cancellation);
        }


        /// <inheritdoc/>
        public async Task<UploadPartCopyResult> UploadPartCopyAsync(UploadPartCopyRequest uploadPartCopyRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(uploadPartCopyRequest);
            var cmd = UploadPartCopyCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, uploadPartCopyRequest.TargetBucket, uploadPartCopyRequest.TargetKey),
                                                  uploadPartCopyRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>                
        public async Task<PartListing> ListPartsAsync(ListPartsRequest listPartsRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(listPartsRequest);
            var cmd = ListPartsCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Get, listPartsRequest.BucketName, listPartsRequest.Key),
                                             listPartsRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>                
        public async Task<CompleteMultipartUploadResult> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest completeMultipartUploadRequest, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(completeMultipartUploadRequest);
            var cmd = CompleteMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.Post, completeMultipartUploadRequest.BucketName, completeMultipartUploadRequest.Key),
                                                           completeMultipartUploadRequest);
            return await cmd.ExecuteAsync(cancellation);
        }

        #endregion

        #region Live Channel

        /// <inheritdoc/>                
        public async Task<CreateLiveChannelResult> CreateLiveChannelAsync(CreateLiveChannelRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = CreateLiveChannelCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, request.BucketName, request.ChannelName),
                                                 request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>                
        public async Task<ListLiveChannelResult> ListLiveChannelAsync(ListLiveChannelRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = ListLiveChannelCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, request.BucketName, null),
                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task DeleteLiveChannelAsync(DeleteLiveChannelRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = DeleteLiveChannelCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, request.BucketName, request.ChannelName),
                                                   request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task SetLiveChannelStatusAsync(SetLiveChannelStatusRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = SetLiveChannelStatusCommand.Create(_serviceClient, _endpoint,
                                       CreateContext(HttpMethod.Put, request.BucketName, request.ChannelName),
                                       request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task<GetLiveChannelInfoResult> GetLiveChannelInfoAsync(GetLiveChannelInfoRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = GetLiveChannelInfoCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, request.BucketName, request.ChannelName),
                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetLiveChannelStatResult> GetLiveChannelStatAsync(GetLiveChannelStatRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = GetLiveChannelStatCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, request.BucketName, request.ChannelName),
                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task<GetLiveChannelHistoryResult> GetLiveChannelHistoryAsync(GetLiveChannelHistoryRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = GetLiveChannelHistoryCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, request.BucketName, request.ChannelName),
                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        /// <inheritdoc/>
        public async Task PostVodPlaylistAsync(PostVodPlaylistRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = PostVodPlaylistCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Post, request.BucketName, request.ChannelName + "/" + request.PlaylistName),
                                     request);
            using (await cmd.ExecuteAsync(cancellation))
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public async Task<GetVodPlaylistResult> GetVodPlaylistAsync(GetVodPlaylistRequest request, CancellationToken cancellation = default)
        {
            ThrowIfNullRequest(request);
            var cmd = GetVodPlaylistCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, request.BucketName, request.ChannelName),
                                     request);
            return await cmd.ExecuteAsync(cancellation);
        }

        #endregion
    }
}
