using MTGCardParser.Static;

namespace MTGCardParser;

public interface ITokenCapture
{
    public string RegexTemplate { get; }
    public string RenderedRegex => RenderRegex(RegexTemplate, GetType());

    // Default implementation automatically populates scalar (non-collection) enum, bool, and TokenSegment properties from token value
    public void PopulateScalarValues(string tokenString)
    {
        var type = GetType();
        var regexTemplate = GetInstanceRegexTemplate();
        string regex = RenderRegex(regexTemplate, type);
        var match = Regex.Match(tokenString, regex);

        var enumProps = type.GetProperties().Where(x => x.PropertyType.IsEnum || (Nullable.GetUnderlyingType(x.PropertyType) is not null));
        foreach (var enumProp in enumProps)
        {
            var underlyingEnumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
            var group = match.Groups[enumProp.Name];

            if (!group.Success)
                throw new Exception($"No capture group defined that maches property name {enumProp.Name}");

            if (group.Value is not null)
            {
                var matchingEnumVal = group.Value.ParseTokenEnumIgnoreCase(underlyingEnumType);
                enumProp.SetValue(this, matchingEnumVal);
            }
        }

        var boolProps = type.GetProperties().Where(x => x.PropertyType == typeof(bool));
        foreach (var boolProp in boolProps)
        {
            var group = match.Groups[boolProp.Name];

            if (!group.Success)
                throw new Exception($"No capture group defined that maches property name {boolProp.Name}");

            var textIsPresent = !string.IsNullOrEmpty(group.Value);

            boolProp.SetValue(this, textIsPresent);
        }

        var tokenSegmentProps = type.GetProperties().Where(x => x.PropertyType == typeof(TokenSegment));
        foreach (var tokenSegmentProp in tokenSegmentProps)
        {
            var group = match.Groups[tokenSegmentProp.Name];

            if (!group.Success)
                throw new Exception($"No capture group defined that maches property name {tokenSegmentProp.Name}");

            TokenSegment tokenSegment = new(group.Value);
            tokenSegmentProp.SetValue(this, tokenSegment);
        }
    }

    public string GetInstanceRegexTemplate()
    {
        var regexTemplateProperty = this.GetType().GetProperty(nameof(RegexTemplate), BindingFlags.Static | BindingFlags.Public);
        var regexTemplate = regexTemplateProperty?.GetValue(null) as string;
        return regexTemplate;
    }

    public static string RenderRegex(string regexTemplate, Type type, string delimiter = "§")
    {
        var delimiterCount = Regex.Count(regexTemplate, delimiter);
        if (delimiterCount % 2 != 0)
            throw new Exception($"Regex templates must have an even number of {delimiter} delimiters");

        var finalRegex = regexTemplate;
        var autoCaptureGroupNames = Regex.Matches(regexTemplate, @"§(?<name>[^§]+)§");

        var regOpt = type.GetCustomAttribute<RegOptAttribute>() ?? new();
        bool shouldWrapAlternationsInWordBoundaries = !regOpt.DoNotWrapInWordBoundaries;

        foreach (Match autoCaptureGroupName in autoCaptureGroupNames.Cast<Match>())
        {
            var name = autoCaptureGroupName.Groups["name"].Value;
            var property = type.GetProperty(name);

            if (property is null)
                throw new Exception($"No property named {name} found on type {type.Name}");

            var propertyType = property.PropertyType;

            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (underlyingType.IsEnum)
            {
                var enumValues = Enum.GetValues(underlyingType)
                    .Cast<object>()
                    .Select(x => x.ToRegexPattern());

                var autoEnumCaptureGroupStr = enumValues.GetAlternation(name, shouldWrapAlternationsInWordBoundaries);
                finalRegex = Regex.Replace(finalRegex, autoCaptureGroupName.Value, autoEnumCaptureGroupStr);
            }
            else if (underlyingType == typeof(bool))
            {
                var regPattern = property.ToRegexPattern();
                finalRegex = Regex.Replace(finalRegex, autoCaptureGroupName.Value, regPattern);
            }
            else
                throw new Exception($"Property {name} on type {type.Name} is not an enum or nullable enum");
        }


        finalRegex = finalRegex.WrapInWordBoundaries();

        return finalRegex;
    }
}

