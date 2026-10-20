namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Clash's reminder text, second sentence ("greater" in current wording, "higher" in older).</summary>
/// <exampledoc>Adder-Staff Boggart</exampledoc>
/// <examplecapture>a player wins if their card had a greater mana value</examplecapture>
public class ClashWinner : Glyph
{
    public override Nib[] Nibs => ["a player wins if their card had a", Alt("greater", "higher"), "mana value"];
}
