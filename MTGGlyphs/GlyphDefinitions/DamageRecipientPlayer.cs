namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to you", "to an opponent": the player damage was dealt to.</summary>
/// <exampledoc>Discordant Spirit</exampledoc>
/// <examplecapture>to you</examplecapture>
[Dependent]
public class DamageRecipientPlayer : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Player)];

    public WhichPlayer Player { get; set; }
}
