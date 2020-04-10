using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to set the object's tagging.
     /// </summary>
    public class SetObjectTaggingRequest
    {
        private IList<Tag> _tags = new List<Tag>();

        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the object key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Gets or sets the tagging.
        /// </summary>
        public IList<Tag> Tags
        {
            get { return ((List<Tag>)_tags).AsReadOnly(); }
            set
            {
                _tags = value ?? throw new ArgumentException("tag list should not be null.");
            }

        }

        /// <summary>
        /// Creates a new intance of <see cref="SetObjectTaggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public SetObjectTaggingRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }

        public SetObjectTaggingRequest(string bucketName, string key, IList<Tag> tags)
        {
            BucketName = bucketName;
            Key = key;
            _tags = tags ?? throw new ArgumentException("tag list should not be null.");
        }
        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="tag">tag</param>
        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentException("tag should not be null.");
            }

            _tags.Add(tag);
        }
    }
}

