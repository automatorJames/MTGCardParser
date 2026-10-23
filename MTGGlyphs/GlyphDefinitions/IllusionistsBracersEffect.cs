namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Illusionist's Bracers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionist's Bracers</exampledoc>
[Dependent]
public class IllusionistsBracersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it isn't a mana ability, copy that ability"];
}
