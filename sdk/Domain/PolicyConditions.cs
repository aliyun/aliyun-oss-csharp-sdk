/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Aliyun.OSS
{
    /// <summary>
    /// The match mode enum
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Exactly match
        /// </summary>
        Exact,

        /// <summary>
        /// Match the prefix only
        /// </summary>
        StartWith,

        /// <summary>
        /// Match the size range. For example, the policy could be applied the files of size between 1KB to 4KB.
        /// </summary>
        Range
    };

    /// <summary>
    /// Tuplre type enum.!-- Currently only two tuple {key:value} and three tuple type (tuple1,tuple2,tuple3) are supported.
    /// </summary>
    internal enum TupleType
    {
        Unknown,
        Two,
        Three
    };

    /// <summary>
    /// The abstract Condition Item.
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
    /// EqualConditionItem definition
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
    /// StartwithConditionItem definition.
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
    /// Content size's RangeConditionItem definition.
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
    /// Conditions list. It specifies all valid fields in the post form.
    /// </summary>
    public class PolicyConditions
    {
        /// <summary>
        /// Content length range
        /// </summary>
        public const string CondContentLengthRange = "content-length-range";

        /// <summary>
        /// The cache control behavior for downloading files
        /// </summary>
        public const string CondCacheControl = "Cache-Control";

        /// <summary>
        /// Content types defined in RFC2616
        /// </summary>
        public const string CondContentType = "Content-Type";

        /// <summary>
        /// Content disposition behavior 
        /// </summary>
        public const string CondContentDisposition = "Content-Disposition";

        /// <summary>
        /// The content encoding
        /// </summary>
        public const string CondContentEncoding = "Content-Encoding";

        /// <summary>
        /// Expiration time
        /// </summary>
        public const string CondExpires = "Expires";

        /// <summary>
        /// object key
        /// </summary>
        public const string CondKey = "key";

        /// <summary>
        /// redirect upon success
        /// </summary>
        public const string CondSuccessActionRedirect = "success_action_redirect";

        /// <summary>
        /// The action status upon success
        /// </summary>
        public const string CondSuccessActionStatus = "success_action_status";

        /// <summary>
        /// The custom metadata prefix
        /// </summary>
        public const string CondXOssMetaPrefix = "x-oss-meta-";

        private IList<AbstractConditionItem> _conds = new List<AbstractConditionItem>(); 

        /// <summary>
        /// Adds a condition item with exact MatchMode
        /// </summary>
        /// <param name="name">Condition name</param>
        /// <param name="value">Condition value</param>
        public void AddConditionItem(string name, string value)
        {
            MatchRuleChecker.Check(MatchMode.Exact, name);
            _conds.Add(new EqualConditionItem(name, value));
        }

        /// <summary>
        /// Adds a condition item with specified MatchMode
        /// </summary>
        /// <param name="matchMode">Conditions match mode</param>
        /// <param name="name">Condition name</param>
        /// <param name="value">Condition value</param>
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
        /// Adds a condition with range match mode.
        /// </summary>
        /// <param name="name">Condition name</param>
        /// <param name="min">Range's low end</param>
        /// <param name="max">Range's high end</param>
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
