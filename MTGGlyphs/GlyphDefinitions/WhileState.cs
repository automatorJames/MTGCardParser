namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static effect holding while a state lasts: "as long as it's attacking", "as long as it has a +1/+1 counter on it", "as long as there are seven or more cards in your graveyard", "as long as you gained life this turn", "as long as an opponent has 10 or less life", "as long as it's enchanted".</summary>
/// <exampledoc>Kor Scythemaster</exampledoc>
/// <examplecapture>as long as it's attacking</examplecapture>
[Dependent]
public class WhileState : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(State)];

    public GameState State { get; set; }
}
