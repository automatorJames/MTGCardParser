namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"them", where only a player can be meant - the player damage is dealt to in "deals 2 damage to them". The built-in <see cref="Them"/> is plural and of any kind; in a place only a player fits, this one says so.</summary>
/// <exampledoc>Browbeat</exampledoc>
/// <examplecapture>them</examplecapture>
[Dependent]
public class ThemPlayer : BackReference<PlayerIdentity>
{
    public override Nib[] Nibs => ["them"];
}
