namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raph & Leo, Sibling Rivals, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raph &amp; Leo, Sibling Rivals</exampledoc>
[TokenizationOrder(-1)]
public class RaphLeoSiblingRivalsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever raph & leo attack, if it's the first combat phase of the turn, untap one or two target attacking creatures"];
}
