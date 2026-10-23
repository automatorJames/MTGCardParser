namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hundred-Handed One, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hundred-Handed One</exampledoc>
[TokenizationOrder(-1)]
public class HundredHandedOneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this creature is monstrous, it has reach and can block an additional ninety-nine creatures each combat"];
}
