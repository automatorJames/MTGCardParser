namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition that must hold as a triggered event happens: "while saddled", "while you control a creature with power 4 or greater", "while there are four or more card types among cards in your graveyard", "while you don't control another dinosaur".</summary>
/// <exampledoc>Brightfield Glider</exampledoc>
/// <examplecapture>while saddled</examplecapture>
[Dependent]
public class WhileEventCondition : Glyph
{
    public override Nib[] Nibs => ["while", Prop(Condition)];

    public OneOf<PermanentDesignation?, OneOf<BothStates, GameState>, ControlsPermanent> Condition { get; set; }
}
