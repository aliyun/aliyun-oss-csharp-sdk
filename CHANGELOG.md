# ChangeLog - Aliyun OSS SDK for C# 

## 版本号：2.4.0  日期：2016/12/14
### 变更内容
- 增加：SetObjectAcl/GetObjectAcl接口
- 增加：图片处理功能
- 添加：OssClient的构造函数，方便使用
- 修复：GetObject中使用MemoryStream占用大内存的问题
- 修复：ObjectMetadata设置ContentLength时5G限制的问题
- 修复：使用Proxy时，个别请求没有Proxy-Authorization头的问题

## 版本号：2.3.0  日期：2016/03/28
### 变更内容
- ObjectMetadata新增ContentMd5属性，支持上传文件时验证md5
- 拷贝时验证目标bucket和object名称合法性
- 解决无法从Metadata获取Expires的问题
- 解决重试机制失效的问题
- 解决设置Content-Encoding等值为null时抛异常的问题
- 解决Endpoint头尾含空字符报错的问题

## 版本号：2.2.0  日期：2015/12/12
### 新增内容
- 支持Mono 3.12.0及其以上版本
- 新增追加文件接口：AppendObject
- 新增断点续传上传接口：ResumableUploadObject
- 新增断点续传拷贝接口：ResumableCopyObject
- 新增部分示例程序

### 修改内容
- 移除对System.Web库的依赖

## 版本号：2.1.0  日期：2015/11/24
### 新增内容
- .NET Framework 2.0和.NET Framework 3.5支持
- 大文件拷贝接口：CopyBigObject
- 大文件上传接口：UploadBigObject
- 文件meta修改接口：ModifyObjectMeta

### 修改内容
- 提升SDK健壮性
- ContentType支持大多数MIME种类（226种）
- 补齐SDK API速查文档中的缺失注释
- 删除废弃的某些类中ObjectMetaData，请使用类中对应的的ObjectMetadata 

## 版本号：2.0.0  日期：2015/11/18
### 新增内容
- 自动根据对象key和上传文件名后缀判断ContentType
- ListObjects，ListMultipartUploads，DeleteObjects接口默认增加EncodingType参数
- UploadPart接口新增内容md5校验
- 新增部分示例程序

### 修改内容
- 精简命名空间
- 合并重复目录
- 统一目录名称
- 删除重复的测试项目
- 修改cname支持形式
- 当存储空间或者文件不存在时，DoesObjectExist不再抛出异常，而是返回false

### 注意事项
- 此版本相对于1.0.* 版本，内部目录结构和命名空间有变动，如果需要从1.0.*版本升级到此版本，需要您修改现有程序。

## 版本号：1.0.10 日期：2015/01/14
### 新增内容
- OSS: 添加Copy Part、Delete Objects、Bucket Referer List等接口。
- OSS: 添加ListBuckets分页功能。
- OSS: 添加CNAME支持。
- OSS: 添加若干Samples。

### 修改内容
- 程序集命名更改为Aliyun.OSS.dll
- .NET Framework版本升至4.0及以上

### 修复问题
- 修复Put/GetObject流中断问题。

## 版本号：1.0.9 时间：2014/06/26
### 新增内容
- 添加对cors、Logging、website等接口的支持。

## 版本号：1.0.8 时间：2013/09/02容
### 修复问题
- 修复了某些情况下无法抛出正确的异常的Bug。
- 优化了SDK的性能。

## 版本号：1.0.7 时间：2013/06/04
### 修改内容
- 将默认OSS服务访问方式修改为三级域名方式。

## 版本号：1.0.6 时间：2013/05/20
### 修复问题
- 修复了SDK中的几处Bug，使其运行更稳定。

## 版本号：1.0.5 时间：2013/04/10
### 新增内容
- 添加了Object分块上传（Multipart Upload）功能。
- 添加了Copy Object功能。
- 添加了生成预签名URL的功能。

### 修改内容
- 分离出IOss接口，并由OssClient继承此接口。

## 版本号：1.0.4 时间：2012/10/10
### 修改内容
- 将默认的OSS服务地址更新为：http://oss.aliyuncs.com

## 版本号：1.0.3 时间：2012/09/05
### 新增内容
- 解决ListObjects时Prefix等参数无效的问题。

## 版本号：1.0.2 时间：2012/06/15
### 新增内容
- 首次加入对OSS的支持。包含了OSS Bucket、ACL、Object的创建、修改、读取、删除等基本操作。
- 加入对特定请求错误的自动处理机制。
- 增加HTML格式的帮助文件。

## 版本号：1.0.1 时间：2012/05/16
### 新增内容
- OTSClient.GetRowsByRange支持反向读取。

## 版本号：1.0.0 时间：2012/03/16
### 新增内容
- OTS访问接口，包括对表、表组的创建、修改和删除等操作，对数据的插入、修改、删除和查询等操作。
- 访问的客户端设置，如果代理设置、HTTP连接属性设置等。
- 统一的结构化异常处理。
