namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Desolation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desolation</exampledoc>
[Dependent]
public class DesolationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who tapped a land for mana this turn sacrifices a land of their choice"];
}
