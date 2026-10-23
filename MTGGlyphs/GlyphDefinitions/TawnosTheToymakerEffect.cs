namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tawnos, the Toymaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tawnos, the Toymaker</exampledoc>
[Dependent]
public class TawnosTheToymakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy it, except the copy is an artifact in addition to its other types"];
}
