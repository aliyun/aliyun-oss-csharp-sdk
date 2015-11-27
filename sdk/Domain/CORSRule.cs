using System;
using System.Collections.Generic;


namespace Aliyun.OSS
{
    /// <summary>
    /// 表示一条CORS规则。
    /// </summary>
    public class CORSRule
    {
        private IList<String> _allowedOrigins = new List<String>();
        private IList<String> _allowedMethods = new List<String>();
        private IList<String> _allowedHeaders = new List<String>();
        private IList<String> _exposeHeaders = new List<String>();
        private Int32 _maxAgeSeconds;

        /// <summary>
        /// 指定允许的跨域请求的来源，允许使用多个元素来指定多个允许的来源。
        /// 允许使用最多一个*通配符。如果指定为“ *”则表示允许所有的来源的跨域请求。
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
        /// 指定允许的跨域请求方法（GET,PUT,DELETE,POST,HEAD）。
        /// </summary>
        public IList<String> AllowedMethods
        {
            get { return ((List<string>)_allowedMethods).AsReadOnly(); }
            set { _allowedMethods = value; }
        }

        /// <summary>
        /// 控制在 OPTIONS 预取指令中 Access-Control-Request-Headers 头中指定的 header
        /// 是否允许。在 Access-Control-Request-Headers 中指定的每个 header 都必须在
        /// AllowedHeader 中有一条对应的项。允许使用最多一个*通配符。
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
        /// 指定允许用户从应用程序中访问的响应头（例如一个 Javascript 的
        /// XMLHttpRequest 对象。不允许使用 *通配符。
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
        /// 指定浏览器对特定资源的预取（ OPTIONS）请求返回结果的缓存时间，单位为秒，
        /// 最大值不超过999999999，且一个 CORSRule 里面最多允许出现一个。
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
        /// 添加一条AllowedOrigin。
        /// </summary>
        /// <param name="allowedOrigin">指定允许的跨域请求的来源，允许使用最多一个“*”通配符。 </param>
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
        /// 添加一条AllowedMethod。
        /// </summary>
        /// <param name="allowedMethod">指定允许的跨域请求方法。 类型：GET,PUT,DELETE,POST,HEAD</param>
        public void AddAllowedMethod(String allowedMethod)
        {
            if (!InAllowedMethods(allowedMethod))
                throw new ArgumentException("allowedMethod not in allowed methods(GET/PUT/DELETED/POST/HEAD)");

            _allowedMethods.Add(allowedMethod);
        }

        /// <summary>
        /// 添加一条AllowedHeader。
        /// </summary>
        /// <param name="allowedHeader">控制在OPTIONS预取指令中Access-Control-Request-Headers头中指定的header是否允许 </param>
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
        /// 添加一条ExposeHeader。
        /// </summary>
        /// <param name="exposedHeader">指定允许用户从应用程序中访问的响应头（例如一个Javascript的XMLHttpRequest对象）</param>
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
        /// 获取*通配符的个数。
        /// </summary>
        /// <param name="items">获取这些数据里的通配符个数</param>
        /// <returns>通配符的个数</returns>
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
        /// 判断某个method是否被允许
        /// </summary>
        /// <param name="allowedMethod">需要判断的method</param>
        /// <returns>是否被允许</returns>
        private static bool InAllowedMethods(string allowedMethod)
        {
            if (string.IsNullOrEmpty(allowedMethod))
                throw new ArgumentException("allowedMethod should not be null or empty");

            var tmp = allowedMethod.Trim();
            if (tmp == "GET" || tmp == "PUT" || tmp == "DELETED" || 
                tmp == "POST" || tmp == "HEAD")
            {
                return true;
            }

            return false;
        }
    }
}
