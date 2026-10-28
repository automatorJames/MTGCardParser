namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Feral Encounter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feral Encounter</exampledoc>
[TokenizationOrder(-1)]
public class FeralEncounterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of the next combat phase this turn, target creature you control deals damage equal to its power to up to one target creature you don't control"];
}
