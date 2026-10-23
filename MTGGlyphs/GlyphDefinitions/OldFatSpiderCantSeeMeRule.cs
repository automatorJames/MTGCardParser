namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Old Fat Spider Can't See Me, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Old Fat Spider Can't See Me</exampledoc>
[TokenizationOrder(-1)]
public class OldFatSpiderCantSeeMeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ii — prevent all damage that would be dealt by up to one target creature for as long as this saga remains on the battlefield"];
}
