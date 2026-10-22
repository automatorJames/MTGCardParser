namespace MTGGlyphs.GlyphDefinitions;

public enum AttachmentChange
{
    [RegexPattern("becomes attached to")]
    Attached,

    [RegexPattern("becomes unattached from")]
    Unattached
}
