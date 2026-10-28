namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Giant Shark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Shark</exampledoc>
[TokenizationOrder(-1)]
public class GiantSharkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature blocks or becomes blocked by a creature that has been dealt damage this turn, this creature gets +2/+0 and gains trample until end of turn"];
}
