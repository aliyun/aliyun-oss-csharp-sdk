/*
///Copyright (C) Alibaba Cloud Computing
///All rights reserved.
///
///版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Globalization;

namespace Aliyun.OSS
{
    ///<summary>
    ///Bucket是OSS上的命名空间，可以理解为存储空间
    ///</summary>
    /// <remarks>
    ///<para>
    ///Bucket名在整个 OSS 中具有全局唯一性，且不能修改；存储在OSS上的每个Object必须都包含在某个Bucket中。
    ///一个应用，例如图片分享网站，可以对应一个或多个 Bucket。一个用户最多可创建 10 个Bucket，
    ///但每个Bucket 中存放的Object的数量和大小总和没有限制，用户不需要考虑数据的可扩展性。
    ///</para>
    ///<para>
    ///Bucket 命名规范
    ///<list type="">
    /// <item>只能包括小写字母，数字和短横线（-）</item>
    /// <item>必须以小写字母或者数字开头</item>
    /// <item>长度必须在 3-63 字节之间</item>
    ///</list>
    ///</para>
    /// </remarks>
    public class Bucket
    {
        /// <summary>
        /// 获取/设置Bucket的Location。
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// 获取/设置Bucket的名称。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 获取/设置Bucket的<see cref="Owner" />
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// 获取/设置Bucket的创建时间。
        /// </summary>
        public DateTime CreationDate { get; internal set; }
        
        /// <summary>
        /// 使用指定的Bucket名称构造一个新的<see cref="Bucket" />实例。
        /// </summary>
        /// <param name="name">Bucket的名称。</param>
        internal Bucket(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 返回该对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示形式</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "OSS Bucket [Name={0}], [Location={1}] [Owner={2}], [CreationTime={3}]",
                                 Name, Location, Owner, CreationDate);
        }

    }
}
