namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A whole imperative sentence of two alternative actions joined by "or", as discover's reminder says it: "cast it without paying its mana cost or put it into your hand". Tried after other glyphs, so a keyword line with a cost alternative ("equip—pay {3} or discard a card") stays a keyword line.</summary>
/// <exampledoc>Chimil, the Inner Sun</exampledoc>
/// <examplecapture>cast it without paying its mana cost or put it into your hand</examplecapture>
[TokenizationOrder(-1)]
public class ImperativeEither : Glyph
{
    public override Nib[] Nibs => [Prop(Actions)];

    public PredicateEither Actions { get; set; }
}
