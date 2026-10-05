namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An unspecified object of a type: "a land", "an artifact".</summary>
[Dependent]
public class IndefiniteObject : Glyph
{
    public override Nib[] Nibs => [Alt("a", "an"), Prop(CardOrCreatureType)];

    public CardOrCreatureType CardOrCreatureType { get; set; }
}
