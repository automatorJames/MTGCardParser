namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Labyrinth Raptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Labyrinth Raptor</exampledoc>
[Dependent]
public class LabyrinthRaptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["defending player sacrifices a creature of their choice that's blocking it"];
}
