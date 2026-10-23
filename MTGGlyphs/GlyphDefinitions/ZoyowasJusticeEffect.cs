namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zoyowa's Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zoyowa's Justice</exampledoc>
[Dependent]
public class ZoyowasJusticeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they exile cards from the top of their library until they exile a nonland card with that mana value or less"];
}
