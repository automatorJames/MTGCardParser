namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who divided damage goes among, after "any number of" or "one or two": "targets", "target creatures" - "divided as you choose among any number of targets".</summary>
public enum DividedRecipients
{
    [RegexPattern("targets")]
    Targets,

    [RegexPattern("target creatures")]
    TargetCreatures
}
