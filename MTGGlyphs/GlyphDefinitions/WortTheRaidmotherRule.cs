namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wort, the Raidmother, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wort, the Raidmother</exampledoc>
[TokenizationOrder(-1)]
public class WortTheRaidmotherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each red or green instant or sorcery spell you cast has conspire"];
}
