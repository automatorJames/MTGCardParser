namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Interface Ace, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Interface Ace</exampledoc>
[TokenizationOrder(-1)]
public class InterfaceAceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature saddles mounts and crews vehicles using its toughness rather than its power"];
}
