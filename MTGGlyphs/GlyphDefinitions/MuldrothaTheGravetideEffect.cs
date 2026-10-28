namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Muldrotha, the Gravetide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Muldrotha, the Gravetide</exampledoc>
[Dependent]
public class MuldrothaTheGravetideEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card has multiple permanent types, choose one as you play it"];
}
