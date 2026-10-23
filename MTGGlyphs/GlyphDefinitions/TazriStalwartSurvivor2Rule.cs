namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tazri, Stalwart Survivor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tazri, Stalwart Survivor</exampledoc>
[TokenizationOrder(-1)]
public class TazriStalwartSurvivor2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control has \"{t}: add one mana of any of this creature's colors"];
}
