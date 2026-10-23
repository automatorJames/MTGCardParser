namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Noctis, Prince of Lucis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noctis, Prince of Lucis</exampledoc>
[TokenizationOrder(-1)]
public class NoctisPrinceOfLucisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast artifact spells from your graveyard by paying 3 life in addition to paying their other costs"];
}
