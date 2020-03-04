
namespace Aliyun.OSS
{
    public class Tag
    {
        /// <summary>
        /// Gets or sets the tag key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the tag value
        /// </summary>
        public string Value { get; set; }

        public bool Equals(Tag obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            if (obj == null) return false;

            return this.Key.Equals(obj.Key)  && this.Value.Equals(obj.Value);
        }
    }
}
