/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Util
{
    internal static class RequestParameters
    {
        public const string SUBRESOURCE_ACL = "acl";
        public const string SUBRESOURCE_REFERER = "referer";
        public const string SUBRESOURCE_LOGGING = "logging"; 
        public const string SUBRESOURCE_WEBSITE = "website"; 
        public const string SUBRESOURCE_LIFECYCLE = "lifecycle";
        public const string SUBRESOURCE_UPLOADS = "uploads";
        public const string SUBRESOURCE_DELETE = "delete";
        public const string SUBRESOURCE_CORS = "cors";
        public const string SUBRESOURCE_RESTORE = "restore";
        public const string SUBRESOURCE_BUCKETINFO = "bucketInfo";
        public const string SUBRESOURCE_BUCKETSTAT = "stat";
        public const string SUBRESOURCE_SYMLINK = "symlink";
		public const string SUBRESOURCE_LOCATION = "location";
		public const string SUBRESOURCE_QOS = "qos";
        public const string SUBRESOURCE_POLICY = "policy";
        public const string SUBRESOURCE_TAGGING = "tagging";
        public const string SUBRESOURCE_REQUESTPAYER = "requestPayment";
        public const string SUBRESOURCE_OBJECTMETA = "objectMeta";
        public const string SUBRESOURCE_ENCRYPTION = "encryption";
        public const string SUBRESOURCE_VERSIONING = "versioning";
        public const string SUBRESOURCE_VERSIONID = "versionId";
        public const string PREFIX = "prefix";
        public const string DELIMITER = "delimiter";
        public const string MARKER = "marker";    
        public const string MAX_KEYS = "max-keys";
        public const string ENCODING_TYPE = "encoding-type";
        public const string KYE_MARKER = "key-marker";
        public const string VERSION_ID_MARKER = "version-id-marker";
        public const string VERSIONS = "versions";

        public const string UPLOAD_ID = "uploadId";
        public const string PART_NUMBER = "partNumber";
        public const string MAX_UPLOADS = "max-uploads";
        public const string UPLOAD_ID_MARKER = "upload-id-marker";
        public const string KEY_MARKER = "key-marker";
        public const string MAX_PARTS = "max-parts";    
        public const string PART_NUMBER_MARKER = "part-number-marker";

        public const string EXPIRES = "Expires";
        public const string SIGNATURE = "Signature";
        public const string OSS_ACCESS_KEY_ID = "OSSAccessKeyId";
    
        public const string SECURITY_TOKEN = "security-token";

        public const string OSS_PROCESS = "x-oss-process";

        public const string OSS_TAGGING = "x-oss-tagging";

        public const string TAG_KEY = "tag-key";
        public const string TAG_VALUE = "tag-value";

        public const string OSS_TRAFFIC_LIMIT = "x-oss-traffic-limit";
    }
}
