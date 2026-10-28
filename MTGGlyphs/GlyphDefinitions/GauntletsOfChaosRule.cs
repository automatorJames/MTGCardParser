namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gauntlets of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gauntlets of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GauntletsOfChaosRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if those permanents are exchanged this way, destroy all auras attached to them"];
}
