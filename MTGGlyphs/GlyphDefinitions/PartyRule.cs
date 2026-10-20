namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Party's reminder text.</summary>
/// <exampledoc>Journey to Oblivion</exampledoc>
/// <examplecapture>your party consists of up to one each of cleric, rogue, warrior, and wizard</examplecapture>
public class PartyRule : Glyph
{
    public override Nib[] Nibs => ["your party consists of up to one each of cleric, rogue, warrior, and wizard"];
}
