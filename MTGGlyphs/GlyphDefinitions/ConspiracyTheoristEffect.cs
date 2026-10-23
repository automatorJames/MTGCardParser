namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Conspiracy Theorist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conspiracy Theorist</exampledoc>
[Dependent]
public class ConspiracyTheoristEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile one of them from your graveyard"];
}
