namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mythos of Nethroi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mythos of Nethroi</exampledoc>
[TokenizationOrder(-1)]
public class MythosOfNethroiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonland permanent if it's a creature or if {g}{w} was spent to cast this spell"];
}
