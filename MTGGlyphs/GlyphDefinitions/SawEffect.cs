namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Saw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Saw</exampledoc>
[Dependent]
public class SawEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice a permanent other than that creature or this equipment"];
}
