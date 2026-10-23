namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Plargg and Nassari, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plargg and Nassari</exampledoc>
[TokenizationOrder(-1)]
public class PlarggAndNassariRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses a nonland card exiled this way"];
}
