namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An optional action not taken, in the past: "didn't put the revealed card onto the battlefield this way". Needs "this way": "didn't cast a spell this turn" is a game state (PlayerDidThisTurn).</summary>
/// <exampledoc>Break Out</exampledoc>
/// <examplecapture>didn't put the revealed card onto the battlefield this way</examplecapture>
[Dependent]
public class DidNotDoThisWay : Glyph
{
    public override Nib[] Nibs => ["didn't", Prop(Action), "this way"];

    public OneOf<PlayCard, PutIntoZone, DrawOrDiscardCards> Action { get; set; }
}
