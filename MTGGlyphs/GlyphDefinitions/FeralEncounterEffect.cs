namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Feral Encounter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feral Encounter</exampledoc>
/// <examplecapture>target creature you control deals damage equal to its power to up to one target creature you don't control</examplecapture>
[Dependent]
public class FeralEncounterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you control deals damage equal to its power to up to one target creature you don't control"];
}
