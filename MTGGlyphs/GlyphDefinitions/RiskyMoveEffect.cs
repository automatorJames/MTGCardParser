namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Risky Move, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risky Move</exampledoc>
[Dependent]
public class RiskyMoveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature you control and an opponent"];
}
