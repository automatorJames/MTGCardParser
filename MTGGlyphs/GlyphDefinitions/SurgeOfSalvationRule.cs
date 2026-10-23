namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Surge of Salvation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Surge of Salvation</exampledoc>
[TokenizationOrder(-1)]
public class SurgeOfSalvationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that black and/or red sources would deal to creatures you control this turn"];
}
