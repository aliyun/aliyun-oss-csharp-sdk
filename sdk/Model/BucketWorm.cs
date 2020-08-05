using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("InitiateWormConfiguration")]
    public class InitiateBucketWormModel
    {
        [XmlElement("RetentionPeriodInDays")]
        public int Days { get; set; }
    }

    [XmlRoot("ExtendWormConfiguration")]
    public class ExtendBucketWormModel
    {
        [XmlElement("RetentionPeriodInDays")]
        public int Days { get; set; }
    }

    [XmlRoot("WormConfiguration")]
    public class WormConfigurationModel
    {
        [XmlElement("WormId")]
        public string WormId { get; set; }

        [XmlElement("State")]
        public BucketWormState State { get; set; }

        [XmlElement("RetentionPeriodInDays")]
        public int Days { get; set; }

        [XmlElement("CreationDate")]
        public string CreationDate { get; set; }
    }
}
