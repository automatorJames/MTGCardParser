namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action with a number, as something a player does: "scry 2", "surveil 1", "scry x". A predicate, so it can follow another action: "you gain 1 life and scry 1", "untap it and scry 1".</summary>
/// <exampledoc>Samite Herbalist</exampledoc>
/// <examplecapture>scry 1</examplecapture>
[Dependent]
public class KeywordActionAmount : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Amount)];

    public Keyword Action { get; set; }
    public Quantity Amount { get; set; }
}
