namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Edgewall Inn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgewall Inn</exampledoc>
[Dependent]
public class EdgewallInnEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target card that has an adventure from your graveyard to your hand"];
}
