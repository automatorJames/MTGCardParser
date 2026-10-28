namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Banish into Fable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Banish into Fable</exampledoc>
[TokenizationOrder(-1)]
public class BanishIntoFableRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you cast this spell from your hand, copy it if you control an artifact, then copy it if you control an enchantment"];
}
