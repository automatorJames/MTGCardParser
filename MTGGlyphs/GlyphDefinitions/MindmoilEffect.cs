namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mindmoil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindmoil</exampledoc>
[Dependent]
public class MindmoilEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the cards in your hand on the bottom of your library in any order, then draw that many cards"];
}
