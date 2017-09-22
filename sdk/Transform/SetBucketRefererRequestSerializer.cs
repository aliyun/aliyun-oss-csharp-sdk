/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketRefererRequestSerializer : RequestSerializer<SetBucketRefererRequest, RefererConfiguration>
    {
        public SetBucketRefererRequestSerializer(ISerializer<RefererConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        { }

       public override Stream Serialize(SetBucketRefererRequest request)
        {
           var model  = new RefererConfiguration
           {
               AllowEmptyReferer = request.AllowEmptyReferer,
               RefererList = new RefererConfiguration.RefererListModel {Referers = new string[request.RefererList.Count]}
           };
           for (var i = 0; i < request.RefererList.Count ; i++)
               model.RefererList.Referers[i] = request.RefererList[i];

            return ContentSerializer.Serialize(model);
        }
    }
}
