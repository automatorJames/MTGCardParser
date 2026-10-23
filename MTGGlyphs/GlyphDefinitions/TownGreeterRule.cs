namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Town Greeter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Town Greeter</exampledoc>
[TokenizationOrder(-1)]
public class TownGreeterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a town card into your hand this way, you gain 2 life"];
}
