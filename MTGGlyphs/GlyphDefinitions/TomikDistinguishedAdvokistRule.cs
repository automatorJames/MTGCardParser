namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tomik, Distinguished Advokist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Distinguished Advokist</exampledoc>
[TokenizationOrder(-1)]
public class TomikDistinguishedAdvokistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands on the battlefield and land cards in graveyards can't be the targets of spells or abilities your opponents control"];
}
