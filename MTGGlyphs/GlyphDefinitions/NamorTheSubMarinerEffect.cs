namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Namor the Sub-Mariner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Namor the Sub-Mariner</exampledoc>
[Dependent]
public class NamorTheSubMarinerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create that many 1/1 blue merfolk creature tokens"];
}
