/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Properties;

namespace Aliyun.OSS.Commands
{
    internal class ListPartsCommand : OssCommand<PartListing>
    {
        private readonly ListPartsRequest _listPartsRequest;
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        protected override string Bucket
        {
            get { return _listPartsRequest.BucketName; }
        }

        protected override string Key
        {
            get { return _listPartsRequest.Key; }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get 
            {
                var parameters = base.Parameters;
                Populate(_listPartsRequest, parameters);
                return parameters;
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_listPartsRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private static void Populate(ListPartsRequest listPartsRequst, IDictionary<string, string> parameters)
        {
            parameters[RequestParameters.UPLOAD_ID] = listPartsRequst.UploadId;
            
            if (listPartsRequst.MaxParts != null)
                parameters[RequestParameters.MAX_PARTS] = listPartsRequst.MaxParts.ToString();
            
            if (listPartsRequst.PartNumberMarker != null)
                parameters[RequestParameters.PART_NUMBER_MARKER] = listPartsRequst.PartNumberMarker.ToString();

            if (listPartsRequst.EncodingType != null)
                parameters[RequestParameters.ENCODING_TYPE] = listPartsRequst.EncodingType;
        }
        
        private ListPartsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, PartListing> deserializeMethod,
                                ListPartsRequest listPartsRequest)
            : base(client, endpoint, context, deserializeMethod)
        {
            OssUtils.CheckBucketName(listPartsRequest.BucketName);
            OssUtils.CheckObjectKey(listPartsRequest.Key);

            if (string.IsNullOrEmpty(listPartsRequest.UploadId))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "uploadId");
            
            _listPartsRequest = listPartsRequest;
        }
        
        public static ListPartsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                ListPartsRequest listPartsRequest)
        {
            return new ListPartsCommand(client, endpoint, context, 
                                        DeserializerFactory.GetFactory().CreateListPartsResultDeserializer(),
                                        listPartsRequest);
        }
    }
}
