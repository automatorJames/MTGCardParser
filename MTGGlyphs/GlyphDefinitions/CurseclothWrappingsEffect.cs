namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cursecloth Wrappings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cursecloth Wrappings</exampledoc>
/// <examplecapture>create a token that's a copy of it, except it's a white zombie in addition to its other types and has no mana cost</examplecapture>
[Dependent]
public class CurseclothWrappingsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a token that's a copy of it, except it's a white zombie in addition to its other types and has no mana cost"];
}
