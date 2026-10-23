namespace MTGGlyphs.GlyphDefinitions;

public enum RepeatPhrase
{
    [RegexPattern("repeats? this process")]
    RepeatThisProcess,

    [RegexPattern("do it again")]
    DoItAgain,

    [RegexPattern("do(?:es)? the same")]
    DoTheSame
}
