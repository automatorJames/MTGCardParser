namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Su-Chi Cave Guard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Su-Chi Cave Guard</exampledoc>
[Dependent]
public class SuChiCaveGuardEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add eight {c}"];
}
