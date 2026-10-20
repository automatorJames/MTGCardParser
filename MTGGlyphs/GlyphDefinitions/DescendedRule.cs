namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Descend's reminder: "you descended if a permanent card was put into your graveyard from anywhere".</summary>
/// <exampledoc>Deep Goblin Skulltaker</exampledoc>
/// <examplecapture>you descended if a permanent card was put into your graveyard from anywhere</examplecapture>
public class DescendedRule : Glyph
{
    public override Nib[] Nibs => ["you descended if a permanent card was put into your graveyard from anywhere"];
}
