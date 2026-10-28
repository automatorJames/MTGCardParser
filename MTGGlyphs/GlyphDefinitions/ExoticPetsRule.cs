namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Exotic Pets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exotic Pets</exampledoc>
[TokenizationOrder(-1)]
public class ExoticPetsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then for each kind of counter among creatures you control, put a counter of that kind on either of those tokens"];
}
