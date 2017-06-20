# Alibaba Cloud OSS SDK for C# 

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](LICENSE)
[![GitHub Version](https://badge.fury.io/gh/aliyun%2Faliyun-oss-csharp-sdk.svg)](https://badge.fury.io/gh/aliyun%2Faliyun-oss-csharp-sdk)
[![Build Status](https://travis-ci.org/aliyun/aliyun-oss-csharp-sdk.svg?branch=master)](https://travis-ci.org/aliyun/aliyun-oss-csharp-sdk)

## [README of Chinese](https://github.com/aliyun/aliyun-oss-csharp-sdk/blob/master/README-CN.md)

## About
 Alibaba Cloud Object Storage Service (OSS) is a cloud storage service provided by [Alibaba Cloud](https://www.aliyun.com), featuring massive capacity, security, a low cost, and high reliability.
 - The OSS C# SDK is built based on [OSS REST API](https://help.aliyun.com/document_detail/31948.html).
 - OSS C# SDK[Online Documentation](https://gosspublic.alicdn.com/AliyunNetSDK/latest/apidocs/index.html). 

## Version
 - Current version: 2.5.1.

## Run environment

### Windows
 - Applicable to `.NET 2.0` or above. 
 - Applicable to `Visual Studio 2010` or above. 

### Linux/Mac
 - Applicable to `Mono 3.12` or above. 

## Install OSS C# SDK
### Install in Windows
#### Install the SDK through NuGet
 - If NuGet hasn't been installed for Visual Studio, install [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) first. 
 - After NuGet is installed, access Visual Studio to create a project or open an existing project, and then select `TOOLS` > `NuGet Package Manager` > `Manage NuGet Packages for Solution`.
 - Type `aliyun.oss.sdk` in the search box and click *Search*, find `Aliyun.OSS.SDK` in the search results, select the latest version, and click *Install*. After installation, the SDK is added to the project.

#### Install the SDK through GitHub
 - If Git hasn't been installed, install [Git](https://git-scm.com/downloads) first. 
 - Clone project via `git clone https://github.com/aliyun/aliyun-oss-csharp-sdk.git`. 
 - After the source code is downloaded, install the SDK by entering `Install via Project Introduction`.

#### Install the SDK through DLL reference
 - Download the SDK packagefrom the Alibaba Cloud OSS official website. Unzip the package and you will find the ***Aliyun.OSS.dll*** file in the *bin* directory.
 - In the Visual Studio, access `Solution Explorer`, select your project, right click *Project Name*, select `Add Reference` from the pop-up menu.
In the `Reference Manager` dialog box, click *Browse*, find the directory that the SDK is unzipped to, select the ***Aliyun.OSS.dll*** file in the *bin* directory, and click *OK*.

#### Install the SDK through project introduction
 - If you have downloaded the SDK package or the source code from GitHub and you want to install the SDK package using the source code, you can right click ' `Solution Explorer`' and select `Add` > `Existing Projects` from the pop-up menu.
 - In the pop-up dialog box, select the `aliyun-oss-sdk.csproj` file, and click *Open*.
 - Right click *Your Projects* and select `Add Reference`. In the `Reference Manager` dialog box, click the `Projects` tab, select the ***aliyun-oss-sdk*** project, and click *OK*.

### Install in Unix/Mac
#### Install the SDK through NuGet
 - In `Xamarin`, create a project or open an existing project, and select `Tools` > `Add NuGet Packages`.
 - Type `aliyun.oss.sdk` in the search box and click *Search*, find 'Aliyun.OSS.SDK' in the search results, select the latest version, and click `Add Package`. After installation, the SDK is added to the project.

#### Install the SDK through GitHub
 - If Git hasn't been installed, install [Git](https://git-scm.com/downloads) first.
 - Clone project via `git clone https://github.com/aliyun/aliyun-oss-csharp-sdk.git`.
 - After the source code is downloaded, open it in *Xamarin*. Compile the *aliyun-oss-sdk* project in ***Release*** mode to generate the `Aliyun.OSS.dll` file. Then install the SDK through *DLL reference*.

#### Install the SDK through DLL reference
 - Download the SDK package from Alibaba Cloud OSS official website. Unzip the package and you will find the ***Aliyun.OSS.dll*** file in the *bin* directory.
 - In the Xamarin, access `Solution`, select your project, right click *Project Name*, select `Reference`' > `Edit References` from the pop-up menu.
In the `Edit References` dialog box, click `.Net Assembly` > `Browse`. Find the directory that the SDK is unzipped to, select the `Aliyun.OSS.dll` file in the *bin* directory, and click *Open*.

## Quick use
#### Get the bucket list (List Bucket)
```csharp
    OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);    
	var buckets = client.ListBuckets();
	
    foreach (var bucket in buckets)
    {
    	Console.WriteLine(bucket.Name + ", " + bucket.Location + ", " + bucket.Owner);
    }
```
    
#### Create a bucket (Create Bucket)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	client.CreateBucket(bucketName);
```
	
#### Delete a bucket (Delete Bucket)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	client.DeleteBucket(bucketName);
```

#### Upload a file (Put Object)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	client.PutObject(bucketName, key, filePathToUpload);
```

#### Download an object (Get Object)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret); 
	var object = ossClient.GetObject(bucketName, key);	
```

#### Get the object list (List Objects)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	var listResult = client.ListObjects(bucketName);
	foreach (var summary in listResult.ObjectSummaries)
	{   
		Console.WriteLine(summary.Key);
	}
```
	
#### Delete an object (Delete Object)
```csharp
	OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
	client.DeleteObject(bucketName, key)
```

#### Others
 - In the example above, if no exception was thrown, it indicates the execution was successful. Otherwise, it indicates the execution failed. More detailed examples can be found and run in the aliyun-oss-sample project. 
	
## Notes
 - If you want to run a sample project, you must set the aliyun-oss-sdk-sample project as the 'Startup Project' and add your own AccessKeyId, AccessKeySecret, buckets and keys, and then run the project. 

## Contact us
- [Alibaba Cloud OSS official website](http://oss.aliyun.com).
- [Alibaba Cloud OSS official forum](http://bbs.aliyun.com).
- [Alibaba Cloud OSS official documentation center](http://www.aliyun.com/product/oss#Docs).
- Alibaba Cloud official technical support: [Submit a ticket](https://workorder.console.aliyun.com/#/ticket/createIndex).
