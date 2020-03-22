/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;

using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class UploadPartCopyCommand : OssCommand<UploadPartCopyResult>
    {
        private readonly UploadPartCopyRequest _uploadPartCopyRequest;
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _uploadPartCopyRequest.TargetBucket; }
        } 
        
        protected override string Key
        {
            get { return _uploadPartCopyRequest.TargetKey; }
        } 
        
        protected override IDictionary<string, string> Parameters
        {
            get {
                var parameters = base.Parameters;
                parameters[RequestParameters.PART_NUMBER] = _uploadPartCopyRequest.PartNumber.ToString();
                parameters[RequestParameters.UPLOAD_ID] = _uploadPartCopyRequest.UploadId;
                return parameters;
            }
        }
        
        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                
                headers[HttpHeaders.ContentLength] = _uploadPartCopyRequest.PartSize.ToString();
                if (!string.IsNullOrEmpty(_uploadPartCopyRequest.Md5Digest))
                    headers[HttpHeaders.ContentMd5] = _uploadPartCopyRequest.Md5Digest;

                var copyHeaderValue = OssUtils.BuildPartCopySource(_uploadPartCopyRequest.SourceBucket,
                    _uploadPartCopyRequest.SourceKey);
                if (!string.IsNullOrEmpty(_uploadPartCopyRequest.VersionId))
                {
                    copyHeaderValue = copyHeaderValue + "?versionId=" + _uploadPartCopyRequest.VersionId;
                }
                headers[HttpHeaders.CopySource] = copyHeaderValue;

                headers[HttpHeaders.CopySourceRange] = "bytes=" + _uploadPartCopyRequest.BeginIndex
                    + "-" + (_uploadPartCopyRequest.BeginIndex + _uploadPartCopyRequest.PartSize - 1);

                if (_uploadPartCopyRequest.MatchingETagConstraints.Count > 0)
                    headers[OssHeaders.CopySourceIfMatch] =
                        OssUtils.JoinETag(_uploadPartCopyRequest.MatchingETagConstraints);
                if (_uploadPartCopyRequest.NonmatchingETagConstraints.Count > 0)
                    headers[OssHeaders.CopySourceIfNoneMatch] =
                        OssUtils.JoinETag(_uploadPartCopyRequest.NonmatchingETagConstraints);
                if (_uploadPartCopyRequest.ModifiedSinceConstraint != null)
                { 
                    headers[OssHeaders.CopySourceIfModifedSince] 
                        = DateUtils.FormatRfc822Date(_uploadPartCopyRequest.ModifiedSinceConstraint.Value);
                }
                if (_uploadPartCopyRequest.UnmodifiedSinceConstraint != null)
                { 
                    headers[OssHeaders.CopySourceIfUnmodifiedSince]
                        = DateUtils.FormatRfc822Date(_uploadPartCopyRequest.UnmodifiedSinceConstraint.Value);
                }
                if (_uploadPartCopyRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                if (_uploadPartCopyRequest.TrafficLimit > 0)
                {
                    headers.Add(OssHeaders.OssTrafficLimit, _uploadPartCopyRequest.TrafficLimit.ToString());
                }
                return headers;
            }
        }
        
        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }

        private UploadPartCopyCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, UploadPartCopyResult> deserializer,                                  
                                                UploadPartCopyRequest uploadPartCopyRequest)
            : base(client, endpoint, context, deserializer)
        {
            _uploadPartCopyRequest = uploadPartCopyRequest;
        }
        
        public static UploadPartCopyCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 UploadPartCopyRequest uploadPartCopyRequest)
        {
            OssUtils.CheckBucketName(uploadPartCopyRequest.SourceBucket);
            OssUtils.CheckObjectKey(uploadPartCopyRequest.SourceKey);
            OssUtils.CheckBucketName(uploadPartCopyRequest.TargetBucket);
            OssUtils.CheckObjectKey(uploadPartCopyRequest.TargetKey);

            if (!uploadPartCopyRequest.PartNumber.HasValue)
                throw new ArgumentException("partNumber should be specfied");
            if (!uploadPartCopyRequest.PartSize.HasValue)
                throw new ArgumentException("partSize should be specfied");
            if (!uploadPartCopyRequest.BeginIndex.HasValue)
                throw new ArgumentException("beginIndex should be specfied");
            
            if (uploadPartCopyRequest.PartSize < 0 || uploadPartCopyRequest.PartSize > OssUtils.MaxFileSize)
                throw new ArgumentException("partSize not live in valid range");
            if (!OssUtils.IsPartNumberInRange(uploadPartCopyRequest.PartNumber))
                throw new ArgumentException("partNumber not live in valid range");

            return new UploadPartCopyCommand(client, endpoint, context, 
                DeserializerFactory.GetFactory().CreateUploadPartCopyResultDeserializer(uploadPartCopyRequest.PartNumber.Value),
                uploadPartCopyRequest);
        }
    }
}
