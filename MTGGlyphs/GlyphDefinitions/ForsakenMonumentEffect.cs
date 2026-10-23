namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Forsaken Monument, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forsaken Monument</exampledoc>
[Dependent]
public class ForsakenMonumentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an additional {c}"];
}
