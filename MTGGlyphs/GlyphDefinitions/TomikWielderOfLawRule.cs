namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tomik, Wielder of Law, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Wielder of Law</exampledoc>
[TokenizationOrder(-1)]
public class TomikWielderOfLawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent attacks with creatures, if two or more of those creatures are attacking you and/or planeswalkers you control, that opponent loses 3 life and you draw a card"];
}
