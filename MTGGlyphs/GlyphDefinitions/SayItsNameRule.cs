namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Say Its Name, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Say Its Name</exampledoc>
[TokenizationOrder(-1)]
public class SayItsNameRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile this card and two other cards named", Nib.This, "from your graveyard: search your graveyard, hand, and/or library for a card named altanak, the thrice-called and put it onto the battlefield"];
}
