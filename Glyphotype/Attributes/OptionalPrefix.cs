namespace Glyphotype.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class OptionalPrefix(string prefixNib) : Attribute
{
    public string PrefixNib { get; set; } = prefixNib;
}