namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stormscale Scion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormscale Scion</exampledoc>
[TokenizationOrder(-1)]
public class StormscaleScionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copies become tokens"];
}
