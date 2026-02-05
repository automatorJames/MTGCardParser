namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DistilledValueAttribute : Attribute
{
    public string Pattern { get; set; }

    public DistilledValueAttribute(string pattern)
    {
        Pattern = pattern;
    }
}

