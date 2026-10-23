namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Outrageous Robbery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outrageous Robbery</exampledoc>
[TokenizationOrder(-1)]
public class OutrageousRobberyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at and play those cards for as long as they remain exiled"];
}
