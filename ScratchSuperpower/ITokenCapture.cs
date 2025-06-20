namespace MTGCardParser;

public interface ITokenCapture
{
    public static string RegexTemplate { get; }

    // Default implementation automatically populates scalar (non-collection) enum properties TokenSegment properties from token value
    public void PopulateScalarValues(string tokenString)
    {
        var type = GetType();
        var regexTemplate = GetInstanceRegexTemplate();
        string regex = regexTemplate.GetRegex(type);
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

    //public void PopulateScalarValuesFromToken(Token<Type> token)
    //{
    //    var regexTemplate = GetInstanceRegexTemplate();
    //    string regex = regexTemplate.GetRegex(token.Kind);
    //    var match = Regex.Match(token.Span.Source, regex);
    //
    //    var enumProps = token.Kind.GetProperties().Where(x => x.PropertyType.IsEnum || (Nullable.GetUnderlyingType(x.PropertyType) is not null));
    //    foreach (var enumProp in enumProps)
    //    {
    //        var underlyingEnumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
    //        var group = match.Groups[enumProp.Name];
    //
    //        if (!group.Success)
    //            throw new Exception($"No capture group defined that maches property name {enumProp.Name}");
    //
    //        if (group.Value is not null)
    //        {
    //            var matchingEnumVal = group.Value.ParseTokenEnumIgnoreCase(underlyingEnumType);
    //            enumProp.SetValue(this, matchingEnumVal);
    //        }
    //    }
    //
    //    var tokenSegmentProps = token.Kind.GetProperties().Where(x => x.PropertyType == typeof(TokenSegment));
    //    foreach (var tokenSegmentProp in tokenSegmentProps)
    //    {
    //        var group = match.Groups[tokenSegmentProp.Name];
    //
    //        if (!group.Success)
    //            throw new Exception($"No capture group defined that maches property name {tokenSegmentProp.Name}");
    //
    //        TokenSegment tokenSegment = new(group.Value);
    //        tokenSegmentProp.SetValue(this, tokenSegment);
    //    }
    //}


    public string GetInstanceRegexTemplate()
    {
        var regexTemplateProperty = this.GetType().GetProperty(nameof(RegexTemplate), BindingFlags.Static | BindingFlags.Public);
        var regexTemplate = regexTemplateProperty?.GetValue(null) as string;
        return regexTemplate;
    }
}

