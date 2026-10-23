namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pull from Eternity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pull from Eternity</exampledoc>
[TokenizationOrder(-1)]
public class PullFromEternityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target face-up exiled card into its owner's graveyard"];
}
