/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS.Common
{
    /// <summary>
    /// 表示来自对象存储服务（Object Storage Service，OSS）的错误代码。 
    /// </summary>
    /// <seealso cref="P:OssException.ErrorCode" />。
    public static class OssErrorCode
    {
        /// <summary>
        /// 访问被拒绝。
        /// </summary>
        public const string AccessDenied = "AccessDenied";

        /// <summary>
        /// Bucket已经存在。
        /// </summary>
        public const string BucketAlreadyExists = "BucketAlreadyExists";

        /// <summary>
        /// Bucket不为空。
        /// </summary>
        public const string BucketNotEmtpy = "BucketNotEmtpy";

        /// <summary>
        /// 实体过大。
        /// </summary>
        public const string EntityTooLarge = "EntityTooLarge";

        /// <summary>
        /// 实体过小。
        /// </summary>
        public const string EntityTooSmall = "EntityTooSmall";

        /// <summary>
        /// 文件组过大。
        /// </summary>
        public const string FileGroupTooLarge = "FileGroupTooLarge";

        /// <summary>
        /// Object Link 与指向的 Object 同名。
        /// </summary>
        public const string InvalidLinkName = "InvalidLinkName";

        /// <summary>
        /// Object Link 中指向的 Object 不存在。
        /// </summary>
        public const string LinkPartNotExist = "LinkPartNotExist";

        /// <summary>
        /// Object Link 中 Object 个数过多。
        /// </summary>
        public const string ObjectLinkTooLarge = "ObjectLinkTooLarge";

        /// <summary>
        /// Post 请求中表单域过大。
        /// </summary>
        public const string FieldItemTooLong = "FieldItemTooLong";

        /// <summary>
        /// 文件 Part 已改变。
        /// </summary>
        public const string FilePartInterity = "FilePartInterity";

        /// <summary>
        /// 文件 Part 不存在。
        /// </summary>
        public const string FilePartNotExist = "FilePartNotExist";

        /// <summary>
        /// 文件 Part 过时。
        /// </summary>
        public const string FilePartStale = "FilePartStale";

        /// <summary>
        /// Post 请求中文件个数非法
        /// </summary>
        public const string IncorrectNumberOfFilesInPOSTRequest = "IncorrectNumberOfFilesInPOSTRequest";

        /// <summary>
        /// 参数格式错误。
        /// </summary>
        public const string InvalidArgument = "InvalidArgument";

        /// <summary>
        /// Access ID不存在
        /// </summary>
        public const string InvalidAccessKeyId = "InvalidAccessKeyId";

        /// <summary>
        /// 无效的Bucket名字。
        /// </summary>
        public const string InvalidBucketName = "InvalidBucketName";

        /// <summary>
        /// 无效的摘要。
        /// </summary>
        public const string InvalidDigest = "InvalidDigest";

        /// <summary>
        /// Logging 操作中有无效的目标 bucket。
        /// </summary>
        public const string InvalidTargetBucketForLogging = "InvalidTargetBucketForLogging";

        /// <summary>
        /// 无效的Object名字。
        /// </summary>
        public const string InvalidObjectName = "InvalidObjectName";

        /// <summary>
        /// 无效的 Part。
        /// </summary>
        public const string InvalidPart = "InvalidPart";

        /// <summary>
        /// 无效的 Part 顺序。
        /// </summary>
        public const string InvalidPartOrder = "InvalidPartOrder";

        /// <summary>
        /// 无效的 Policy 文档。
        /// </summary>
        public const string InvalidPolicyDocument = "InvalidPolicyDocument";

        /// <summary>
        /// OSS内部错误。
        /// </summary>
        public const string InternalError = "InternalError";

        /// <summary>
        /// XML格式非法。
        /// </summary>
        public const string MalformedXML = "MalformedXML";

        /// <summary>
        /// Post 请求的 body 格式非法。
        /// </summary>
        public const string MalformedPOSTRequest = "MalformedPOSTRequest";

        /// <summary>
        /// Post 请求上传文件内容之外的 body过大。
        /// </summary>
        public const string MaxPOSTPreDataLengthExceededError = "MaxPOSTPreDataLengthExceededError";

        /// <summary>
        /// 不支持的方法。
        /// </summary>
        public const string MethodNotAllowed = "MethodNotAllowed";

        /// <summary>
        /// 缺少参数。
        /// </summary>
        public const string MissingArgument = "MissingArgument";

        /// <summary>
        /// 缺少内容长度。
        /// </summary>
        public const string MissingContentLength = "MissingContentLength";

        /// <summary>
        /// Bucket不存在。
        /// </summary>
        public const string NoSuchBucket = "NoSuchBucket";

        /// <summary>
        /// 文件不存在。
        /// </summary>
        public const string NoSuchKey = "NoSuchKey";

        /// <summary>
        /// Multipart Upload ID不存在。
        /// </summary>
        public const string NoSuchUpload = "NoSuchUpload";

        /// <summary>
        /// 无法处理的方法。
        /// </summary>
        public const string NotImplemented = "NotImplemented";

        /// <summary>
        /// 预处理错误
        /// </summary>
        public const string PreconditionFailed = "PreconditionFailed";

        /// <summary>
        /// 发起请求的时间和服务器时间超出15分钟。
        /// </summary>
        public const string RequestTimeTooSkewed = "RequestTimeTooSkewed";

        /// <summary>
        /// 请求超时。
        /// </summary>
        public const string RequestTimeout = "RequestTimeout";

        /// <summary>
        /// Post 请求 content-type 非法。
        /// </summary>
        public const string RequestIsNotMultiPartContent = "RequestIsNotMultiPartContent";

        /// <summary>
        /// 签名错误。
        /// </summary>
        public const string SignatureDoesNotMatch = "SignatureDoesNotMatch";

        /// <summary>
        /// 用户的Bucket数目超过限制。
        /// </summary>
        public const string TooManyBuckets = "TooManyBuckets";

        /// <summary>
        /// 指定的熵编码加密算法错误。
        /// </summary>
        public const string InvalidEncryptionAlgorithmError = "InvalidEncryptionAlgorithmError";

        /// <summary>
        /// 源Bucket未设置静态网站托管功能
        /// </summary>
        public const string NoSuchWebsiteConfiguration = "NoSuchWebsiteConfiguration";

        /// <summary>
        /// CORS规则不存在。
        /// </summary>
        public const string NoSuchCORSConfiguration = "NoSuchCORSConfiguration";

        /// <summary>
        /// 304未修改。
        /// </summary>
        public const string NotModified = "NotModified";
    }
}
