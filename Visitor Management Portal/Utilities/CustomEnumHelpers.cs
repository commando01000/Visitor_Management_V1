using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XDesk.Helpers
{
    public class CustomEnumHelpers
    {
        public static string GetEnumNameByValue<T>(int enumValue) where T : Enum
        {
            var enumType = typeof(T);
            if (Enum.IsDefined(enumType, enumValue))
            {
                var name = Enum.GetName(enumType, enumValue);
                return name;
            }
            return null;
        }

        //use in enum created by you
        public static List<EnumItem> EnumToList<T>() where T : Enum
        {
            var enumType = typeof(T);
            var enumValues = Enum.GetValues(enumType);
            var enumList = new List<EnumItem>();

            foreach (var value in enumValues)
            {
                var intValue = (int)value;
                var name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    enumList.Add(new EnumItem
                    {
                        Name = name,
                        Value = intValue
                    });
                }
            }

            return enumList;
        }

        //use in optionset from CRM
        public static IEnumerable<EnumItem> EnumToListCRM<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<int>().Select(e => new EnumItem()
            {
                Name = GetEnumNameByValueCRM<T>(Enum.GetName(typeof(T), e)),
                Value = e
            })).ToList();
        }

        //Helper method to get Enum description from OptionSetMetadataAttribute
        public static string GetEnumDescription<T>(int value) where T : Enum
        {
            var enumType = typeof(T);
            var field = enumType.GetField(Enum.GetName(enumType, value));
            var attribute = field.GetCustomAttribute<OptionSetMetadataAttribute>();
            return attribute != null ? attribute.Name : Enum.GetName(enumType, value);
        }

        //Adjusted EnumToList to use the Arabic names from OptionSetMetadataAttribute
        public static List<EnumItem> EnumToList2<T>() where T : Enum
        {
            var enumType = typeof(T);
            var enumValues = Enum.GetValues(enumType);
            var enumList = new List<EnumItem>();

            foreach (var value in enumValues)
            {
                var intValue = (int)value;
                var name = GetEnumDescription<T>(intValue);
                enumList.Add(new EnumItem
                {
                    Name = name,
                    Value = intValue
                });
            }

            return enumList;
        }

        // Use this method to get the enum item by value from CRM OptionSet
        public static EnumItem GetEnumItemByValueCRM<T>(int enumValue) where T : Enum
        {
            var name = GetEnumDescription<T>(enumValue);
            return new EnumItem
            {
                Name = name,
                Value = enumValue
            };
        }

        public static string GetEnumNameByValueCRM<T>(string EnumValue)
        {
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(EnumValue);
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(OptionSetMetadataAttribute), false);
            var name = ((OptionSetMetadataAttribute)valueAttributes[0]).Name;
            return name;
        }

    }

    public class EnumItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}