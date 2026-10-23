namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ion Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ion Storm</exampledoc>
[Dependent]
public class IonStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals 2 damage to any target"];
}
