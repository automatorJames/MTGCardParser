namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spore Cloud, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spore Cloud</exampledoc>
[TokenizationOrder(-1)]
public class SporeCloudRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each attacking creature and each blocking creature doesn't untap during its controller's next untap step"];
}
