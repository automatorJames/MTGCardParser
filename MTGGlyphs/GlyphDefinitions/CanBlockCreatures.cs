namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A predicate that a creature can block creatures with a quality: "can block creatures with flying", as reach's reminder text.</summary>
/// <exampledoc>Aerie Bowmasters</exampledoc>
/// <examplecapture>can block creatures with flying</examplecapture>
[Dependent]
public class CanBlockCreatures : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can block creatures", Prop(Condition)];

    public WithKeywords Condition { get; set; }
}
