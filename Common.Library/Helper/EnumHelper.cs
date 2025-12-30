using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.Library.Helper
{
    public class EnumHelper
    {
        /// <summary>
        /// Get enum by value
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">value</param>
        /// <returns>Enum</returns>
        public static object GetEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Convert from Enums to IEnumerable.
        /// IEnumerable.Select(v => new EnumModel() { Value = (int)v, Field = v.ToString(), Description = GetDescription(v) }).ToList()
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Get Description from Enum
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>string</returns>
        public static string GetDescription(System.Enum value)
        {
            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? string.Empty : attribute.Description;
        }

        /// <summary>
        /// Get XmlEnum from Enum
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>string</returns>
        public static string GetXmlEnum(System.Enum value)
        {
            XmlEnumAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(XmlEnumAttribute), false)
                .SingleOrDefault() as XmlEnumAttribute;
            return attribute == null ? string.Empty : attribute.Name;
        }

        /// <summary>
        /// Get Value from Enum
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>string</returns>
        public static string GetEnumValue(System.Enum value)
        {
            EnumMemberAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .SingleOrDefault() as EnumMemberAttribute;
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
