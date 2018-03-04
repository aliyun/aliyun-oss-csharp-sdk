/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class for selecting csv content from OSS.
    /// </summary>
    public class SelectObjectRequest : GetObjectRequest
    {
        private char _inputNewline = '\n';
        private char _inputQuote = '"';
        private char _inputDelimiter = ',';
        private char _outputNewLine = '\n';
        private char _outputQuote = '"';
        private char _outputDelimiter = ',';
        private char _inputComment = '#';
        private QuoteFields _outputQuoteFields = QuoteFields.AsNeeded;
        private HeaderInfo _headerInfo = HeaderInfo.Ignore;
        private CompressionType _compressionType = CompressionType.None;
        public SelectObjectRequest(string bucket, string obj, string sql)
            :base(bucket, obj)
        {
            Sql = sql;    
        }

        public string Sql
        {
            get;
            set;
        }

        public char InputNewLine
        {
            get { return _inputNewline; } 
            set { _inputNewline = value; }
        }

        public char InputQuote
        {
            get { return _inputQuote; }
            set { _inputQuote = value; }
        }

        public char InputDelimiter
        {
            get { return _inputDelimiter; }
            set { _inputDelimiter = value; }
        }

        public char OutputNewLine
        {
            get { return _outputNewLine; }
            set { _outputNewLine = value; }
        }

        public char OutputQuote
        {
            get { return _outputQuote; }
            set { _outputQuote = value; }
        }

        public char OutputDelimiter
        {
            get { return _outputDelimiter; }
            set { _outputDelimiter = value; }
        }

        public char InputComment
        {
            get { return _inputComment; }
            set { _inputComment = value; }
        }

        public HeaderInfo Header
        {
            get { return _headerInfo; }
            set { _headerInfo = value; }
        }

        public CompressionType Compression
        {
            get { return _compressionType; }
            set { _compressionType = value; }
        }

        public QuoteFields OutputQuoteFields
        {
            get { return _outputQuoteFields; }
            set { _outputQuoteFields = value; }
        }

        public enum HeaderInfo
        {
            None,
            Ignore,
            Use
        }

        public enum CompressionType
        {
            None,
            GZIP
        }

        public enum QuoteFields
        {
            AsNeeded,
            Always
        }
    }
}
