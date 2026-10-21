namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exiling the cards found by a search: "exile them", "exile that card".</summary>
/// <exampledoc>Foresight</exampledoc>
/// <examplecapture>exile them</examplecapture>
[Dependent]
public class ExileFoundCards : Glyph
{
    public override Nib[] Nibs => ["exile", Alt("it", "them", "that card", "those cards")];
}
