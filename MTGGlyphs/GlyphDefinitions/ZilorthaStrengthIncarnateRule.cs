namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zilortha, Strength Incarnate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zilortha, Strength Incarnate</exampledoc>
[TokenizationOrder(-1)]
public class ZilorthaStrengthIncarnateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lethal damage dealt to creatures you control is determined by their power rather than their toughness"];
}
