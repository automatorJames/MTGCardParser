namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Armored Kincaller, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Armored Kincaller</exampledoc>
[TokenizationOrder(-1)]
public class ArmoredKincallerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do or if you control another dinosaur, you gain 3 life"];
}
