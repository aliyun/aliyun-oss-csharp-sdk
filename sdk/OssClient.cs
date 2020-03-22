/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

using Aliyun.OSS.Domain;
using Aliyun.OSS.Commands;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Model;
using Aliyun.OSS.Properties;
using Aliyun.OSS.Transform;
using ExecutionContext = Aliyun.OSS.Common.Communication.ExecutionContext;
using ICredentials = Aliyun.OSS.Common.Authentication.ICredentials;

namespace Aliyun.OSS
{
    /// <summary>
    /// The OSS's access entry point interface's implementation.
    /// </summary>
    public class OssClient : IOss
    {
        #region Fields & Properties

        private volatile Uri _endpoint;
        private readonly ICredentialsProvider _credsProvider;
        private readonly IServiceClient _serviceClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="OssClient" /> with OSS endpoint, access key Id, access key secret (cound be found from web console).
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">OSS access key Id</param>
        /// <param name="accessKeySecret">OSS key secret</param>
        public OssClient(string endpoint, string accessKeyId, string accessKeySecret)
            : this(FormatEndpoint(endpoint), accessKeyId, accessKeySecret) { }

        /// <summary>
        /// Creates an instance of <see cref="OssClient" /> with OSS endpoint, access key Id, access key secret (cound be found from web console) and STS token.
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">STS's temp access key Id</param>
        /// <param name="accessKeySecret">STS's temp access key secret</param>
        /// <param name="securityToken">STS security token</param>
        public OssClient(string endpoint, string accessKeyId, string accessKeySecret, string securityToken)
            : this(FormatEndpoint(endpoint), accessKeyId, accessKeySecret, securityToken) { }

        /// <summary>
        /// Creates an instance of <see cref="OssClient" /> with OSS endpoint, access key Id, access key secret and client configuration. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">OSS access key Id</param>
        /// <param name="accessKeySecret">OSS access key secret</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(string endpoint, string accessKeyId, string accessKeySecret, ClientConfiguration configuration)
            : this(FormatEndpoint(endpoint), new DefaultCredentialsProvider(new DefaultCredentials(accessKeyId, accessKeySecret, null)), configuration) { }

        /// <summary>
        /// Creates an instance of <see cref="OssClient" /> with OSS endpoint, access key Id, access key secret (cound be found from web console) and STS token.
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">STS's temp access key Id</param>
        /// <param name="accessKeySecret">STS's temp access key secret</param>
        /// <param name="securityToken">STS security token</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(string endpoint, string accessKeyId, string accessKeySecret, string securityToken, ClientConfiguration configuration)
            : this(endpoint, new DefaultCredentialsProvider(new DefaultCredentials(accessKeyId, accessKeySecret, securityToken)), configuration) { }

        /// <summary>
        /// Creates an instance with specified credential information.
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="credsProvider">Credentials information</param>
        public OssClient(string endpoint, ICredentialsProvider credsProvider)
            : this(FormatEndpoint(endpoint), credsProvider, new ClientConfiguration()) { }

        /// <summary>
        /// Creates an instance with specified credential information and client side configuration.
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="credsProvider">Credentials provider</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(string endpoint, ICredentialsProvider credsProvider, ClientConfiguration configuration)
            : this(FormatEndpoint(endpoint), credsProvider, configuration) { }

        /// <summary>
        /// Creates an instance with specified endpoint, access key Id and access key secret. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">OSS access key Id</param>
        /// <param name="accessKeySecret">OSS access key secret</param>
        public OssClient(Uri endpoint, string accessKeyId, string accessKeySecret)
            : this(endpoint, accessKeyId, accessKeySecret, null, new ClientConfiguration()) { }

        /// <summary>
        /// Creates an instance with specified endpoint, access key Id and access key secret and STS token. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">STS access key Id</param>
        /// <param name="accessKeySecret">STS security token</param>
        /// <param name="securityToken">STS security token</param>
        public OssClient(Uri endpoint, string accessKeyId, string accessKeySecret, string securityToken)
            : this(endpoint, accessKeyId, accessKeySecret, securityToken, new ClientConfiguration()) { }

        /// <summary>
        /// Creates an instance with specified endpoint, access key Id and access key secret and configuration. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">OSS access key id</param>
        /// <param name="accessKeySecret">OSS access key secret</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(Uri endpoint, string accessKeyId, string accessKeySecret, ClientConfiguration configuration)
            : this(endpoint, new DefaultCredentialsProvider(new DefaultCredentials(accessKeyId, accessKeySecret, null)), configuration) { }

        /// <summary>
        /// Creates an instance with specified endpoint, access key Id, access key secret, STS security token and configuration. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="accessKeyId">STS access key</param>
        /// <param name="accessKeySecret">STS access key secret</param>
        /// <param name="securityToken">STS security token</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(Uri endpoint, string accessKeyId, string accessKeySecret, string securityToken, ClientConfiguration configuration)
            : this(endpoint, new DefaultCredentialsProvider(new DefaultCredentials(accessKeyId, accessKeySecret, securityToken)), configuration) { }

        /// <summary>
        /// Creates an instance with specified endpoint and credential information. 
        /// </summary>
        /// <param name="endpoint">OSS的访问地址。</param>
        /// <param name="credsProvider">Credentials提供者。</param>
        public OssClient(Uri endpoint, ICredentialsProvider credsProvider)
            : this(endpoint, credsProvider, new ClientConfiguration()) { }

        /// <summary>
        /// Creates an instance with specified endpoint, credential information and credential information. 
        /// </summary>
        /// <param name="endpoint">OSS endpoint</param>
        /// <param name="credsProvider">Credentials information</param>
        /// <param name="configuration">client side configuration</param>
        public OssClient(Uri endpoint, ICredentialsProvider credsProvider, ClientConfiguration configuration)
        {
            if (endpoint == null)
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "endpoint");

            if (!endpoint.ToString().StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !endpoint.ToString().StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(OssResources.EndpointNotSupportedProtocal, "endpoint");

            if (credsProvider == null)
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "credsProvider");

            _endpoint = endpoint;
            _credsProvider = credsProvider;
            _serviceClient = ServiceClientFactory.CreateServiceClient(configuration ?? new ClientConfiguration());
        }

        #endregion

        #region Switch Credentials & Endpoint

        /// <inheritdoc/>
        public void SwitchCredentials(ICredentials creds)
        {
            if (creds == null)
                throw new ArgumentNullException("creds");
            _credsProvider.SetCredentials(creds);
        }

