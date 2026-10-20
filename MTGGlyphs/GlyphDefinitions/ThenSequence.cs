namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two effects done in order: "draw a card, then discard a card", "exile up to two target creatures you control, then return those cards to the battlefield under their owner's control". Also stands as a sentence on its own, tried after other glyphs so reminders, modes and triggers keep their own frames.</summary>
/// <exampledoc>Jalum Tome</exampledoc>
/// <examplecapture>draw a card, then discard a card</examplecapture>
[TokenizationOrder(-1)]
public class ThenSequence : Glyph
{
    public override Nib[] Nibs => [Prop(First), ", then", Prop(Next)];

    public DynamicGlyph First { get; set; }
    public DynamicGlyph Next { get; set; }
}
