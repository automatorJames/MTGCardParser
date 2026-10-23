namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balduvian Fallen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balduvian Fallen</exampledoc>
[TokenizationOrder(-1)]
public class BalduvianFallenCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature's cumulative upkeep is paid, it gets +1/+0 until end of turn for each {b} or {r} spent this way"];
}
