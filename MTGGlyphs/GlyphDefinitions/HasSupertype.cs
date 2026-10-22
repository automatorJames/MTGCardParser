namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having a supertype, set by a static effect: "(enchanted permanent) is legendary", "(enchanted land) is snow", "(all nonland permanents) are legendary".</summary>
/// <exampledoc>In Bolas's Clutches</exampledoc>
/// <examplecapture>is legendary</examplecapture>
[Dependent]
public class HasSupertype : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("is", "are"), Prop(Supertype)];

    public Supertype Supertype { get; set; }
}
