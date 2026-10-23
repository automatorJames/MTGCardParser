namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[TokenizationOrder(-1)]
public class MeliraTheLivingCureRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would get one or more poison counters, instead you get one poison counter and you can't get additional poison counters this turn"];
}
