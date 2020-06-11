using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to delete the bucket's tagging.
     /// </summary>
    public class DeleteBucketTaggingRequest
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
        /// Creates a new intance of <see cref="DeleteBucketTaggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public DeleteBucketTaggingRequest(string bucketName)
        {
            BucketName = bucketName;
        }

        /// <summary>
        /// Creates a new intance of <see cref="DeleteBucketTaggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="tags">tag list</param>
        public DeleteBucketTaggingRequest(string bucketName, IList<Tag> tags)
        {
            BucketName = bucketName;
            Tags = tags;
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
