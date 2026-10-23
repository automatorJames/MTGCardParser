namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Visions of Phyrexia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Visions of Phyrexia</exampledoc>
[Dependent]
public class VisionsOfPhyrexiaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you didn't play a card from exile this turn, create a tapped powerstone token"];
}
