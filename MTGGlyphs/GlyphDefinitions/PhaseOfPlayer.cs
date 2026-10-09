namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A step or phase named before whose it is: "the upkeep of enchanted land's controller". The player is a referent, for a later "that player".</summary>
/// <exampledoc>Cursed Land</exampledoc>
/// <examplecapture>the upkeep of enchanted land's controller</examplecapture>
[Dependent]
public class PhaseOfPlayer : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Phase), "of", Prop(Player)];

    public Phase Phase { get; set; }
    public EnchantedPermanentsController Player { get; set; }
}
