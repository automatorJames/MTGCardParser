namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dreadlight Monstrosity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dreadlight Monstrosity</exampledoc>
[TokenizationOrder(-1)]
public class DreadlightMonstrosityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if you own a card in exile"];
}
