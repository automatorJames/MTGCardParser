namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a player controls a permanent now or did earlier: "you control {this}", "defending player controls a snow land", "you controlled that permanent".</summary>
public enum ControlTense
{
    [RegexPattern("controls?")]
    Controls,

    Controlled
}
