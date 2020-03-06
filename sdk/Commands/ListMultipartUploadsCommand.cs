/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class ListMultipartUploadsCommand : OssCommand<MultipartUploadListing>
    {
        private readonly ListMultipartUploadsRequest _listMultipartUploadsRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        protected override string Bucket
        {
            get { return _listMultipartUploadsRequest.BucketName; }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get 
            {
                var parameters = base.Parameters;
                Populate(_listMultipartUploadsRequest, parameters);
                return parameters;
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_listMultipartUploadsRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private ListMultipartUploadsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                            IDeserializer<ServiceResponse, MultipartUploadListing> deserializeMethod,
                                            ListMultipartUploadsRequest listMultipartUploadsRequest)
            : base(client, endpoint, context, deserializeMethod)
        {
            OssUtils.CheckBucketName(listMultipartUploadsRequest.BucketName);
            _listMultipartUploadsRequest = listMultipartUploadsRequest;
        }
        
        public static ListMultipartUploadsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                         ListMultipartUploadsRequest listMultipartUploadsRequest)
        {
            return new ListMultipartUploadsCommand(client, endpoint, context, 
                                                   DeserializerFactory.GetFactory().CreateListMultipartUploadsResultDeserializer(),
                                                   listMultipartUploadsRequest);
        }
        
        private static void Populate(ListMultipartUploadsRequest listMultipartUploadsRequest, 
                                    IDictionary<string, string> parameters)
        {
            parameters[RequestParameters.SUBRESOURCE_UPLOADS] = null;
            if (listMultipartUploadsRequest.Delimiter != null)
            {
                parameters[RequestParameters.DELIMITER] = listMultipartUploadsRequest.Delimiter;
            }
            
            if (listMultipartUploadsRequest.KeyMarker != null)
            {
                parameters[RequestParameters.KEY_MARKER] = listMultipartUploadsRequest.KeyMarker;
            }
            
            if (listMultipartUploadsRequest.MaxUploads.HasValue)
            {
                parameters[RequestParameters.MAX_UPLOADS] = 
                    listMultipartUploadsRequest.MaxUploads.Value.ToString(CultureInfo.InvariantCulture); ;
            }
            
            if (listMultipartUploadsRequest.Prefix != null)
            {
                parameters[RequestParameters.PREFIX] = listMultipartUploadsRequest.Prefix;
            }
            
            if (listMultipartUploadsRequest.UploadIdMarker != null)
            {
                parameters[RequestParameters.UPLOAD_ID_MARKER] = listMultipartUploadsRequest.UploadIdMarker;
            }

            if (listMultipartUploadsRequest.EncodingType != null)
            {
                parameters[RequestParameters.ENCODING_TYPE] = listMultipartUploadsRequest.EncodingType;
            }
        }
        
    }
}
