namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent narrowed to those that did something this turn: "that attacked this turn", "that blocked this turn", "that dealt damage this turn", "that dealt damage to you this turn".</summary>
/// <exampledoc>Spear of Heliod</exampledoc>
/// <examplecapture>that dealt damage to you this turn</examplecapture>
[Dependent]
public class ThatDidThisTurn : Glyph
{
    public override Nib[] Nibs => ["that", Prop(Event), "this turn"];

    public PastPermanentEvent Event { get; set; }
}
