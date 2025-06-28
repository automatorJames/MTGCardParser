using System.Text;

namespace MTGCardParser;

public abstract class TokenCaptureBase<T> : ITokenCapture where T : TokenCaptureBase<T>
{
    public abstract RegexTemplate<T> RegexTemplate { get; }
    public string RenderedRegex => RegexTemplate.RenderedRegex;
    RegexTemplate ITokenCapture.RegexTemplate => this.RegexTemplate;

    //protected TokenCaptureBase()
    //{
    //}
    //
    //public TokenCaptureBase(string capturedTokenText)
    //{
    //    PopulateValues(capturedTokenText);
    //}
    //
    //// Default implementation automatically populates scalar (non-collection) enum, bool, and TokenSegment properties from token value
    //public virtual void PopulateValues(string tokenString)
    //{
    //    var type = GetType();
    //    var props = TypeRegistry.CaptureProps[type];
    //    var match = Regex.Match(tokenString, RenderedRegex);
    //
    //    foreach (var prop in props)
    //        if (!match.Groups.ContainsKey(prop.Name))
    //            throw new Exception($"No capture group defined on type {type.Name} that maches property name {prop.Name}");
    //
    //    var enumProps = props.Where(x => x.PropertyType.IsEnum || (Nullable.GetUnderlyingType(x.PropertyType) is not null));
    //    foreach (var enumProp in enumProps)
    //    {
    //        var underlyingEnumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
    //        var group = match.Groups[enumProp.Name];
    //
    //        if (group.Value is not null)
    //        {
    //            var matchingEnumVal = ParseTokenEnumValue(group.Value, underlyingEnumType, type);
    //            enumProp.SetValue(this, matchingEnumVal);
    //        }
    //    }
    //
    //    var boolProps = props.Where(x => x.PropertyType == typeof(bool));
    //    foreach (var boolProp in boolProps)
    //    {
    //        var group = match.Groups[boolProp.Name];
    //        var textIsPresent = !string.IsNullOrEmpty(group.Value);
    //        boolProp.SetValue(this, textIsPresent);
    //    }
    //
    //    var tokenSegmentProps = props.Where(x => x.PropertyType == typeof(TokenSegment));
    //    foreach (var tokenSegmentProp in tokenSegmentProps)
    //    {
    //        var group = match.Groups[tokenSegmentProp.Name];
    //        TokenSegment tokenSegment = new(group.Value);
    //        tokenSegmentProp.SetValue(this, tokenSegment);
    //    }
    //
    //    var tokenCaptureProps = props.Where(x => x.PropertyType.IsAssignableTo(typeof(ITokenCapture)));
    //    foreach (var tokenCaptureProp in tokenCaptureProps)
    //    {
    //        var instance = ITokenCapture.InstantiateFromString(tokenCaptureProp.PropertyType, tokenString);
    //        tokenCaptureProp.SetValue(this, instance);
    //    }
    //}
    //
    //object ParseTokenEnumValue(string input, Type enumType, Type containingType)
    //{
    //    if (!enumType.IsEnum)
    //        throw new ArgumentException($"Type {enumType.Name} is not an enum.");
    //
    //    foreach (var val in Enum.GetValues(enumType))
    //    {
    //        var field = enumType.GetField(val.ToString());
    //
    //        if
    //        (
    //            field.GetCustomAttributes(typeof(RegexPatternAttribute), false).FirstOrDefault() is RegexPatternAttribute attr
    //            && attr.Patterns is not null
    //            && attr.Patterns.Length > 0
    //        )
    //        {
    //            var pattern = attr.Patterns.GetAlternation();
    //            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
    //                return val;
    //        }
    //        else if (enumType.ShouldPluralize(containingType))
    //        {
    //            var pluralizedVal = val.ToString().ToLower().Pluralize();
    //            if (Regex.IsMatch(input, pluralizedVal, RegexOptions.IgnoreCase))
    //                return val;
    //        }
    //        else if (val.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
    //            return val;
    //    }
    //
    //    throw new ArgumentException($"No matching enum value of type {enumType.Name} for string '{input}'.");
    //}
}

