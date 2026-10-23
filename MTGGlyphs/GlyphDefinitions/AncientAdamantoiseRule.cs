namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ancient Adamantoise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ancient Adamantoise</exampledoc>
[TokenizationOrder(-1)]
public class AncientAdamantoiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage isn't removed from this creature during cleanup steps"];
}
