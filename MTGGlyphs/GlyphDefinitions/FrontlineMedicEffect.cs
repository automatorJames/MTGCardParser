namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Frontline Medic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frontline Medic</exampledoc>
[Dependent]
public class FrontlineMedicEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell with {x} in its mana cost unless its controller pays {3}"];
}
