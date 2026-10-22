namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"while your library has no cards in it", "while you have no cards in hand": a state that holds when an event happens.</summary>
/// <exampledoc>Laboratory Maniac</exampledoc>
/// <examplecapture>while your library has no cards in it</examplecapture>
[Dependent]
public class WhileStateHolds : Glyph
{
    public override Nib[] Nibs => ["while", Prop(State)];

    public OneOf<LibraryHasNoCards, GameState> State { get; set; }
}
