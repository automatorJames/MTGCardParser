namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exchanging control of two permanents: "exchange control of target land you control and target land an opponent controls", "exchange control of this creature and target creature", "exchange control of two target creatures". The pair is a <see cref="PermanentPair"/>, or a count of targets.</summary>
/// <exampledoc>Political Trickery</exampledoc>
/// <examplecapture>exchange control of target land you control and target land an opponent controls</examplecapture>
public class ExchangeControl : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("exchanges? control of"), Prop(Exchanged)];

    public OneOf<PermanentPair, CountedTargets> Exchanged { get; set; }
}
