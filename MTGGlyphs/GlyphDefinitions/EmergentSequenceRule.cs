namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emergent Sequence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emergent Sequence</exampledoc>
[TokenizationOrder(-1)]
public class EmergentSequenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on it for each land you had enter the battlefield under your control this turn"];
}
