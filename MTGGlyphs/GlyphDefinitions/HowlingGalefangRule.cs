namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Howling Galefang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Howling Galefang</exampledoc>
[TokenizationOrder(-1)]
public class HowlingGalefangRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has haste as long as you own a card in exile that has an adventure"];
}
