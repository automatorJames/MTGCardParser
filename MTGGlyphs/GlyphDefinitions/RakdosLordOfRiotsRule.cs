namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rakdos, Lord of Riots, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, Lord of Riots</exampledoc>
[TokenizationOrder(-1)]
public class RakdosLordOfRiotsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast rakdos unless an opponent lost life this turn"];
}
