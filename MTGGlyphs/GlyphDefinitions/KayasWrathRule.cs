namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya's Wrath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya's Wrath</exampledoc>
[TokenizationOrder(-1)]
public class KayasWrathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the number of creatures you controlled that were destroyed this way"];
}
