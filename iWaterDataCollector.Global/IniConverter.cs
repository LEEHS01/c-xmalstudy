using iWaterDataCollector.INI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/********************************************
 * Class ↔ ini data간 Converter
 ********************************************/
namespace iWaterDataCollector.Global
{
    public class IniConverter
    {
        /// <summary>
        /// ini->Class 변환
        /// </summary>
        /// <typeparam name="T">형식</typeparam>
        /// <param name="cls">class</param>
        /// <param name="section">section 이름</param>
        public void UseGenericMethod<T>(T cls, Dictionary<string, IniValue> lKey)
        {
            try
            {
                //T가 어떤 값인지 모르지만 그안에 들어있는 Property만 가지고 오겠다
                IEnumerable<PropertyInfo> pInfos = typeof(T).GetProperties().Where(pInfo => lKey.ContainsKey(pInfo.Name));
                foreach (PropertyInfo pInfo in pInfos)
                {
                    if (pInfo.PropertyType == typeof(int))
                    {
                        pInfo.SetValue(cls, lKey[pInfo.Name].ToInt());
                    }
                    else if (pInfo.PropertyType == typeof(double))
                    {
                        pInfo.SetValue(cls, lKey[pInfo.Name].ToDouble());
                    }
                    else if (pInfo.PropertyType == typeof(bool))
                    {
                        pInfo.SetValue(cls, lKey[pInfo.Name].ToBool());
                    }
                    else if (pInfo.PropertyType == typeof(string))
                    {
                        pInfo.SetValue(cls, lKey[pInfo.Name].ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Class->ini 변환
        /// </summary>
        /// <typeparam name="T">형식</typeparam>
        /// <param name="cls">class</param>
        /// <returns>ini형식 <see cref="Dictionary{TKey, TValue}"/></returns>
        public Dictionary<string, IniValue> UseGenericMethod<T>(T cls)
        {
            Dictionary<string, IniValue> dic = new Dictionary<string, IniValue>();
            try
            {
                foreach (PropertyInfo pInfo in typeof(T).GetProperties())
                {
                    string obj = pInfo.GetValue(cls).ToString();
                    dic.Add(pInfo.Name, obj);
                }
            }
            catch (Exception ex)
            {

            }
            return dic;
        }

        public string GetGenericMethodValue<T>(T cls, string name)
        {
            var rtnVal = string.Empty;
            try
            {
                var pInfo = typeof(T).GetProperties().FirstOrDefault(t => t.Name.Equals(name));
                rtnVal = pInfo.GetValue(cls).ToString();
            }
            catch (Exception ex)
            {

            }
            return rtnVal;
        }
    }
}
