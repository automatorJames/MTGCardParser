namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rosheen Meanderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rosheen Meanderer</exampledoc>
[TokenizationOrder(-1)]
public class RosheenMeandererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only on costs that contain {x}"];
}
