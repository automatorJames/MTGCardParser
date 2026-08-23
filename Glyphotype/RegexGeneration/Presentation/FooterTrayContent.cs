using System.ComponentModel;

namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// What TypeRegexPage shows in each type-card's footer tray, below its formatted regex - the three
/// states the page's footer-tray toggle cycles through. <see cref="Description"/> gives the toggle's
/// tooltip text; <see cref="ColorAttribute"/> gives its icon color for that state.
/// </summary>
public enum FooterTrayContent
{
    /// <summary>No footer tray content - just the formatted regex, the default.</summary>
    [Description("Hidden")]
    [Color("#808080")]
    Hidden,

    /// <summary>A table of every matched occurrence of this type across the corpus: which document it came from, and its captured text colored per named group.</summary>
    [Description("Matches")]
    [Color("#7b8dcf")]
    Matches,

    /// <summary>This type's named-group structure as a collapsible tree - the same view previously toggled by the old boolean "Hide type tree" setting.</summary>
    [Description("Type tree")]
    [Color("#7d9e5b")]
    TypeTree,

    /// <summary>This type's declared C# source, reflected fresh and colored per named group - navigable into its own properties' types via breadcrumbs.</summary>
    [Description("C# class")]
    [Color("#c77e59")]
    CSharpClass,
}
