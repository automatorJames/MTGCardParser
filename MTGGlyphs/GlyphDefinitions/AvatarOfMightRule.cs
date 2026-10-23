namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avatar of Might, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar of Might</exampledoc>
[TokenizationOrder(-1)]
public class AvatarOfMightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent controls at least four more creatures than you, this spell costs {6} less to cast"];
}
