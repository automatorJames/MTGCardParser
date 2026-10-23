namespace MTGGlyphs.GlyphDefinitions;

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
