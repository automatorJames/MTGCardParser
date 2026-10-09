namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A trigger on something happening to {this} or to any permanent of a kind: "when {this} dies, …", "whenever a land
/// enters the battlefield, …". A permanent of a kind is a referent, so the effect can say "that land's controller".
/// </summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>whenever a land enters the battlefield, {this} deals 2 damage to that land's controller</examplecapture>
public class PermanentEventTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), Prop(Event), ",", Prop(Effect)];

    public OneOf<This, IndefinitePermanent> Subject { get; set; }
    public PermanentEvent Event { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}

public enum PermanentEvent
{
    Dies,
    EntersTheBattlefield,
    LeavesTheBattlefield,
    IsPutIntoAGraveyardFromTheBattlefield,
    Attacks,
    Blocks,
    BecomesBlocked,
    BecomesTapped,
}
