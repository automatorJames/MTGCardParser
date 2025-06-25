using MTGCardParser.Static;

namespace MTGCardParser;

public interface ITokenCapture
{
    public string RegexTemplate { get; }
    public string RenderedRegex => GetRenderedRegex();


    // Default implementation automatically populates scalar (non-collection) enum, bool, and TokenSegment properties from token value
    public void PopulateScalarValues(string tokenString)
    {
        var type = GetType();
        var match = Regex.Match(tokenString, RenderedRegex);

        var enumProps = type.GetProperties().Where(x => x.PropertyType.IsEnum || (Nullable.GetUnderlyingType(x.PropertyType) is not null));
        foreach (var enumProp in enumProps)
        {
            if (!match.Groups.ContainsKey(enumProp.Name))
                throw new Exception($"No capture group defined on type {type.Name} that maches property name {enumProp.Name}");

            var underlyingEnumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
            var group = match.Groups[enumProp.Name];

            if (!group.Success)
                throw new Exception($"No capture group defined that maches property name {enumProp.Name}");

            if (group.Value is not null)
            {
                var matchingEnumVal = group.Value.ParseTokenEnumValue(underlyingEnumType, type);
                enumProp.SetValue(this, matchingEnumVal);
            }
        }

        var boolProps = type.GetProperties().Where(x => x.PropertyType == typeof(bool));
        foreach (var boolProp in boolProps)
        {
            if (!match.Groups.ContainsKey(boolProp.Name))
                throw new Exception($"No capture group defined on type {type.Name} that maches property name {boolProp.Name}");

            var group = match.Groups[boolProp.Name];
            var textIsPresent = !string.IsNullOrEmpty(group.Value);
            boolProp.SetValue(this, textIsPresent);
        }

        var tokenSegmentProps = type.GetProperties().Where(x => x.PropertyType == typeof(TokenSegment));
        foreach (var tokenSegmentProp in tokenSegmentProps)
        {
            if (!match.Groups.ContainsKey(tokenSegmentProp.Name))
                throw new Exception($"No capture group defined on type {type.Name} that maches property name {tokenSegmentProp.Name}");

            var group = match.Groups[tokenSegmentProp.Name];
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

    public string GetRenderedRegex(string delimiter = "§")
    {
        var type = this.GetType();
        var delimiterCount = Regex.Count(RegexTemplate, delimiter);

        if (delimiterCount % 2 != 0)
            throw new Exception($"Regex templates must have an even number of {delimiter} delimiters, but found {delimiterCount}");

        var finalRegex = RegexTemplate;
        var autoCaptureGroupNames = Regex.Matches(RegexTemplate, @"§(?<name>[^§]+)§");

        var classRegOptions = type.GetCustomAttribute<RegOptAttribute>() ?? new();
        bool shouldWrapAlternationsInWordBoundaries = !classRegOptions.DoNotWrapInWordBoundaries;

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
                var enumRegOptions = underlyingType.GetCustomAttribute<RegOptAttribute>() ?? new();
                var addOptionalPlural = classRegOptions.OptionalPlural || enumRegOptions.OptionalPlural;

                var enumValues = Enum.GetValues(underlyingType)
                    .Cast<object>()
                    .Select(x => x.ToRegexPattern(addOptionalPlural: addOptionalPlural));

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

    public static ITokenCapture InstantiateFromToken(Token<Type> token)
    {
        var type = token.Kind;

        if (!type.IsAssignableTo(typeof(ITokenCapture)))
            throw new Exception($"{type.Name} does not implement ITokenCapture");

        var instance = (ITokenCapture)Activator.CreateInstance(type);
        instance.PopulateScalarValues(token.ToStringValue());

        return instance;
    }
}

