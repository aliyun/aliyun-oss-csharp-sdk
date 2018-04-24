/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;
using System.IO;
using Aliyun.OSS.Model;
using System.Net;

namespace Aliyun.OSS
{
    public class OssSelectObject : OssObject
    {
        private OssSelectStream ossStream;
        public OssSelectObject()
        {}

        /// <summary>
        /// Creates a new instance of <see cref="OssObject" /> with the key name.
        /// </summary>
        internal OssSelectObject(string key) : base(key)
        {
        }

        public override Stream Content
        {
            get
            {
                if (ossStream == null){
                    ossStream = new OssSelectStream(this.RequestId, base.Content);
                }

                return ossStream;
            }
        }
    }

    class OssSelectStream : Stream{

        private Stream _stream;
        private byte[] _frameTypeBytes;
        private byte[] _framePayloadLengthBytes;
        private byte[] _frameHeaderChecksumBytes;
        private byte[] _framePayloadChecksumBytes;
        private byte[] _frameOffsetBytes;
        private int _offset;
        private int _payloadLength;
        private long _processingOffset;
        bool flag = false;
        private string _reqId;
        public OssSelectStream(string reqId, Stream stream){
            _stream = stream;
            _frameTypeBytes = new byte[4];
            _framePayloadLengthBytes = new byte[4];
            _frameHeaderChecksumBytes = new byte[4];
            _frameOffsetBytes = new byte[8];
            _framePayloadChecksumBytes = new byte[4];
            _reqId = reqId;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return _stream.ReadTimeout;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return _stream.WriteTimeout;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        } 

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            _stream.Close();
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return _stream.CreateObjRef(requestedType);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return _stream.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return _stream.InitializeLifetimeService();
        }

        public override string ToString()
        {
            return _stream.ToString();
        }

        private void _Read(byte[] buf, int offset, int len){
            int bytes = 0;
            while(bytes < len){
                bytes += _stream.Read(buf, offset + bytes, len - bytes);
            }
        }

        private void ReadNextFrameIfNecessary()
        {
            while (_offset >= _payloadLength)
            {
                if (flag)
                {
                    _Read(_framePayloadChecksumBytes, 0, 4); //
                }
                _Read(_frameTypeBytes, 0, 4);
                _Read(_framePayloadLengthBytes, 0, 4);
                _Read(_frameHeaderChecksumBytes, 0, 4);

                _frameTypeBytes[0] = 0; // don't care the version bits
                int type = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_frameTypeBytes, 0));
                if (type == SelectResponseFrame.DataFrameType)
                {
                    _payloadLength = IPAddress.NetworkToHostOrder((BitConverter.ToInt32(_framePayloadLengthBytes, 0)));
                    _payloadLength -= 8; // the payload length includes the offset
                    _offset = 0;
                    _Read(_frameOffsetBytes, 0, 8); //
                    _processingOffset = IPAddress.NetworkToHostOrder((BitConverter.ToInt64(_frameOffsetBytes, 0)));
                    flag = true;
                }
                else if (type == SelectResponseFrame.ContiniousFrameType)
                {
                    _Read(_frameOffsetBytes, 0, 8); //
                    _processingOffset = IPAddress.NetworkToHostOrder((BitConverter.ToInt64(_frameOffsetBytes, 0)));
                    flag = true;
                }
                else if (type == SelectResponseFrame.EndFrameType){
                    _offset = 0;
                    _payloadLength = IPAddress.NetworkToHostOrder((BitConverter.ToInt32(_framePayloadLengthBytes, 0)));
                    _Read(_frameOffsetBytes, 0, 8); //
                    _processingOffset = IPAddress.NetworkToHostOrder((BitConverter.ToInt64(_frameOffsetBytes, 0)));


                    byte[] scannedSizeBuf = new byte[8];
                    _Read(scannedSizeBuf, 0, 8);
                    byte[] statusBuf = new byte[4];
                    _Read(statusBuf, 0, 4); // skip payload checksum;
                    _Read(statusBuf, 0, 4);
                    int status = IPAddress.NetworkToHostOrder((BitConverter.ToInt32(statusBuf, 0)));
                    int errorMsgSize = _payloadLength - 20;
                    string error = "Oss Select encounter an error during data processing.";
                    if (errorMsgSize > 0)
                    {
                        byte[] errorMsg = new byte[errorMsgSize];
                        _Read(errorMsg, 0, errorMsgSize);
                        error += System.Text.Encoding.UTF8.GetString(errorMsg);
                    }
                    if (status >= 400){
                        throw Aliyun.OSS.Util.ExceptionFactory.CreateException(status.ToString(), error, _reqId, ""); 
                    }
                    _payloadLength = 0;
                    break;
                }
                else{
                    throw new Exception("Unexpected frame type");
                }
            }
        }
        public override int ReadByte()
        {
           ReadNextFrameIfNecessary();
           int val = _stream.ReadByte();
            _offset++;
            return val;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ReadNextFrameIfNecessary();
            int bytesToRead = Math.Min(count, _payloadLength - _offset);

            if (bytesToRead != 0)
            {
                int bytes = _stream.Read(buffer, offset, bytesToRead);
                _offset += bytes;
                return bytes;
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
    }

    class SelectResponseFrame
    {
        public const int ContiniousFrameType = 8388612;
        public const int DataFrameType = 8388609;
        public const int EndFrameType = 8388613;

        public FrameType Type{
            get;
            set;
        }

        public int Offset{
            get;
            set;
        }

        public int PayloadLength
        {
            get;
            set;
        }

        public int HeaderChecksum
        {
            get;
            set;
        }

        public int PayloadChecksum
        {
            get;
            set;
        }

        public int TotalScannedSize
        {
            get;
            set;
        }
    }

    enum FrameType
    {
        Continious,
        Data,
        End
    }
}
