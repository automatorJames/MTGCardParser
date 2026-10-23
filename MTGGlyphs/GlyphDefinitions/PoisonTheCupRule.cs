namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Poison the Cup, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Poison the Cup</exampledoc>
[TokenizationOrder(-1)]
public class PoisonTheCupRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was foretold, scry 2"];
}
