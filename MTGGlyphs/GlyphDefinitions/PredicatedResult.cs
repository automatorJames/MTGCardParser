namespace MTGGlyphs.GlyphDefinitions;

public class PredicatedResult : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(Predicate), ",", Prop(Result)];

    public DynamicGlyph Predicate { get; set; }
    public DynamicGlyph Result { get; set; }
}

public class SomeTestGlyph : Glyph
{
    public override Nib[] Nibs => ["first text snippet", Prop(CardType), Prop(CreatureType)];

    public CardType CardType { get; set; }
    public CreatureType CreatureType { get; set; }
}
