namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something happening to a permanent, said passively after "would": "be destroyed", "be dealt damage or destroyed". A predicate, so <see cref="WouldHappen"/> takes it.</summary>
/// <exampledoc>Clergy of the Holy Nimbus</exampledoc>
/// <examplecapture>be destroyed</examplecapture>
[Dependent]
public class BeAffected : Glyph, IPredicate
{
    public override Nib[] Nibs => ["be", Prop(Events)];

    public OneOf<ManyOf<PassiveEvent>, PassiveEvent?> Events { get; set; }
}
