namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of objects having done something this turn, as a condition: "three or more creatures died this turn", "two or more nonland permanents entered the battlefield under your control this turn" (celebration), "three or more creature cards were put into graveyards from anywhere this turn".</summary>
/// <exampledoc>Darklight Phoenix</exampledoc>
/// <examplecapture>two or more creatures died this turn</examplecapture>
[Dependent]
public class ObjectsEventCount : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "or more", Prop(What), Prop(Event), Prop(Turn)];

    public Quantity Amount { get; set; }
    public Permanents What { get; set; }
    public ObjectTurnEvent Event { get; set; }
    public TurnSpan Turn { get; set; }
}
