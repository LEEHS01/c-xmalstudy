using System;
using System.Globalization;

namespace iWaterDataCollector.INI
{
    /// <summary>
    /// ini Value Class
    /// </summary>
    public struct IniValue
    {
        #region value 변환 static 함수
        /// <summary>
        /// ini value 변환 함수
        /// </summary>
        /// <param name="text">ini value</param>
        /// <param name="value">변환된 value</param>
        /// <returns>성공여부</returns>
        private static bool TryParseInt(string text, out int value)
        {
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int res))
            {
                value = res;
                return true;
            }
            value = 0;
            return false;
        }
        /// <summary>
        /// ini value 변환 함수
        /// </summary>
        /// <param name="text">ini value</param>
        /// <param name="value">변환된 value</param>
        /// <returns>성공여부</returns>
        private static bool TryParseDouble(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double res))
            {
                value = res;
                return true;
            }
            value = double.NaN;
            return false;
        }

        #endregion
        /// <summary>
        /// ini value
        /// </summary>
        public string Value;
        #region 생성자
        public IniValue(object value)
        {
            Value = value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value?.ToString();
        }
        public IniValue(string value)
        {
            Value = value;
        }
        #endregion

        public bool ToBool(bool valueIfInvalid = false)
        {
            return TryConvertBool(out bool res) ? res : valueIfInvalid;
        }

        public bool TryConvertBool(out bool result)
        {
            if (Value != null)
            {
                string boolStr = Value.Trim().ToLowerInvariant();
                switch (boolStr)
                {
                    case "true":
                        result = true;
                        return true;
                    case "false":
                        result = false;
                        return true;
                    default:
                        break;
                }
                result = default;
                return false;
            }
            result = default;
            return false;
        }

        public int ToInt(int valueIfInvalid = 0)
        {
            return TryConvertInt(out int res) ? res : valueIfInvalid;
        }

        public bool TryConvertInt(out int result)
        {
            if (Value != null)
            {
                return TryParseInt(Value.Trim(), out result);
            }
            result = default;
            return false;
        }

        public double ToDouble(double valueIfInvalid = 0)
        {
            return TryConvertDouble(out double res) ? res : valueIfInvalid;
        }

        public bool TryConvertDouble(out double result)
        {
            if (Value != null)
            {
                return TryParseDouble(Value.Trim(), out result);
            }
            result = default;
            return false;
        }

        public string GetString()
        {
            return GetString(true, false);
        }

        public string GetString(bool preserveWhitespace)
        {
            return GetString(true, preserveWhitespace);
        }

        public string GetString(bool allowOuterQuotes, bool preserveWhitespace)
        {
            if (Value == null)
            {
                return "";
            }
            string trimmed = Value.Trim();
            if (allowOuterQuotes && trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[trimmed.Length - 1] == '"')
            {
                string inner = trimmed.Substring(1, trimmed.Length - 2);
                return preserveWhitespace ? inner : inner.Trim();
            }
            else
            {
                return preserveWhitespace ? Value : Value.Trim();
            }
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator IniValue(byte o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(short o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(int o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(sbyte o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(ushort o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(uint o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(float o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(double o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(bool o)
        {
            return new IniValue(o);
        }

        public static implicit operator IniValue(string o)
        {
            return new IniValue(o);
        }

        public static IniValue Default { get; } = new IniValue();
    }
}
