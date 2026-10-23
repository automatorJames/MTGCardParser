namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fulgent Distraction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fulgent Distraction</exampledoc>
[TokenizationOrder(-1)]
public class FulgentDistractionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap those creatures, then unattach all equipment from them"];
}
