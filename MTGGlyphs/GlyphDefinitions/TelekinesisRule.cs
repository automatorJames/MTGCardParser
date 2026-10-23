namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Telekinesis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Telekinesis</exampledoc>
[TokenizationOrder(-1)]
public class TelekinesisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it doesn't untap during its controller's next two untap steps"];
}
