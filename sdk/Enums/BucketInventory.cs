using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.OSS
{
    public enum InventoryFormat
    {
        NotSet,
        CSV
    }

    public enum InventoryFrequency
    {
        NotSet,
        Daily,
        Weekly
    }

    public enum InventoryIncludedObjectVersions
    {
        NotSet,
        All,
        Current
    }

    public enum InventoryOptionalField
    {
        NotSet,
        Size,
        LastModifiedDate,
        ETag,
        StorageClass,
        IsMultipartUploaded,
        EncryptionStatus
    }

}
