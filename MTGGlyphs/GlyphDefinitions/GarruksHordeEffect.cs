namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Garruk's Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk's Horde</exampledoc>
[Dependent]
public class GarruksHordeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["do this only any time you could cast that creature spell"];
}
