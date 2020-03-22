/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    internal class ResumablePartContext
    {
        protected static readonly char TokenSeparator = '_';

        public int PartId { get; set; }

        public long Position { get; set; }

        public long Length { get; set; }

        public PartETag PartETag { get; set; }

        public bool IsCompleted { get; set; }

        public ulong Crc64 { get; set; }

        public bool FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var tokens = value.Split(TokenSeparator);
            if (tokens.Length != 7)
            {
                return false;
            }

            int partId;
            if (!int.TryParse(tokens[0], out partId) || partId <= 0)
            {
                return false;
            }
            PartId = partId;

            long position;
            if (!long.TryParse(tokens[1], out position) || position < 0)
            {
                return false;
            }
            Position = position;

            long length;
            if (!long.TryParse(tokens[2], out length) || Length < 0)
            {
                return false;
            }
            Length = length;

            bool isCompleted;
            if (!bool.TryParse(tokens[3], out isCompleted))
            {
                return false;
            }
            IsCompleted = isCompleted;

            int partNum;
            if (string.IsNullOrEmpty(tokens[4]) && string.IsNullOrEmpty(tokens[5]))
            {
                PartETag = null;
            }
            else if (!(int.TryParse(tokens[4], out partNum) && partNum > 0 && !string.IsNullOrEmpty(tokens[5])))
            {
                return false;
            }
            else
            {
                PartETag = new PartETag(partNum, tokens[5]);
            }

            ulong crc = 0;
            if (!ulong.TryParse(tokens[6], out crc))
            {
                return false;
            }
            Crc64 = crc;

            return true;
        }

        override public string ToString()
        {
            string result = PartId.ToString() + TokenSeparator + Position.ToString() + TokenSeparator
                            + Length.ToString() + TokenSeparator + IsCompleted.ToString() + TokenSeparator;
            if (PartETag != null)
            {
                result += PartETag.PartNumber.ToString() + TokenSeparator + PartETag.ETag + TokenSeparator;
            }
            else
            {
                result += "" + TokenSeparator + "" + TokenSeparator;
            }

            result += Crc64.ToString();

            return result;
        }
    }

    internal class ResumableContext
    {
        protected static readonly char ContextSeparator = ':';
        protected static readonly char PartContextSeparator = ',';

        public string BucketName { get; protected set; }

        public string Key { get; protected set; }

        public List<ResumablePartContext> PartContextList { get; set; }

        public string UploadId { get; set; }

        public string ContentMd5 { get; set; }

        public string Crc64 { get; set; }

        public string CheckpointFile
        {
            get
            {
                return GenerateCheckpointFile();
            }
        }

        public string CheckpointDir { get; protected set; }

        public void Clear()
        {
            if (!string.IsNullOrEmpty(CheckpointFile) && File.Exists(CheckpointFile))
            {
                File.Delete(CheckpointFile);
            }
        }

        public bool Load()
        {
            if (!string.IsNullOrEmpty(CheckpointFile) && File.Exists(CheckpointFile))
            {
                var content = File.ReadAllText(CheckpointFile);
                return FromString(content);
            }
            return false;
        }

        public void Dump()
        {
            if (!string.IsNullOrEmpty(CheckpointFile))
            {
                string serialize = ToString();
                if (!string.IsNullOrEmpty(serialize))
                {
                    File.WriteAllText(CheckpointFile, serialize);
                }
            }
        }

        virtual public bool FromString(string value)
        {
            var tokens = value.Split(ContextSeparator);
            if (tokens.Length != 4)
            {
                return false;
            }

            UploadId = tokens[0];
            ContentMd5 = tokens[1];
            Crc64 = tokens[2];
            var partStr = tokens[3];

            var partTokens = partStr.Split(PartContextSeparator);
            if (partTokens.Length < 1)
            {
                return false;
            }

            PartContextList = PartContextList ?? new List<ResumablePartContext>();
            for (int i = 0; i < partTokens.Length; i++)
            {
                var partContext = new ResumablePartContext();
                if (!partContext.FromString(partTokens[i]))
                {
                    return false;
                }
                 
                PartContextList.Add(partContext);
            }
            return true;
        }

        override public string ToString()
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(UploadId))
            {
                return string.Empty;
            }
            result += UploadId.ToString() + ContextSeparator;

            if (ContentMd5 == null)
            {
                ContentMd5 = string.Empty;
            }

            if (Crc64 == null)
            {
                Crc64 = string.Empty;
            }

            result += ContentMd5 + ContextSeparator + Crc64 + ContextSeparator;

            if (PartContextList.Count == 0)
            {
                return string.Empty;
            }
            
            foreach (var part in PartContextList)
            {
                string partStr = part.ToString();
                if (string.IsNullOrEmpty(partStr))
                {
                    return string.Empty;
                }
                result += partStr + PartContextSeparator;
            }

            if (result.EndsWith(new string(PartContextSeparator, 1)))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public ResumableContext(string bucketName, string key, string checkpointDir)
        {
            BucketName = bucketName;
            Key = key;

            if (!string.IsNullOrEmpty(checkpointDir))
            {
                CheckpointDir = checkpointDir;
            }
        }

        public long GetUploadedBytes()
        {
            long uploadedBytes = 0;
            foreach (var part in PartContextList)
            {
                if (part.IsCompleted)
                {
                    uploadedBytes += part.Length;
                }  
            }
            return uploadedBytes;
        }

        virtual protected string GenerateCheckpointFile()
        {
            return GetCheckpointFilePath(CheckpointDir, Md5Hex(BucketName) + "_" + Md5Hex(Key));
        }

        protected string GetCheckpointFilePath(string checkpointDir, string checkpointFileName)
        {
            if (checkpointDir != null && (checkpointDir.Length > OssUtils.MaxPathLength - OssUtils.MinPathLength))
            {
                throw new ArgumentException("Invalid checkpoint directory {0}", CheckpointDir);
            }

            var maxFileNameSize = Math.Min(OssUtils.MaxPathLength - 1, checkpointFileName.Length);
            if (checkpointDir != null)
            {
                maxFileNameSize = Math.Min(OssUtils.MaxPathLength - CheckpointDir.Length - 1, checkpointFileName.Length);
            }
            return CheckpointDir + Path.DirectorySeparatorChar + checkpointFileName.Substring(0, maxFileNameSize);
        }

        protected string Base64(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var base64Str = Convert.ToBase64String(bytes);
            return base64Str.Replace("+", "_").Replace("/", "-");
        }

        protected string Md5Hex(string str)
        {
            var sBuilder = new StringBuilder();
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var data = md5.ComputeHash(new MemoryStream(bytes));
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
            }
            return sBuilder.ToString().ToUpperInvariant();
        }
    }

    internal class ResumableCopyContext : ResumableContext
    {
        public string SourceBucketName { get; private set; }

        public string SourceKey { get; private set; }

        public string SourceVersionId { get; private set; }

        override protected string GenerateCheckpointFile()
        {
            var srcPath = SourceBucketName + "-" + SourceKey;
            if (!string.IsNullOrEmpty(SourceVersionId))
            {
                srcPath = srcPath + "?versionId=" + SourceVersionId;
            }

            var dstPath = BucketName + "-" + Key;

            return GetCheckpointFilePath(CheckpointDir, "Copy_" + Md5Hex(srcPath) + "_" + Md5Hex(dstPath));
        }

        public ResumableCopyContext (string sourceBucketName, string sourceKey, string sourceVersionId, string destBucketName, 
                                     string destKey, string checkpointDir)
            : base(destBucketName,
            destKey, checkpointDir)
        {
            SourceBucketName = sourceBucketName;
            SourceKey = sourceKey;
            SourceVersionId = sourceVersionId;
        }
    }

    internal class ResumableDownloadContext : ResumableContext
    {
        public string VersionId { get; private set; }

        public ResumableDownloadContext(string bucketName, string key, string versionId, string checkpointDir)
            :base(bucketName, key, checkpointDir)
        {
            VersionId = versionId;
        }

        override protected string GenerateCheckpointFile()
        {
            var key = string.IsNullOrEmpty(VersionId) ? Key : Key + "?versionId=" + VersionId;
            return GetCheckpointFilePath(CheckpointDir, "Download_" + Md5Hex(BucketName) + "_" + Md5Hex(key));
        }
        public long GetDownloadedBytes()
        {
            return GetUploadedBytes();
        }

        public long GetTotalBytes()
        {
            long totalBytes = 0;
            foreach (var part in PartContextList)
            {
               totalBytes += part.Length;
            }
            return totalBytes;
        }
        
        public string ETag
        {
            get;
            set;
        }

        public override bool FromString(string value)
        {
            var tokens = value.Split(ContextSeparator);
            if (tokens.Length != 4)
            {
                return false;
            }

            ETag = tokens[0];
            if (!string.IsNullOrEmpty(tokens[1]))
            {
                ContentMd5 = tokens[1];
            }

            if (!string.IsNullOrEmpty(tokens[2]))
            {
                Crc64 = tokens[2];
            }

            var partStr = tokens[3];
            var partTokens = partStr.Split(PartContextSeparator);
            if (partTokens.Length <= 1)
            {
                return false;
            }

            PartContextList = PartContextList ?? new List<ResumablePartContext>();
            for (int i = 0; i < partTokens.Length; i++)
            {
                var partContext = new ResumablePartContext();
                if (!partContext.FromString(partTokens[i]))
                {
                    return false;
                }
                 
                PartContextList.Add(partContext);
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append(ETag);
            result.Append(ContextSeparator);

            result.Append(ContentMd5);
            result.Append(ContextSeparator);

            result.Append(Crc64);
            result.Append(ContextSeparator);

            foreach (var part in PartContextList)
            {
                string partStr = part.ToString();
               
                result.Append(partStr); 
                result.Append(PartContextSeparator);
            }

            string ss = result.ToString();
            if (ss.EndsWith(new string(PartContextSeparator, 1)))
            {
                ss = ss.Substring(0, ss.Length - 1);
            }
            return ss;
        }
    }
}
