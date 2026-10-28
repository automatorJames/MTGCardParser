namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niko Defies Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko Defies Destiny</exampledoc>
[TokenizationOrder(-1)]
public class NikoDefiesDestinyRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to foretell cards or cast spells that have foretell"];
}
