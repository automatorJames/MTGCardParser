namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ajani's Aid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani's Aid</exampledoc>
[Dependent]
public class AjanisAidEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage a creature of your choice would deal this turn"];
}
