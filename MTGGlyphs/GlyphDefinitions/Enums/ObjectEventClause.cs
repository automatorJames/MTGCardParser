namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The clause that names an object again by what happened to it, after "the creature" or "the card": "that died", "you exiled", "you chose", "you revealed", "you sacrificed", "you discarded" ("the power of the creature that died").</summary>
public enum ObjectEventClause
{
    [RegexPattern("that died")]
    Died,

    [RegexPattern("you exiled")]
    YouExiled,

    [RegexPattern("you chose")]
    YouChose,

    [RegexPattern("you revealed")]
    YouRevealed,

    [RegexPattern("you sacrificed")]
    YouSacrificed,

    [RegexPattern("you discarded")]
    YouDiscarded
}
