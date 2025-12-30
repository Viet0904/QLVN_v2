using System.ComponentModel;
using System.Xml.Serialization;

namespace Common.Library.Enum
{
    public enum GenderEnum
    {
        [XmlEnum("Nu")]
        [Description("Nữ")]
        Nu = 0,

        [XmlEnum("Nam")]
        [Description("Nam")]
        Name = 1,

        [XmlEnum("Khac")]
        [Description("Khác")]
        Another = 2
    }
}
