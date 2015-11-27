/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Aliyun.OSS
{
    /// <summary>
    /// Conditions匹配方式。
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// 未知的
        /// </summary>
        Unknown,

        /// <summary>
        /// 精确匹配
        /// </summary>
        Exact,

        /// <summary>
        /// 前缀匹配
        /// </summary>
        StartWith,

        /// <summary>
        /// 范围匹配
        /// </summary>
        Range
    };

    /// <summary>
    /// Conditions元组类型，目前支持二元组（{ ... }）、三元组（[ ... ]）。
    /// </summary>
    internal enum TupleType
    {
        Unknown,
        Two,
        Three
    };

    /// <summary>
    /// 抽象Condition项。
    /// </summary>
    internal abstract class AbstractConditionItem
    {
        public string Name { get; set; }
        public MatchMode MatchMode { get; set; }
        public TupleType TupleType { get; set; }

        protected AbstractConditionItem(string name, MatchMode matchMode, TupleType tupleType)
        {
            Name = name;
            MatchMode = matchMode;
            TupleType = tupleType;
        }

        public abstract string Jsonize();
    }

    /// <summary>
    /// Equal类型条件项。
    /// </summary>
    internal class EqualConditionItem : AbstractConditionItem
    {
        public string Value { get; set; }

        public EqualConditionItem(string name, string value)
            : this(name, value, TupleType.Two)
        { }

        public EqualConditionItem(string name, string value, TupleType tupleType)
            : base(name, MatchMode.Exact, tupleType)
        {
            Value = value;
        }

        public override string Jsonize()
        {
            string jsonizedCond = null;
            switch (TupleType)
            {
                case TupleType.Two:
                    jsonizedCond = String.Format("{{\"{0}\":\"{1}\"}},", Name, Value);
                    break;
                case TupleType.Three:
                    jsonizedCond = String.Format("[\"eq\",\"${0}\",\"{1}\"],", Name, Value);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Invalid tuple type " + TupleType.ToString());
            }
            return jsonizedCond;
        }
    }

    /// <summary>
    /// StartWith类型条件项。
    /// </summary>
    internal class StartWithConditionItem : AbstractConditionItem
    {
        public string Value { get; set; }

        public StartWithConditionItem(string name, string value)
            : base(name, MatchMode.StartWith, TupleType.Three)
        {
            Value = value;
        }

        public override string Jsonize()
        {
            return String.Format("[\"starts-with\",\"${0}\",\"{1}\"],", Name, Value);
        }
    }

    /// <summary>
    /// ContentLengthRange类型条件项。
    /// </summary>
    internal class RangeConditionItem : AbstractConditionItem
    {
        public long Minimum { get; set; }
        public long Maximum { get; set; }

        public RangeConditionItem(string name, long min, long max)
            : base(name, MatchMode.Range, TupleType.Three)
        {
            Minimum = min;
            Maximum = max;
        }

        public override string Jsonize()
        {
            return String.Format("[\"content-length-range\",{0},{1}],", Minimum, Maximum);
        }
    }

    /// <summary>
    /// Conditions列表，用于指定 Post 请求表单域的合法值。
    /// </summary>
    public class PolicyConditions
    {
        /// <summary>
        /// 文件长度范围
        /// </summary>
        public const string CondContentLengthRange = "content-length-range";

        /// <summary>
        /// 文件被下载时的网页的缓存行为
        /// </summary>
        public const string CondCacheControl = "Cache-Control";

        /// <summary>
        /// RFC2616中定义的HTTP请求内容类型
        /// </summary>
        public const string CondContentType = "Content-Type";

        /// <summary>
        /// 文件被下载时的名称
        /// </summary>
        public const string CondContentDisposition = "Content-Disposition";

        /// <summary>
        /// 文件被下载时的内容编码格式
        /// </summary>
        public const string CondContentEncoding = "Content-Encoding";

        /// <summary>
        /// 过期时间
        /// </summary>
        public const string CondExpires = "Expires";

        /// <summary>
        /// 名称
        /// </summary>
        public const string CondKey = "key";

        /// <summary>
        /// 成功时的重定向
        /// </summary>
        public const string CondSuccessActionRedirect = "success_action_redirect";

        /// <summary>
        /// 成功时的状态
        /// </summary>
        public const string CondSuccessActionStatus = "success_action_status";

        /// <summary>
        /// 用户自定义meta元素的前缀x-oss-meta-
        /// </summary>
        public const string CondXOssMetaPrefix = "x-oss-meta-";

        private IList<AbstractConditionItem> _conds = new List<AbstractConditionItem>(); 

        /// <summary>
        /// 采用默认匹配方式（精确匹配）添加Conditions项。
        /// </summary>
        /// <param name="name">Condition名称。</param>
        /// <param name="value">Condition数值。</param>
        public void AddConditionItem(string name, string value)
        {
            MatchRuleChecker.Check(MatchMode.Exact, name);
            _conds.Add(new EqualConditionItem(name, value));
        }

        /// <summary>
        /// 采用指定匹配模式添加Conditions项。
        /// </summary>
        /// <param name="matchMode">Conditions匹配方式。</param>
        /// <param name="name">Condition名称。</param>
        /// <param name="value">Condition数值。</param>
        public void AddConditionItem(MatchMode matchMode, string name, string value)
        {
            MatchRuleChecker.Check(matchMode, name);
            switch (matchMode)
            {
                case MatchMode.Exact:
                    _conds.Add(new EqualConditionItem(name, value, TupleType.Three));
                    break;
                
                case MatchMode.StartWith:
                    _conds.Add(new StartWithConditionItem(name, value));
                    break;

                default:
                    throw new InvalidEnumArgumentException("Unsupported match mode " + 
                        matchMode);
            }
        }

        /// <summary>
        /// 采用范围匹配模式添加Conditions项。
        /// </summary>
        /// <param name="name">Condition名称。</param>
        /// <param name="min">范围最小值。</param>
        /// <param name="max">范围最大值。</param>
        public void AddConditionItem(string name, long min, long max)
        {
            if (min > max)
                throw new ArgumentException(String.Format("Invalid range [{0}, {1}].", min, max));
            _conds.Add(new RangeConditionItem(name, min, max));
        }

        internal string Jsonize()
        {
            var jsonizedConds = new StringBuilder();
            jsonizedConds.Append("\"conditions\":[");
            foreach (var cond in _conds)
                jsonizedConds.Append(cond.Jsonize());
            if (_conds.Count > 0)
                jsonizedConds.Remove(jsonizedConds.Length - 1, 1);
            jsonizedConds.Append("]");
            return jsonizedConds.ToString();
        }
    }

    internal static class MatchRuleChecker
    {
        private static IDictionary<string, IList<MatchMode>> _supportedMatchRules 
            = new Dictionary<string, IList<MatchMode>>();

        static MatchRuleChecker()
        {
            var ordinaryMatchModes = new List<MatchMode> {MatchMode.Exact, MatchMode.StartWith};
            var specialMatchModes = new List<MatchMode> {MatchMode.Range};

            _supportedMatchRules.Add(PolicyConditions.CondContentLengthRange, specialMatchModes);

            _supportedMatchRules.Add(PolicyConditions.CondCacheControl, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondContentType, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondContentDisposition, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondContentEncoding, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondExpires, ordinaryMatchModes);

            _supportedMatchRules.Add(PolicyConditions.CondKey, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondSuccessActionRedirect, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondSuccessActionStatus, ordinaryMatchModes);
            _supportedMatchRules.Add(PolicyConditions.CondXOssMetaPrefix, ordinaryMatchModes);
        }

        public static void Check(MatchMode matchMode, string condName)
        {
            if (_supportedMatchRules.ContainsKey(condName))
            {
                var mms = _supportedMatchRules[condName];
                if (!mms.Contains(matchMode))
                {
                    throw new ArgumentException(
                        String.Format("Unsupported match mode for condition item {0}", condName));
                }
            }
        }
    }
}