        /// <inheritdoc/>
        public void SetEndpoint(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        #endregion

        #region Bucket Operations

        /// <inheritdoc/>
        public Bucket CreateBucket(string bucketName)
        {
            return CreateBucket(bucketName, null);
        }

        /// <inheritdoc/>
        public Bucket CreateBucket(string bucketName, StorageClass? storageClass)
        {
            var cmd = CreateBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, bucketName, null),
                                                 bucketName, storageClass);
            using (cmd.Execute())
            {
                // Do nothing
            }

            return new Bucket(bucketName);
        }

        /// <inheritdoc/>
        public Bucket CreateBucket(CreateBucketRequest createBucketRequest)
        {
            ThrowIfNullRequest(createBucketRequest);

            var cmd = CreateBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, createBucketRequest.BucketName, null),
                                                 createBucketRequest.BucketName, createBucketRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }

            return new Bucket(createBucketRequest.BucketName);
        }
        /// <inheritdoc/>
        public void DeleteBucket(string bucketName)
        {
            var cmd = DeleteBucketCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Delete, bucketName, null),
                                                 bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Bucket> ListBuckets()
        {
            var cmd = ListBucketsCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Get, null, null), null);
            var result = cmd.Execute();
            return result.Buckets;
        }

        /// <inheritdoc/>
        public ListBucketsResult ListBuckets(ListBucketsRequest listBucketsRequest)
        {
            var cmd = ListBucketsCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Get, null, null), listBucketsRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public BucketInfo GetBucketInfo(string bucketName)
        {
            var cmd = GetBucketInfoCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, bucketName, null), bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public BucketStat GetBucketStat(string bucketName)
        {
            var cmd = GetBucketStatCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, bucketName, null), bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketAcl(string bucketName, CannedAccessControlList acl)
        {
            var setBucketAclRequest = new SetBucketAclRequest(bucketName, acl);
            SetBucketAcl(setBucketAclRequest);
        }

        /// <inheritdoc/>
        public void SetBucketAcl(SetBucketAclRequest setBucketAclRequest)
        {
            ThrowIfNullRequest(setBucketAclRequest);

            var cmd = SetBucketAclCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Put, setBucketAclRequest.BucketName, null),
                                                setBucketAclRequest.BucketName, setBucketAclRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public AccessControlList GetBucketAcl(string bucketName)
        {
            var cmd = GetBucketAclCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public BucketLocationResult GetBucketLocation(string bucketName)
        {
            var cmd = GetBucketLocationCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Get, bucketName, null),
                                                      bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public BucketMetadata GetBucketMetadata(string bucketName)
        {
            var cmd = GetBucketMetadataCommand.Create(_serviceClient, _endpoint,
                                          CreateContext(HttpMethod.Head, bucketName, null),
                                          bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketCors(SetBucketCorsRequest setBucketCorsRequest)
        {
            ThrowIfNullRequest(setBucketCorsRequest);
            var cmd = SetBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Put, setBucketCorsRequest.BucketName, null),
                                                 setBucketCorsRequest.BucketName,
                                                 setBucketCorsRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public IList<CORSRule> GetBucketCors(string bucketName)
        {
            var cmd = GetBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void DeleteBucketCors(string bucketName)
        {
            var cmd = DeleteBucketCorsCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Delete, bucketName, null),
                                                    bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void SetBucketLogging(SetBucketLoggingRequest setBucketLoggingRequest)
        {
            ThrowIfNullRequest(setBucketLoggingRequest);
            var cmd = SetBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Put, setBucketLoggingRequest.BucketName, null),
                                                    setBucketLoggingRequest.BucketName,
                                                    setBucketLoggingRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public BucketLoggingResult GetBucketLogging(string bucketName)
        {
            var cmd = GetBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, bucketName, null),
                                                    bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void DeleteBucketLogging(string bucketName)
        {
            var cmd = DeleteBucketLoggingCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Delete, bucketName, null),
                                                        bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void SetBucketWebsite(SetBucketWebsiteRequest setBucketWebSiteRequest)
        {
            ThrowIfNullRequest(setBucketWebSiteRequest);
            var cmd = SetBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Put, setBucketWebSiteRequest.BucketName, null),
                                                     setBucketWebSiteRequest.BucketName,
                                                     setBucketWebSiteRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public BucketWebsiteResult GetBucketWebsite(string bucketName)
        {
            var cmd = GetBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Get, bucketName, null),
                                                    bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void DeleteBucketWebsite(string bucketName)
        {
            var cmd = DeleteBucketWebsiteCommand.Create(_serviceClient, _endpoint,
                                                       CreateContext(HttpMethod.Delete, bucketName, null),
                                                       bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }


        /// <inheritdoc/>
        public void SetBucketReferer(SetBucketRefererRequest setBucketRefererRequest)
        {
            ThrowIfNullRequest(setBucketRefererRequest);
            var cmd = SetBucketRefererCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Put, setBucketRefererRequest.BucketName, null),
                                                    setBucketRefererRequest.BucketName,
                                                    setBucketRefererRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public RefererConfiguration GetBucketReferer(string bucketName)
        {
            var cmd = GetBucketRefererCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Get, bucketName, null),
                                                     bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketLifecycle(SetBucketLifecycleRequest setBucketLifecycleRequest)
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
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void DeleteBucketLifecycle(string bucketName)
        {
            var cmd = DeleteBucketLifecycleCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, bucketName, null),
                                                   bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public IList<LifecycleRule> GetBucketLifecycle(string bucketName)
        {
            var cmd = GetBucketLifecycleCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Get, bucketName, null),
                                                      bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketStorageCapacity(SetBucketStorageCapacityRequest setBucketStorageCapacityRequest)
        {
            ThrowIfNullRequest(setBucketStorageCapacityRequest);
            var cmd = SetBucketStorageCapacityCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Put, setBucketStorageCapacityRequest.BucketName, null),
                                                             setBucketStorageCapacityRequest.BucketName,
                                                             setBucketStorageCapacityRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetBucketStorageCapacityResult GetBucketStorageCapacity(string bucketName)
        {
            var cmd = GetBucketStorageCapacityCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return cmd.Execute();
        }


        /// <inheritdoc/>
        public void SetBucketTagging(SetBucketTaggingRequest setBucketTaggingRequest)
        {
            ThrowIfNullRequest(setBucketTaggingRequest);

            var cmd = SetBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketTaggingRequest.BucketName, null),
                                                      setBucketTaggingRequest.BucketName,
                                                      setBucketTaggingRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void DeleteBucketTagging(string bucketName)
        {
            var cmd = DeleteBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, bucketName, null),
                                                   bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetBucketTaggingResult GetBucketTagging(string bucketName)
        {
            var cmd = GetBucketTaggingCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public bool DoesBucketExist(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "bucketName");
            if (!OssUtils.IsBucketNameValid(bucketName))
                throw new ArgumentException(OssResources.BucketNameInvalid, "bucketName");

            try
            {
                GetBucketAcl(bucketName);
            }
            catch (OssException e)
            {
                if (e.ErrorCode.Equals(OssErrorCode.NoSuchBucket))
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public void SetBucketPolicy(SetBucketPolicyRequest setBucketPolicyRequest)
        {
            ThrowIfNullRequest(setBucketPolicyRequest);

            var cmd = SetBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Put, setBucketPolicyRequest.BucketName, null),
                                                setBucketPolicyRequest.BucketName, setBucketPolicyRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetBucketPolicyResult GetBucketPolicy(string bucketName)
        {
            var cmd = GetBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void DeleteBucketPolicy(string bucketName)
        {
            var cmd = DeleteBucketPolicyCommand.Create(_serviceClient, _endpoint,
                                                    CreateContext(HttpMethod.Delete, bucketName, null),
                                                    bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void SetBucketRequestPayment(SetBucketRequestPaymentRequest setBucketRequestPaymentRequest)
        {
            ThrowIfNullRequest(setBucketRequestPaymentRequest);

            var cmd = SetBucketRequestPaymentCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketRequestPaymentRequest.BucketName, null),
                                                      setBucketRequestPaymentRequest.BucketName,
                                                      setBucketRequestPaymentRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetBucketRequestPaymentResult GetBucketRequestPayment(string bucketName)
        {
            var cmd = GetBucketRequestPaymentCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, bucketName, null),
                                                             bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketEncryption(SetBucketEncryptionRequest setBucketEncryptionRequest)
        {
            ThrowIfNullRequest(setBucketEncryptionRequest);

            var cmd = SetBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketEncryptionRequest.BucketName, null),
                                                      setBucketEncryptionRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void DeleteBucketEncryption(string bucketName)
        {
            var cmd = DeleteBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                       CreateContext(HttpMethod.Delete, bucketName, null),
                                       bucketName);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public BucketEncryptionResult GetBucketEncryption(string bucketName)
        {
            var cmd = GetBucketEncryptionCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Get, bucketName, null),
                                                 bucketName);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public void SetBucketVersioning(SetBucketVersioningRequest setBucketVersioningRequest)
        {
            ThrowIfNullRequest(setBucketVersioningRequest);

            var cmd = SetBucketVersioningCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setBucketVersioningRequest.BucketName, null),
                                                      setBucketVersioningRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetBucketVersioningResult GetBucketVersioning(string bucketName)
        {
            var cmd = GetBucketVersioningCommand.Create(_serviceClient, _endpoint,
                                     CreateContext(HttpMethod.Get, bucketName, null),
                                     bucketName);
            return cmd.Execute();
        }

        #endregion

        #region Object Operations

        /// <inheritdoc/>
        public ObjectListing ListObjects(string bucketName)
        {
            return ListObjects(bucketName, null);
        }

        /// <inheritdoc/>
        public IAsyncResult BeginListObjects(string bucketName, AsyncCallback callback, object state)
        {
            return BeginListObjects(bucketName, null, callback, state);
        }

        /// <inheritdoc/>
        public ObjectListing ListObjects(string bucketName, string prefix)
        {
            var listObjectsRequest = new ListObjectsRequest(bucketName)
            {
                Prefix = prefix
            };
            return ListObjects(listObjectsRequest);
        }

        /// <inheritdoc/>
        public IAsyncResult BeginListObjects(string bucketName, string prefix, AsyncCallback callback, object state)
        {
            var listObjectsRequest = new ListObjectsRequest(bucketName)
            {
                Prefix = prefix
            };
            return BeginListObjects(listObjectsRequest, callback, state);
        }

        /// <inheritdoc/>
        public ObjectListing ListObjects(ListObjectsRequest listObjectsRequest)
        {
            ThrowIfNullRequest(listObjectsRequest);
            var cmd = ListObjectsCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, listObjectsRequest.BucketName, null),
                                               listObjectsRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginListObjects(ListObjectsRequest listObjectsRequest, AsyncCallback callback, object state)
        {
            if (listObjectsRequest == null)
                throw new ArgumentNullException("listObjectsRequest");

            var cmd = ListObjectsCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, listObjectsRequest.BucketName, null),
                                               listObjectsRequest);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public ObjectListing EndListObjects(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<ObjectListing>(_serviceClient, asyncResult);
        }

        public ObjectVersionList ListObjectVersions(ListObjectVersionsRequest listObjectVersionsRequest)
        {
            ThrowIfNullRequest(listObjectVersionsRequest);
            var cmd = ListObjectVersionsCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Get, listObjectVersionsRequest.BucketName, null),
                                               listObjectVersionsRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(string bucketName, string key, Stream content)
        {
            return PutObject(bucketName, key, content, null);
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPutObject(string bucketName, string key, Stream content, AsyncCallback callback, object state)
        {
            return BeginPutObject(bucketName, key, content, null, callback, state);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(string bucketName, string key, Stream content, ObjectMetadata metadata)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest(bucketName, key, content, metadata);
            return PutObject(putObjectRequest);
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPutObject(string bucketName, string key, Stream content, ObjectMetadata metadata,
            AsyncCallback callback, object state)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest(bucketName, key, content, metadata);
            return BeginPutObject(putObjectRequest, callback, state);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(PutObjectRequest putObjectRequest)
        {
            ObjectMetadata metadata = putObjectRequest.Metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(putObjectRequest.Key, null, ref metadata);
            putObjectRequest.Metadata = metadata;

            var cmd = PutObjectCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, putObjectRequest.BucketName, putObjectRequest.Key),
                                              putObjectRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPutObject(PutObjectRequest putObjectRequest, AsyncCallback callback, object state)
        {
            ObjectMetadata metadata = putObjectRequest.Metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(putObjectRequest.Key, null, ref metadata);

            var cmd = PutObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Put, putObjectRequest.BucketName, putObjectRequest.Key),
                                             putObjectRequest);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(string bucketName, string key, string fileToUpload)
        {
            return PutObject(bucketName, key, fileToUpload, null);
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload, AsyncCallback callback, object state)
        {
            return BeginPutObject(bucketName, key, fileToUpload, null, callback, state);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            metadata = metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref metadata);

            PutObjectResult result;
            using (Stream content = File.OpenRead(fileToUpload))
            {
                result = PutObject(bucketName, key, content, metadata);
            }
            return result;
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPutObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata,
            AsyncCallback callback, object state)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            metadata = metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref metadata);

            IAsyncResult result;
            Stream content = File.OpenRead(fileToUpload); // content will be disposed after EndPutObject is called;
            result = BeginPutObject(bucketName, key, content, metadata, callback, state);
            return result;
        }

        /// <inheritdoc/>
        public PutObjectResult EndPutObject(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<PutObjectResult>(_serviceClient, asyncResult);
        }


        /// <inheritdoc/>
        public PutObjectResult PutObject(Uri signedUrl, string fileToUpload)
        {
            return PutObject(signedUrl, fileToUpload, null);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(Uri signedUrl, string fileToUpload, ObjectMetadata metadata)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            PutObjectResult result;
            using (Stream content = File.OpenRead(fileToUpload))
            {
                result = PutObject(signedUrl, content, metadata);
            }
            return result;
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(Uri signedUrl, Stream content)
        {
            return PutObject(signedUrl, content, null);
        }

        /// <inheritdoc/>
        public PutObjectResult PutObject(Uri signedUrl, Stream content, ObjectMetadata metadata)
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
            var serviceResponse = _serviceClient.Send(request, context);

            // build result
            var putObjectRequest = new PutObjectRequest(null, null, null, metadata);
            var ResponseDeserializer = new PutObjectResponseDeserializer(putObjectRequest);
            return ResponseDeserializer.Deserialize(serviceResponse);
        }

        /// <inheritdoc/>
        public PutObjectResult PutBigObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, long? partSize = null)
        {
            return ResumableUploadObject(bucketName, key, fileToUpload, metadata, null, partSize);
        }

        /// <inheritdoc/>
        public PutObjectResult PutBigObject(string bucketName, string key, Stream content, ObjectMetadata metadata, long? partSize = null)
        {
            return ResumableUploadObject(bucketName, key, content, metadata, null, partSize);
        }

        /// <inheritdoc/>
        public PutObjectResult ResumableUploadObject(string bucketName, string key, string fileToUpload, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                                     EventHandler<StreamTransferProgressArgs> streamTransferProgress = null)
        {
            if (!File.Exists(fileToUpload) || Directory.Exists(fileToUpload))
                throw new ArgumentException(String.Format("Invalid file path {0}.", fileToUpload));

            // calculates content-type
            metadata = metadata ?? new ObjectMetadata();
            SetContentTypeIfNull(key, fileToUpload, ref metadata);

            using (var fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ResumableUploadObject(bucketName, key, fs, metadata, checkpointDir, partSize, streamTransferProgress);
            }
        }

        public PutObjectResult ResumableUploadObject(string bucketName, string key, Stream content, ObjectMetadata metadata, string checkpointDir, long? partSize = null,
                                                     EventHandler<StreamTransferProgressArgs> streamTransferProgress = null)
        {
            UploadObjectRequest request = new UploadObjectRequest(bucketName, key, content);
            request.CheckpointDir = checkpointDir;
            request.PartSize = partSize;
            request.StreamTransferProgress = streamTransferProgress;
            request.Metadata = metadata;

            return ResumableUploadObject(request);
        }

        public PutObjectResult ResumableUploadObject(UploadObjectRequest request)
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
                    return PutObject(putObjectRequest);
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
        public AppendObjectResult AppendObject(AppendObjectRequest request)
        {
            ThrowIfNullRequest(request);

            var cmd = AppendObjectCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, request.BucketName, request.Key),
                                                 request);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginAppendObject(AppendObjectRequest request, AsyncCallback callback, Object state)
        {
            ThrowIfNullRequest(request);

            var cmd = AppendObjectCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, request.BucketName, request.Key),
                                                 request);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public AppendObjectResult EndAppendObject(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<AppendObjectResult>(_serviceClient, asyncResult);
        }

        /// <inheritdoc/>
        public CreateSymlinkResult CreateSymlink(string bucketName, string symlink, string target)
        {
            var cmd = CreateSymlinkCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, bucketName, symlink),
                                                  new CreateSymlinkRequest(bucketName, symlink, target));
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public CreateSymlinkResult CreateSymlink(CreateSymlinkRequest createSymlinkRequest)
        {
            var cmd = CreateSymlinkCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, createSymlinkRequest.BucketName, createSymlinkRequest.Symlink),
                                                  createSymlinkRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public OssSymlink GetSymlink(string bucketName, string symlink)
        {
            var cmd = GetSymlinkCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, bucketName, symlink),
                                               new GetSymlinkResultDeserializer(),
                                               new GetSymlinkRequest(bucketName, symlink));
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public OssSymlink GetSymlink(GetSymlinkRequest getSymlinkRequest)
        {
            var cmd = GetSymlinkCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, getSymlinkRequest.BucketName, getSymlinkRequest.Key),
                                               new GetSymlinkResultDeserializer(),
                                               getSymlinkRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public OssObject GetObject(Uri signedUrl)
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
            var serviceResponse = _serviceClient.Send(request, context);

            // build result
            var getObjectRequest = new GetObjectRequest(null, null);
            var ResponseDeserializer = new GetObjectResponseDeserializer(getObjectRequest, _serviceClient);
            return ResponseDeserializer.Deserialize(serviceResponse);
        }

        /// <inheritdoc/>
        public OssObject GetObject(string bucketName, string key)
        {
            return GetObject(new GetObjectRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public IAsyncResult BeginGetObject(string bucketName, string key, AsyncCallback callback, Object state)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest(bucketName, key);
            return BeginGetObject(getObjectRequest, callback, state);
        }

        /// <inheritdoc/>
        public OssObject GetObject(GetObjectRequest getObjectRequest)
        {
            ThrowIfNullRequest(getObjectRequest);

            var cmd = GetObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Get, getObjectRequest.BucketName, getObjectRequest.Key),
                                             getObjectRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginGetObject(GetObjectRequest getObjectRequest, AsyncCallback callback, Object state)
        {
            ThrowIfNullRequest(getObjectRequest);

            var cmd = GetObjectCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Get, getObjectRequest.BucketName, getObjectRequest.Key),
                                             getObjectRequest);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public OssObject EndGetObject(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<OssObject>(_serviceClient, asyncResult);
        }

        /// <inheritdoc/>
        public ObjectMetadata GetObject(GetObjectRequest getObjectRequest, Stream output)
        {
            var ossObject = GetObject(getObjectRequest);
            using(ossObject.Content)
            {
                IoUtils.WriteTo(ossObject.Content, output);
            }
            return ossObject.Metadata;
        }

        /// <inheritdoc/>
        public ObjectMetadata GetObjectMetadata(string bucketName, string key)
        {
            return GetObjectMetadata(new GetObjectMetadataRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public ObjectMetadata GetObjectMetadata(GetObjectMetadataRequest request)
        {
            var cmd = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Head, request.BucketName, request.Key),
                                                     request);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public ObjectMetadata GetSimplifiedObjectMetadata(GetObjectMetadataRequest request)
        {
            var cmd = GetObjectMetadataCommand.Create(_serviceClient, _endpoint,
                                                     CreateContext(HttpMethod.Head, request.BucketName, request.Key),
                                                     request, true);
            return cmd.Execute();
        }


    /// <inheritdoc/>
    public ObjectMetadata ResumableDownloadObject(DownloadObjectRequest request)
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
            ObjectMetadata objectMeta = this.GetObjectMetadata(metaRequest);
            var fileSize = objectMeta.ContentLength;

            // Adjusts part size
            long actualPartSize = AdjustPartSize(request.PartSize);
            var config = OssUtils.GetClientConfiguration(_serviceClient);
            if (fileSize <= actualPartSize)
            {
                using (Stream fs = File.Open(request.DownloadFile, FileMode.Create))
                {
                    using(var ossObject = GetObject(request.ToGetObjectRequest()))
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
                                ulong crcVal = 0;
                                if (UInt64.TryParse(objectMeta.Crc64, out crcVal))
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
                            ResumableDownloadManager.WriteTo(streamWrapper, fs);
                        }
                        finally
                        {
                            if(!Object.Equals(streamWrapper, fs))
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
        public DeleteObjectResult DeleteObject(string bucketName, string key)
        {
            return DeleteObject(new DeleteObjectRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public DeleteObjectResult DeleteObject(DeleteObjectRequest deleteObjectRequest)
        {
            var cmd = DeleteObjectCommand.Create(_serviceClient, _endpoint,
                                                CreateContext(HttpMethod.Delete, deleteObjectRequest.BucketName, deleteObjectRequest.Key),
                                                deleteObjectRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public DeleteObjectsResult DeleteObjects(DeleteObjectsRequest deleteObjectsRequest)
        {
            ThrowIfNullRequest(deleteObjectsRequest);

            var cmd = DeleteObjectsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, deleteObjectsRequest.BucketName, null),
                                                 deleteObjectsRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public DeleteObjectVersionsResult DeleteObjectVersions(DeleteObjectVersionsRequest deleteObjectVersionsRequest)
        {
            ThrowIfNullRequest(deleteObjectVersionsRequest);

            var cmd = DeleteObjectVersionsCommand.Create(_serviceClient, _endpoint,
                                                 CreateContext(HttpMethod.Post, deleteObjectVersionsRequest.BucketName, null),
                                                 deleteObjectVersionsRequest);
            return cmd.Execute();
        }


        /// <inheritdoc/>
        public CopyObjectResult CopyObject(CopyObjectRequest copyObjectRequst)
        {
            ThrowIfNullRequest(copyObjectRequst);
            var cmd = CopyObjectCommand.Create(_serviceClient, _endpoint,
                                               CreateContext(HttpMethod.Put, copyObjectRequst.DestinationBucketName, copyObjectRequst.DestinationKey),
                                               copyObjectRequst);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCopyObject(CopyObjectRequest copyObjectRequst, AsyncCallback callback, Object state)
        {
            ThrowIfNullRequest(copyObjectRequst);

            var cmd = CopyObjectCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, copyObjectRequst.DestinationBucketName, copyObjectRequst.DestinationKey),
                                              copyObjectRequst);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public CopyObjectResult EndCopyResult(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<CopyObjectResult>(_serviceClient, asyncResult);
        }

        /// <inheritdoc/>
        public CopyObjectResult CopyBigObject(CopyObjectRequest copyObjectRequest, long? partSize = null, string checkpointDir = null)
        {
            return ResumableCopyObject(copyObjectRequest, checkpointDir, partSize);
        }

        /// <inheritdoc/>
        public CopyObjectResult ResumableCopyObject(CopyObjectRequest copyObjectRequest, string checkpointDir, long? partSize = null)
        {
            ThrowIfNullRequest(copyObjectRequest);

            // Adjusts part size
            long actualPartSize = AdjustPartSize(partSize);

            // Gets the file size
            var metaRequest = new GetObjectMetadataRequest(copyObjectRequest.SourceBucketName, copyObjectRequest.SourceKey)
            {
                RequestPayer = copyObjectRequest.RequestPayer,
                VersionId = copyObjectRequest.SourceVersionId
            };
            var objectMeta = GetObjectMetadata(metaRequest);
            var fileSize = objectMeta.ContentLength;

            if (fileSize <= actualPartSize)
            {
                return CopyObject(copyObjectRequest);
            }

            var resumableCopyContext = LoadResumableCopyContext(copyObjectRequest, objectMeta, checkpointDir, actualPartSize);

            if (resumableCopyContext.UploadId == null)
            {
                var initRequest = new InitiateMultipartUploadRequest(copyObjectRequest.DestinationBucketName,
                                                                     copyObjectRequest.DestinationKey,
                                                                     copyObjectRequest.NewObjectMetadata)
                {
                    RequestPayer = copyObjectRequest.RequestPayer
                };
                var initResult = InitiateMultipartUpload(initRequest);
                resumableCopyContext.UploadId = initResult.UploadId;
            }
            
            // Executes the copy
            ResumableCopyWithRetry(copyObjectRequest, resumableCopyContext);

            // Completes the copy
            var completeRequest = new CompleteMultipartUploadRequest(copyObjectRequest.DestinationBucketName,
                                                                     copyObjectRequest.DestinationKey, resumableCopyContext.UploadId)
            {
                RequestPayer = copyObjectRequest.RequestPayer
            };
            foreach (var part in resumableCopyContext.PartContextList)
            {
                completeRequest.PartETags.Add(part.PartETag);
            }
            var result = CompleteMultipartUpload(completeRequest);

            resumableCopyContext.Clear();

            // Gets the last modified time
            metaRequest = new GetObjectMetadataRequest(copyObjectRequest.DestinationBucketName, copyObjectRequest.DestinationKey)
            {
                RequestPayer = copyObjectRequest.RequestPayer,
                VersionId = result.VersionId
            };
            objectMeta = GetObjectMetadata(metaRequest);
            return new CopyObjectResult() { ETag = result.ETag, LastModified = objectMeta.LastModified };
        }

        /// <inheritdoc/>
        public void ModifyObjectMeta(string bucketName, string key, ObjectMetadata newMeta, long? partSize = null, string checkpointDir = null)
        {
            var copyObjectRequest = new CopyObjectRequest(bucketName, key, bucketName, key)
            {
                NewObjectMetadata = newMeta
            };

            CopyBigObject(copyObjectRequest, partSize, checkpointDir);
        }

        /// <inheritdoc/>
        public bool DoesObjectExist(string bucketName, string key)
        {
            try
            {
                var cmd = HeadObjectCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Head, bucketName, key),
                                                  bucketName, key);

                using (cmd.Execute())
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
        public void SetObjectAcl(string bucketName, string key, CannedAccessControlList acl)
        {
            var setObjectAclRequest = new SetObjectAclRequest(bucketName, key, acl);
            SetObjectAcl(setObjectAclRequest);
        }

        /// <inheritdoc/>
        public void SetObjectAcl(SetObjectAclRequest setObjectAclRequest)
        {
            ThrowIfNullRequest(setObjectAclRequest);

            var cmd = SetObjectAclCommand.Create(_serviceClient, 
                                                 _endpoint,
                                                 CreateContext(HttpMethod.Put, setObjectAclRequest.BucketName, setObjectAclRequest.Key),
                                                 setObjectAclRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public AccessControlList GetObjectAcl(string bucketName, string key)
        {
            return GetObjectAcl(new GetObjectAclRequest(bucketName, key));
        }

        public AccessControlList GetObjectAcl(GetObjectAclRequest getObjectAclRequest)
        {
            var cmd = GetObjectAclCommand.Create(_serviceClient,
                                                 _endpoint,
                                                 CreateContext(HttpMethod.Get, getObjectAclRequest.BucketName, getObjectAclRequest.Key),
                                                 getObjectAclRequest);
            return cmd.Execute();
        }

    /// <inheritdoc/>
    public RestoreObjectResult RestoreObject(string bucketName, string key)
        {
            return RestoreObject(new RestoreObjectRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public RestoreObjectResult RestoreObject(RestoreObjectRequest restoreObjectRequest)
        {
            ExecutionContext context = CreateContext(HttpMethod.Post, restoreObjectRequest.BucketName, restoreObjectRequest.Key);
            var cmd = RestoreObjectCommand.Create(_serviceClient,
                                                  _endpoint,
                                                  context,
                                                  restoreObjectRequest);
            return cmd.Execute();
        }


        /// <inheritdoc/>
        public void SetObjectTagging(SetObjectTaggingRequest setObjectTaggingRequest)
        {
            ThrowIfNullRequest(setObjectTaggingRequest);

            var cmd = SetObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                      CreateContext(HttpMethod.Put, setObjectTaggingRequest.BucketName, setObjectTaggingRequest.Key),
                                                      setObjectTaggingRequest);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public void DeleteObjectTagging(string bucketName, string key)
        {
            DeleteObjectTagging(new DeleteObjectTaggingRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public void DeleteObjectTagging(DeleteObjectTaggingRequest request)
        {
            var cmd = DeleteObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                   CreateContext(HttpMethod.Delete, request.BucketName, request.Key),
                                                   request);
            using (cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>
        public GetObjectTaggingResult GetObjectTagging(string bucketName, string key)
        {
            return GetObjectTagging(new GetObjectTaggingRequest(bucketName, key));
        }

        /// <inheritdoc/>
        public GetObjectTaggingResult GetObjectTagging(GetObjectTaggingRequest request)
        {
            var cmd = GetObjectTaggingCommand.Create(_serviceClient, _endpoint,
                                                             CreateContext(HttpMethod.Get, request.BucketName, request.Key),
                                                             request);
            return cmd.Execute();
        }

        #endregion

        #region Generate URL

        /// <inheritdoc/>        
        public Uri GeneratePresignedUri(string bucketName, string key)
        {
            var request = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
            return GeneratePresignedUri(request);
        }

        /// <inheritdoc/> 
        public Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration)
        {
            var request = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get)
            {
                Expiration = expiration
            };
            return GeneratePresignedUri(request);
        }
        
        /// <inheritdoc/>        
        public Uri GeneratePresignedUri(string bucketName, string key, SignHttpMethod method)
        {
            var request = new GeneratePresignedUriRequest(bucketName, key, method);
            return GeneratePresignedUri(request);            
        }

        /// <inheritdoc/>  
        public Uri GeneratePresignedUri(string bucketName, string key, DateTime expiration, 
            SignHttpMethod method)
        {
            var request = new GeneratePresignedUriRequest(bucketName, key, method)
            {
                Expiration = expiration
            };
            return GeneratePresignedUri(request);
        }
        
        /// <inheritdoc/>        
        public Uri GeneratePresignedUri(GeneratePresignedUriRequest generatePresignedUriRequest)
        {
            ThrowIfNullRequest(generatePresignedUriRequest);

            var creds = _credsProvider.GetCredentials();
            var accessKeyId = creds.AccessKeyId;
            var accessKeySecret = creds.AccessKeySecret;
            var securityToken = creds.SecurityToken;
            var useToken = creds.UseToken;
            var bucketName = generatePresignedUriRequest.BucketName;
            var key = generatePresignedUriRequest.Key;
            
            const long ticksOf1970 = 621355968000000000;
            var expires = ((generatePresignedUriRequest.Expiration.ToUniversalTime().Ticks - ticksOf1970) / 10000000L)
                .ToString(CultureInfo.InvariantCulture);
            var resourcePath = OssUtils.MakeResourcePath(_endpoint, bucketName, key);
                
            var request = new ServiceRequest();
            var conf = OssUtils.GetClientConfiguration(_serviceClient);
            request.Endpoint = OssUtils.MakeBucketEndpoint(_endpoint, bucketName, conf);
            request.ResourcePath = resourcePath;

            switch (generatePresignedUriRequest.Method)
            {
                case SignHttpMethod.Get:
                    request.Method = HttpMethod.Get;
                    break;
                case SignHttpMethod.Put:
                    request.Method = HttpMethod.Put;
                    break;
                default:
                    throw new ArgumentException("Unsupported http method.");
            }
            
            request.Headers.Add(HttpHeaders.Date, expires);
            if (!string.IsNullOrEmpty(generatePresignedUriRequest.ContentType))
                request.Headers.Add(HttpHeaders.ContentType, generatePresignedUriRequest.ContentType);
            if (!string.IsNullOrEmpty(generatePresignedUriRequest.ContentMd5))
                request.Headers.Add(HttpHeaders.ContentMd5, generatePresignedUriRequest.ContentMd5);
            if (!string.IsNullOrEmpty(generatePresignedUriRequest.Callback))
                request.Headers.Add(HttpHeaders.Callback, generatePresignedUriRequest.Callback);
            if (!string.IsNullOrEmpty(generatePresignedUriRequest.CallbackVar))
                request.Headers.Add(HttpHeaders.CallbackVar, generatePresignedUriRequest.CallbackVar);

            foreach (var pair in generatePresignedUriRequest.UserMetadata)
                request.Headers.Add(OssHeaders.OssUserMetaPrefix + pair.Key, pair.Value);
            
            if (generatePresignedUriRequest.ResponseHeaders != null)
                generatePresignedUriRequest.ResponseHeaders.Populate(request.Parameters);

            foreach (var param in generatePresignedUriRequest.QueryParams)
                request.Parameters.Add(param.Key, param.Value);

            if (useToken)
                request.Parameters.Add(RequestParameters.SECURITY_TOKEN, securityToken);
            
            var canonicalResource = "/" + (bucketName ?? "") + ((key != null ? "/" + key : ""));
            var httpMethod = generatePresignedUriRequest.Method.ToString().ToUpperInvariant();
            
            var canonicalString =
                SignUtils.BuildCanonicalString(httpMethod, canonicalResource, request/*, expires*/);
            var signature = ServiceSignature.Create().ComputeSignature(accessKeySecret, canonicalString);

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(RequestParameters.EXPIRES, expires);
            queryParams.Add(RequestParameters.OSS_ACCESS_KEY_ID, accessKeyId);
            queryParams.Add(RequestParameters.SIGNATURE, signature);
            foreach (var param in request.Parameters)
                queryParams.Add(param.Key, param.Value);

            var queryString = HttpUtils.ConbineQueryString(queryParams);
            var uriString = request.Endpoint.ToString();
            if (!uriString.EndsWith("/"))
                uriString += "/";
            uriString += resourcePath + "?" + queryString;
            
            return new Uri(uriString);
        }
        
        #endregion

        #region Generate Post Policy

        /// <inheritdoc/>
        public string GeneratePostPolicy(DateTime expiration, PolicyConditions conds)
        {
            if (conds == null)
            {
                throw new ArgumentNullException("conds");
            }

            var formatedExpiration = DateUtils.FormatIso8601Date(expiration);
            var jsonizedExpiration = string.Format("\"expiration\":\"{0}\"", formatedExpiration);
            var jsonizedConds = conds.Jsonize();
            return String.Format("{{{0},{1}}}", jsonizedExpiration, jsonizedConds);
        }

        #endregion

        #region Multipart Operations
        /// <inheritdoc/>
        public MultipartUploadListing ListMultipartUploads(ListMultipartUploadsRequest listMultipartUploadsRequest)
        {
            ThrowIfNullRequest(listMultipartUploadsRequest);
            var cmd = ListMultipartUploadsCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Get, listMultipartUploadsRequest.BucketName, null),
                                                        listMultipartUploadsRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public InitiateMultipartUploadResult InitiateMultipartUpload(InitiateMultipartUploadRequest initiateMultipartUploadRequest)
        {
            ThrowIfNullRequest(initiateMultipartUploadRequest);
            var cmd = InitiateMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.Post, initiateMultipartUploadRequest.BucketName, initiateMultipartUploadRequest.Key),
                                                           initiateMultipartUploadRequest);
            return cmd.Execute();
        }
        
        /// <inheritdoc/>
        public void AbortMultipartUpload(AbortMultipartUploadRequest abortMultipartUploadRequest)
        {
            ThrowIfNullRequest(abortMultipartUploadRequest);
            var cmd = AbortMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                        CreateContext(HttpMethod.Delete, abortMultipartUploadRequest.BucketName, abortMultipartUploadRequest.Key),
                                                        abortMultipartUploadRequest);
            using(cmd.Execute())
            {
                // Do nothing
            }
        }

        /// <inheritdoc/>        
        public UploadPartResult UploadPart(UploadPartRequest uploadPartRequest)
        {
            ThrowIfNullRequest(uploadPartRequest);
            var cmd = UploadPartCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, uploadPartRequest.BucketName, uploadPartRequest.Key),
                                              uploadPartRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>        
        public IAsyncResult BeginUploadPart(UploadPartRequest uploadPartRequest, AsyncCallback callback, object state)
        {
            ThrowIfNullRequest(uploadPartRequest);

            var cmd = UploadPartCommand.Create(_serviceClient, _endpoint,
                                              CreateContext(HttpMethod.Put, uploadPartRequest.BucketName, uploadPartRequest.Key),
                                              uploadPartRequest);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>        
        public UploadPartResult EndUploadPart(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<UploadPartResult>(_serviceClient, asyncResult);
        }

        /// <inheritdoc/>
        public UploadPartCopyResult UploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest)
        {
            ThrowIfNullRequest(uploadPartCopyRequest);
            var cmd = UploadPartCopyCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, uploadPartCopyRequest.TargetBucket, uploadPartCopyRequest.TargetKey),
                                                  uploadPartCopyRequest);
            return cmd.Execute();
        }

        /// <inheritdoc/>
        public IAsyncResult BeginUploadPartCopy(UploadPartCopyRequest uploadPartCopyRequest,
            AsyncCallback callback, object state)
        {
            ThrowIfNullRequest(uploadPartCopyRequest);

            var cmd = UploadPartCopyCommand.Create(_serviceClient, _endpoint,
                                                  CreateContext(HttpMethod.Put, uploadPartCopyRequest.TargetBucket, uploadPartCopyRequest.TargetKey),
                                                  uploadPartCopyRequest);
            return OssUtils.BeginOperationHelper(cmd, callback, state);
        }

        /// <inheritdoc/>
        public UploadPartCopyResult EndUploadPartCopy(IAsyncResult asyncResult)
        {
            return OssUtils.EndOperationHelper<UploadPartCopyResult>(_serviceClient, asyncResult);
        }


        /// <inheritdoc/>                
        public PartListing ListParts(ListPartsRequest listPartsRequest)
        {
            ThrowIfNullRequest(listPartsRequest);
            var cmd = ListPartsCommand.Create(_serviceClient, _endpoint,
                                             CreateContext(HttpMethod.Get, listPartsRequest.BucketName, listPartsRequest.Key),
                                             listPartsRequest);   
            return cmd.Execute();            
        }
        
        /// <inheritdoc/>                
        public CompleteMultipartUploadResult CompleteMultipartUpload(CompleteMultipartUploadRequest completeMultipartUploadRequest)
        {
            ThrowIfNullRequest(completeMultipartUploadRequest);
            var cmd = CompleteMultipartUploadCommand.Create(_serviceClient, _endpoint,
                                                           CreateContext(HttpMethod.Post, completeMultipartUploadRequest.BucketName, completeMultipartUploadRequest.Key),
                                                           completeMultipartUploadRequest);
            return cmd.Execute();
        }
        
        #endregion

        #region Private Methods
        private ExecutionContext CreateContext(HttpMethod method, string bucket, string key)
        {
            var builder = new ExecutionContextBuilder
            {
                Bucket = bucket,
                Key = key,
                Method = method,
                Credentials = _credsProvider.GetCredentials()
            };
            builder.ResponseHandlers.Add(new ErrorResponseHandler());
            return builder.Build();
        }

        virtual protected void ThrowIfNullRequest<TRequestType> (TRequestType request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
        }

        private static void SetContentTypeIfNull(string key, string fileName, ref ObjectMetadata metadata)
        {
            if (metadata.ContentType == null)
            {
                metadata.ContentType = HttpUtils.GetContentType(key, fileName);
            }
        }

        private static long AdjustPartSize(long? partSize)
        {
            var actualPartSize = partSize ?? OssUtils.DefaultPartSize;
            actualPartSize = actualPartSize < OssUtils.PartSizeLowerLimit ? OssUtils.PartSizeLowerLimit : actualPartSize;
            return actualPartSize;
        }

        private ResumableContext LoadResumableCopyContext(CopyObjectRequest request, ObjectMetadata metadata, string checkpointDir, long partSize)
        {
            ResumableContext resumableContext = new ResumableCopyContext(request.SourceBucketName, request.SourceKey, request.SourceVersionId,
                                                            request.DestinationBucketName, request.DestinationKey, checkpointDir);
            if (resumableContext.Load() && resumableContext.ContentMd5 == metadata.ETag)
            {
                return resumableContext;
            }

            resumableContext = NewResumableContext(metadata.ContentLength, partSize, resumableContext);
            resumableContext.ContentMd5 = metadata.ETag;
            return resumableContext;
        }

        private ResumableContext LoadResumableUploadContext(string bucketName, string key, Stream content,
                                                            string checkpointDir, long partSize)
        {
            var resumableContext = new ResumableContext(bucketName, key, checkpointDir);
            if (resumableContext.Load())
            {
                return resumableContext;
            }

            resumableContext = NewResumableContext(content.Length, partSize, resumableContext);
            resumableContext.ContentMd5 = "fakeMD5"; // ContentMd5 is required for ResumableContext. Use a fake one. 
            return resumableContext;
        }

        private ResumableDownloadContext LoadResumableDownloadContext(string bucketName, string key, string versionId, ObjectMetadata metadata, string checkpointDir, long partSize)
        {
            var resumableContext = new ResumableDownloadContext(bucketName, key, versionId, checkpointDir);
            if (resumableContext.Load())
            {
                if (resumableContext.ETag == metadata.ETag
                    && resumableContext.ContentMd5 == metadata.ContentMd5
                    && resumableContext.Crc64 == metadata.Crc64)
                {
                    return resumableContext;
                }
            } 

            NewResumableContext(metadata.ContentLength, partSize, resumableContext);
            resumableContext.ContentMd5 = metadata.ContentMd5;
            resumableContext.ETag = metadata.ETag;
            resumableContext.Crc64 = metadata.Crc64;
            return resumableContext;
        }

        private static ResumableContext NewResumableContext(long contentLength, long partSize, ResumableContext resumableContext)
        {
            var fileSize = contentLength;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            if (partCount >= OssUtils.PartNumberUpperLimit)
            {
                partCount = OssUtils.PartNumberUpperLimit;
                partSize = fileSize / partCount + 1;
            }

            var partContextList = new List<ResumablePartContext>();
            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = partSize * i;
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var partContext = new ResumablePartContext()
                {
                    PartId = i + 1,
                    Position = skipBytes,
                    Length = size,
                    IsCompleted = false,
                    PartETag = null
                };

                partContextList.Add(partContext);
            }

            resumableContext.PartContextList = partContextList;
            return resumableContext;
        }

        internal Stream SetupProgressListeners(Stream originalStream,
                                                      long contentLength,
                                                      long totalBytesRead,
                                                      long progressUpdateInterval,
                                                      EventHandler<StreamTransferProgressArgs> callback)
        {
            return OssUtils.SetupProgressListeners(originalStream, contentLength, totalBytesRead, progressUpdateInterval, _serviceClient, callback);
        }

        internal Stream SetupDownloadProgressListeners(Stream originalStream,
                                                        long contentLength,
                                                        long totalBytesWritten,
                                                        long progressUpdateInterval,
                                                        EventHandler<StreamTransferProgressArgs> callback)
        {
           return OssUtils.SetupDownloadProgressListeners(originalStream, contentLength, totalBytesWritten, progressUpdateInterval, _serviceClient, callback); 
        }

        private void ResumableCopyWithRetry(CopyObjectRequest request, ResumableContext context)
        {
            int maxRetryTimes = ((RetryableServiceClient)_serviceClient).MaxRetryTimes;

            for (int i = 0; i < maxRetryTimes; i++)
            {
                try
                {
                    DoResumableCopy(request, context);
                    break;
                }
                catch (Exception ex)
                {
                    if (i != maxRetryTimes - 1)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private void DoResumableCopy(CopyObjectRequest request, ResumableContext resumableContext)
        {
            foreach (var part in resumableContext.PartContextList)
            {
                if (part.IsCompleted)
                {
                    continue;
                }

                var copyRequest = new UploadPartCopyRequest(request.DestinationBucketName,
                                                            request.DestinationKey,
                                                            request.SourceBucketName,
                                                            request.SourceKey, resumableContext.UploadId)
                {
                    PartSize = part.Length,
                    PartNumber = part.PartId,
                    BeginIndex = part.Position,
                    RequestPayer = request.RequestPayer,
                    TrafficLimit = request.TrafficLimit,
                    VersionId = request.SourceVersionId
                };
                var copyResult = UploadPartCopy(copyRequest);
 
                part.PartETag = copyResult.PartETag;
                part.IsCompleted = true;
                resumableContext.Dump();
            }
        }

        private static Uri FormatEndpoint(string endpoint)
        {
            string canonicalizedEndpoint = endpoint.Trim().ToLower();

            if (canonicalizedEndpoint.StartsWith(HttpUtils.HttpProto) ||
                canonicalizedEndpoint.StartsWith(HttpUtils.HttpsProto))
            {
                return new Uri(endpoint.Trim());
            }
            else
            {
                return new Uri(HttpUtils.HttpProto + endpoint.Trim());
            }
        }

        #endregion
    }
}
