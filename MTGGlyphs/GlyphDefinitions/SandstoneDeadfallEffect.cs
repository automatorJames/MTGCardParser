namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sandstone Deadfall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sandstone Deadfall</exampledoc>
[Dependent]
public class SandstoneDeadfallEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target attacking creature"];
}
