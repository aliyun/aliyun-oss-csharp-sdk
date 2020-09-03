namespace Aliyun.OSS
{
    public enum InputFormatType
    {
        CSV,
        JSON
    }

    public enum CompressionType
    {
        None,
        GZIP
    }

    public enum JSONType
    {
        DOCUMENT = 0,
        LINES
    }

    public enum FileHeaderInfo
    {
        None = 0, // there is no CSV header
        Ignore,   // we should ignore CSV header and should not use CSV header in select SQL
        Use       // we can use CSV header in select SQL
    }
}
