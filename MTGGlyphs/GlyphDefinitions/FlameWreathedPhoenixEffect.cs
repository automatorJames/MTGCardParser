namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flame-Wreathed Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flame-Wreathed Phoenix</exampledoc>
[Dependent]
public class FlameWreathedPhoenixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if tribute wasn't paid, it gains haste and \"when this creature dies, return it to its owner's hand"];
}
