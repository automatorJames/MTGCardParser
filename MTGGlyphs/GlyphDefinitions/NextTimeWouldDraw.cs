namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A one-shot replacement of a player's next draw this turn: "the next time you would draw a card this turn, you gain 5 life instead", "the next time you would draw a card this turn, instead put the top card of the exiled pile into its owner's hand" (the Words cycle, Aladdin's Lamp). The draw-side sibling of NextTimeWouldDeal.</summary>
/// <exampledoc>Words of Worship</exampledoc>
/// <examplecapture>the next time you would draw a card this turn, you gain 5 life instead</examplecapture>
public class NextTimeWouldDraw : Glyph
{
    public override Nib[] Nibs => ["the next time", Prop(Player), "would draw a card this turn,", Prop(Replacement)];

    public WhichPlayer Player { get; set; }
    public OneOf<InsteadThenEffect, EffectThenInstead> Replacement { get; set; }
}
