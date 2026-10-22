namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Ending the turn, first step of its reminder: "exile all spells and abilities from the stack, including this card" (older: "exile all spells and abilities, including this spell"). The comma is inside the alternatives, since a nested part can't open with one.</summary>
/// <exampledoc>Glorious End</exampledoc>
/// <examplecapture>exile all spells and abilities from the stack, including this card</examplecapture>
public class ExileTheStack : Glyph
{
    public override Nib[] Nibs => ["exile all spells and abilities", Alt("from the stack, including this card", "from the stack", ", including this spell")];
}
