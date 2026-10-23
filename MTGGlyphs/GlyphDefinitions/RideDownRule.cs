namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ride Down, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ride Down</exampledoc>
[TokenizationOrder(-1)]
public class RideDownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that were blocked by that creature this combat gain trample until end of turn"];
}
