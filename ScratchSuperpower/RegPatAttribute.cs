namespace MTGCardParser;

public class RegPatAttribute : Attribute
{
    public string[] Patterns { get; set; }

    public RegPatAttribute(params string[] patterns)
    {
        Patterns = patterns;
    }
}

