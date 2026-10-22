namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A tie for a superlative an effect picked by, as a condition: "two or more creatures are tied for least power", "two or more players are tied for lowest life total", "two or more cards are tied for greatest power", "two or more permanents a player controls are tied for greatest".</summary>
/// <exampledoc>Drop of Honey</exampledoc>
/// <examplecapture>two or more creatures are tied for least power</examplecapture>
[Dependent]
public class TiedForExtreme : Glyph
{
    public override Nib[] Nibs => ["two or more", Prop(Tied), "are tied for", Prop(Extreme), Prop(Characteristic)];

    public OneOf<Permanents, CardsTied?> Tied { get; set; }
    public Superlative Extreme { get; set; }
    [Optional]
    public Characteristic? Characteristic { get; set; }
}
