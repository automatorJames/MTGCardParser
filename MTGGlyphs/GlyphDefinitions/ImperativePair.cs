namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A whole imperative sentence of two actions joined by "and", as spells and follow-up sentences say them: "reveal the top card of your library and put that card into your hand", "draw two cards and create a treasure token". Wraps <see cref="PredicatePair"/> so the pair can stand as a sentence without changing what nested slots resolve to.</summary>
/// <exampledoc>Dark Confidant</exampledoc>
/// <examplecapture>reveal the top card of your library and put that card into your hand</examplecapture>
public class ImperativePair : Glyph
{
    public override Nib[] Nibs => [Prop(Actions)];

    public PredicatePair Actions { get; set; }
}
