using MTGCardParser.Static;

namespace MTGCardParser;

public interface ITokenCapture
{
    public RegexTemplate RegexTemplate { get; }
    public string RenderedRegex { get; }
    public virtual bool HandleInstantiation(string tokenMatchString)
    {
        // Default implementation handles nothing and leaves the work to TypeRegistry
        return false;
    }
}

