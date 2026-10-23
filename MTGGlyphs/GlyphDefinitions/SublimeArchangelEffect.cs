namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sublime Archangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sublime Archangel</exampledoc>
[Dependent]
public class SublimeArchangelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of exalted, each triggers separately"];
}
