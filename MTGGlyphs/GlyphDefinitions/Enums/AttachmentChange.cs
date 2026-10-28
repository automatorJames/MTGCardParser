namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An aura or equipment attaching or coming off, as a trigger event: "an aura becomes attached to this creature", "this equipment becomes unattached from a permanent".</summary>
public enum AttachmentChange
{
    [RegexPattern("becomes attached to")]
    Attached,

    [RegexPattern("becomes unattached from")]
    Unattached
}
