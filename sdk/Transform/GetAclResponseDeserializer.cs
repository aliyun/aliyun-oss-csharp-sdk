/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class GetAclResponseDeserializer : ResponseDeserializer<AccessControlList, AccessControlPolicy>
    {
        public GetAclResponseDeserializer(IDeserializer<Stream, AccessControlPolicy> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override AccessControlList Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            var acl = new AccessControlList {Owner = new Owner(model.Owner.Id, model.Owner.DisplayName)};
            foreach(var grant in model.Grants)
            {
                if (grant == EnumUtils.GetStringValue(CannedAccessControlList.PublicRead))
                {
                    acl.GrantPermission(GroupGrantee.AllUsers, Permission.Read);
                    acl.ACL = CannedAccessControlList.PublicRead;
                }
                else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.PublicReadWrite))
                {
                    acl.GrantPermission(GroupGrantee.AllUsers, Permission.FullControl);
                    acl.ACL = CannedAccessControlList.PublicReadWrite;
                }
                else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.Private))
                {
                    acl.ACL = CannedAccessControlList.Private;
                }
                else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.Default)) 
                {
                    acl.ACL = CannedAccessControlList.Default;
                }
            }
            DeserializeGeneric(xmlStream, acl);
            return acl;
        }
    }
}
