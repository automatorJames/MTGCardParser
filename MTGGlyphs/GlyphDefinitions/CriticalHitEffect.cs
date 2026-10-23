namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Critical Hit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Critical Hit</exampledoc>
[Dependent]
public class CriticalHitEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return this card from your graveyard to your hand"];
}
