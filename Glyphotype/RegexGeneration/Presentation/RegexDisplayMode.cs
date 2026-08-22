using System.ComponentModel;

namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// How much of an enum group's member list <see cref="EnumSectionBuilder"/> shows, or whether the whole
/// regex renders unformatted at all - the four states TypeRegexPage's format toggle cycles through.
/// <see cref="Description"/> gives the toggle's tooltip text; <see cref="ColorAttribute"/> gives its icon
/// color for that state.
/// </summary>
public enum RegexDisplayMode
{
    /// <summary>Every enum member shown, alphabetically, including zero-occurrence ones with their real (zero) count - nothing omitted.</summary>
    [Description("Full")]
    [Color("#C0C0C0")]
    Full,

    /// <summary>Only members that actually occurred, ranked by occurrence count - today's default view.</summary>
    [Description("Matched enums only")]
    [Color("#008000")]
    MatchedOnly,

    /// <summary>Up to three occurring members, ranked by occurrence count then alphabetically, one row per member (no synonym breakdown).</summary>
    [Description("Sample enums")]
    [Color("#FFA500")]
    Sample,

    /// <summary>The compiled regex text alone, colored per named group but otherwise unformatted.</summary>
    [Description("Minified")]
    [Color("#FF00FF")]
    Minified,
}
