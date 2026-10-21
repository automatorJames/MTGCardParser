namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Setting a player's life total to a value: "target player's life total becomes 10", "each player's life total becomes the number of creatures they control", "your life total becomes 15".</summary>
/// <exampledoc>Magister Sphinx</exampledoc>
/// <examplecapture>target player's life total becomes 10</examplecapture>
public class LifeTotalBecomes : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Player), "life total becomes", Prop(NewTotal)];

    public Owner Player { get; set; }
    public OneOf<Quantity?, EqualTo, Amount> NewTotal { get; set; }
}
