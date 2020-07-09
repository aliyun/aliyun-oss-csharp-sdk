
using System;

namespace Aliyun.OSS
{
    public class LiveChannel
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the last modified time
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the publish url
        /// </summary>
        public string PublishUrl { get; set; }

        /// <summary>
        /// Gets or sets the play url
        /// </summary>
        public string PlayUrl { get; set; }
    }
}
