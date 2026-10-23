namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Angel of Jubilation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angel of Jubilation</exampledoc>
[TokenizationOrder(-1)]
public class AngelOfJubilationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't pay life or sacrifice creatures to cast spells or activate abilities"];
}
