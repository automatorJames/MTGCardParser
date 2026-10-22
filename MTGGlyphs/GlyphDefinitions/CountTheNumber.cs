namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counting something now, for a later "that number": "count the number of permanents you control", "count the number of cards in your library".</summary>
/// <exampledoc>Invincible Hymn</exampledoc>
/// <examplecapture>count the number of cards in your library</examplecapture>
public class CountTheNumber : Glyph, IPredicate
{
    public override Nib[] Nibs => ["count", Prop(Number)];

    public NumberOf Number { get; set; }
}
