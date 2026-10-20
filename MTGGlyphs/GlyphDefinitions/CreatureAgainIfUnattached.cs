namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bestow's (and reconfigure's) reminder text, last sentence.</summary>
/// <exampledoc>Baleful Eidolon</exampledoc>
/// <examplecapture>it becomes a creature again if it's not attached</examplecapture>
public class CreatureAgainIfUnattached : Glyph
{
    public override Nib[] Nibs => ["it becomes a creature again if it's not attached"];
}
