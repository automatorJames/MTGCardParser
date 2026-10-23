namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sorcerer Class, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorcerer Class</exampledoc>
[TokenizationOrder(-1)]
public class SorcererClass2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast an instant or sorcery spell or to gain a class level"];
}
