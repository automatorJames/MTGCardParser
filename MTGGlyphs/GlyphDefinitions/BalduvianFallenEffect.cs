namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Balduvian Fallen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balduvian Fallen</exampledoc>
[Dependent]
public class BalduvianFallenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +1/+0 until end of turn for each {b} or {r} spent this way"];
}
