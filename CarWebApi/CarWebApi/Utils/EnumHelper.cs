using CarWebApi.EvDataModels.DTOs;
using System.ComponentModel;

namespace CarWebApi.Utils;

public static class EnumHelper
{
    public static List<EnumIdentifier> GetEnumIdentifiers<TEnum>() where TEnum : Enum
    {
        var enumValues = new List<EnumIdentifier>();

        foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
        {
            var id = (int)enumValue;
            var description = GetEnumDescription((Enum)enumValue);

            enumValues.Add(new EnumIdentifier
            {
                Id = id,
                Description = description
            });
        }

        return enumValues;
    }

    public static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString())!;
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))!;
        return attribute?.Description ?? value.ToString(); // Default to ToString if no description is provided
    }

    public static EnumIdentifier GetEnumIdentifierById<TEnum>(int id) where TEnum : Enum
    {
        var enumValues = GetEnumIdentifiers<TEnum>();
        return enumValues.FirstOrDefault(e => e.Id == id)!;
    }

    public static TEnum GetEnumValueFromDescription<TEnum>(string description) where TEnum : Enum
    {
        foreach (var field in typeof(TEnum).GetFields())
        {
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .FirstOrDefault() as DescriptionAttribute;

            if (attribute != null && attribute.Description == description)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }

        // Return default value if no match is found
#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
