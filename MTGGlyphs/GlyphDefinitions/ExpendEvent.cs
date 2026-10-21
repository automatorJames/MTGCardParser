namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The expend trigger event: "you expend 4" - a player spending their Nth total mana on spells in a turn. Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Bakersbane Duo</exampledoc>
/// <examplecapture>you expend 4</examplecapture>
[Dependent]
public class ExpendEvent : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "expend", Prop(Amount)];

    public WhichPlayer Player { get; set; }
    public int Amount { get; set; }
}
