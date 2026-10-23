namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jumbo Cactuar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jumbo Cactuar</exampledoc>
[TokenizationOrder(-1)]
public class JumboCactuarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["10,000 needles — whenever this creature attacks, it gets +9999/+0 until end of turn"];
}
