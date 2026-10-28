namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raging River, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raging River</exampledoc>
[TokenizationOrder(-1)]
public class RagingRiverRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't be blocked this combat except by creatures with flying and creatures in a pile with the chosen label"];
}
