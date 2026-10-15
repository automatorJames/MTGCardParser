namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Flashback's reminder text, first sentence.</summary>
/// <exampledoc>Ancient Grudge</exampledoc>
/// <examplecapture>you may cast this card from your graveyard for its flashback cost</examplecapture>
public class FlashbackRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard for its flashback cost"];
}
