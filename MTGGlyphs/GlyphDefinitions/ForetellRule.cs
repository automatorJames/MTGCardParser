namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Foretell's reminder text, first sentence.</summary>
/// <exampledoc>Jarl of the Forsaken</exampledoc>
/// <examplecapture>during your turn, you may pay {2} and exile this card from your hand face down</examplecapture>
public class ForetellRule : Glyph
{
    public override Nib[] Nibs => ["during your turn, you may pay {2} and exile this card from your hand face down"];
}
