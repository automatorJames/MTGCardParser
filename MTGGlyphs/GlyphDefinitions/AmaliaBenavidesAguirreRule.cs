namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Amalia Benavides Aguirre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Amalia Benavides Aguirre</exampledoc>
[TokenizationOrder(-1)]
public class AmaliaBenavidesAguirreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all other creatures if its power is exactly 20"];
}
