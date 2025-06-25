namespace MTGCardParser.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class RegPatAttribute : Attribute
{
    public string[] Patterns { get; set; }

    public RegPatAttribute(params string[] patterns)
    {
        Patterns = patterns;
    }
}