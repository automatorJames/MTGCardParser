namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "they", where only a player can be meant - "creatures they control". Rules text uses "they" for one player as
/// well as for several things, so the built-in <see cref="They"/> (plural, any kind) can't tell; in a place only a
/// player fits, this one says so.
/// </summary>
[Dependent]
public class TheyPlayer : BackReference<PlayerIdentity>
{
    public override Nib[] Nibs => ["they"];
}
