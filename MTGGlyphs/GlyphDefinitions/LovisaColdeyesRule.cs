namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lovisa Coldeyes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lovisa Coldeyes</exampledoc>
[TokenizationOrder(-1)]
public class LovisaColdeyesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature that's a barbarian, a warrior, or a berserker gets +2/+2 and has haste"];
}
