/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics;
using System.Net;
using System.Xml;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Model;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Handlers
{
    internal class ErrorResponseHandler : ResponseHandler
    {
        public override void Handle(ServiceResponse response)
        {
            base.Handle(response);

            if (response.IsSuccessful())
                return;

            ErrorHandle(response);
        }

        protected void ErrorHandle(ServiceResponse response)
        {
            // Treats NotModified(Http status code) specially.
            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                throw ExceptionFactory.CreateException(HttpStatusCode.NotModified.ToString(),
                    response.Failure.Message, null, null);
            }

            ErrorResult errorResult = null;
            try
            {
                var deserializer = DeserializerFactory.GetFactory().CreateErrorResultDeserializer();
                if (deserializer == null)
                {
                    // Re-throw the web exception if the response cannot be parsed.
                    response.EnsureSuccessful();
                }
                else
                {
                    errorResult = deserializer.Deserialize(response);
                }
            }
            catch (XmlException)
            {
                // Re-throw the web exception if the response cannot be parsed.
                response.EnsureSuccessful();
            }
            catch (InvalidOperationException)
            {
                // Re-throw the web exception if the response cannot be parsed.
                response.EnsureSuccessful();
            }

            // This throw must be out of the try block because otherwise
            // the exception would be caught be the following catch.
            Debug.Assert(errorResult != null);
            throw ExceptionFactory.CreateException(errorResult.Code,
                                                   errorResult.Message,
                                                   errorResult.RequestId,
                                                   errorResult.HostId);
        }
    }
}

