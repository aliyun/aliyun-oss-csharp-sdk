using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Domain;
using NUnit.Framework;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using Aliyun.OSS.Model;
using Aliyun.OSS;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class SelectObjectTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;
        private static string _sqlMessage;
        private static string _jsonMessage;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
            //create sample object
            _keyName = OssTestUtils.GetObjectKey(_className);
            _sqlMessage = "name,school,company,age\r\n" +
            "Lora Francis,School A,Staples Inc,27\r\n" +
            "Eleanor Little,School B,\"Conectiv, Inc\",43\r\n" +
            "Rosie Hughes,School C,Western Gas Resources Inc,44\r\n" +
            "Lawrence Ross,School D,MetLife Inc.,24";

            _jsonMessage = "{\n"+
            "\t\"name\": \"Lora Francis\",\n"+
            "\t\"age\": 27,\n"+
            "\t\"company\": \"Staples Inc\"\n"+
            "}\n"+
            "{\n"+
            "\t\"name\": \"Eleanor Little\",\n"+
            "\t\"age\": 43,\n"+
            "\t\"company\": \"Conectiv, Inc\"\n"+
            "}\n"+
            "{\n"+
            "\t\"name\": \"Rosie Hughes\",\n"+
            "\t\"age\": 44,\n"+
            "\t\"company\": \"Western Gas Resources Inc\"\n"+
            "}\n"+
            "{\n"+
            "\t\"name\": \"Lawrence Ross\",\n"+
            "\t\"age\": 24,\n"+
            "\t\"company\": \"MetLife Inc.\"\n"+
            "}";
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void NormalSelectObjectWithCsvDataTest()
        {
            const string key = "SqlObjectWithCsvData";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(_sqlMessage);
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.InputFormatType = InputFormatTypes.CSV;
                request.Expression = "select * from ossobject";
                request.lineRangeIsSet = false;
                request.splitRangeIsSet = false;

                SelectObjectInputFormat InputFormat = new SelectObjectInputFormat();
                InputFormat.CSVs = new SelectObjectInputFormat.InputCSV();
                InputFormat.CSVs.FileHeaderInfos = FileHeaderInfo.Use;
                InputFormat.CSVs.RecordDelimiter = "\r\n";
                InputFormat.CSVs.FieldDelimiter = ",";
                InputFormat.CSVs.QuoteCharacter = "\"";
                InputFormat.CSVs.CommentCharacter = "#";
                InputFormat.CompressionTypes = CompressionType.NONE ;


                request.InputFormats = InputFormat;

                SelectObjectOutputFormat OutputFormat = new SelectObjectOutputFormat();
                OutputFormat.KeepAllColumns = false;
                OutputFormat.OutputRawData = false;
                OutputFormat.OutputHeader = false;
                OutputFormat.EnablePayloadCrc = true;

                OutputFormat.CSVs = new SelectObjectOutputFormat.OutputCSV();
                OutputFormat.CSVs.FieldDelimiter = ",";
                OutputFormat.CSVs.RecordDelimiter = "\n";

                request.OutputFormats = OutputFormat;
                request.SkipPartialDataRecord = false;
                request.MaxSkippedRecordsAllowed = 0;

                var result = _ossClient.SelectObject(request);

                string expect = "Lora Francis,School A,Staples Inc,27\n"+
                "Eleanor Little,School B,\"Conectiv, Inc\",43\n" +
                "Rosie Hughes,School C,Western Gas Resources Inc,44\n"+
                "Lawrence Ross,School D,MetLife Inc.,24\n";

                var buf = new byte[200];
                result.Content.Read(buf, 0, 10);
                string str1 = System.Text.Encoding.Default.GetString(buf);
                Assert.AreEqual(expect.Substring(0, 10), str1.Substring(0, 10));

                result.Content.Read(buf, 10, 10);
                string str2 = System.Text.Encoding.Default.GetString(buf);
                Assert.AreEqual(expect.Substring(0, 20), str2.Substring(0, 20));

                result.Content.Read(buf, 20, 50);
                string str3 = System.Text.Encoding.Default.GetString(buf);
                Assert.AreEqual(expect.Substring(0, 70), str3.Substring(0, 70));

                result.Content.Read(buf, 70, 50);
                string str4 = System.Text.Encoding.Default.GetString(buf);
                Assert.AreEqual(expect.Substring(0, 120), str4.Substring(0, 120));

                result.Content.Read(buf, 120, 50);
                string str5 = System.Text.Encoding.Default.GetString(buf);
                Assert.AreEqual(expect, str5.Substring(0, 170));

                var metarequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metarequest.InputFormatType = InputFormatTypes.CSV;
                metarequest.OverwriteIfExists = false;

                var input = new SelectObjectMetaInputFormat();
                input.CompressionTypes = CompressionType.NONE;
                input.CSVs = new SelectObjectMetaInputFormat.InputCSV();
                input.CSVs.RecordDelimiter = "\r\n";
                input.CSVs.FieldDelimiter = ",";
                input.CSVs.QuoteCharacter = "\"";

                metarequest.InputFormats = input;

                var metaresult = _ossClient.CreateSelectObjectMeta(metarequest);
                Assert.AreEqual(metaresult.SplitsCount, 1);
                Assert.AreEqual(metaresult.RowsCount, 5);
                Assert.AreEqual(metaresult.ColsCount, 4);

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void NormalSelectObjectWithOutputRawTest()
        {
            const string key = "SqlObjectWithoutOutputRaw";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(_sqlMessage);
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.InputFormatType = InputFormatTypes.CSV;
                request.Expression = "select * from ossobject";
                request.lineRangeIsSet = false;
                request.splitRangeIsSet = false;

                SelectObjectInputFormat InputFormat = new SelectObjectInputFormat();
                InputFormat.CSVs = new SelectObjectInputFormat.InputCSV();
                InputFormat.CSVs.FileHeaderInfos = FileHeaderInfo.Use;
                InputFormat.CSVs.RecordDelimiter = "\r\n";
                InputFormat.CSVs.FieldDelimiter = ",";
                InputFormat.CSVs.QuoteCharacter = "\"";
                InputFormat.CSVs.CommentCharacter = "#";
                InputFormat.CompressionTypes = CompressionType.NONE;


                request.InputFormats = InputFormat;

                SelectObjectOutputFormat OutputFormat = new SelectObjectOutputFormat();
                OutputFormat.KeepAllColumns = false;
                OutputFormat.OutputRawData = true;
                OutputFormat.OutputHeader = false;
                OutputFormat.EnablePayloadCrc = false;

                OutputFormat.CSVs = new SelectObjectOutputFormat.OutputCSV();
                OutputFormat.CSVs.FieldDelimiter = ",";
                OutputFormat.CSVs.RecordDelimiter = "\n";

                request.OutputFormats = OutputFormat;
                request.SkipPartialDataRecord = false;
                request.MaxSkippedRecordsAllowed = 0;

                _ossClient.SelectObject(request);

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void NormalSelectObjectWithJsonType()
        {
            const string key = "SqlObjectWithJsonType";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(_jsonMessage);
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.InputFormatType = InputFormatTypes.JSON;
                request.Expression = "select * from ossobject";
                request.lineRangeIsSet = false;
                request.splitRangeIsSet = false;

                SelectObjectInputFormat JSONInputFormat = new SelectObjectInputFormat();
                JSONInputFormat.CompressionTypes = CompressionType.NONE;
                JSONInputFormat.JSONInputs = new SelectObjectInputFormat.JSONInput();
                JSONInputFormat.JSONInputs.Types = JsonType.LINES;
                request.InputFormats = JSONInputFormat;


                SelectObjectOutputFormat JSONOutputFormat = new SelectObjectOutputFormat();
                JSONOutputFormat.KeepAllColumns = false;
                JSONOutputFormat.OutputRawData = false;
                JSONOutputFormat.OutputHeader = false;
                JSONOutputFormat.EnablePayloadCrc = true;

                JSONOutputFormat.JSONOutputs = new SelectObjectOutputFormat.JSONOutput();
                JSONOutputFormat.JSONOutputs.RecordDelimiter = "\n";

                request.OutputFormats = JSONOutputFormat;

                request.SkipPartialDataRecord = false;
                request.MaxSkippedRecordsAllowed = 0;


                _ossClient.SelectObject(request);

                var metarequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metarequest.InputFormatType = InputFormatTypes.JSON;
                metarequest.OverwriteIfExists = false;

                var input = new SelectObjectMetaInputFormat();
                input.CompressionTypes = CompressionType.NONE;
                input.JSONInputs = new SelectObjectMetaInputFormat.JSONInput();
                input.JSONInputs.Types = JsonType.LINES;

                metarequest.InputFormats = input;

                var metaresult = _ossClient.CreateSelectObjectMeta(metarequest);

                Assert.AreEqual(metaresult.SplitsCount, 1);
                Assert.AreEqual(metaresult.RowsCount, 4);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }
    }
}

