namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Primalcrux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primalcrux</exampledoc>
[TokenizationOrder(-1)]
public class PrimalcruxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["chroma — this creature's power and toughness are each equal to the number of green mana symbols in the mana costs of permanents you control"];
}
