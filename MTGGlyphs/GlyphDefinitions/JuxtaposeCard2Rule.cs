namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Juxtapose, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Juxtapose</exampledoc>
[TokenizationOrder(-1)]
public class JuxtaposeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then exchange control of artifacts the same way"];
}
