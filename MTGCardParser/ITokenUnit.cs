namespace MTGCardParser;

public interface ITokenUnit
{
    static string RegexTemplatePropName = "RegexTemplate";

    public RegexTemplate RegexTemplate => GetRegexTemplate();

    RegexTemplate GetRegexTemplate()
    {
        var prop = GetType().GetProperty(RegexTemplatePropName);

        if (prop is null)
            throw new Exception($"{GetType().Name} doesn't contain a property named {RegexTemplate})");

        return prop.GetValue(this) as RegexTemplate;
    }

    public virtual bool HandleInstantiation(string tokenMatchString)
    {
        // Default implementation handles nothing and leaves the work to TypeRegistry
        return false;
    }
}

