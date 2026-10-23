namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Alania, Divergent Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alania, Divergent Storm</exampledoc>
/// <examplecapture>if it's the first instant spell, the first sorcery spell, or the first otter spell other than alania you've cast this turn, you may have target opponent draw a card</examplecapture>
[Dependent]
public class AlaniaDivergentStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first instant spell, the first sorcery spell, or the first otter spell other than alania you've cast this turn, you may have target opponent draw a card"];
}
