namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Haunt's exile: "exile it haunting target creature" - the card goes to exile attached in name to the creature it haunts, so a later "the creature it haunts" refers to that creature.</summary>
/// <exampledoc>Absolver Thrull</exampledoc>
/// <examplecapture>exile it haunting target creature</examplecapture>
public class ExileHaunting : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["exile", Prop(Card), "haunting", Prop(Haunted)];

    public PermanentPhrase Card { get; set; }
    public PermanentPhrase Haunted { get; set; }
}
