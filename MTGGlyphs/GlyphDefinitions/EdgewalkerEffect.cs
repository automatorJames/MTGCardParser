namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Edgewalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgewalker</exampledoc>
[Dependent]
public class EdgewalkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if you cast a cleric spell with mana cost {1}{w}, it costs {1} to cast"];
}
