namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A whole imperative sentence of two alternative actions joined by "or", as discover's reminder says it: "cast it without paying its mana cost or put it into your hand".</summary>
/// <exampledoc>Chimil, the Inner Sun</exampledoc>
/// <examplecapture>cast it without paying its mana cost or put it into your hand</examplecapture>
public class ImperativeEither : Glyph
{
    public override Nib[] Nibs => [Prop(Actions)];

    public PredicateEither Actions { get; set; }
}
