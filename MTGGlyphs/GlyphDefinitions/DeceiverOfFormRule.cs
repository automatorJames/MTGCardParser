namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deceiver of Form, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deceiver of Form</exampledoc>
/// <examplecapture>if a creature card is revealed this way, you may have creatures you control other than this creature become copies of that card until end of turn</examplecapture>
[TokenizationOrder(-1)]
public class DeceiverOfFormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature card is revealed this way, you may have creatures you control other than this creature become copies of that card until end of turn"];
}
