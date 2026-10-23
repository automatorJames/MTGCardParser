namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Artisans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Artisans</exampledoc>
[TokenizationOrder(-1)]
public class GoblinArtisansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, counter target artifact spell you control that isn't the target of an ability from another creature named", Nib.This];
}
