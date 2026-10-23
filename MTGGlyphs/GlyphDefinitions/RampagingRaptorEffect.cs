namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rampaging Raptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rampaging Raptor</exampledoc>
[Dependent]
public class RampagingRaptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals that much damage to target planeswalker that player controls or battle that player protects"];
}
