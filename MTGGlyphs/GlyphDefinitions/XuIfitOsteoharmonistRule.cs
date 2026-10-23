namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Xu-Ifit, Osteoharmonist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Xu-Ifit, Osteoharmonist</exampledoc>
[TokenizationOrder(-1)]
public class XuIfitOsteoharmonistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's a skeleton in addition to its other types and has no abilities"];
}
