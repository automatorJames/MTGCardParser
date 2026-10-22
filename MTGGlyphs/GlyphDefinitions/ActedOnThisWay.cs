namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether an effect just described acted on an object of a kind: "a creature is dealt damage this way", "an instant or sorcery card is revealed this way", "that spell is countered this way", "a red card is discarded this way", "that creature dies this way", "a player is dealt damage this way", "damage is prevented this way". The condition of "if …, …" riders after damage, prevention, reveal, discard, mill, destroy and counter effects.</summary>
/// <exampledoc>Provoke the Trolls</exampledoc>
/// <examplecapture>a creature is dealt damage this way</examplecapture>
[Dependent]
public class ActedOnThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Opt(Pattern("(is|was|are|were)")), Prop(Action), "this way"];

    public OneOf<IndefinitePermanent, IndefiniteCard, OneOf<ThatStackObject, ThatCardPhrase, OneOf<PermanentPhrase, WhichPlayer?, DamageFromSource>>> Object { get; set; }
    public ThisWayAction Action { get; set; }
}
