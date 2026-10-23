namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Planewide Celebration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planewide Celebration</exampledoc>
[TokenizationOrder(-1)]
public class PlanewideCelebrationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• create a 2/2 citizen creature token that's all colors"];
}
