namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent narrowed to those that did something this turn: "that attacked this turn", "that blocked this turn", "that dealt damage this turn", "that dealt damage to you this turn", "that entered the battlefield under your control this turn", or met another in combat: "that blocked or were blocked by it this turn", "that blocked or was blocked by a zombie this turn".</summary>
/// <exampledoc>Spear of Heliod</exampledoc>
/// <examplecapture>that dealt damage to you this turn</examplecapture>
[Dependent]
public class ThatDidThisTurn : Glyph
{
    public override Nib[] Nibs => ["that", Prop(Event), Prop(Detail), "this turn"];

    public PastPermanentEvent Event { get; set; }
    [Optional]
    public OneOf<UnderYourControl?, CombatPartner> Detail { get; set; }
}
