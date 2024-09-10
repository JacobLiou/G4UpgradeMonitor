using System.ComponentModel;
using System.Reflection;

namespace Sofar.Common.Helper
{
    public static class EnumHelper
    {
        public static string? GetDescriptionAttr<T>(this T source)
        {
            FieldInfo? fi = source.GetType().GetField(source.ToString()!);
            if (fi is null) return null;
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
