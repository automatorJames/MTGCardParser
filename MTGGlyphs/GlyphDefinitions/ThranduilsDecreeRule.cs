namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thranduil's Decree, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thranduil's Decree</exampledoc>
[TokenizationOrder(-1)]
public class ThranduilsDecreeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent spell is countered this way, exile it instead of putting it into its owner's graveyard"];
}
