namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zack Fair, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zack Fair</exampledoc>
[TokenizationOrder(-1)]
public class ZackFairRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put", Nib.This, "'s counters on that creature and attach an equipment that was attached to", Nib.This, "to that creature"];
}
