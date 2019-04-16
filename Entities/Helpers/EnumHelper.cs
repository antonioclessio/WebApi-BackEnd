using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Entities.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(int itemEnum)
        {
            var itemEnumDescription = System.Enum.GetName(typeof(T), itemEnum);
            if (string.IsNullOrEmpty(itemEnumDescription)) return string.Empty;

            return GetAttributeOfType<DisplayAttribute>(System.Enum.Parse(typeof(T), itemEnumDescription)).Name;
        }

        private static T GetAttributeOfType<T>(this object enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            if (!attributes.Any()) return null;
            return (T)attributes[0];
        }
    }
}
