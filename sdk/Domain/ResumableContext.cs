/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

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

        public bool FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var tokens = value.Split(TokenSeparator);
            if (tokens.Length != 6)
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
            if (int.TryParse(tokens[4], out partNum) && partNum > 0 && !string.IsNullOrEmpty(tokens[5]))
            {
                PartETag = new PartETag(partNum, tokens[5]);
            }

            return true;
        }

        override public string ToString()
        {
            string result = PartId.ToString() + TokenSeparator + Position.ToString() + TokenSeparator
                            + Length.ToString() + TokenSeparator + IsCompleted.ToString() + TokenSeparator;
            if (PartETag != null)
            {
                result += PartETag.PartNumber.ToString() + TokenSeparator + PartETag.ETag;
            } else
            {
                result += TokenSeparator;
            }

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
            if (tokens.Length != 3)
            {
                return false;
            }

            UploadId = tokens[0];
            ContentMd5 = tokens[1];
            var partStr = tokens[2];

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

        override public string ToString()
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(UploadId))
            {
                return string.Empty;
            }
            result += UploadId.ToString() + ContextSeparator;

            if (string.IsNullOrEmpty(ContentMd5))
            {
                return string.Empty;
            }
            result += ContentMd5.ToString() + ContextSeparator;

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

        virtual protected string GenerateCheckpointFile()
        {
            return CheckpointDir + Path.PathSeparator + Base64(BucketName) + "_" + Base64(Key);
        }

        protected string Base64(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
    }

    internal class ResumableCopyContext : ResumableContext
    {
        public string SourceBucketName { get; private set; }

        public string SourceKey { get; private set; }

        override protected string GenerateCheckpointFile()
        {
            return base.CheckpointDir + Path.PathSeparator + Base64(SourceBucketName) + "_" + Base64(SourceKey) + "_" 
                   + Base64(BucketName) + "_" + Base64(Key);
        }

        public ResumableCopyContext (string sourceBucketName, string sourceKey, string destBucketName, 
                                     string destKey, string checkpointDir)
            : base(destBucketName, destKey, checkpointDir)
        {
            SourceBucketName = sourceBucketName;
            SourceKey = sourceKey; 
        }
    }
}
