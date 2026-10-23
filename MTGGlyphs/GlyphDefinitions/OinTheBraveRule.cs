namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Óin the Brave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Óin the Brave</exampledoc>
[TokenizationOrder(-1)]
public class OinTheBraveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you have an enduring story, óin gets +1/+0 and has haste"];
}
