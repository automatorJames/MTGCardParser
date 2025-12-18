namespace MTGPlexer.Analysis;

/// <summary>
/// A unified node representing any captured element (Root, Branch, Leaf, or Collection)
/// in the token hierarchy.
/// </summary>
public class TokenAnalysisNode
{
    // --- Identity ---
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } // e.g. "Mana Value", "Target Creature"
    public AnalysisNodeType NodeType { get; set; }

    // --- Type Information ---
    public Type ClrType { get; set; } // The actual C# type (e.g. typeof(int), typeof(Keyword))
    public string FriendlyTypeName { get; set; } // "int", "enum", "keyword"

    // --- Capture Data ---
    public string Text { get; set; } // The actual captured string
    public object Value { get; set; } // The hydrated object (Enum, Int, TokenUnit instance)
    public int StartIndex { get; set; }
    public int Length { get; set; }
    public int EndIndex => StartIndex + Length;

    /// <summary>
    /// The dot-notation path used to map back to specific RegexTemplate lines for highlighting.
    /// </summary>
    public string RegexEnclosurePath { get; set; }

    // --- Visualization ---
    public Palette Palette { get; set; }
    public bool IsCollapsed { get; set; } // Default UI state
    public bool IsTerminal { get; set; } // True if this is a leaf node (Enum, Bool, Distilled Value)

    // --- Tree Structure ---
    public List<TokenAnalysisNode> Children { get; set; } = new();

    public IEnumerable<TokenAnalysisNode> DescendantsAndSelf()
    {
        yield return this;
        foreach (var child in Children)
            foreach (var desc in child.DescendantsAndSelf())
                yield return desc;
    }

    public override string ToString() => $"{Name}: {Text} ({NodeType})";
}

public enum AnalysisNodeType
{
    Root,
    Structural,      // A TokenUnit that contains other properties
    Collection,      // A ManyOf container
    CollectionItem,  // An item within a ManyOf
    Terminal,        // An Enum, Bool, or String placeholder
    Derived,         // A Distilled value (virtual node)
    Unmatched        // Text not captured by the grammar
}