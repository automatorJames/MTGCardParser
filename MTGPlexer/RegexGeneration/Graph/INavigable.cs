namespace MTGPlexer.RegexGeneration.Graph;

/// <summary>
/// Represents a behavioral contract for traversing a node hierarchy. 
/// Acts as a logical bridge between a parent and a child node, abstracting the underlying 
/// mechanism of access—whether it is a physical C# property, a virtual generic type argument, 
/// or a dynamic collection element. Allows the node graph to treat "wrapped" generic types and 
/// collection items as first-class navigable branches, even when they do not correspond to a direct CLR Property
/// </summary>

public interface INavigable
{
    /// <summary>
    /// Gets the logical name of this navigation point (e.g. the property name or a generated identifier).
    /// </summary>
    public string Name { get; }

    public Type Type { get; }

    /// <summary>
    /// Gets the configuration and mapping options associated with this specific branch.
    /// </summary>
    public Proptions Proptions { get; }
}