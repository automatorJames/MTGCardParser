namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ayesha Tanaka, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ayesha Tanaka</exampledoc>
[Dependent]
public class AyeshaTanakaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability from an artifact source unless that ability's controller pays {w}"];
}
