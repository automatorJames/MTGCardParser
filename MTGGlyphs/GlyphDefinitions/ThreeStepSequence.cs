namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Three effects done in order: "look at the top four cards of your library, exile one face down, then put the rest on the bottom in a random order" (hideaway), or joined by ", and": "search your library and/or graveyard for a card named ajani, valiant protector, reveal it, and put it into your hand". Like <see cref="ThenSequence"/>, tried after other glyphs; each step resolves on its own.</summary>
/// <exampledoc>Howltooth Hollow</exampledoc>
/// <examplecapture>look at the top four cards of your library, exile one face down, then put the rest on the bottom in a random order</examplecapture>
[TokenizationOrder(-1)]
public class ThreeStepSequence : Glyph
{
    public override Nib[] Nibs => [Prop(First), ",", Prop(Second), ",", Alt("then", "and"), Prop(Next)];

    public DynamicGlyph First { get; set; }
    public DynamicGlyph Second { get; set; }
    public DynamicGlyph Next { get; set; }
}
