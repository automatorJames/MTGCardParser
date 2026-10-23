namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Silent Gravestone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silent Gravestone</exampledoc>
[Dependent]
public class SilentGravestoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile this artifact and all cards from all graveyards"];
}
