namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "at the beginning of each player's upkeep, …": a trigger whose player is a referent, so the effect can say
/// "that player" (see <see cref="ThatPlayer"/>).
/// </summary>
public class AtBeginningOfEachPlayersStep : Glyph
{
    public override Nib[] Nibs => ["at the beginning of each", Prop(Player), "'s", Prop(Phase), ",", Prop(Effect)];

    [Referent]
    public PlayerIdentity Player { get; set; }

    public Phase Phase { get; set; }
    public DynamicGlyph Effect { get; set; }
}
