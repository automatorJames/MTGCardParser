namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Liliana of the Dark Realms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana of the Dark Realms</exampledoc>
[Dependent]
public class LilianaOfTheDarkRealmsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"swamps you control have '{t}: add {b}{b}{b}{b}"];
}
