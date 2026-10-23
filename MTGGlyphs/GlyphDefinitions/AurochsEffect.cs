namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aurochs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aurochs</exampledoc>
[Dependent]
public class AurochsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +1/+0 until end of turn for each other attacking", Nib.This];
}
