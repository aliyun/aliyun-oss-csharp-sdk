using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to set the bucket's tagging.
     /// </summary>
    public class SetBucketTaggingRequest
    {
        private IList<Tag> _tags = new List<Tag>();

        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the tags.
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
        /// Creates a new intance of <see cref="SetBucketTaggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public SetBucketTaggingRequest(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }

            BucketName = bucketName;
        }

        /// <summary>
        /// Creates a new intance of <see cref="SetBucketTaggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="tags">tag list</param>
        public SetBucketTaggingRequest(string bucketName, IList<Tag> tags)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }

            BucketName = bucketName;

            _tags = tags ?? throw new ArgumentException("tag list should not be null.");
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="tag"></param>
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
