/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Util
{
    internal static class OssHeaders
    {
        public const string OssPrefix = "x-oss-";
        public const string OssUserMetaPrefix = "x-oss-meta-";

        public const string OssCannedAcl = "x-oss-acl";
        public const string OssObjectCannedAcl = "x-oss-object-acl";

        public const string GetObjectIfModifiedSince = "If-Modified-Since";
        public const string GetObjectIfUnmodifiedSince = "If-Unmodified-Since";
        public const string GetObjectIfMatch = "If-Match";
        public const string GetObjectIfNoneMatch = "If-None-Match";
        
        public const string CopyObjectSource = "x-oss-copy-source";
        public const string CopySourceIfMatch = "x-oss-copy-source-if-match";
        public const string CopySourceIfNoneMatch = "x-oss-copy-source-if-none-match";
        public const string CopySourceIfUnmodifiedSince = "x-oss-copy-source-if-unmodified-since";
        public const string CopySourceIfModifedSince = "x-oss-copy-source-if-modified-since";
        public const string CopyObjectMetaDataDirective = "x-oss-metadata-directive";
        public const string SymlinkTarget = "x-oss-symlink-target";

        public const string SelectInputCompression = "x-oss-select-input-compression";
        public const string SelectInputFileHeader = "x-oss-select-input-file-header";
        public const string SelectInputRecordDelimiter = "x-oss-select-input-record-delimiter";
        public const string SelectInputFieldDelimiter = "x-oss-select-input-field-delimiter";
        public const string SelectInputQuoteCharacter = "x-oss-select-input-quote-character";
        public const string SelectInputCommentCharacter = "x-oss-select-input-comment-character";
        public const string SelectOutputQuoteFields = "x-oss-select-output-quote-fields";
        public const string SelectOutputRecordDelimiter = "x-oss-select-output-record-delimiter";
        public const string SelectOutputFieldDelimiter = "x-oss-select-output-field-delimiter";
        public const string SelectOutputQuoteCharacter = "x-oss-select-output-quote-character";
        public const string SelectOutputEnableCrc = "x-oss-select-output-enable-crc";
        public const string SelectOutputKeepAllColumns = "x-oss-select-output-keep-all-columns";
    }
}
