namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Buyback's second reminder sentence.</summary>
/// <exampledoc>Capsize</exampledoc>
/// <examplecapture>if you do, put this card into your hand as it resolves</examplecapture>
public class BuybackReturn : Glyph
{
    public override Nib[] Nibs => ["if you do, put this card into your hand as it resolves"];
}
