using System;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Aliyun SDK for .NET Samples!");

            try
            {
                // 1. Create bucket sample
                //const string bucketName = "cname-test-for-sdk";
                //CreateBucketSample.CreateBucket(bucketName);

                // 2. List bucket sample
                //ListBucketsSample.ListBuckets();

                // 3. Set bucket cors sample
                //SetBucketCorsSample.SetBucketCors();

                // 4. Set bucket acl sample
                //SetBucketAclSample.SetBucketAcl();

                // 5. CName sample
                //CNameSample.CNameOperation();

                // 6. Get/Set bucket referer list sample
                //SetBucketRefererSample.SetBucketReferer();

                // 7.1 Put object sample
                //PutObjectSample.PutObject();

                // 7.2 Async put object sample
                //PutObjectSample.AsyncPutObject();

                // 8.1 Append object sample
                // AppendObjectSample.AppendObject();

                // 8.2 Async append object sample 
                // AppendObjectSample.AsyncAppendObject();

                // 9. Delete objects sample
                // DeleteObjectsSample.DeleteObjects();

                // 10.1 Upload multipart sample
                //const string bucketName = "<bucket name>";
                //const string objectName = "<object key>";
                //const string fileToUpload = "<local file to upload>";
                //const int partSize = 5 * 1024 * 1024; // 5MB
                //MultipartUploadSample.UploadMultipart(bucketName, objectName, fileToUpload, partSize);

                // 10.2 Async upload multipart sample
                //const string bucketName = "<bucket name>";
                //const string objectName = "<object name>";
                //const string fileToUpload = "<file to upload>";
                //const int partSize = 5 * 1024 * 1024; // 5MB
                //MultipartUploadSample.AsyncUploadMultipart(bucketName, objectName, fileToUpload, partSize);

                // 11.1 Upload multipart copy sample 
                //const string targetName = "<target bucket>";
                //const string targetKey = "<target key>";
                //const string sourceBucket = "<source bucket>";
                //const string sourceKey = "<source key>";
                //const int partSize = 5 * 1024 * 1024; // 5MB
                //MultipartUploadSample.UploadMultipartCopy(targetName, targetKey, sourceBucket, sourceKey, partSize);

                // 11.2 Async upload multipart copy sample 
                //const string targetName = "<target bucket>";
                //const string targetKey = "<target key>";
                //const string sourceBucket = "<source bucket>";
                //const string sourceKey = "<source key>";
                //const int partSize = 5 * 1024 * 1024; // 5MB
                //MultipartUploadSample.AsyncUploadMultipartCopy(targetName, targetKey, sourceBucket, sourceKey, partSize);

                // 11.3 Upload multipart copy sample 
                //const string bucketName = "shaoqiang";
                //MultipartUploadSample.ListMultipartUploads(bucketName);

                // 12. Get object by range sample
                //GetObjectByRangeSample.GetObjectPartly();

                // 13.1 Get object once sample.
                //GetObjectSample.GetObject();

                // 13.2 Async get object once sample.
                //GetObjectSample.AsyncGetObject();

                // 14. Get object by using url signature.
                //UrlSignatureSample.GetObjectBySignedUrl();

                // 15. Put object by using url signature.
                //UrlSignatureSample.PutObjectBySignedUrl();

                // 16.1 Copy object sample
                //CopyObjectSample.CopyObject();

                // 16.2 Async copy object sample
                //CopyObjectSample.AsyncCopyObject();

                // 17. Create empty folder
                //CreateEmptyFolderSample.CreateEmptyFolder();

                // 18.1 List objects
                //ListObjectsSample.ListObjects();

                // 18.2 Async list objects
                //ListObjectsSample.AsyncListObjects();

                // 19. Generate post policy
                //PostPolicySample.GenPostPolicy();

                // 20. Set bucket lifecycle
                //BucketLifecycleSample.SetBucketLifecycle();

                // 21. Get bucket lifecycle
                //BucketLifecycleSample.GetBucketLifecycle();

                // 22. Determine bucket exist
                //DoesBucketExistSample.DoesBucketExist("oss");

                // 23. Determine object exist
                //DoesObjectExistSample.DoesObjectExist("oss-test", "conf.ini2");
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                 ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}