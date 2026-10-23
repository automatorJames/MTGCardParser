namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having been dealt at least an amount of damage this turn, as a condition: "an opponent was dealt 7 or more damage this turn", "a player was dealt 6 or more combat damage this turn", "you were dealt 4 or more damage this turn".</summary>
/// <exampledoc>Spinerock Knoll</exampledoc>
/// <examplecapture>an opponent was dealt 7 or more damage this turn</examplecapture>
[Dependent]
public class PlayerDealtDamageAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("was|were"), "dealt", Prop(Amount), Prop(Combat), "damage", Prop(Turn)];

    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
    public AtLeastCount Amount { get; set; }
    [Optional]
    [RegexPattern("combat")]
    public bool? Combat { get; set; }
    public TurnSpan Turn { get; set; }
}
