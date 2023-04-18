using System.ComponentModel;
using System.Reflection;

namespace Application.Helpers
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute.Description;
        }

        public static int ParseStringToEnum(this string str, Type value)
        {
            var isValid = Enum.TryParse(value, str, true, out var result);

            if (isValid)
            {
                return (int)result;
            }

            throw new ArgumentException($"{str} must be an enumerated type");
        }
    }
}
