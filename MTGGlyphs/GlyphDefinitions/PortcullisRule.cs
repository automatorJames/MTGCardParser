namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Portcullis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Portcullis</exampledoc>
[TokenizationOrder(-1)]
public class PortcullisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to the battlefield under its owner's control when this artifact leaves the battlefield"];
}
