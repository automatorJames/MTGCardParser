namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Resolute Archangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Resolute Archangel</exampledoc>
[Dependent]
public class ResoluteArchangelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if your life total is less than your starting life total, it becomes equal to your starting life total"];
}
