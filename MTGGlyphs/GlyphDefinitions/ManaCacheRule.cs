namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mana Cache, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Cache</exampledoc>
[TokenizationOrder(-1)]
public class ManaCacheRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during their turn before the end step"];
}
