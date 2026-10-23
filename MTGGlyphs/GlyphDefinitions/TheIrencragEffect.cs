namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Irencrag, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Irencrag</exampledoc>
[Dependent]
public class TheIrencragEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have", Nib.This, "become a legendary equipment artifact named everflame, heroes' legacy"];
}
