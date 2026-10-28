namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Monument to Perfection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Monument to Perfection</exampledoc>
[TokenizationOrder(-1)]
public class MonumentToPerfectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if there are nine or more lands with different names among the basic, sphere, and locus lands you control"];
}
