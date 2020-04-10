# Aliyun OSS SDK for C# 

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](LICENSE)
[![GitHub version](https://badge.fury.io/gh/aliyun%2Faliyun-oss-csharp-sdk.svg)](https://badge.fury.io/gh/aliyun%2Faliyun-oss-csharp-sdk)
[![Build Status](https://travis-ci.org/aliyun/aliyun-oss-csharp-sdk.svg?branch=master)](https://travis-ci.org/aliyun/aliyun-oss-csharp-sdk)

## [README of English](https://github.com/aliyun/aliyun-oss-csharp-sdk/blob/master/README.md)

## 关于
 - 阿里云对象存储（Object Storage Service，OSS），是[阿里云](https://www.aliyun.com)对外提供的海量，安全，低成本，高可靠的云存储服务。
 - OSS C# SDK基于[OSS REST API](https://help.aliyun.com/document_detail/31948.html)构建。
 - OSS C# SDK[在线文档](http://gosspublic.alicdn.com/AliyunNetSDK/apidocs/latest/index.html)。

## 版本
 - 当前版本：2.10.0

## 运行环境

### Windows
 - 适用于`.NET 2.0` 及以上版本
 - 适用于`Visual Studio 2010`及以上版本

### Linux/Mac
 - 适用于`Mono 3.12` 及以上版本

## 安装方法
### Windows环境安装
#### NuGet安装
 - 如果您的Visual Studio没有安装NuGet，请先安装 [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget).
 - 安装好NuGet后，先在`Visual Studio`中新建或者打开已有的项目，然后选择`<工具>`－`<NuGet程序包管理器>`－`<管理解决方案的NuGet程序包>`，
 - 搜索`aliyun.oss.sdk`，在结果中找到`Aliyun.OSS.SDK`或`Aliyun.OSS.SDK.NetCore`，选择最新版本，点击安装，成功后添加到项目应用中。

#### GitHub安装
 - 如果没有安装git，请先安装 [git](https://git-scm.com/downloads) 
 - git clone https://github.com/aliyun/aliyun-oss-csharp-sdk.git
 - 下载好源码后，按照`项目引入方式安装`即可

#### DLL引用方式安装
 - 从阿里云OSS官网下载SDK包，解压后bin目录包括了Aliyun.OSS.dll文件。
 - 在Visual Studio的`<解决方案资源管理器>`中选择您的项目，然后右键`<项目名称>`-`<引用>`，在弹出的菜单中选择`<添加引用>`，
在弹出`<添加引用>`对话框后，选择`<浏览>`，找到SDK包解压的目录，在bin目录下选中`<Aliyun.OSS.dll>`文件,点击确定即可

#### 项目引入方式安装
 - 如果是下载了SDK包或者从GitHub上下载了源码，希望源码安装，可以右键`<解决方案>`，在弹出的菜单中点击`<添加>`->`<现有项目>`。
 - 在弹出的对话框中选择`aliyun-oss-sdk.csproj`文件，点击打开。
 - 接下来右键`<您的项目>`－`<引用>`，选择`<添加引用>`，在弹出的对话框选择`<项目>`选项卡后选中`aliyun-oss-sdk`项目，点击确定即可。

### Unix/Mac环境安装
#### NuGet安装
 - 先在`Xamarin`中新建或者打开已有的项目，然后选择`<工具>`－`<Add NuGet Packages>`。
 - 搜索`aliyun.oss.sdk`，在结果中找到`Aliyun.OSS.SDK`，选择最新版本，点击`<Add Package>`，成功后添加到项目应用中。

#### GitHub安装
 - 如果没有安装git，请先安装 [git](https://git-scm.com/downloads) 
 - git clone https://github.com/aliyun/aliyun-oss-csharp-sdk.git
 - 下载好源码后，使用Xamarin打开，在Release模式下编译aliyun-oss-sdk项目，生成Aliyun.OSS.dll，然后通过DLL引用方式安装

#### DLL引用方式安装
 - 从阿里云OSS官网下载SDK包，解压后bin目录包括了Aliyun.OSS.dll文件。
 - 在Xamarin的`<解决方案>`中选择您的项目，然后右键`<项目名称>`-`<引用>`，在弹出的菜单中选择`<Edit References>`，
在弹出`<Edit References>`对话框后，选择`<.Net Assembly>-<浏览>`，找到SDK包解压的目录，在bin目录下选中`<Aliyun.OSS.dll>`文件,点击`<Open>`即可

## 快速使用
#### 获取存储空间列表（List Bucket）
```csharp
    OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);    
	var buckets = client.ListBuckets();
	
    foreach (var bucket in buckets)
    {
    	Console.WriteLine(bucket.Name + ", " + bucket.Location + ", " + bucket.Owner);
    }
```
    
#### 创建存储空间（Create Bucket）
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	client.CreateBucket(bucketName);
```
	
#### 删除存储空间（Delete Bucket）
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	client.DeleteBucket(bucketName);
```

#### 上传文件（Put Object）
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	client.PutObject(bucketName, key, filePathToUpload);
```

#### 下载文件 (Get Object)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	var object = ossClient.GetObject(bucketName, key);	
```

#### 获取文件列表（List Objects）
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	var listResult = client.ListObjects(bucketName);
	foreach (var summary in listResult.ObjectSummaries)
	{   
		Console.WriteLine(summary.Key);
	}
```
	
#### 删除文件(Delete Object)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	client.DeleteObject(bucketName, key)
```

#### 其它
 - 上面的例子中，如果没有抛出异常则说明执行成功，否则失败，更详细的例子可以在aliyun-oss-sample项目中查看并运行。
	
## 注意事项
 - 如果要运行sample，需要将aliyun-oss-sdk-sample项目设为`启动项目`，并添加您自己的AccessKeyId，AccessKeySecret，bucket，key等后即可运行。

## 联系我们
- [阿里云OSS官方网站](http://oss.aliyun.com)
- [阿里云OSS官方论坛](http://bbs.aliyun.com)
- [阿里云OSS官方文档中心](http://www.aliyun.com/product/oss#Docs)
- 阿里云官方技术支持：[提交工单](https://workorder.console.aliyun.com/#/ticket/createIndex)
