namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dead Ringers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dead Ringers</exampledoc>
[TokenizationOrder(-1)]
public class DeadRingersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy two target nonblack creatures unless either one is a color the other isn't"];
}
