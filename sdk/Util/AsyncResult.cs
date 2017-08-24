/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics;
using System.Threading;
using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Util
{
    /// <summary>
    /// The implementation of <see cref="IAsyncResult"/>
    /// that represents the status of an async operation.
    /// </summary>
    internal abstract class AsyncResult : IAsyncResult, IDisposable
    {
        #region Fields

        private readonly object _asyncState;
        private bool _isCompleted;
        private readonly AsyncCallback _userCallback;
        private ManualResetEvent _asyncWaitEvent;
        private Exception _exception;

        #endregion

        #region IAsyncResult Members

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        public object AsyncState
        {
            get { return _asyncState; }
        }

        public AsyncCallback Callback
        {
            get { return _userCallback; }
        }

        /// <summary>
        /// Gets a <see cref="WaitHandle"/> that is used to wait for an asynchronous operation to complete. 
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_asyncWaitEvent != null)
                    return _asyncWaitEvent;

                _asyncWaitEvent = new ManualResetEvent(false);

                if (IsCompleted)
                    _asyncWaitEvent.Set();

                return _asyncWaitEvent;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        public bool CompletedSynchronously { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        [DebuggerNonUserCode]
        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        #endregion

        /// <summary>
        /// Initializes an instance of <see cref="AsyncResult"/>.
        /// </summary>
        /// <param name="callback">The callback method when the async operation completes.</param>
        /// <param name="state">A user-defined object that qualifies or contains information about an asynchronous operation.</param>
        protected AsyncResult(AsyncCallback callback, object state)
        {
            _userCallback = callback;
            _asyncState = state;
        }

        /// <summary>
        /// Completes the async operation with an exception.
        /// </summary>
        /// <param name="ex">Exception from the async operation.</param>
        public void Complete(Exception ex)
        {
            _exception = ex;
            NotifyCompletion();
        }

        /// <summary>
        /// When called in the dervied classes, wait for completion.
        /// It throws exception if the async operation ends with an exception.
        /// </summary>
        protected void WaitForCompletion()
        {
            if (!IsCompleted)
                AsyncWaitHandle.WaitOne();

            if (_exception != null)
                throw _exception;
        }

        /// <summary>
        /// When called in the derived classes, notify operation completion
        /// by setting <see cref="P:AsyncWaitHandle"/> and calling the user callback.
        /// </summary>
        protected void NotifyCompletion()
        {
            _isCompleted = true;
            if (_asyncWaitEvent != null)
                _asyncWaitEvent.Set();

            if (_userCallback != null)
            {
                var httpAsyncResult = this as ServiceClientImpl.HttpAsyncResult;
                Debug.Assert(httpAsyncResult != null);
                _userCallback(httpAsyncResult.AsyncState as RetryableAsyncResult);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object and release resource.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// When overrided in the derived classes, release resources.
        /// </summary>
        /// <param name="disposing">Whether the method is called <see cref="M:Dispose"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _asyncWaitEvent != null)
            {
                _asyncWaitEvent.Close();
                _asyncWaitEvent = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the status of an async operation.
    /// It also holds the result of the operation.
    /// </summary>
    /// <typeparam name="T">Type of the operation result.</typeparam>
    internal class AsyncResult<T> : AsyncResult
    {
        /// <summary>
        /// The result of the async operation.
        /// </summary>
        private T _result;

        /// <summary>
        /// Initializes an instance of <see cref="AsyncResult&lt;T&gt;"/>.
        /// </summary>
        /// <param name="callback">The callback method when the async operation completes.</param>
        /// <param name="state">A user-defined object that qualifies or contains information about an asynchronous operation.</param>
        public AsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        { }

        /// <summary>
        /// Gets result and release resources.
        /// </summary>
        /// <returns>The instance of result.</returns>
        public T GetResult()
        {
            base.WaitForCompletion();
            return _result;
        }

        /// <summary>
        /// Sets result and notify completion.
        /// </summary>
        /// <param name="result">The instance of result.</param>
        public void Complete(T result)
        {
            // Complete should not throw if disposed.
            _result = result;
            base.NotifyCompletion();
        }
    }
}
