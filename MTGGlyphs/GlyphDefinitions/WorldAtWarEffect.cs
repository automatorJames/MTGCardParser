namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of World at War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>World at War</exampledoc>
[Dependent]
public class WorldAtWarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures that attacked this turn"];
}
