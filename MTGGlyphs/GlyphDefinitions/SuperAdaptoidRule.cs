namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Super-Adaptoid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Super-Adaptoid</exampledoc>
[TokenizationOrder(-1)]
public class SuperAdaptoidRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature has haste and", Nib.This, "doesn't, put a haste counter on", Nib.This];
}
