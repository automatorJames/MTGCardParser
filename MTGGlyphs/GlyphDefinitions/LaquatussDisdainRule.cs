namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Laquatus's Disdain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Laquatus's Disdain</exampledoc>
[TokenizationOrder(-1)]
public class LaquatussDisdainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell cast from a graveyard"];
}
