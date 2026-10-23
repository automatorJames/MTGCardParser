namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Time Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Elemental</exampledoc>
[Dependent]
public class TimeElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target permanent that isn't enchanted to its owner's hand"];
}
