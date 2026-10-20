namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bestow's reminder text, first sentence.</summary>
/// <exampledoc>Baleful Eidolon</exampledoc>
/// <examplecapture>if you cast this card for its bestow cost, it's an aura spell with enchant creature</examplecapture>
public class BestowRule : Glyph
{
    public override Nib[] Nibs => ["if you cast this card for its bestow cost, it's an aura spell with enchant creature"];
}
