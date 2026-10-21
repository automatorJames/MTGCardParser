namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Splice's reminder text: "as you cast an arcane spell, you may reveal this card from your hand and pay its splice cost".</summary>
/// <exampledoc>Dampen Thought</exampledoc>
/// <examplecapture>as you cast an arcane spell, you may reveal this card from your hand and pay its splice cost</examplecapture>
public class SpliceRule : Glyph
{
    public override Nib[] Nibs => ["as you cast an arcane spell, you may reveal this card from your hand and pay its splice cost"];
}
