namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Doubling a player's life total: "double target player's life total", "double its controller's life total".</summary>
/// <exampledoc>Beacon of Immortality</exampledoc>
/// <examplecapture>double target player's life total</examplecapture>
public class DoubleLifeTotal : Glyph, IPredicate
{
    public override Nib[] Nibs => ["double", Prop(Player), "life total"];

    public Owner Player { get; set; }
}
