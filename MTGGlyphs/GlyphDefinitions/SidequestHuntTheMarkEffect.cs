namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sidequest: Hunt the Mark // Yiazmat, Ultimate Mark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sidequest: Hunt the Mark // Yiazmat, Ultimate Mark</exampledoc>
[Dependent]
public class SidequestHuntTheMarkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature died under an opponent's control this turn, create a treasure token"];
}
