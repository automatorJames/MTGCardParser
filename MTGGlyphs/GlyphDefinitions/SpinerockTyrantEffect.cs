namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spinerock Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinerock Tyrant</exampledoc>
[Dependent]
public class SpinerockTyrantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy it"];
}
