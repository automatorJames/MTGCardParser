namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement sending a countered spell somewhere other than its owner's graveyard: "if that spell is countered this way, put it on top of its owner's library instead of into that player's graveyard", "put it into its owner's hand instead of into that player's graveyard", "if an artifact or creature spell is countered this way, put that card onto the battlefield under your control instead of into its owner's graveyard".</summary>
/// <exampledoc>Remand</exampledoc>
/// <examplecapture>if that spell is countered this way, put it into its owner's hand instead of into that player's graveyard</examplecapture>
public class InsteadOfGraveyard : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Spell), Prop(Event), ",", Prop(Effect), "instead of into", Prop(Graveyard)];

    public OneOf<SpellReference?, IndefiniteSpell> Spell { get; set; }
    public CounteredThisWay Event { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Effect { get; set; }
    public PlayersZone Graveyard { get; set; }
}
