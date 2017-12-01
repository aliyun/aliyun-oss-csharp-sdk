/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal abstract class OssCommand
    {
        protected ExecutionContext Context { get; private set; }

        private IServiceClient Client { get; set; }

        private Uri Endpoint { get; set; }

        protected virtual bool LeaveRequestOpen
        {
            get { return false; }
        }

        protected virtual HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        protected virtual String Bucket
        {
            get { return null; }
        }

        protected virtual String Key
        {
            get { return null; }
        }
        
        protected virtual IDictionary<String, String> Headers 
        {
            get { return new Dictionary<String, String>(); }
        }
        
        protected virtual IDictionary<String, String> Parameters
        {
            get { return new Dictionary<String, String>(); }
        }
        
        protected virtual Stream Content
        {
            get { return null; }
        }

        protected bool UseChunkedEncoding
        {
            get;
            private set;
        }

        protected bool ParametersInUri
        {
            get;
            set;
        }

        protected OssCommand(IServiceClient client, Uri endpoint, ExecutionContext context)
            : this(client, endpoint, context, false)
        {
        }

        protected OssCommand(IServiceClient client, Uri endpoint, ExecutionContext context, bool useChunkedEncoding)
        {
            Client = client;
            Endpoint = endpoint;
            Context = context;
            UseChunkedEncoding = useChunkedEncoding;
        }
        
        public ServiceResponse Execute()
        {
            var request = BuildRequest();
            try 
            {
                return Client.Send(request, Context);
            } 
            finally 
            {
                if (!LeaveRequestOpen) 
                    request.Dispose();
            }
        }

        public IAsyncResult AsyncExecute(AsyncCallback callback, Object state)
        {
            var request = BuildRequest();
            return Client.BeginSend(request, Context, callback, state);
        }

        private ServiceRequest BuildRequest()
        {
            var conf = OssUtils.GetClientConfiguration(Client);
            var request = new ServiceRequest
            {
                Method = Method,
                Endpoint = OssUtils.MakeBucketEndpoint(Endpoint, Bucket, conf),
                ResourcePath = OssUtils.MakeResourcePath(Endpoint, Bucket, Key),
                UseChunkedEncoding = UseChunkedEncoding,
                ParametersInUri = ParametersInUri
            };

            foreach (var p in Parameters) 
                request.Parameters.Add(p.Key, p.Value);      
          
            var adjustedTime = DateTime.UtcNow.AddSeconds(conf.TickOffset);
            request.Headers[HttpHeaders.Date] = DateUtils.FormatRfc822Date(adjustedTime);

            if (!Headers.ContainsKey(HttpHeaders.ContentType))
                request.Headers[HttpHeaders.ContentType] = string.Empty;
            
            foreach(var h in Headers)
                request.Headers.Add(h.Key, h.Value);
            if (Context.Credentials.UseToken)
                request.Headers[HttpHeaders.SecurityToken] = Context.Credentials.SecurityToken;
            
            request.Content = Content;

            return request;
        }
    }
    
    internal abstract class OssCommand<T> : OssCommand
    {
        private readonly IDeserializer<ServiceResponse, T> _deserializer;

        protected virtual bool LeaveResponseOpen { get { return false; } }

        protected OssCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                          IDeserializer<ServiceResponse, T> deserializer)
            : this(client, endpoint, context, deserializer, false)
        {
        }

        protected OssCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                          IDeserializer<ServiceResponse, T> deserializer, bool useChunkedEncoding)
            : base(client, endpoint, context, useChunkedEncoding)
        
        {
            _deserializer = deserializer;
            Context.Command = this;
        }

        public new T Execute()
        {
            var response = base.Execute();
            return DeserializeResponse(response);
        }

        public T DeserializeResponse(ServiceResponse response)
        {
            try
            {
                return _deserializer.Deserialize(response);
            }
            catch (ResponseDeserializationException ex)
            {
                throw ExceptionFactory.CreateInvalidResponseException(ex);
            }
            finally
            {
                if (!LeaveResponseOpen)
                    response.Dispose();
            }
        }

    }
}
