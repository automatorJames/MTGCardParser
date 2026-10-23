namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Frankenstein's Monster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frankenstein's Monster</exampledoc>
[TokenizationOrder(-1)]
public class FrankensteinsMonster2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you can't, put this creature into its owner's graveyard instead of onto the battlefield"];
}
