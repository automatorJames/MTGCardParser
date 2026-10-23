namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Surrak, Elusive Hunter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Surrak, Elusive Hunter</exampledoc>
[Dependent]
public class SurrakElusiveHunterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
