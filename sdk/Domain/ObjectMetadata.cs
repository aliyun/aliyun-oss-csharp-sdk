/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Net;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// OSS object's metadata, which is the collection of 'key,value' pair.
    /// <para>
    /// It includes user's custom metadata, as well as standard HTTP headers such as Content-Length, ETag, etc.
    /// </para>
    /// </summary>
    public class ObjectMetadata
    {      
        private readonly IDictionary<string, string> _userMetadata = 
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<string, object> _metadata = 
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 256 bit ASE encryption algorithm. 
        /// </summary>
        public const string Aes256ServerSideEncryption = "AES256";

        /// <summary>
        /// Gets the user's custom metadata.
        /// </summary>
        /// <remarks>
        /// In OSS server side, it will add "x-oss-meta-" as the prefix for the keys of custom metadata. 
        /// However, here the key in UserMetadata should not include "x-oss-meta-".
        /// And the key is case insensitive--in fact all the keys returned from server will be in lowercase anyway.
        /// For example, for a key MyUserMeta, it will be myusermeta from the result of GetObjectMetadata().
        /// </remarks>
        public IDictionary<string, string> UserMetadata
        {
            get { return _userMetadata; }
        }

        /// <summary>
        /// Gets HTTP standard headers and their values.
        /// </summary>
        public IDictionary<string, object> HttpMetadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// Gets or sets the last modified timestamp of the OSS object.
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.LastModified)
                    ? (DateTime)_metadata[HttpHeaders.LastModified] : DateTime.MinValue;
            }
            internal set
            {
                _metadata[HttpHeaders.LastModified] = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration time of the object.
        /// </summary>
        public DateTime ExpirationTime
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.Expires)
                     ? DateUtils.ParseRfc822Date((string)_metadata[HttpHeaders.Expires]) : DateTime.MinValue;
            }
            set
            {
                _metadata[HttpHeaders.Expires] = DateUtils.FormatRfc822Date(value);
            }
        }

        /// <summary>
        /// Gets or sets the content length of the object.
        /// </summary>
        public long ContentLength
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentLength)
                    ? (long)_metadata[HttpHeaders.ContentLength] : 0;
            }
            set
            {
                _metadata[HttpHeaders.ContentLength] = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type of the objeft. It's the standard MIME type.
        /// </summary>
        public string ContentType
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentType)
                    ? _metadata[HttpHeaders.ContentType] as string : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _metadata[HttpHeaders.ContentType] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the content encoding of the object.
        /// </summary>
        public string ContentEncoding
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentEncoding)
                    ? _metadata[HttpHeaders.ContentEncoding] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.ContentEncoding] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of HTTP Cache-Control header.
        /// </summary>
        public string CacheControl
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.CacheControl)
                    ? _metadata[HttpHeaders.CacheControl] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.CacheControl] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of HTTP Content-Disposition header.
        /// </summary>
        public string ContentDisposition
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentDisposition) 
                    ? _metadata[HttpHeaders.ContentDisposition] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.ContentDisposition] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of HTTP ETag header. Note that this is set by OSS server. 
        /// To set the Content-MD5 value, use HTTP COntent-MD5 header instead.
        /// </summary>
        public string ETag
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ETag) 
                    ? _metadata[HttpHeaders.ETag] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.ETag] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the HTTP Content-MD5 header, which is the MD5 summary in Hex string of the object.
        /// </summary>
        public string ContentMd5
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ContentMd5)
                    ? _metadata[HttpHeaders.ContentMd5] as string : null;
            }
            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.ContentMd5] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the crc64.
        /// </summary>
        /// <value>The crc64.</value>
        public string Crc64
        {
            get{
                return _metadata.ContainsKey(HttpHeaders.HashCrc64Ecma)
                                ? _metadata[HttpHeaders.HashCrc64Ecma] as string : null;
            }

            set
            {
                if (value != null)
                {
                    _metadata[HttpHeaders.HashCrc64Ecma] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the server side encryption algorithm. Only AES256 is support for now.
        /// </summary>
        public string ServerSideEncryption
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ServerSideEncryption)
                    ? _metadata[HttpHeaders.ServerSideEncryption] as string : null;
            }
            set
            {
                if (Aes256ServerSideEncryption != value)
                    throw new ArgumentException("Unsupported server side encryption");
                _metadata[HttpHeaders.ServerSideEncryption] = value;
            }
        }

        /// <summary>
        /// Gets the object type (Normal or Appendable)
        /// </summary>
        public string ObjectType
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.ObjectType)
                    ? _metadata[HttpHeaders.ObjectType] as string : null;
            }
        }

        /// <summary>
        /// Gets the object version id
        /// </summary>
        public string VersionId
        {
            get
            {
                return _metadata.ContainsKey(HttpHeaders.VersionId)
                    ? _metadata[HttpHeaders.VersionId] as string : null;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectMetadata" />.
        /// </summary>
        public ObjectMetadata()
        {
            ContentLength = -1L;
        }

        /// <summary>
        /// Adds one HTTP header and its value.
        /// </summary>
        /// <param name="key">header name</param>
        /// <param name="value">header value</param>
        public void AddHeader(string key, object value)
        {
            _metadata.Add(key, value);
        }

        /// <summary>
        /// Populates the request header dictionary with the metdata and user metadata.
        /// </summary>
        /// <param name="requestHeaders"></param>
        internal void Populate(IDictionary<string, string> requestHeaders)
        {
            foreach(var entry in _metadata)
                requestHeaders.Add(entry.Key, entry.Value.ToString());
            
            if (!requestHeaders.ContainsKey(HttpHeaders.ContentType))
                requestHeaders.Add(HttpHeaders.ContentType, HttpUtils.DefaultContentType);

            foreach(var entry in _userMetadata)
                requestHeaders.Add(OssHeaders.OssUserMetaPrefix + entry.Key, entry.Value);
        }

        /// <summary>
        /// Populates the request header dictionary with the metdata and user metadata.
        /// </summary>
        /// <param name="webRequest"></param>
        internal void Populate(HttpWebRequest webRequest)
        {
            foreach (var entry in _metadata)
            {
                if (entry.Key.Equals(HttpHeaders.ContentLength) )
                {
                    if (ContentLength > 0)
                    {
                        webRequest.ContentLength = ContentLength;
                    }
                }
                else if (entry.Key.Equals(HttpHeaders.ContentType))
                {
                    webRequest.ContentType = ContentType;
                }
                else
                {
                    webRequest.Headers.Add(entry.Key, entry.Value.ToString());
                }
            }

            foreach (var entry in _userMetadata)
            {
                webRequest.Headers.Add(OssHeaders.OssUserMetaPrefix + entry.Key, entry.Value);
            }
        }
        
        /// <summary>
        /// Get the flag which indicates if the metadata specifies the callback.
        /// </summary>
        /// <param name="metadata">The metadata object to check</param>
        /// <returns></returns>
        internal static bool HasCallbackHeader(ObjectMetadata metadata)
        {
            if (metadata != null && metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback))
            {
                return true;
            }
            return false;
        }
    }
}
