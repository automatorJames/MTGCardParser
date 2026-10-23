namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Deep Water, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deep Water</exampledoc>
[Dependent]
public class DeepWaterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if you tap a land you control for mana, it produces {u} instead of any other type"];
}
