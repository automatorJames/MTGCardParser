namespace MTGCardParser.Static;

public static class StringExtensions
{
    public static string GetRegex(this string regexTemplate, Type type, string delimiter = "§")
    {
        var delimiterCount = Regex.Count(regexTemplate, delimiter);
        if (delimiterCount % 2 != 0)
            throw new Exception($"Regex templates must have an even number of {delimiter} delimiters");

        var finalRegex = regexTemplate;

        var autoCaptureGroupNames = Regex.Matches(regexTemplate, @"§(?<name>[^§]+)§");

        foreach (Match autoCaptureGroupName in autoCaptureGroupNames.Cast<Match>())
        {
            var name = autoCaptureGroupName.Groups["name"].Value;
            var property = type.GetProperty(name);

            if (property is null)
                throw new Exception($"No property named {name} found on type {type.Name}");

            var propertyType = property.PropertyType;
            var enumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (!enumType.IsEnum)
                throw new Exception($"Property {name} on type {type.Name} is not an enum (or nullable enum)");

            var enumValues = Enum.GetValues(enumType)
                .Cast<object>()
                .Select(x => x.ToRegexPattern());

            var autoEnumCaptureGroupStr = GetAlternation(enumValues, name);
            finalRegex = Regex.Replace(finalRegex, autoCaptureGroupName.Value, autoEnumCaptureGroupStr);
        }

        return finalRegex;
    }

    public static string ToRegexPattern(this object obj, bool toLower = true)
    {
        if (obj is null)
            return null;

        string val = null;

        var type = obj.GetType();
        if (type.GetField(obj.ToString()).IsDefined(typeof(RegPatAttribute), false))
        {
            var descriptionAttribute = type.GetField(obj.ToString()).GetCustomAttributes(typeof(RegPatAttribute), false)[0] as RegPatAttribute;
            val = descriptionAttribute.Patterns.GetAlternation();
        }
        else
            val = obj.ToString();

        return val.ToLower();
    }

    public static string GetAlternation(this IEnumerable<string> items, string namedCaptureGroup = null)
    {
        var alternation = "(" + string.Join("|", items.OrderByDescending(s => s.Length)) + ")";

        if (namedCaptureGroup is not null)
            alternation = $@"\b(?<{namedCaptureGroup}>{alternation})\b";

        return alternation;
    }

    public static object ParseTokenEnumIgnoreCase(this string input, Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException($"Type {enumType.Name} is not an enum.");

        foreach (var val in Enum.GetValues(enumType))
        {
            var field = enumType.GetField(val.ToString());

            if 
            (
                field is not null 
                && field.GetCustomAttributes(typeof(RegPatAttribute), false).FirstOrDefault() is RegPatAttribute attr 
                && attr.Patterns is not null && attr.Patterns.Length > 0
            )
            {
                var pattern = attr.Patterns.GetAlternation();
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    return val;
            }
            else if (val.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return val;
            }
        }

        throw new ArgumentException($"No matching enum value of type {enumType.Name} for string '{input}'.");
    }
}

