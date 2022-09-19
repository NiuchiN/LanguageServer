using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class StringValAttribute : Attribute
    {
        public string StringVal { get; protected set; }
        public StringValAttribute(string strVal) { this.StringVal = strVal; }
    }

    public class ClassificationAttribute : Attribute
    {
        public string Classification { get; protected set; }
        public ClassificationAttribute(string str) { this.Classification = str; }
    }

    public static class CommonAttr
    {
        public static string GetStr(this Enum eVal)
        {
            Type type = eVal.GetType();
            System.Reflection.FieldInfo fieldInfo = type.GetField(eVal.ToString());

            if (fieldInfo == null) {
                return null;
            }

            StringValAttribute[] attr = fieldInfo.GetCustomAttributes(typeof(StringValAttribute), false) as StringValAttribute[];
            return attr.Length > 0 ? attr[0].StringVal : null;
        }

        public static string GetClassification(this Enum eVal)
        {
            Type type = eVal.GetType();
            System.Reflection.FieldInfo fieldInfo = type.GetField(eVal.ToString());

            if (fieldInfo == null) {
                return null;
            }

            ClassificationAttribute[] attr = fieldInfo.GetCustomAttributes(typeof(ClassificationAttribute), false) as ClassificationAttribute[];
            return attr.Length > 0 ? attr[0].Classification : null;
        }
    }

}
