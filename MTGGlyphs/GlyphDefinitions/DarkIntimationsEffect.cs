namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dark Intimations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dark Intimations</exampledoc>
[Dependent]
public class DarkIntimationsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile this card from your graveyard"];
}
