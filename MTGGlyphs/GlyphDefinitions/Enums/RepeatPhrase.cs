namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The wording that sends the reader back over the preceding instructions: "repeat this process", "do it again" ("then do it again"), or "do the same" ("do the same for flying, first strike, …").</summary>
public enum RepeatPhrase
{
    [RegexPattern("repeats? this process")]
    RepeatThisProcess,

    [RegexPattern("do it again")]
    DoItAgain,

    [RegexPattern("do(?:es)? the same")]
    DoTheSame
}
