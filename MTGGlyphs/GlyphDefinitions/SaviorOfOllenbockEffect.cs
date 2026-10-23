namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Savior of Ollenbock, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Savior of Ollenbock</exampledoc>
[Dependent]
public class SaviorOfOllenbockEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile up to one other target creature from the battlefield or creature card from a graveyard"];
}
