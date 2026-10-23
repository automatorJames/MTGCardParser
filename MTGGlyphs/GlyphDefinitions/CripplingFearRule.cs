namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crippling Fear, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crippling Fear</exampledoc>
[TokenizationOrder(-1)]
public class CripplingFearRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that aren't of the chosen type get -3/-3 until end of turn"];
}
