/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Common
{
    /// <summary>
	/// The OSS (Object Storage Service) Erro code definitions
    /// </summary>
    /// <seealso cref="P:OssException.ErrorCode" />。
    public static class OssErrorCode
    {
        /// <summary>
        /// Access Denied
        /// </summary>
        public const string AccessDenied = "AccessDenied";

        /// <summary>
        /// Bucket already exists
        /// </summary>
        public const string BucketAlreadyExists = "BucketAlreadyExists";

        /// <summary>
		/// Bucket is not empty (so that deletion will not work)
        /// </summary>
        public const string BucketNotEmtpy = "BucketNotEmtpy";

        /// <summary>
        /// Entity is too large
        /// </summary>
        public const string EntityTooLarge = "EntityTooLarge";

        /// <summary>
        /// Entity is too small (this could happen when trying to use multipart upload for a small file.
        /// </summary>
        public const string EntityTooSmall = "EntityTooSmall";

        /// <summary>
        /// File group is too large.
        /// </summary>
        public const string FileGroupTooLarge = "FileGroupTooLarge";

        /// <summary>
        /// Object Link has the same name of the object it points to.
        /// </summary>
        public const string InvalidLinkName = "InvalidLinkName";

        /// <summary>
        /// Object Link points to a non-existing object.
        /// </summary>
        public const string LinkPartNotExist = "LinkPartNotExist";

        /// <summary>
        /// Object Link's object count is more than 1. One symlink could only point to one object.
        /// </summary>
        public const string ObjectLinkTooLarge = "ObjectLinkTooLarge";

        /// <summary>
        /// The item is too long in the post request.
        /// </summary>
        public const string FieldItemTooLong = "FieldItemTooLong";

        /// <summary>
        /// File part has been changed.
        /// </summary>
        public const string FilePartInterity = "FilePartInterity";

        /// <summary>
        /// File part does not exist
        /// </summary>
        public const string FilePartNotExist = "FilePartNotExist";

        /// <summary>
        /// File part has been expired.
        /// </summary>
        public const string FilePartStale = "FilePartStale";

        /// <summary>
        /// File count is invalid in the post.
        /// </summary>
        public const string IncorrectNumberOfFilesInPOSTRequest = "IncorrectNumberOfFilesInPOSTRequest";

        /// <summary>
        /// Invalid argument
        /// </summary>
        public const string InvalidArgument = "InvalidArgument";

        /// <summary>
        /// Access ID does not exist
        /// </summary>
        public const string InvalidAccessKeyId = "InvalidAccessKeyId";

        /// <summary>
        /// Invalid bucket name
        /// </summary>
        public const string InvalidBucketName = "InvalidBucketName";

        /// <summary>
        /// Invalid digest
        /// </summary>
        public const string InvalidDigest = "InvalidDigest";

        /// <summary>
        /// Invalid target bucket for logginbg
        /// </summary>
        public const string InvalidTargetBucketForLogging = "InvalidTargetBucketForLogging";

        /// <summary>
        /// Invalid object name
        /// </summary>
        public const string InvalidObjectName = "InvalidObjectName";

        /// <summary>
        /// Invalid part
        /// </summary>
        public const string InvalidPart = "InvalidPart";

        /// <summary>
		/// Invalid part order (the part Ids must be in ascending order)
        /// </summary>
        public const string InvalidPartOrder = "InvalidPartOrder";

        /// <summary>
        /// Invalid policy document
        /// </summary>
        public const string InvalidPolicyDocument = "InvalidPolicyDocument";

        /// <summary>
		/// OSS internal error (possibly OSS bug)
        /// </summary>
        public const string InternalError = "InternalError";

        /// <summary>
        /// Malformed XML
        /// </summary>
        public const string MalformedXML = "MalformedXML";

        /// <summary>
        /// Malformed body in the post request.
        /// </summary>
        public const string MalformedPOSTRequest = "MalformedPOSTRequest";

        /// <summary>
        /// The non-content body size in a file upload request is too big
        /// </summary>
        public const string MaxPOSTPreDataLengthExceededError = "MaxPOSTPreDataLengthExceededError";

        /// <summary>
		/// HTTP Method is not allowed.(for example some CORS rules could define allowed methods)
        /// </summary>
        public const string MethodNotAllowed = "MethodNotAllowed";

        /// <summary>
        /// Missing argument
        /// </summary>
        public const string MissingArgument = "MissingArgument";

        /// <summary>
        /// Missing content length--in HTTP post/put requests, the content length is needed.
        /// </summary>
        public const string MissingContentLength = "MissingContentLength";

        /// <summary>
        /// Bucket does not exist.
        /// </summary>
        public const string NoSuchBucket = "NoSuchBucket";

        /// <summary>
        /// Object does not exist in OSS
        /// </summary>
        public const string NoSuchKey = "NoSuchKey";

        /// <summary>
        /// Multipart Upload ID does not exist
        /// </summary>
        public const string NoSuchUpload = "NoSuchUpload";

        /// <summary>
        /// Not implemented methods
        /// </summary>
        public const string NotImplemented = "NotImplemented";

        /// <summary>
        /// Precondition failed.
        /// </summary>
        public const string PreconditionFailed = "PreconditionFailed";

        /// <summary>
		/// The time skew is too big (more than 15 minutes)
        /// </summary>
        public const string RequestTimeTooSkewed = "RequestTimeTooSkewed";

        /// <summary>
        /// Request timeout
        /// </summary>
        public const string RequestTimeout = "RequestTimeout";

        /// <summary>
        /// Invalid content-type in the post request.
        /// </summary>
        public const string RequestIsNotMultiPartContent = "RequestIsNotMultiPartContent";

        /// <summary>
        /// Signature does not match
        /// </summary>
        public const string SignatureDoesNotMatch = "SignatureDoesNotMatch";

        /// <summary>
        /// Bucket counts exceeds the limit
        /// </summary>
        public const string TooManyBuckets = "TooManyBuckets";

        /// <summary>
        /// Invalid Encryption Algorithems error
        /// </summary>
        public const string InvalidEncryptionAlgorithmError = "InvalidEncryptionAlgorithmError";

        /// <summary>
        /// The source bucket is not enabled with static website
        /// </summary>
        public const string NoSuchWebsiteConfiguration = "NoSuchWebsiteConfiguration";

        /// <summary>
        /// CORS rules do not exist
        /// </summary>
        public const string NoSuchCORSConfiguration = "NoSuchCORSConfiguration";

        /// <summary>
        /// 304 Not modified
        /// </summary>
        public const string NotModified = "NotModified";

        /// <summary>
        /// 203 callback call failed
        /// </summary>
        public const string CallbackFailed = "CallbackFailed";
    }
}
