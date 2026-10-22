namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A floor on a player's life total from damage, a replacement effect: "damage that would reduce your life total to less than 1 reduces it to 1 instead".</summary>
/// <exampledoc>Worship</exampledoc>
/// <examplecapture>damage that would reduce your life total to less than 1 reduces it to 1 instead</examplecapture>
public class LifeTotalFloor : Glyph
{
    public override Nib[] Nibs => ["damage that would reduce", Prop(Whose), "life total to less than", Prop(Floor), "reduces it to", Prop(ReducedTo), "instead"];

    public Whose Whose { get; set; }
    public Quantity Floor { get; set; }
    public Quantity ReducedTo { get; set; }
}
