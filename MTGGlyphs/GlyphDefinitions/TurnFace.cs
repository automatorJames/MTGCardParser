namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Turning a permanent face up or face down: "turn target face-down creature face up", "turn this creature face down", "turn target creature with a morph ability face down", "turn all other nontoken creatures face down".</summary>
/// <exampledoc>Ixidor, Reality Sculptor</exampledoc>
/// <examplecapture>turn target face-down creature face up</examplecapture>
public class TurnFace : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("turn", "turns"), Prop(Permanent), Prop(Face)];

    public OneOf<PermanentPhrase, AllPermanents> Permanent { get; set; }
    public FaceDirection Face { get; set; }
}
