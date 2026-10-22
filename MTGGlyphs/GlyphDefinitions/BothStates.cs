namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two game states that must both hold: "this card is in your graveyard and you control a mountain", "you have 30 or more life and an opponent has 10 or less life".</summary>
/// <exampledoc>Anger</exampledoc>
/// <examplecapture>this card is in your graveyard and you control a mountain</examplecapture>
[Dependent]
public class BothStates : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    public GameState First { get; set; }
    public GameState Second { get; set; }
}
