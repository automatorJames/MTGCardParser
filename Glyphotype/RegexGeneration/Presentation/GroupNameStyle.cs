using System.ComponentModel;

namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// How a formatted regex names its groups - display-only, since the engine always matches with fully qualified
/// names - the three states TypeRegexPage's group-name toggle cycles through. <see cref="Description"/> gives the
/// toggle's tooltip text; <see cref="ColorAttribute"/> gives its icon color for that state.
/// </summary>
public enum GroupNameStyle
{
    /// <summary>
    /// The shortest trailing part of each group's fully qualified name that no other group in its type's graph
    /// ends with (see <see cref="RegexGraph.SimpleUniqueNames"/>) - short in a small graph, but long wherever a
    /// name recurs throughout a large one.
    /// </summary>
    [Description("Shortest unique")]
    [Color("#C0C0C0")]
    ShortestUnique,

    /// <summary>
    /// Each group's own name, prefixed with its position among the groups shown (<c>_17_IsNegated</c>): unique
    /// within the regex as shown however deep the group sits, the groups enclosing it saying which one it is.
    /// </summary>
    [Description("Numbered")]
    [Color("#1E90FF")]
    Numbered,

    /// <summary>
    /// Each group's own name alone, the groups enclosing it saying which one it is. Names recur, so a regex copied
    /// with them merges every same-named group's captures into one.
    /// </summary>
    [Description("Own name only")]
    [Color("#FFA500")]
    OwnName,
}
