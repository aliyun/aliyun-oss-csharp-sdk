/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
 
using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to sign the URL
    /// </summary>
    public class GeneratePresignedUriRequest
    {
        private SignHttpMethod _method;
        private IDictionary<string, string> _userMetadata = new Dictionary<string, string>();
        private IDictionary<string, string> _queryParams = new Dictionary<string, string>();
        private ResponseHeaderOverrides _responseHeader = new ResponseHeaderOverrides();

        /// <summary>
        /// HTTP method getter/setter.
        /// </summary>
        public SignHttpMethod Method
        {
            get { return _method; }
            set
            {
                if (_method != SignHttpMethod.Get && _method != SignHttpMethod.Put)
                    throw new ArgumentException("Only supports Get & Put method.");
                _method = value;
            }
        }

        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Object key getter/setter
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Getter/setter of the target file's content-type header.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Getter/setter of the target file's MD5.
        /// Note that the setter should only be called by the SDK internally.
        /// </summary>
        public string ContentMd5 { get; set; }

        /// <summary>
        /// Getter/setter of the expiration time of the signed URL.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Process getter/setter.
        /// Process is specific to image files on which a specific operation (such as resize, sharpen,etc ) could be applied.
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Callback getter/setter, encoded in base64
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// Callback parameters, in base64
        /// </summary>
        public string CallbackVar { get; set; }

        /// <summary>
        /// ResponseHeaders getter/setter
        /// Response headers is to ask OSS service to return these headers (and their values) in the response.
        /// </summary>
        public ResponseHeaderOverrides ResponseHeaders
        {
            get { return _responseHeader; }
            set
            {
                if (value == null)
                    throw new ArgumentException("ResponseHeaderOverrides should not be null");
                _responseHeader = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the UserMetadata dictionary. 
        /// The SDK will automatically add the x-oss-meta- as the prefix of the metadata. 
        /// So the key in this property should not include x-oss-meta- prefix anymore.
        /// </summary>
        public IDictionary<string, string> UserMetadata 
        {
            get { return _userMetadata; }
            set
            {
                if (value == null)
                    throw new ArgumentException("UserMetadata should not be null");
                _userMetadata = value;
            }
        }

        /// <summary>
        /// Gets or sets query parameters
        /// </summary>
        public IDictionary<string, string> QueryParams
        {
            get 
            {
                if (Process != null)
                    _queryParams[RequestParameters.OSS_PROCESS] = Process;
                return _queryParams; 
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("QueryParams should not be null");
                _queryParams = value;
            }
        }

        /// <summary>
        /// Add a user metadata
        /// The metaItem should not start with 'x-oss-meta-'.
        /// </summary>
        /// <param name="metaItem">meta name</param>
        /// <param name="value">value of the metaItem</param>
        public void AddUserMetadata(string metaItem, string value)
        {
            _userMetadata.Add(metaItem, value);
        }

        /// <summary>
        /// Add a query parameter
        /// </summary>
        /// <param name="param">param name</param>
        /// <param name="value">param value</param>
        public void AddQueryParam(string param, string value)
        {
            _queryParams.Add(param, value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneratePresignedUriRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public GeneratePresignedUriRequest(string bucketName, string key)
            : this(bucketName, key, SignHttpMethod.Get)
        {
        }

        /// <summary>
        /// Creates a <see cref="GeneratePresignedUriRequest" /> instance.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="httpMethod">http method</param>
        public GeneratePresignedUriRequest(string bucketName, string key, SignHttpMethod httpMethod)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            BucketName = bucketName;
            Key = key;
            Method = httpMethod;
            // Default expiration(15 minutes from now) for signed url.
            Expiration = DateTime.Now.AddMinutes(15);
        }
        
    }
}
