namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ertai's Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Familiar</exampledoc>
[Dependent]
public class ErtaisFamiliar2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next upkeep, this creature can't phase out"];
}
