namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of a counter placement: "put" ("put a +1/+1 counter on target creature") or "remove" ("remove a doom counter from {this}"), or "puts"/"removes" of a named subject.</summary>
public enum CounterAction
{
    [RegexPattern("puts?")]
    Put,

    [RegexPattern("removes?")]
    Remove
}
