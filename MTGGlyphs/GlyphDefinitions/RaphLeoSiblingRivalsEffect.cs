namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Raph & Leo, Sibling Rivals, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raph &amp; Leo, Sibling Rivals</exampledoc>
[Dependent]
public class RaphLeoSiblingRivalsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first combat phase of the turn, untap one or two target attacking creatures"];
}
