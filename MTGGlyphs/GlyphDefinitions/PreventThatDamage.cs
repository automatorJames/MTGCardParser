namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Preventing damage a replacement condition just named: "prevent that damage", "prevent 1 of that damage", "you may prevent x of that damage". A predicate, so subjects and "may" take it.</summary>
/// <exampledoc>Guardian Seraph</exampledoc>
/// <examplecapture>prevent 1 of that damage</examplecapture>
public class PreventThatDamage : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["prevent", Prop(Part), "that damage"];

    [Optional]
    public PartOfDamage Part { get; set; }
}
