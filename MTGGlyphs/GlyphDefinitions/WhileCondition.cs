namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition that must hold as a triggered event happens, after "while": a designation ("while saddled"), a game state ("while you control a creature with power 4 or greater", "while there are four or more card types among cards in your graveyard", "while you don't control another dinosaur", "while your library has no cards in it", "while you have no cards in hand"), or counters on a permanent ("while it has a +1/+1 counter on it").</summary>
/// <exampledoc>Gishath, Sun's Avatar</exampledoc>
/// <examplecapture>while you control a creature with power 4 or greater</examplecapture>
[Dependent]
public class WhileCondition : Glyph
{
    public override Nib[] Nibs => ["while", Prop(Condition)];

    public OneOf<PermanentDesignation?, BothStates, GameState, ControlsPermanent, HasCounters, LibraryHasNoCards> Condition { get; set; }
}
