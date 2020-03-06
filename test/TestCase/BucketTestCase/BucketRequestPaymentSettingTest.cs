using System;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void BucketRequestPaymentBasicTest()
        {
            //default
            var result = _ossClient.GetBucketRequestPayment(_bucketName);
            Assert.AreEqual(result.Payer, RequestPayer.BucketOwner);

            //set to Requester
            var request = new SetBucketRequestPaymentRequest(_bucketName, RequestPayer.Requester);
            _ossClient.SetBucketRequestPayment(request);

            result = _ossClient.GetBucketRequestPayment(_bucketName);
            Assert.AreEqual(result.Payer, RequestPayer.Requester);

            //set to BucketOwner
            request = new SetBucketRequestPaymentRequest(_bucketName, RequestPayer.BucketOwner);
            _ossClient.SetBucketRequestPayment(request);

            result = _ossClient.GetBucketRequestPayment(_bucketName);
            Assert.AreEqual(result.Payer, RequestPayer.BucketOwner);
        }
    }
}

