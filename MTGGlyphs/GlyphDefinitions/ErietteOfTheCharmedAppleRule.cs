namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eriette of the Charmed Apple, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eriette of the Charmed Apple</exampledoc>
[TokenizationOrder(-1)]
public class ErietteOfTheCharmedAppleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature that's enchanted by an aura you control can't attack you or planeswalkers you control"];
}
