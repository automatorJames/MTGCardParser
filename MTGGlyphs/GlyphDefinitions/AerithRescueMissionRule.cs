namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aerith Rescue Mission, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aerith Rescue Mission</exampledoc>
[TokenizationOrder(-1)]
public class AerithRescueMissionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• take 59 flights of stairs — tap up to three target creatures"];
}
