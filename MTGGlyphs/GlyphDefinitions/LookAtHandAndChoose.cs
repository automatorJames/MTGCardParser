namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Look at a targeted player's hand and choose a number of cards from it, setting them up for a following effect ("that player discards those cards").</summary>
/// <exampledoc>Abandon Hope</exampledoc>
/// <examplecapture>look at target opponent's hand and choose x cards from it</examplecapture>
public class LookAtHandAndChoose : Glyph
{
    public override Nib[] Nibs => ["look at", Prop(Hand), "and choose", Prop(Quantity), "cards from it"];

    public TargetPlayersHand Hand { get; set; }
    public Quantity Quantity { get; set; }
}
