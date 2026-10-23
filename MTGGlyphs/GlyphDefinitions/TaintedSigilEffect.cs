namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tainted Sigil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tainted Sigil</exampledoc>
[Dependent]
public class TaintedSigilEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the total life lost by all players this turn"];
}
