namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Talon of Pain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Talon of Pain</exampledoc>
[TokenizationOrder(-1)]
public class TalonOfPainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control other than this artifact deals damage to an opponent, put a charge counter on this artifact"];
}
