namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Containment Priest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Containment Priest</exampledoc>
[TokenizationOrder(-1)]
public class ContainmentPriestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a nontoken creature would enter and it wasn't cast, exile it instead"];
}
