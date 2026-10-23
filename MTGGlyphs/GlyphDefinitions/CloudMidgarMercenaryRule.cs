namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cloud, Midgar Mercenary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cloud, Midgar Mercenary</exampledoc>
/// <examplecapture>as long as cloud is equipped, if a triggered ability of cloud or an equipment attached to it triggers, that ability triggers an additional time</examplecapture>
[TokenizationOrder(-1)]
public class CloudMidgarMercenaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as cloud is equipped, if a triggered ability of cloud or an equipment attached to it triggers, that ability triggers an additional time"];
}
