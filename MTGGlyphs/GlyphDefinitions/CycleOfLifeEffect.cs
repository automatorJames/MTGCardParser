namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cycle of Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cycle of Life</exampledoc>
[Dependent]
public class CycleOfLifeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you cast this turn has base power and toughness 0/1 until your next upkeep"];
}
