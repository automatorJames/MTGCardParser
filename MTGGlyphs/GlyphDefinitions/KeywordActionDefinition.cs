namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action's reminder, defining it: "to surveil 2, look at the top two cards of your library, then put any number of them into your graveyard and the rest on top of your library in any order", "to scry 1, look at the top card of your library, then you may put that card on the bottom", "to investigate, create a clue token". Both halves are any glyph that resolves.</summary>
/// <exampledoc>Cruel Truths</exampledoc>
/// <examplecapture>to surveil 2, look at the top two cards of your library, then put any number of them into your graveyard and the rest on top of your library in any order</examplecapture>
public class KeywordActionDefinition : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Action), ",", Prop(Definition)];

    public DynamicGlyph Action { get; set; }
    public DynamicGlyph Definition { get; set; }
}
