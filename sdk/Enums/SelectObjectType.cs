using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.OSS
{
    public enum InputFormatTypes
    {
        CSV,
        JSON
    }

    public enum CompressionType
    {
        NONE,
        GZIP
    }

    public enum JsonType
    {
        DOCUMENT = 0,
        LINES
    }

    public enum FileHeaderInfo
    {
        None = 0, // there is no csv header
        Ignore,   // we should ignore csv header and should not use csv header in select sql
        Use       // we can use csv header in select sql
    }
}
