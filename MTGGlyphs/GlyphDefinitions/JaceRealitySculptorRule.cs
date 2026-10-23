namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jace, Reality Sculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Reality Sculptor</exampledoc>
[TokenizationOrder(-1)]
public class JaceRealitySculptorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if there are twenty-five or more loyalty counters among jaces you control"];
}
