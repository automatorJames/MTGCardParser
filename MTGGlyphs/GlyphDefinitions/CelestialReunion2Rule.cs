namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Celestial Reunion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Reunion</exampledoc>
[TokenizationOrder(-1)]
public class CelestialReunion2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, you may choose a creature type and behold two creatures of that type"];
}
