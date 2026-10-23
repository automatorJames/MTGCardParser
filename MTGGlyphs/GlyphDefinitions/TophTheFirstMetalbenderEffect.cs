namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Toph, the First Metalbender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toph, the First Metalbender</exampledoc>
[Dependent]
public class TophTheFirstMetalbenderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they don't gain the ability to {t} for mana"];
}
