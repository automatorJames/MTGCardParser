namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bria, Riptide Rogue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bria, Riptide Rogue</exampledoc>
[Dependent]
public class BriaRiptideRogueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of prowess, each triggers separately"];
}
