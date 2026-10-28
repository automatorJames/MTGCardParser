namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What happens to counters in a counters trigger: "is put on"/"are put on" or "is removed from"/"are removed from", in "whenever a +1/+1 counter is put on this creature".</summary>
public enum CounterMovement
{
    [RegexPattern("is put on", "are put on")]
    PutOn,

    [RegexPattern("is removed from", "are removed from")]
    RemovedFrom
}
