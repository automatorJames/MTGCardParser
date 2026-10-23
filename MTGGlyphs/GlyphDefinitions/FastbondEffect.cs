namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fastbond, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fastbond</exampledoc>
[Dependent]
public class FastbondEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it wasn't the first land you played this turn, this enchantment deals 1 damage to you"];
}
