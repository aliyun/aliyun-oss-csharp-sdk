using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectSelectTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;
        private static string _localCsvFile;
        private static string _localCsvOutputFile;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = "oss-select";
            //_ossClient.CreateBucket(_bucketName);
            //create sample object
            _keyName = "bigcsv_normal.csv";


            _localCsvFile = Config.LocalCsvFile;
            _localCsvOutputFile = Config.LocalCsvOutputFile;
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            //OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void SelectCsvBasicTest()
        {
            try
            {
                // put example csv
                //_ossClient.PutObject(_bucketName, _keyName, _localCsvFile);
                /*OssObject ossObject = _ossClient.GetObject(_bucketName, _keyName);
                using (FileStream fs = new FileStream(_localCsvOutputFile, FileMode.Create))
                {
                    OssTestUtils.WriteTo(ossObject.Content, fs);
                }*/

                // select the csv
                SelectObjectRequest request = new SelectObjectRequest(_bucketName, _keyName, "select _1, _4 from ossobject where _1 like  'Tom%' and _4 > 50");
                OssObject ossObject = _ossClient.SelectObject(request);
                using (FileStream fs = new FileStream(_localCsvOutputFile, FileMode.Create))
                {
                    OssTestUtils.WriteTo(ossObject.Content, fs);
                }

            }
            finally
            {
                //_ossClient.DeleteObject(_bucketName, _keyName);
            }
        }
    }
}
