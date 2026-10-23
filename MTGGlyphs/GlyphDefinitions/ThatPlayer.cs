namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "that player", "the player", "that opponent", "the first player", "the second player": refers back to a player - a <see cref="PlayerIdentity"/> marked
/// [Referent] earlier in the line. Skips anything else in between, {this} included: in "at the beginning of each
/// player's upkeep, {this} deals 1 damage to that player", it's the player whose upkeep it is. "The first/second player" (the Oaths) tell apart a chooser and the player they chose.
/// </summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>that player</examplecapture>
[Dependent]
public class ThatPlayer : BackReference<PlayerIdentity>
{
    public override Nib[] Nibs => [Alt("that", "the", "the first", "the second"), Prop(Player)];

    public PlayerIdentity Player { get; set; }
}
