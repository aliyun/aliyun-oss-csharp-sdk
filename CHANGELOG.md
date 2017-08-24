# ChangeLog - Aliyun OSS SDK for C# 

## Version：2.5.1  Date：2017/05/17
### Changes in the version
- Fix：The progress bar in ResumableUploadObject does not work when uploading a small file

## Version：2.5.0  Date：2017/04/20
### Changes in the version
- Add：Request Id in the normal response
- Add：Support upload progress callback
- Add：Support progress bar
- Add：Support MD5 checksum
- Fix：ModifyObjectMeta can specify checkpoint file path and limit the file name's max length.
- Fix：GetObject(Uri)/PutObject(Uri) return the full valid OssObject as other operations.
- Fix：ComputeContentMd5 big memory footprint issue
- Fix：GetObject.Metadata cannot be empty

## Version：2.4.0  Date：2016/12/14
### Changes in the version
- Add：SetObjectAcl/GetObjectAcl APIs
- Add：Image processing feature
- Add：More OssClient constructors to simplify the consume
- Fix：GetObject uses MemoryStream and has big memory footprint
- Fix：ObjectMetadata has 5G limit when setting the ContentLength
- Fix：Some request does not have Proxy-Authorization header when proxy is used

## Version：2.3.0  Date：2016/03/28
### Changes in the version
- Add ContentMd5 property in class ObjectMetadata, for MD5 checksum when uploading files
- Validates the target bucket and object name before copying object
- Fix the problem that fails to retrieve Expires attribute from Metadata
- Fix the broken retry mechanism.
- Fix the null pointer exception when setting Content-Encoding with null
- Fix the issue that exceptions are thrown when Endppoint has leading or trailing spaces.

## Version：2.2.0  Date：2015/12/12
### new features in the version
- Support Mono 3.12.0 or higher version
- Add object append API：AppendObject
- Add resumable upload API：ResumableUploadObject
- Add resumable copy API：ResumableCopyObject
- Add more sample code

### Fixes
- Removes the dependency on System.web

## Version：2.1.0  Date：2015/11/24
### New features in the version
- Support .NET Framework 2.0 and .NET Framework 3.5
- Big file copy API：CopyBigObject
- Big file upload API：UploadBigObject
- Update object meta API：ModifyObjectMeta

### Fix
- Improve the SDK's rubustness
- ContentType supports most of MIME types, now it has 226 MIME types.
- Add the missing comments in SDK API document
- Delete ObjectMetaData property from some obsolete classes. Use ObjectMetadata instead.删除废弃的某些类中ObjectMetaData. 

## Version：2.0.0  Date：2015/11/18
### New features in the version
- Automatic content-type detection according to the extension name in object's key and source file.
- Add EncodingType parameter in ListObjects，ListMultipartUploads，DeleteObjects APIs
- Add MD5 validation in UploadPart API
- Add more sample code

### Fix
- Refine the namespace
- Merge the duplicate directory
- Unifiy directories name
- Delete duplicate test items
- Fix the way to support CName
- When the bucket or file does not exist, DoesObjectExist returns false instead of throwing exceptions.

### Notes
- The version changed the internal directory structure and name space, comparing to version 1.0.*. So it's not compatible to the older version. Some code changes are necessary to upload to this version.

## Version：1.0.10 Date：2015/01/14
### New features
- Add Copy Part, Delete Objects, Bucket Referer List APIs
- Add paging functionality in ListBuckets
- Add CNAME support
- Add some Samples

### Changes
- The assembly name is changed to Aliyun.OSS.dll
- .NET Framework version is upgraded to 4.0 or higher

### Fix
- Fix the stream interruptions in Put/GetObject

## Version：1.0.9 Date：2014/06/26
### New features
- Support cors、Logging、website related APIs

## Version：1.0.8 Date：2013/09/02容
### Fix
- Fix the issue that in some cases the correct exceptions could not be thrown properly.
- Improve the SDK performance

## Version：1.0.7 Date：2013/06/04
### Updates
- Use three level domain as the default way to access OSS

## Version：1.0.6 Date：2013/05/20
### Fix
- Some bug fixes in SDK to stablize it

## Version：1.0.5 Date：2013/04/10
### New features
- Add Object Multipart Upload API
- Add Copy Object API
- Add presign URL API

### Fix
- Create a IOSS interface and make OssClient implement it

## Version：1.0.4 Date：2012/10/10
### Updates
- Use http://oss.aliyuncs.com as default OSS endpoint

## Version：1.0.3 Date：2012/09/05
### New features
- Fix the issue that the prefix in ListObjects does not take effect.

## Version：1.0.2 Date：2012/06/15
### New features
- Add OSS support, including Bucket, ACL, Object's CRUD (Create,Read,Update,Delete) operations.
- Add the hanlding on some specific response errors
- Add HTML help doc

## Version：1.0.1 Date：2012/05/16
### New Features
- OTSClient.GetRowsByRange支持反向读取。

## Version：1.0.0 Date：2012/03/16
### New features
- OTS access APIs, including table and table group and data's CRUD(Create,Read,Update,Delete) operations.
- The client side configuration, such as proxy and http connection settings.
- United structured exception hanlding
