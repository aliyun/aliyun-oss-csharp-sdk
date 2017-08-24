/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to list the parts of a multipart upload.
    /// </summary>
    public class PartListing : GenericResult 
    {
        private readonly IList<Part> _parts = new List<Part>();
        
        /// <summary>
        /// Gets bucket name.
        /// </summary>
        public string BucketName { get; internal set; }
               
        /// <summary>
        /// Gets target object key.
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// Gets the value from <see cref="P:ListPartsRequest.UploadId" />.
        /// </summary>
        public string UploadId { get; internal set; }
        
        /// <summary>
        /// Gets the value from <see cref="P:ListPartsRequest.PartNumberMarker" />.
        /// </summary>
        public int PartNumberMarker { get; internal set; }
        
        /// <summary>
        /// If the result does not have all data, the response will have the value of this property for the next call to start with
        /// That is assign this value to the PartNumberMarker property in the next call.
        /// </summary>
        public int NextPartNumberMarker { get; internal set; }
        
        /// <summary>
        /// The max parts to return. The value comes from <see cref="P:ListPartsRequest.MaxParts" />.
        /// </summary>
        public int MaxParts { get; internal set; }
        
        /// <summary>
        /// Flag if the result is truncated.
        /// “true” means it's truncated;“false” means the result is complete.
        /// </summary>
        public bool IsTruncated { get; internal set; }
        
        /// <summary>
        /// Gets the parts iterator.
        /// </summary>
        public IEnumerable<Part> Parts 
        {
            get { return _parts; }
        }
        
        /// <summary>
        /// Adds a <see cref="Part"/> information---internal only
        /// </summary>
        /// <param name="part">one part instance</param>
        internal void AddPart(Part part)
        {
            _parts.Add(part);
        }

        /// <summary>
        /// Creates a new instance of <see cref="PartListing" />---internal only.
        /// </summary>
        internal PartListing()
        { }
        
    }
}
