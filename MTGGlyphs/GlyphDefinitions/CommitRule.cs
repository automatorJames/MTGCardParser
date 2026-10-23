namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Commit // Memory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Commit // Memory</exampledoc>
[TokenizationOrder(-1)]
public class CommitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target spell or nonland permanent into its owner's library second from the top"];
}
