
namespace Aliyun.OSS
{
    /// <summary>
    /// The output format of the inventory results
    /// </summary>
    public enum InventoryFormat
    {
        CSV
    }

    /// <summary>
    /// How frequently inventory results are produced
    /// </summary>
    public enum InventoryFrequency
    {
        Daily,
        Weekly
    }

    /// <summary>
    /// Object versions to include in the inventory list
    /// All, the list includes all the object versions, which adds the version-related fields VersionId , IsLatest , and DeleteMarker to the list
    /// Current, the list does not contain these version-related fields.
    /// </summary>
    public enum InventoryIncludedObjectVersions
    {
        All,
        Current
    }

    /// <summary>
    /// The optional fields that are included in the inventory results
    /// </summary>
    public enum InventoryOptionalField
    {
        Size,
        LastModifiedDate,
        ETag,
        StorageClass,
        IsMultipartUploaded,
        EncryptionStatus
    }
}
