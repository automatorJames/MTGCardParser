namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Seer of Stolen Sight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seer of Stolen Sight</exampledoc>
[Dependent]
public class SeerOfStolenSightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["surveil 1"];
}
