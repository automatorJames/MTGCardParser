namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Keywords granted alongside a quoted ability, with their conjunction: the "haste and" of `gains haste and "when this creature dies, draw a card"`, the "trample, haste, and" of `has trample, haste, and "…"`.</summary>
/// <exampledoc>Run Wild</exampledoc>
/// <examplecapture>trample and</examplecapture>
[Dependent]
public class KeywordsAlongside : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [" ", Prop(Keywords), Alt(" and", ", and")];

    public CompoundOf<Keyword> Keywords { get; set; }
}
