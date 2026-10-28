namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avatar Aang // Aang, Master of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar Aang // Aang, Master of Elements</exampledoc>
[TokenizationOrder(-1)]
public class AvatarAangRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you waterbend, earthbend, firebend, or airbend, draw a card"];
}
