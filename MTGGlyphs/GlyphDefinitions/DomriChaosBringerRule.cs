namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Domri, Chaos Bringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Domri, Chaos Bringer</exampledoc>
[TokenizationOrder(-1)]
public class DomriChaosBringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a creature spell, it gains riot"];
}
