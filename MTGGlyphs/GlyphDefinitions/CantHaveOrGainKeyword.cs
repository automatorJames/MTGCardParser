namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword a permanent is barred from: "can't have or gain flying" (after "lose flying and", as the Archetypes say).</summary>
/// <exampledoc>Archetype of Imagination</exampledoc>
/// <examplecapture>can't have or gain flying</examplecapture>
[Dependent]
public class CantHaveOrGainKeyword : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't have or gain", Prop(Keyword)];

    public Keyword Keyword { get; set; }
}
