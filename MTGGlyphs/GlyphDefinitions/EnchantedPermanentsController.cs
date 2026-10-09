namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"enchanted land's controller": the player who controls the permanent an aura is attached to. A player referent, so a later "that player" refers to them.</summary>
/// <exampledoc>Cursed Land</exampledoc>
/// <examplecapture>enchanted land's controller</examplecapture>
[Dependent]
public class EnchantedPermanentsController : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(Kind), "'s", Prop(Player)];

    public PermanentKind Kind { get; set; }
    [Referent]
    [Singular]
    public PlayerIdentity Player { get; set; }
}
