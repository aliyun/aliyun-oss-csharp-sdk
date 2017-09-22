/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;


namespace Aliyun.OSS
{
    /// <summary>
    /// Defining a cross origin resource sharing rule
    /// </summary>
    public class CORSRule
    {
        private IList<String> _allowedOrigins = new List<String>();
        private IList<String> _allowedMethods = new List<String>();
        private IList<String> _allowedHeaders = new List<String>();
        private IList<String> _exposeHeaders = new List<String>();
        private Int32 _maxAgeSeconds;

        /// <summary>
        /// Allowed origins. One origin could contain at most one wildcard (*).
        /// </summary>
        public IList<String> AllowedOrigins
        {
            get { return ((List<string>) _allowedOrigins).AsReadOnly(); }
            set
            {
                if (CountOfAsterisk(value) > 1)
                    throw new ArgumentException("At most one asterisk wildcard allowed.");
                _allowedOrigins = value;
            }
        }

        /// <summary>
        /// Allowed HTTP Method. Valid values are GET,PUT,DELETE,POST,HEAD.
        /// This property is to specify the value of Access-Control-Allow-Methods header in the preflight response.
        /// It means the allowed methods in the actual CORS request. 
        /// </summary>
        public IList<String> AllowedMethods
        {
            get { return ((List<string>)_allowedMethods).AsReadOnly(); }
            set { _allowedMethods = value; }
        }

        /// <summary>
        /// Get or set Allowed Headers.
        /// This property is to specify the value of Access-Control-Allowed-Headers in the preflight response.
        /// It defines the allowed headers in the actual CORS request.
        /// Each allowed header can have up to one wildcard (*).
        /// </summary>
        public IList<String> AllowedHeaders
        {
            get { return ((List<string>)_allowedHeaders).AsReadOnly(); }
            set
            {
                if (CountOfAsterisk(value) > 1)
                    throw new ArgumentException("At most one asterisk wildcard allowed.");
                _allowedHeaders = value;
            }
        }

        /// <summary>
        /// Get or set exposed headers in the CORS response. Wildcard(*) is not allowed.
        /// This property is to specify the value of Access-Control-Expose-Headers in the preflight response.
        /// </summary>
        public IList<String> ExposeHeaders
        {
            get { return ((List<string>)_exposeHeaders).AsReadOnly(); }
            set
            {
                if (CountOfAsterisk(value) > 0)
                    throw new ArgumentException("Asterisk wildcard not allowed.");
                _exposeHeaders = value;
            }
        }

        /// <summary>
        /// HTTP Access-Control-Max-Age's getter and setter, in seconds.
        /// The Access-Control-Max-Age header indicates how long the results of a preflight request (OPTIONS) can be cached in a preflight result cache.
        /// The max value is 999999999.
        /// </summary>
        public Int32 MaxAgeSeconds 
        {
            get { return _maxAgeSeconds; }
            set
            {
                if (value < 0 || value > 999999999)
                    throw new ArgumentException("MaxAge should not less than 0 or greater than 999999999");
                _maxAgeSeconds = value;
            }
        }

        /// <summary>
        /// Adds one allowed origin.
        /// </summary>
        /// <param name="allowedOrigin">Allowed origin </param>
        public void AddAllowedOrigin(String allowedOrigin)
        {
            if (allowedOrigin == null)
                throw new ArgumentNullException("allowedOrigin");

            var hasAsterisk = allowedOrigin.Contains("*");
            if (hasAsterisk && CountOfAsterisk(_allowedOrigins) > 0)
                throw new ArgumentException("At most one asterisk wildcard allowed.");
            _allowedOrigins.Add(allowedOrigin);
        }

        /// <summary>
        /// Adds one allowed HTTP method
        /// </summary>
        /// <param name="allowedMethod">allowed http method, such as GET,PUT,DELETE,POST,HEAD</param>
        public void AddAllowedMethod(String allowedMethod)
        {
            if (!InAllowedMethods(allowedMethod))
                throw new ArgumentException("allowedMethod not in allowed methods(GET/PUT/DELETE/POST/HEAD)");

            _allowedMethods.Add(allowedMethod);
        }

        /// <summary>
        /// Adds a allowed header.
        /// </summary>
        /// <param name="allowedHeader">allowed header</param>
        public void AddAllowedHeader(String allowedHeader)
        {
            if (allowedHeader == null)
                throw new ArgumentNullException("allowedHeader");

            var hasAsterisk = allowedHeader.Contains("*");
            if (hasAsterisk && CountOfAsterisk(AllowedHeaders) > 0)
                throw new ArgumentException("At most one asterisk wildcard allowed.");
            _allowedHeaders.Add(allowedHeader);
        }

        /// <summary>
        /// adds an expose header.
        /// </summary>
        /// <param name="exposedHeader">an expose-header</param>
        public void AddExposeHeader(String exposedHeader)
        {
            if (exposedHeader == null)
                throw new ArgumentNullException("exposedHeader");

            var hasAsterisk = exposedHeader.Contains("*");
            if (hasAsterisk)
                throw new ArgumentException("Asterisk wildcard not allowed.");
            _exposeHeaders.Add(exposedHeader);
        }

        /// <summary>
        /// Gets the wildcard count from the parameter items.
        /// </summary>
        /// <param name="items">items to count wildcard from</param>
        /// <returns>wildcard count</returns>
        private static int CountOfAsterisk(IEnumerable<string> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item) && item.Trim() == "*")
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Checks if a method is allowed.
        /// </summary>
        /// <param name="allowedMethod">the http method to check</param>
        /// <returns>True:the method is allowed; False: The method is not allowed</returns>
        private static bool InAllowedMethods(string allowedMethod)
        {
            if (string.IsNullOrEmpty(allowedMethod))
                throw new ArgumentException("allowedMethod should not be null or empty");

            var tmp = allowedMethod.Trim();
            if (tmp == "GET" || tmp == "PUT" || tmp == "DELETE" || 
                tmp == "POST" || tmp == "HEAD")
            {
                return true;
            }

            return false;
        }
    }
}
