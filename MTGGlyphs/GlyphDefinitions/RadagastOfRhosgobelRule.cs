namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Radagast of Rhosgobel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Radagast of Rhosgobel</exampledoc>
[TokenizationOrder(-1)]
public class RadagastOfRhosgobelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first creature spell you cast each turn costs {2} less to cast and can be cast as though it had flash"];
}
