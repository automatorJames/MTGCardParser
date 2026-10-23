namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ram Through, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ram Through</exampledoc>
[TokenizationOrder(-1)]
public class RamThroughRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature you control has trample, excess damage is dealt to that creature's controller instead"];
}
