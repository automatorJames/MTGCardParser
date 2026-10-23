namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Open the Vaults, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Open the Vaults</exampledoc>
[Dependent]
public class OpenTheVaultsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras with nothing to enchant remain in graveyards"];
}
