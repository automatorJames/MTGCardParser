namespace MTGCardParser.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class RegexPatternAttribute : Attribute
{
    public string[] Patterns { get; set; }

    public RegexPatternAttribute(params string[] patterns)
    {
        Patterns = patterns;
    }
}