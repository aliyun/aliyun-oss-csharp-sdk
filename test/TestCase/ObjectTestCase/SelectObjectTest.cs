using Aliyun.OSS.Test.Util;
using NUnit.Framework;
using System.Text;
using System.IO;
using Aliyun.OSS.Common;

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
        private static string _test_csv_file;
        private static string _test_csv_gzip_file;

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

            _jsonMessage = "{\n" +
            "\t\"name\": \"Lora Francis\",\n" +
            "\t\"age\": 27,\n" +
            "\t\"company\": \"Staples Inc\"\n" +
            "}\n" +
            "{\n" +
            "\t\"name\": \"Eleanor Little\",\n" +
            "\t\"age\": 43,\n" +
            "\t\"company\": \"Conectiv, Inc\"\n" +
            "}\n" +
            "{\n" +
            "\t\"name\": \"Rosie Hughes\",\n" +
            "\t\"age\": 44,\n" +
            "\t\"company\": \"Western Gas Resources Inc\"\n" +
            "}\n" +
            "{\n" +
            "\t\"name\": \"Lawrence Ross\",\n" +
            "\t\"age\": 24,\n" +
            "\t\"company\": \"MetLife Inc.\"\n" +
            "}";

            _test_csv_file = Path.Combine(Config.DownloadFolder, "sample_data.csv");
            _test_csv_gzip_file = Path.Combine(Config.DownloadFolder, "sample_data.csv.gz");
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void CSVSelectObjectNormalTest()
        {
            const string key = "SqlObjectWithCsvData";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(_sqlMessage);
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select * from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.Use;
                inputFormat.RecordDelimiter = "\r\n";
                inputFormat.FieldDelimiter = ",";
                inputFormat.QuoteCharacter = "\"";
                inputFormat.CommentCharacter = "#";
                inputFormat.CompressionType = CompressionType.None;

                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.EnablePayloadCrc = true;
                outputFormat.OutputRawData = false;

                outputFormat.KeepAllColumns = false;
                outputFormat.OutputHeader = false;
                outputFormat.FieldDelimiter = ",";
                outputFormat.RecordDelimiter = "\n";


                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                string expect = "Lora Francis,School A,Staples Inc,27\n" +
                "Eleanor Little,School B,\"Conectiv, Inc\",43\n" +
                "Rosie Hughes,School C,Western Gas Resources Inc,44\n" +
                "Lawrence Ross,School D,MetLife Inc.,24\n";

                var buf = new byte[256];
                int offset = 0;

                for (int i = 1; i < 30; i++)
                {
                    int got = result.Content.Read(buf, offset, i);
                    offset += got;
                }

                Assert.AreEqual(expect.Length, offset);
                string str = Encoding.Default.GetString(buf, 0, offset);
                Assert.AreEqual(expect, str);

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectHeaderTest()
        {
            const string key = "CSVSelectObjectWithHeaderTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("name,job\nabc,def\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select name from ossobject";

                // with header
                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.Use;

                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.OutputHeader = true;

                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("name\nabc\n", Encoding.Default.GetString(buffer, 0, got));


                // without header
                inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.Use;
                request.InputFormat = inputFormat;

                outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.OutputHeader = false;
                request.OutputFormat = outputFormat;

                result = _ossClient.SelectObject(request);

                got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc\n", Encoding.Default.GetString(buffer, 0, got));

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectColumnsTest()
        {
            const string key = "CSVSelectObjectColumnsTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def,ghi,jkl\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _4 from ossobject";

                //with columns
                var inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.KeepAllColumns = true;
                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc,,,jkl\n", Encoding.Default.GetString(buffer, 0, got));


                // without columns

                outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.KeepAllColumns = false;
                request.OutputFormat = outputFormat;

                result = _ossClient.SelectObject(request);

                got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc,jkl\n", Encoding.Default.GetString(buffer, 0, got));

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectRawTest()
        {
            const string key = "CSVSelectObjectRawTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1 from ossobject";

                //with raw
                var inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.OutputRawData = true;
                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc\n", Encoding.Default.GetString(buffer, 0, got));
                Assert.AreEqual(true, result.Metadata.HttpMetadata.ContainsKey("x-oss-select-output-raw"));
                Assert.AreEqual("true", result.Metadata.HttpMetadata["x-oss-select-output-raw"]);

                // without raw
                outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.OutputRawData = false;
                request.OutputFormat = outputFormat;

                result = _ossClient.SelectObject(request);

                got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc\n", Encoding.Default.GetString(buffer, 0, got));
                Assert.AreEqual(true, result.Metadata.HttpMetadata.ContainsKey("x-oss-select-output-raw"));
                Assert.AreEqual("false", result.Metadata.HttpMetadata["x-oss-select-output-raw"]);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectSkipPartialDataTest()
        {
            const string key = "CSVSelectObjectSkipPartialDataTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def\nefg\nhij,klm\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _2 from ossobject";

                //with SkipPartial
                var inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                var options = new SelectObjectOptions();
                options.SkipPartialDataRecord = true;
                request.Options = options;

                try
                {
                    _ossClient.SelectObject(request);
                    Assert.IsTrue(false);
                }
                catch (OssException e)
                {
                    Assert.AreEqual("InvalidCsvLine", e.ErrorCode);
                }

                //without SkipPartial
                binaryData = Encoding.ASCII.GetBytes("abc,def\nefg\nhij,klm\n123,456\n");
                stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _2 from ossobject";
                inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                options = new SelectObjectOptions();
                options.SkipPartialDataRecord = false;
                request.Options = options;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc,def\nefg,\nhij,klm\n123,456\n", Encoding.Default.GetString(buffer, 0, got));

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectOutputDelimitersTest()
        {
            const string key = "CSVSelectObjectOutputDelimitersTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _2 from ossobject";

                //with delimiter
                var inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.RecordDelimiter = "\r\n";
                outputFormat.FieldDelimiter = "|";
                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc|def\r\n", Encoding.Default.GetString(buffer, 0, got));

                //without delimiter
                request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _2 from ossobject";
                inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                outputFormat = new SelectObjectCSVOutputFormat();
                request.OutputFormat = outputFormat;

                result = _ossClient.SelectObject(request);

                got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc,def\n", Encoding.Default.GetString(buffer, 0, got));

            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectLineRangeTest()
        {
            const string key = "CSVSelectObjectLineRangeTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def\n123,456\n789,efg\nhij,klm\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var metaRequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metaRequest.InputFormat = new CreateSelectObjectMetaCSVInputFormat();
                _ossClient.CreateSelectObjectMeta(metaRequest);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1, _2 from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.Range = "line-range=1-2";
                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                request.OutputFormat = outputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("123,456\n789,efg\n", Encoding.Default.GetString(buffer, 0, got));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectCommentCharacterTest()
        {
            const string key = "CSVSelectObjectCommentCharacterTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def\n`123,456\n#ghi,jkl\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var metaRequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metaRequest.InputFormat = new CreateSelectObjectMetaCSVInputFormat();
                _ossClient.CreateSelectObjectMeta(metaRequest);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _1 from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.CommentCharacter = "`";
                request.InputFormat = inputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("abc\n#ghi\n", Encoding.Default.GetString(buffer, 0, got));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectQuoteCharacterTest()
        {
            const string key = "CSVSelectObjectQuoteCharacterTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("'abc','def\n123','456'\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _2 from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                request.InputFormat = inputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("'def\n'456'\n", Encoding.Default.GetString(buffer, 0, got));

                //use '
                inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.QuoteCharacter = "'";
                request.InputFormat = inputFormat;

                result = _ossClient.SelectObject(request);

                got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("'def\n123'\n", Encoding.Default.GetString(buffer, 0, got));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectInputDelimitersTest()
        {
            const string key = "CSVSelectObjectInputDelimitersTest";

            try
            {
                var buffer = new byte[256];
                byte[] binaryData = Encoding.ASCII.GetBytes("abc,def|123,456|7891334\n\n777,888|999,222|012345\n\n");
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select _2,_3 from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.RecordDelimiter = "\n\n";
                inputFormat.FieldDelimiter = "|";
                request.InputFormat = inputFormat;

                var result = _ossClient.SelectObject(request);

                var got = result.Content.Read(buffer, 0, buffer.Length);

                Assert.AreEqual("123,456|7891334\n\n999,222|012345\n\n", Encoding.Default.GetString(buffer, 0, got));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectGzipDataTest()
        {
            const string key = "gzip_data.csv.gz";

            try
            {
                var buffer = new byte[256];
                _ossClient.PutObject(_bucketName, key, _test_csv_gzip_file);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select * from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.None;
                inputFormat.RecordDelimiter = "\n";
                inputFormat.FieldDelimiter = ",";
                inputFormat.QuoteCharacter = "\"";
                inputFormat.CompressionType = CompressionType.GZIP;

                request.InputFormat = inputFormat;

                var result = _ossClient.SelectObject(request);

                var md5 = FileUtils.ComputeContentMd5(result.Content);

                Assert.AreEqual(md5, "2e39f40a7d65fcf0acc95654638482da");
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CSVSelectObjectBigCSVDataTest()
        {
            const string key = "gzip_data.csv";

            try
            {
                var buffer = new byte[256];
                _ossClient.PutObject(_bucketName, key, _test_csv_file);

                var request = new SelectObjectRequest(_bucketName, key);
                request.Expression = "select Year, StateAbbr,StateDesc from ossobject limit 200";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.Use;

                request.InputFormat = inputFormat;

                var result = _ossClient.SelectObject(request);

                StreamReader sr = new StreamReader(result.Content);

                int cnt = 0; 

                while (sr.ReadLine() != null)
                {
                    cnt++;
                }
                Assert.AreEqual(200, cnt);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void CreateSelectObjectWithCsvDataTest()
        {
            const string key = "CreateSelectObjectWithCsvDataTest";
            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(_sqlMessage);
                var stream = new MemoryStream(binaryData);

                _ossClient.PutObject(_bucketName, key, stream);

                var metarequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metarequest.OverwriteIfExists = false;

                var input = new CreateSelectObjectMetaCSVInputFormat();
                Assert.AreEqual(input.CompressionType, CompressionType.None);
                Assert.AreEqual(input.RecordDelimiter, null);
                Assert.AreEqual(input.FieldDelimiter, null);
                Assert.AreEqual(input.QuoteCharacter, null);

                input.RecordDelimiter = "\r\n";
                input.FieldDelimiter = ",";
                input.QuoteCharacter = "\"";

                metarequest.InputFormat = input;

                var metaresult = _ossClient.CreateSelectObjectMeta(metarequest);
                Assert.AreEqual(metaresult.SplitsCount, 1);
                Assert.AreEqual(metaresult.RowsCount, 5);
                Assert.AreEqual(metaresult.ColumnsCount, 4);
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
                request.Expression = "select * from ossobject";

                var inputFormat = new SelectObjectCSVInputFormat();
                inputFormat.FileHeaderInfo = FileHeaderInfo.Use;
                inputFormat.RecordDelimiter = "\r\n";
                inputFormat.FieldDelimiter = ",";
                inputFormat.QuoteCharacter = "\"";
                inputFormat.CommentCharacter = "#";
                inputFormat.CompressionType = CompressionType.None;


                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.KeepAllColumns = false;
                outputFormat.OutputRawData = true;
                outputFormat.OutputHeader = false;
                outputFormat.EnablePayloadCrc = false;

                outputFormat.FieldDelimiter = ",";
                outputFormat.RecordDelimiter = "\n";

                request.OutputFormat = outputFormat;

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
                request.Expression = "select * from ossobject";

                var inputFormat = new SelectObjectJSONInputFormat();
                inputFormat.CompressionType = CompressionType.None;
                inputFormat.Type = JSONType.LINES;
                request.InputFormat = inputFormat;

                var outputFormat = new SelectObjectCSVOutputFormat();
                outputFormat.KeepAllColumns = false;
                outputFormat.OutputRawData = false;
                outputFormat.OutputHeader = false;
                outputFormat.EnablePayloadCrc = true;

                outputFormat.RecordDelimiter = "\n";

                request.OutputFormat = outputFormat;

                _ossClient.SelectObject(request);

                var metarequest = new CreateSelectObjectMetaRequest(_bucketName, key);
                metarequest.OverwriteIfExists = false;

                var input = new CreateSelectObjectMetaJSONInputFormat();
                input.CompressionType = CompressionType.None;
                input.Type = JSONType.LINES;
                metarequest.InputFormat = input;

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

